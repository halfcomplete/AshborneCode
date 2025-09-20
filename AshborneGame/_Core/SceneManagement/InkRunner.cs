using Ink.Runtime;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using System.Reflection.Metadata.Ecma335;
using AshborneGame._Core.Globals.Enums;
using System.IO;
using AshborneGame._Core.Globals.Constants;
#if BLAZOR
using AshborneWASM.Pages;
#endif

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Handles loading, running, and syncing Ink stories with the Ashborne engine.
    /// Syncs variables and flags with GameStateManager.
    /// </summary>
    public class InkRunner
    {
        private Story? _story;
        public Story? Story => _story;
        private Player _player;
        private readonly GameStateManager _gameState;
        private readonly AppEnvironment _appEnvironment;


    private (string, int) _currentSilentPath = ("", 0);
    private CancellationTokenSource? _silentPathCts = null;
    private bool _hasPrintedDialogueEnd = false;

    // --- Silent path jump mechanism ---
    private string? _pendingSilentJumpPath = null;
    private TaskCompletionSource<bool>? _silentJumpTcs = null;

    // Events for UI to start/stop silent timer
    public event Action<int>? OnSilentTimerStart;
    public event Action? OnSilentTimerStop;

        public string CurrentSilentPath
        {
            get => _currentSilentPath.Item1;
        }

        public int SilentPathWait
        {
            get => _currentSilentPath.Item2;
        }

    public bool IsRunning => _story is not null && _story.canContinue;

        public InkRunner(GameStateManager gameState, Player player, AppEnvironment appEnvironment)
        {
            _gameState = gameState;
            _player = player;
            _appEnvironment = appEnvironment;
            _story = null;
        }

        /// <summary>
        /// Loads an Ink JSON story file from disk or HTTP.
        /// </summary>
        public async Task LoadFromFileAsync(string inkJsonPath)
        {
            string json;
            try
            {
                if (OperatingSystem.IsBrowser())
                {
                    // For web, use relative URLs to work with any base path (localhost, GitHub Pages, etc.)
                    string fullUrl = inkJsonPath.TrimStart('/');
                    string ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                    // Cache-busting query param
                    if (fullUrl.Contains("?"))
                        fullUrl += "&ts=" + ts;
                    else
                        fullUrl += "?ts=" + ts;

                    IOService.Output.DisplayDebugMessage($"[WASM] Trying to fetch: {fullUrl}", ConsoleMessageTypes.INFO);
                    
                    // Ensure it's a relative URL (no leading slash)
                    fullUrl = fullUrl.TrimStart('/');
                    
                    IOService.Output.DisplayDebugMessage($"[WASM] Final URL to fetch: {fullUrl}", ConsoleMessageTypes.INFO);
                    
                    // Use HttpClient with proper URL construction
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
                    
                    // For GitHub Pages, construct the full URL manually
                    if (_appEnvironment.IsGithubPages)
                    {
                        // GitHub Pages - use full URL
                        string fullUrlWithBase = _appEnvironment.BaseApiUrl + fullUrl;
                        IOService.Output.DisplayDebugMessage($"[WASM] Using full URL for GitHub Pages: {fullUrlWithBase}", ConsoleMessageTypes.INFO);
                        json = await httpClient.GetStringAsync(fullUrlWithBase);
                    }
                    else
                    {
                        // Local development - construct absolute URL from current page
                        string currentUrl = _appEnvironment.BaseApiUrl;
                        string absoluteUrl = currentUrl.TrimEnd('/') + "/" + fullUrl;
                        IOService.Output.DisplayDebugMessage($"[WASM] Using absolute URL for localhost: {absoluteUrl}", ConsoleMessageTypes.INFO);
                        json = await httpClient.GetStringAsync(absoluteUrl);
                    }
                }
                else
                {
                    // Console / local file system path
                    if (!File.Exists(inkJsonPath))
                        throw new FileNotFoundException($"Ink file not found at path: {inkJsonPath}");

                    IOService.Output.DisplayDebugMessage($"[LOCAL] Reading from: {inkJsonPath}", ConsoleMessageTypes.INFO);
                    json = File.ReadAllText(inkJsonPath);
                }
            }
            catch (Exception ex)
            {
                var stackTrace = Environment.StackTrace;
                IOService.Output.DisplayDebugMessage(
                    $"[DIALOGUE LOAD ERROR] {ex.Message}\nCall Stack:\n{stackTrace}",
                    ConsoleMessageTypes.ERROR
                );
                throw;
            }

            _story = new Story(json);
            InitialiseBindings();
        }


        /// <summary>
        /// Loads an Ink JSON story file from disk (sync version for console compatibility).
        /// </summary>
        public void LoadFromFile(string inkJsonPath)
        {
            LoadFromFileAsync(inkJsonPath).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the Ink story line by line until player input or choice is needed.
        /// </summary>
        public void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the Ink story line by line until player input or choice is needed (async version).
        /// </summary>
        public async Task RunAsync()
        {
            IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: RunAsync entered at {DateTime.Now}, canContinue={_story?.canContinue}, currentChoices={_story?.currentChoices?.Count}", ConsoleMessageTypes.INFO);
            if (_story is null)
                throw new InvalidOperationException("No Ink story loaded.");

            List<string> _globalSceneTags = _story.globalTags ?? new();

            while (true)
            {
                bool _canContinue = _story.canContinue;
                // Continue outputting story lines until we hit a choice or the story ends
                while (_canContinue)
                {
                    string line = _story.Continue().Trim();
                    if (line == OutputConstants.DialogueEndMarker)
                    {
                        _canContinue = true;
                        Console.WriteLine($"RunAsync encountered __END__ marker. Waiting for _hasPrintedDialogueEnd.");
                        IOService.Output.WriteLine(line);
                        while (!_hasPrintedDialogueEnd)
                        {
                            await Task.Delay(100); // Wait for dialogue to finish outputting
                        }
                        Console.WriteLine("_hasPrintedDialogueEnd set to true. Dialogue end confirmed.");
                        break;
                    }
                    IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Line='{line}'", ConsoleMessageTypes.INFO);
                    if (line.TrimEnd().EndsWith(OutputConstants.DialoguePauseMarker))
                    {
                        IOService.Output.WriteLine(line);
                        _canContinue = _story.canContinue;
                        continue;
                    }
                    // Handle async player input in WASM with optional prompt
                    if (line.StartsWith(OutputConstants.GetPlayerInputMarker))
                    {
                        IOService.Output.DisplayDebugMessage($"InkRunner: Awaiting player input at {DateTime.Now}", ConsoleMessageTypes.INFO);
                        string prompt = "What will you say?";
                        // Support __GET_PLAYER_INPUT__:Prompt text
                        if (line.Length > OutputConstants.GetPlayerInputMarker.Length && line[OutputConstants.GetPlayerInputMarker.Length] == ':')
                        {
                            Console.WriteLine("\"__GET_PLAYER_INPUT:Prompt text\" found!");
                            prompt = line.Substring(OutputConstants.GetPlayerInputMarker.Length + 1).Trim();
                            Console.WriteLine(prompt);
                            if (string.IsNullOrWhiteSpace(prompt))
                                prompt = "What will you say?";
                        }
                        string playerInput;
                        if (OperatingSystem.IsBrowser())
                        {
                            playerInput = await IOService.Input.GetPlayerInputAsync(prompt);
                            while (string.IsNullOrWhiteSpace(playerInput))
                            {
                                playerInput = await IOService.Input.GetPlayerInputAsync(prompt);
                            }
                        }
                        else
                        {
                            playerInput = IOService.Input.GetPlayerInput(prompt);
                            while (string.IsNullOrWhiteSpace(playerInput))
                            {
                                playerInput = IOService.Input.GetPlayerInput(prompt);
                            }
                        }
                        GameContext.GameState.SetLabel("player.input", playerInput);
                        IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Player input received at {DateTime.Now}", ConsoleMessageTypes.INFO);
                        _canContinue = _story.canContinue;
                        continue; // Continue the Ink story after input
                    }

                    List<string> rawTags = _story.currentTags ?? new();
                    List<string> lineTags = rawTags.Except(_globalSceneTags).ToList();
                    IOService.Output.DisplayDebugMessage(string.Join(' ', lineTags) ?? string.Empty, ConsoleMessageTypes.INFO);
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string? targetTag = lineTags.FirstOrDefault(t => t.StartsWith(OutputConstants.DialogueSpeedTag));
                        if (targetTag != null)
                        {
                            if (int.TryParse(targetTag.AsSpan(5), out int ms))
                            {
                                IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Custom speed tag found: {targetTag}. Using {ms} ms for typewriter effect.", ConsoleMessageTypes.INFO);
                                IOService.Output.WriteLine(line, ms);
                            }
                            else
                            {
                                IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Invalid speed tag format: {targetTag}. Using default speed of {OutputConstants.DefaultTypeSpeed} ms.", ConsoleMessageTypes.INFO);
                                IOService.Output.WriteLine(line, OutputConstants.DefaultTypeSpeed);
                            }
                        }
                        else
                        {
                            IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: No speed tag found. Using default speed of {OutputConstants.DefaultTypeSpeed} ms.", ConsoleMessageTypes.INFO);
                            IOService.Output.WriteLine(line, OutputConstants.DefaultTypeSpeed);
                        }
                    }
                    _canContinue = _story.canContinue;
                }

                // Check if we hit some choices
                if (_story.currentChoices.Count > 0)
                {
                    IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Presenting choices at {DateTime.Now}", ConsoleMessageTypes.INFO);
                    for (int i = 0; i < _story.currentChoices.Count; i++)
                    {
                        IOService.Output.WriteLine($"[{i + 1}] {_story.currentChoices[i].text}");
                    }

                    // --- Start silent path timer only when player is prompted for input and this isn't browser ---
                    if (!string.IsNullOrWhiteSpace(_currentSilentPath.Item1) && _currentSilentPath.Item2 > 0 && !OperatingSystem.IsBrowser())
                    {
                        // Fire event for UI to start silent timer
                        OnSilentTimerStart?.Invoke(_currentSilentPath.Item2);
                        await StopSilentTimer();
                        _silentJumpTcs = new TaskCompletionSource<bool>();
                    }

                    // Wait for either player input or silent jump
                    var choiceTask = IOService.Input.GetChoiceInputAsync(_story.currentChoices.Count);
                    Task completedTask;
                    if (_silentJumpTcs != null)
                        completedTask = await Task.WhenAny(choiceTask, _silentJumpTcs.Task);
                    else
                        completedTask = await Task.WhenAny(choiceTask);

                    // --- Cancel silent path timer and clear silent path as soon as a choice is made ---
                    OnSilentTimerStop?.Invoke();
                    await StopSilentTimer();
                    _currentSilentPath = ("", 0);

                    if (_silentJumpTcs != null && _silentJumpTcs.Task.IsCompleted && _pendingSilentJumpPath != null)
                    {
                        // Silent jump was triggered
                        IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Silent path jump triggered to '{_pendingSilentJumpPath}'", ConsoleMessageTypes.INFO);
                        _story.ChoosePathString(_pendingSilentJumpPath);
                        _pendingSilentJumpPath = null;
                        _silentJumpTcs = null;
                        continue; // Continue main loop with new path
                    }

                    int choice = choiceTask.IsCompleted ? choiceTask.Result : -1;
                    IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Choice {choice} selected at {DateTime.Now}", ConsoleMessageTypes.INFO);
                    // Defensive: Only process the choice if the story is still running and the choice is valid
                    if (_story.currentChoices.Count > 0 && choice > 0 && choice <= _story.currentChoices.Count)
                    {
                        _story.ChooseChoiceIndex(choice - 1);
                    }
                    else
                    {
                        IOService.Output.DisplayDebugMessage("[DEBUG] InkRunner: Invalid choice or story finished after choice selection. Skipping.", ConsoleMessageTypes.WARNING);
                        return;
                    }
                }

                if (_hasPrintedDialogueEnd)
                {
                    Console.WriteLine("Returning from RunAsync().");
                    return; // Exit if dialogue has finished outputting
                }
            }
        }

        // No longer used: silent timer is now managed by UI via events

        public Task StopSilentTimer()
        {
            _silentPathCts?.Cancel();
            OnSilentTimerStop?.Invoke();
            return Task.CompletedTask;
        }

        private void InitialiseBindings()
        {
            // --- Flags ---
            if (_story is not null)
            {
                _story.BindExternalFunction("setFlag", (string key, bool value) => _gameState.SetFlag(key, value));

                _story.BindExternalFunction("getFlag", (string key) =>
                {
                    if (_gameState.TryGetFlag(key, out var value))
                    {
                        return value;
                    }
                    throw new Exception($"Flag '{key}' does not exist.");
                });

                _story.BindExternalFunction("hasFlag", (string key) => _gameState.HasFlag(key));

                _story.BindExternalFunction("toggleFlag", (string key) =>
                {
                    var result = _gameState.TryToggleFlag(key);
                    if (result == null)
                        throw new Exception($"Toggle failed. Flag '{key}' does not exist.");
                });

                _story.BindExternalFunction("removeFlag", (string key) =>
                {
                    if (!_gameState.HasFlag(key))
                        throw new Exception($"Cannot remove non-existent flag '{key}'.");
                    _gameState.RemoveFlag(key);
                });
            }

            // --- Counters ---
            _story!.BindExternalFunction("setCounter", (string key, int value) => _gameState.SetCounter(key, value));

            _story.BindExternalFunction("getCounter", (string key) =>
            {
                return _gameState.TryGetCounter(key, out var result) ? result : throw new Exception($"Counter '{key}' does not exist.");
            });

            _story.BindExternalFunction("hasCounter", (string key) => _gameState.HasCounter(key));

            _story.BindExternalFunction("incCounter", (string key, int amount) =>
            {
                if (!_gameState.TryIncrementCounter(key, amount))
                    throw new Exception($"Failed to increment. Counter '{key}' does not exist.");
            });

            _story.BindExternalFunction("decCounter", (string key, int amount) =>
            {
                if (!_gameState.TryDecrementCounter(key, amount))
                    throw new Exception($"Failed to decrement. Counter '{key}' does not exist.");
            });

            _story.BindExternalFunction("removeCounter", (string key) =>
            {
                if (!_gameState.HasCounter(key))
                    throw new Exception($"Cannot remove non-existent counter '{key}'.");
                _gameState.RemoveCounter(key);
            });

            // --- Labels ---
            _story.BindExternalFunction("setLabel", (string key, string value) => _gameState.SetLabel(key, value));

            _story.BindExternalFunction("getLabel", (string key) =>
            {
                var result = _gameState.TryGetLabel(key);
                return result ?? throw new Exception($"Label 'key' does not exist.");
            });

            _story.BindExternalFunction("hasLabel", (string key) => _gameState.HasLabel(key));

            // --- Variables (Set only) ---
            _story.BindExternalFunction("setVar", (string key, string value) => _gameState.SetVariable(key, value));

            // --- Other ---
            _story.BindExternalFunction("playerHas", (string itemName) =>
            {
                Item? item = _player.Inventory.GetItem(itemName);
                return item != null;
            });

            _story.BindExternalFunction("playerGiveMask", (string maskName) => _gameState.GivePlayerMask(maskName));

            _story.BindExternalFunction("playerTryTakeMask", (string maskName) => _gameState.TryTakePlayerMask(maskName));

            _story.BindExternalFunction("playerWearingMask", (string maskName) => _gameState.PlayerWearingMask(maskName));

            _story.BindExternalFunction("playerForceMask", (string maskName) => _gameState.ForcePlayerMask(maskName));

            _story.BindExternalFunction("changePlayerStat", (string statName, int amount) =>
            {
                PlayerStatType statType = GameContext.Player.Stats.GetStatTypeByName(statName);
                GameContext.Player.Stats.ChangeBase(statType, amount);
            });

            _story.BindExternalFunction("getPlayerStat", (string statName) =>
            {
                (_, _, int totalValue) = GameContext.Player.Stats.GetStat(statName);
                return totalValue;
            });

            _story.BindExternalFunction("setSilentPath", (string silentPath, int silentMs) =>
            {
                _currentSilentPath = (silentPath, silentMs);
            });
        }

        private Func<bool, Task<string>>? _getPlayerInputFromUIAsync;
        public void SetPlayerInputCallback(Func<bool, Task<string>> callback) => _getPlayerInputFromUIAsync = callback;

        public event Action? OnDialogueEnd;

        /// <summary>
        /// Jump to a specific knot or stitch in the Ink story.
        /// </summary>
        public void JumpTo(string path)
        {
            if (_story is not null)
            {
                _story.ChoosePathString(path);
                Run();
            }
        }

        /// <summary>
        /// Signal to RunAsync that a silent path jump should occur.
        /// </summary>
        public Task TryJumpToSilentPathAsync()
        {
            IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: TryJumpToSilentPathAsync SIGNALLED, path={_currentSilentPath.Item1}, story canContinue={_story?.canContinue}", ConsoleMessageTypes.INFO);
            if (_story is null || (!_story.canContinue && (_story.currentChoices == null || _story.currentChoices.Count == 0)))
            {
                IOService.Output.DisplayDebugMessage("[DEBUG] InkRunner: TryJumpToSilentPathAsync aborted, story is finished.", ConsoleMessageTypes.INFO);
                return Task.CompletedTask;
            }
            if (string.IsNullOrWhiteSpace(_currentSilentPath.Item1))
                return Task.CompletedTask;
            _pendingSilentJumpPath = _currentSilentPath.Item1;
            _silentJumpTcs?.TrySetResult(true);
            return Task.CompletedTask;
        }

        // Reliable delay for WASM: loops in short intervals to avoid browser timer issues
        private static async Task ReliableDelay(int totalMs, CancellationToken token)
        {
            Console.WriteLine($"[DEBUG] InkRunner: ReliableDelay called with {totalMs} ms at {DateTime.UtcNow}.");
            int elapsed = 0;
            int step = 200;
            while (elapsed < totalMs)
            {
                int wait = Math.Min(step, totalMs - elapsed);
                await Task.Delay(wait, token);
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"[DEBUG] InkRunner: ReliableDelay cancelled at {DateTime.UtcNow} after {elapsed} ms.");
                    return;
                }
                elapsed += wait;
            }
            Console.WriteLine($"[DEBUG] InkRunner: ReliableDelay finished at {DateTime.UtcNow}.");
        }

        /// <summary>
        /// Checks if a variable exists in the Ink story.
        /// </summary>
        public bool HasInkVariable(string key)
        {
            if (_story is not null)
                return _story.variablesState.Contains(key);
            return false;
        }

        /// <summary>
        /// Gets a variable value from Ink.
        /// </summary>
        public object GetInkVariable(string key)
        {
            if (_story is not null)
                return _story.variablesState[key];
            throw new InvalidOperationException("No Ink story loaded.");
        }

        public void DialogueFinishedOutputting()
        {
            Console.WriteLine("[InkRunner] DialogueFinishedOutputting called");
            OnDialogueEnd?.Invoke();
            _hasPrintedDialogueEnd = true;
        }
    }
}
