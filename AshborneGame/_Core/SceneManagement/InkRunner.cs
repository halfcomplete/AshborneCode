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
        // Allows Home.razor to cancel the await Task.Delay(-1, ...) for choice input
        private CancellationTokenSource? _externalChoiceAwaitCts;

        /// <summary>
        /// Exposes a method to cancel the external choice await from outside (e.g., Home.razor)
        /// </summary>
        public void CancelExternalChoiceAwait()
        {
            _externalChoiceAwaitCts?.Cancel();
        }
        private Story? _story;
        public Story? Story => _story;
        private Player _player;
        private readonly GameStateManager _gameState;
        private readonly AppEnvironment _appEnvironment;

        private (string, int) _currentSilentPath = ("", 0);
        private CancellationTokenSource? _silentPathCts = null;

        private bool _hasPrintedDialogueEnd = false;

        public string CurrentSilentPath
        {
            get => _currentSilentPath.Item1;
        }

        public int SilentPathWait
        {
            get => _currentSilentPath.Item2;
        }

        public bool IsRunning => _story != null && _story.canContinue;

        public Action StartOssanethTimer;
        public Action StopOssanethTimer;

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

                    await IOService.Output.DisplayDebugMessage($"[WASM] Trying to fetch: {fullUrl}", ConsoleMessageTypes.INFO);

                    // Ensure it's a relative URL (no leading slash)
                    fullUrl = fullUrl.TrimStart('/');

                    await IOService.Output.DisplayDebugMessage($"[WASM] Final URL to fetch: {fullUrl}", ConsoleMessageTypes.INFO);

                    // Use HttpClient with proper URL construction
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");

                    // For GitHub Pages, construct the full URL manually
                    if (_appEnvironment.IsGithubPages)
                    {
                        // GitHub Pages - use full URL
                        string fullUrlWithBase = _appEnvironment.BaseApiUrl + fullUrl;
                        await IOService.Output.DisplayDebugMessage($"[WASM] Using full URL for GitHub Pages: {fullUrlWithBase}", ConsoleMessageTypes.INFO);
                        json = await httpClient.GetStringAsync(fullUrlWithBase);
                    }
                    else
                    {
                        // Local development - construct absolute URL from current page
                        string currentUrl = _appEnvironment.BaseApiUrl;
                        string absoluteUrl = currentUrl.TrimEnd('/') + "/" + fullUrl;
                        await IOService.Output.DisplayDebugMessage($"[WASM] Using absolute URL for localhost: {absoluteUrl}", ConsoleMessageTypes.INFO);
                        json = await httpClient.GetStringAsync(absoluteUrl);
                    }
                }
                else
                {
                    // Console / local file system path
                    if (!File.Exists(inkJsonPath))
                        throw new FileNotFoundException($"Ink file not found at path: {inkJsonPath}");

                    await IOService.Output.DisplayDebugMessage($"[LOCAL] Reading from: {inkJsonPath}", ConsoleMessageTypes.INFO);
                    json = File.ReadAllText(inkJsonPath);
                }
            }
            catch (Exception ex)
            {
                var stackTrace = Environment.StackTrace;
                await IOService.Output.DisplayDebugMessage(
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
            await IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: RunAsync entered at {DateTime.Now}, canContinue={_story?.canContinue}, currentChoices={_story?.currentChoices?.Count}", ConsoleMessageTypes.INFO);
            if (_story is null)
                throw new InvalidOperationException("No Ink story loaded.");

            List<string> _globalSceneTags = _story.globalTags ?? new();

            while (true)
            {
                bool _canContinue = _story.canContinue;
                while (_canContinue)
                {
                    string line = _story.Continue().Trim();
                    if (line == OutputConstants.DialogueEndMarker)
                    {
                        _canContinue = true;
                        Console.WriteLine($"RunAsync encountered __END__ marker. Waiting for _hasPrintedDialogueEnd.");
                        await IOService.Output.WriteDialogueLine(line);
                        while (!_hasPrintedDialogueEnd)
                        {
                            await Task.Delay(100);
                        }
                        Console.WriteLine("_hasPrintedDialogueEnd set to true. Dialogue end confirmed.");
                        break;
                    }
                    await IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Line='{line}'", ConsoleMessageTypes.INFO);
                    if (line.TrimEnd().EndsWith(OutputConstants.DialoguePauseMarker))
                    {
                        await IOService.Output.WriteDialogueLine(line);
                        _canContinue = _story.canContinue;
                        continue;
                    }
                    if (line.StartsWith(OutputConstants.GetPlayerInputMarker))
                    {
                        await IOService.Output.DisplayDebugMessage($"InkRunner: Awaiting player input at {DateTime.Now}", ConsoleMessageTypes.INFO);
                        string prompt = "What will you say?";
                        if (line.Length > OutputConstants.GetPlayerInputMarker.Length && line[OutputConstants.GetPlayerInputMarker.Length] == ':')
                        {
                            await IOService.Output.DisplayDebugMessage("\"__GET_PLAYER_INPUT:Prompt text\" found!");
                            prompt = line.Substring(OutputConstants.GetPlayerInputMarker.Length + 1).Trim();
                            Console.WriteLine(prompt);
                            if (string.IsNullOrWhiteSpace(prompt))
                                prompt = "What will you say?";
                        }
                        string playerInput = string.Empty;
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
                            playerInput = await IOService.Input.GetPlayerInput(prompt);
                            while (string.IsNullOrWhiteSpace(playerInput))
                            {
                                playerInput = await IOService.Input.GetPlayerInput(prompt);
                            }
                        }
                        GameContext.GameState.SetLabel("player.input", playerInput);
                        await IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Player input received at {DateTime.Now}", ConsoleMessageTypes.INFO);
                        _canContinue = _story.canContinue;
                        continue;
                    }

                    List<string> rawTags = _story.currentTags ?? new();
                    List<string> lineTags = rawTags.Except(_globalSceneTags).ToList();
                    await IOService.Output.DisplayDebugMessage(string.Join(' ', lineTags) ?? string.Empty, ConsoleMessageTypes.INFO);
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string? targetTag = lineTags.FirstOrDefault(t => t.StartsWith(OutputConstants.DialogueSpeedTag));
                        if (targetTag != null)
                        {
                            if (int.TryParse(targetTag.AsSpan(5), out int ms))
                            {
                                await IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Custom speed tag found: {targetTag}. Using {ms} ms for typewriter effect.", ConsoleMessageTypes.INFO);
                                await IOService.Output.WriteDialogueLine(line, ms);
                            }
                            else
                            {
                                await IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Invalid speed tag format: {targetTag}. Using default speed of {OutputConstants.DefaultTypeSpeed} ms.", ConsoleMessageTypes.INFO);
                                await IOService.Output.WriteDialogueLine(line, OutputConstants.DefaultTypeSpeed);
                            }
                        }
                        else
                        {
                            await IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: No speed tag found. Using default speed of {OutputConstants.DefaultTypeSpeed} ms.", ConsoleMessageTypes.INFO);
                            await IOService.Output.WriteDialogueLine(line, OutputConstants.DefaultTypeSpeed);
                        }
                    }
                    _canContinue = _story.canContinue;
                }

                if (_story.currentChoices.Count > 0)
                {
                    await IOService.Output.DisplayDebugMessage($"[DEBUG] InkRunner: Enqueued choices at {DateTime.Now}", ConsoleMessageTypes.INFO);
                    for (int i = 0; i < _story.currentChoices.Count; i++)
                    {
                        await IOService.Output.WriteDialogueLine($"[{i + 1}] {_story.currentChoices[i].text}");
                    }

                    _externalChoiceAwaitCts = new CancellationTokenSource();
                    try
                    {
                        await IOService.Output.DisplayDebugMessage("External Choice Await begin.");
                        await Task.Delay(-1, _externalChoiceAwaitCts.Token);
                    }
                    catch (TaskCanceledException) { await IOService.Output.DisplayDebugMessage("External Choice Await cancelled."); }
                }

                if (_hasPrintedDialogueEnd)
                {
                    await IOService.Output.DisplayDebugMessage("Returning from RunAsync().");
                    return;
                }
            }
        }

        public async Task ResetSilentPath()
        {
            _currentSilentPath = ("", 0);
        }

        public async Task StopSilentTimer()
        {
            _silentPathCts?.Cancel();
        }

        private void InitialiseBindings()
        {
            if (_story == null) return;

            // --- Flags ---
            _story.BindExternalFunction("setFlag", (string key, bool value) => ExternalSetFlag(key, value));
            _story.BindExternalFunction("getFlag", (string key) => ExternalGetFlag(key));
            _story.BindExternalFunction("hasFlag", (string key) => ExternalHasFlag(key));
            _story.BindExternalFunction("toggleFlag", (string key) => ExternalToggleFlag(key));
            _story.BindExternalFunction("removeFlag", (string key) => ExternalRemoveFlag(key));

            // --- Counters ---
            _story.BindExternalFunction("setCounter", (string key, int value) => ExternalSetCounter(key, value));
            _story.BindExternalFunction("getCounter", (string key) => ExternalGetCounter(key));
            _story.BindExternalFunction("hasCounter", (string key) => ExternalHasCounter(key));
            _story.BindExternalFunction("incCounter", (string key, int amount) => ExternalIncCounter(key, amount));
            _story.BindExternalFunction("decCounter", (string key, int amount) => ExternalDecCounter(key, amount));
            _story.BindExternalFunction("removeCounter", (string key) => ExternalRemoveCounter(key));

            // --- Labels ---
            _story.BindExternalFunction("setLabel", (string key, string value) => ExternalSetLabel(key, value));
            _story.BindExternalFunction("getLabel", (string key) => ExternalGetLabel(key));
            _story.BindExternalFunction("hasLabel", (string key) => ExternalHasLabel(key));
            _story.BindExternalFunction("removeLabel", (string key) => ExternalRemoveLabel(key));

            // --- Variables (not previously bound) ---
            _story.BindExternalFunction("setVar", (string key, string value) => ExternalSetVar(key, value));
            _story.BindExternalFunction("getVar", (string key) => ExternalGetVar(key));

            // --- Inventory (not previously bound) ---
            _story.BindExternalFunction("playerHas", (string itemName) => ExternalPlayerHas(itemName));

            // --- Masks ---
            _story.BindExternalFunction("playerForceMask", (string maskName) => ExternalPlayerForceMask(maskName));
            _story.BindExternalFunction("playerGiveMask", (string maskName) => ExternalPlayerGiveMask(maskName));
            _story.BindExternalFunction("playerTryTakeMask", (string maskName) => ExternalPlayerTryTakeMask(maskName));
            _story.BindExternalFunction("playerWearingMask", (string maskName) => ExternalPlayerWearingMask(maskName));

            // --- Stats ---
            _story.BindExternalFunction("changePlayerStat", (string statName, int amount) => ExternalChangePlayerStat(statName, amount));
            _story.BindExternalFunction("getPlayerStat", (string statName) => ExternalGetPlayerStat(statName));

            // --- Silent Path ---
            _story.BindExternalFunction("setSilentPath", (string silentPath, int silentMs) => ExternalSetSilentPath(silentPath, silentMs));
        }

        public object ExternalSetFlag(string key, bool value) { _gameState.SetFlag(key, value); return null; }
        public object ExternalGetFlag(string key) => _gameState.TryGetFlag(key, out var value) ? value : -1; // Flag does not exist
        public object ExternalHasFlag(string key) => _gameState.HasFlag(key);
        public object ExternalToggleFlag(string key)
        {
            var result = _gameState.TryToggleFlag(key);
            if (result == null)
                return -1; // Flag does not exist
            return result;
        }
        public object ExternalRemoveFlag(string key)
        {
            if (!_gameState.HasFlag(key))
                return false;
            _gameState.RemoveFlag(key);
            return true;
        }

        public object ExternalSetCounter(string key, int value) { _gameState.SetCounter(key, value); return null; }
        public object ExternalGetCounter(string key) => _gameState.TryGetCounter(key, out var result) ? result : throw new Exception($"Counter '{key}' does not exist.");
        public object ExternalHasCounter(string key) => _gameState.HasCounter(key);
        public object ExternalIncCounter(string key, int amount)
        {
            if (!_gameState.TryIncrementCounter(key, amount))
                return false;
            return true;
        }
        public object ExternalDecCounter(string key, int amount)
        {
            if (!_gameState.TryDecrementCounter(key, amount))
                return false;
            return true;
        }
        public object ExternalRemoveCounter(string key)
        {
            return _gameState.RemoveCounter(key);
        }

        public object ExternalSetLabel(string key, string value)
        {
            _gameState.SetLabel(key, value);
            return value;
        }

        public object ExternalGetLabel(string key)
        {
            var label = _gameState.TryGetLabel(key);
            if (label == null)
                return false;
            return label;
        }
        public object ExternalHasLabel(string key) => _gameState.HasLabel(key);
        public object ExternalRemoveLabel(string key)
        {
            if (!_gameState.HasLabel(key))
                throw new Exception($"Cannot remove non-existent label '{key}'.");
            _gameState.RemoveLabel(key);
            return null;
        }

        public object ExternalSetVar(string key, string value) { _gameState.SetVariable(key, value); return null; }
        public object ExternalGetVar(string key) => _gameState.TryGetVariable(key) ?? throw new Exception($"Variable '{key}' does not exist.");

        public object ExternalPlayerHas(string itemName)
        {
            Item? item = _player.Inventory.GetItem(itemName);
            return item != null;
        }
        public object ExternalPlayerGiveMask(string maskName) { _gameState.GivePlayerMask(maskName); return null; }
        public object ExternalPlayerTryTakeMask(string maskName) => _gameState.TryTakePlayerMask(maskName);
        public object ExternalPlayerWearingMask(string maskName) => _gameState.PlayerWearingMask(maskName);
        public object ExternalPlayerForceMask(string maskName)
        {
            _gameState.ForcePlayerMask(maskName);
            Console.WriteLine($"[DEBUG] Forced player to wear mask: {maskName} at {System.DateTime.Now}");
            return null;
        }

        public object ExternalChangePlayerStat(string statName, int amount)
        {
            if (!GameContext.Player.Stats.TryGetStatTypeByName(statName, out var statType))
                return false;
            PlayerStatType stat = (PlayerStatType)statType;
            GameContext.Player.Stats.ChangeBase(stat, amount);
            return true;
        }
        public object ExternalGetPlayerStat(string statName)
        {
            if (GameContext.Player.Stats.GetStat(statName, out var stat))
                return stat.Item3;
            return -1;
        }
        public object ExternalSetSilentPath(string silentPath, int silentMs)
        {
            _currentSilentPath = (silentPath, silentMs);
            return null;
        }

        private Func<bool, Task<string>>? _getPlayerInputFromUIAsync;
        public void SetPlayerInputCallback(Func<bool, Task<string>> callback) => _getPlayerInputFromUIAsync = callback;

        public event Action? OnDialogueEnd;

        /// <summary>
        /// Jump to a specific knot or stitch in the Ink story.
        /// </summary>
        public void JumpTo(string path)
        {
            if (_story != null)
            {
                _story.ChoosePathString(path);
                Run();
            }
        }

        /// <summary>
        /// Checks if a variable exists in the Ink story.
        /// </summary>
        public bool HasInkVariable(string key)
        {
            if (_story != null)
                return _story!.variablesState.Contains(key);
            return false;
        }

        /// <summary>
        /// Gets a variable value from Ink.
        /// </summary>
        public object GetInkVariable(string key)
        {
            if (_story != null)
                return _story!.variablesState[key];
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
