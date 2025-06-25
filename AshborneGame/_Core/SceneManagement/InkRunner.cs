using Ink.Runtime;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using System.Reflection.Metadata.Ecma335;

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

        public bool IsRunning => _story != null && _story.canContinue;

        public InkRunner(GameStateManager gameState, Player player)
        {
            _gameState = gameState;
            _player = player;
        }

        /// <summary>
        /// Loads an Ink JSON story file from disk.
        /// </summary>
        public void LoadFromFile(string inkJsonPath)
        {
            if (!File.Exists(inkJsonPath))
                throw new FileNotFoundException($"Ink file not found at path: {inkJsonPath}");

            string json = File.ReadAllText(inkJsonPath);
            _story = new Story(json);

            InitialiseBindings();

            SyncGameStateToInk();
        }

        /// <summary>
        /// Runs the Ink story line by line until player input or choice is needed.
        /// </summary>
        public void Run()
        {
            if (_story == null)
                throw new InvalidOperationException("No Ink story loaded.");

            while (_story.canContinue)
            {
                string line = _story.Continue().Trim();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    IOService.Output.WriteLine(line);
                }
            }

            SyncInkToGameState();

            if (_story.currentChoices.Count > 0)
            {
                for (int i = 0; i < _story.currentChoices.Count; i++)
                {
                    IOService.Output.WriteLine($"[{i + 1}] {_story.currentChoices[i].text}");
                }

                int choice = IOService.Input.GetChoiceInput(_story.currentChoices.Count);
                _story.ChooseChoiceIndex(choice - 1);
                Run(); // Continue running after choice
            }
        }

        /// <summary>
        /// Sync GameStateManager's values into the Ink story variables.
        /// </summary>
        private void SyncGameStateToInk()
        {
            foreach (var flag in _gameState.Flags)
                _story.variablesState[flag.Key] = flag.Value;

            foreach (var counter in _gameState.Counters)
                _story.variablesState[counter.Key] = counter.Value;

            foreach (var variable in _gameState.Variables)
                _story.variablesState[variable.Key] = variable.Value;
        }

        /// <summary>
        /// Sync Ink story variables back to GameStateManager.
        /// </summary>
        private void SyncInkToGameState()
        {
            foreach (string varName in _story.variablesState)
            {
                var value = _story.variablesState[varName];

                switch (value)
                {
                    case bool b:
                        _gameState.SetFlag(varName, b);
                        break;
                    case int i:
                        _gameState.SetCounter(varName, i);
                        break;
                    default:
                        _gameState.SetVariable(varName, value);
                        break;
                }
            }
        }

        private void InitialiseBindings()
        {
            // Pacing
            _story.BindExternalFunction("pause", (int ms) => Thread.Sleep(ms));

            // --- Flags ---
            _story.BindExternalFunction("setFlag", (string key) => _gameState.SetFlag(key, true));
            _story.BindExternalFunction("setFlag", (string key, bool value) => _gameState.SetFlag(key, value));

            _story.BindExternalFunction("getFlag", (string key) =>
            {
                var result = _gameState.GetFlag(key);
                return result ?? throw new Exception($"Flag '{key}' does not exist.");
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
                var result = _gameState.TryGetCounter(key);
                return result ?? throw new Exception($"Counter '{key}' does not exist.");
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
