using Ink.Runtime;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using System.Reflection.Metadata.Ecma335;
using AshborneGame._Core.Globals.Enums;
using System.IO;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Handles loading, running, and syncing Ink stories with the Ashborne engine.
    /// Syncs variables and flags with GameStateManager.
    /// </summary>
    public class InkRunner
    {
        private Story _story;
        private Player _player;
        private readonly GameStateManager _gameState;
        private readonly int _defaultWait = 80;

        public bool IsRunning => _story != null && _story.canContinue;

        public InkRunner(GameStateManager gameState, Player player)
        {
            _gameState = gameState;
            _player = player;
        }

        /// <summary>
        /// Loads an Ink JSON story file from disk or HTTP.
        /// </summary>
        public async Task LoadFromFileAsync(string inkJsonPath)
        {
            string json;
            
            // Check if this is a web path (starts with /)
            if (!inkJsonPath.StartsWith("http") && (inkJsonPath.StartsWith("/") || inkJsonPath.StartsWith("Scripts")))
            {
                using var httpClient = new HttpClient();
                
                // Use relative path for GitHub Pages compatibility
                string fullUrl = inkJsonPath.TrimStart('/');
                // Append cache-busting timestamp
                string ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                if (fullUrl.Contains("?"))
                    fullUrl += "&ts=" + ts;
                else
                    fullUrl += "?ts=" + ts;
                IOService.Output.DisplayDebugMessage($"Trying to fetch: {fullUrl}", ConsoleMessageTypes.INFO);
                json = await httpClient.GetStringAsync(fullUrl);
            }
            else
            {
                // Console context - load from file system
                if (!File.Exists(inkJsonPath))
                    throw new FileNotFoundException($"Ink file not found at path: {inkJsonPath}");

                json = File.ReadAllText(inkJsonPath);
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
            if (_story == null)
                throw new InvalidOperationException("No Ink story loaded.");

            List<string> _globalSceneTags = _story.globalTags ?? new();

            while (_story.canContinue)
            {
                string line = _story.Continue().Trim();
                List<string> rawTags = _story.currentTags ?? new();
                List<string> lineTags = rawTags.Except(_globalSceneTags).ToList();
                IOService.Output.DisplayDebugMessage(line, Globals.Enums.ConsoleMessageTypes.INFO);
                IOService.Output.DisplayDebugMessage(string.Join(' ', lineTags) ?? string.Empty, Globals.Enums.ConsoleMessageTypes.INFO);
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string? targetTag = lineTags.FirstOrDefault(t => t.StartsWith("slow:"));
                    if (targetTag != null)
                    {
                        if (int.TryParse(targetTag.AsSpan(5), out int ms))
                        {
                            IOService.Output.WriteLine(line, ms);
                        }
                        else
                        {
                            IOService.Output.WriteLine(line, _defaultWait);
                        }
                    }
                    else
                    {
                        IOService.Output.WriteLine(line, _defaultWait);
                    }
                }
            }

            if (_story.currentChoices.Count > 0)
            {
                for (int i = 0; i < _story.currentChoices.Count; i++)
                {
                    IOService.Output.WriteLine($"[{i + 1}] {_story.currentChoices[i].text}");
                }

                int choice = await IOService.Input.GetChoiceInputAsync(_story.currentChoices.Count);
                _story.ChooseChoiceIndex(choice - 1);
                await RunAsync(); // Continue running after choice
            }
        }

        private void InitialiseBindings()
        {
            // Pacing
            _story.BindExternalFunction("pause", (int ms) => Thread.Sleep(ms));

            // --- Flags ---
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

            // --- Counters ---
            _story.BindExternalFunction("setCounter", (string key, int value) => _gameState.SetCounter(key, value));

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

            _story.BindExternalFunction("getPlayerInput", IOService.Input.GetPlayerInput);

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
        }


        /// <summary>
        /// Jump to a specific knot or stitch in the Ink story.
        /// </summary>
        public void JumpTo(string path)
        {
            _story.ChoosePathString(path);
            Run();
        }

        /// <summary>
        /// Checks if a variable exists in the Ink story.
        /// </summary>
        public bool HasInkVariable(string key)
        {
            return _story.variablesState.Contains(key);
        }

        /// <summary>
        /// Gets a variable value from Ink.
        /// </summary>
        public object GetInkVariable(string key)
        {
            return _story.variablesState[key];
        }
    }
}
