using AshborneGame._Core._Player;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Data.Definitions.Registries;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.QuestManagement;
using AshborneGame._Core.SaveSystem;
using AshborneGame._Core.SaveSystem.Data;
using static AshborneGame._Core.Game.Events.GameEvents.Player;

namespace AshborneGame._Core.Game
{
    public class GameEngine
    {
        private bool _isRunning;
        private bool _dialogueRunning { get; set; }
        public DialogueService DialogueService { get; private set; }
        public InkRunner InkRunner { get; private set; }

        private string _startingActNo = "Act1";
        private string _startingSceneNo = "Scene1";
        private string _startingSceneSection = "Intro_Dialogue";

        public GameEngine(IInputHandler input, IOutputHandler output, AppEnvironment appEnvironment, string? saveJson = null)
        {
            IOService.Initialise(input, output);

            var definitionRegistry = new DefinitionRegistry();
            var definitionRegistrationService = new DefinitionRegistrationService();
            definitionRegistrationService.RegisterAllDefinitions(definitionRegistry);
            var instanceRegistry = new InstanceRegistry();
            var locationRegistry = new LocationRegistry();
            
            Player player = new Player("Hero");
            var timeTracker = new TimeTracker();
            var gameState = new GameStateManager(player, timeTracker);
            gameState.SetCounter(StateKeys.Counters.Player.CurrentActNo, 0);
            var inkRunner = new InkRunner(gameState, player, appEnvironment);
            DialogueService = new DialogueService(inkRunner);
            InkRunner = inkRunner;
            var questTracker = new QuestTracker();
            var ambientTimeManager = new AmbientTimeManager();
            var movementService = new MovementService(locationRegistry);
            
            GameContext.Initialise(player, gameState, DialogueService, inkRunner, this, timeTracker, ambientTimeManager, movementService, definitionRegistry, instanceRegistry, locationRegistry);

            // TODO: initialise masks through the new definition system
            
            DialogueService.DialogueStart += async () =>
            {
                _dialogueRunning = true;
            };
            DialogueService.OnDialogueComplete += async () =>
            {
                _dialogueRunning = false;
            };
            
            if (saveJson == null)
            {
                InitialiseGameWorld(player, gameState, locationRegistry, definitionRegistry, GameContext.BOCSFactory);
            }
            else
            {
                SaveManager.LoadGame(saveJson, player, gameState, inkRunner, instanceRegistry, locationRegistry, DialogueService);
            }
        }

        private void InitialiseGameWorld(Player player, GameStateManager gameState, ILocationRegistry locationRegistry, IDefinitionRegistry definitionRegistry, BOCSFactory factory)
        {
            WorldBuilder.Initialise(locationRegistry, definitionRegistry, factory);

            var ossaneth = GameContext.BOCSFactory.Create(DefinitionIDs.Items.Masks.Ossaneth);

            gameState.Masks["Ossaneth"] = ossaneth;

            var firstLocationID = DefinitionIDs.Locations.Prologue.PrologueStart;

            if (!GameContext.LocationRegistry.TryGetLocationByDefinitionID(firstLocationID, out Location? loc) || loc == null)
            {
                throw new InvalidOperationException($"Location '{firstLocationID}' not found.");
            }

            var _firstLocation = loc;
            var _firstScene = _firstLocation.Scene ?? throw new InvalidOperationException($"Location '{_firstLocation.DefinitionID}' does not have a scene.");

            player.SetupMoveTo(_firstLocation, _firstScene, false).GetAwaiter().GetResult();

            EventBus.Publish(new RuinedSomethingPreciousEvent(0, [new(DefinitionIDs.Player, [MemoryRole.Actor]), new(DefinitionIDs.NPCs.Gardener, [MemoryRole.Target])], DefinitionIDs.Locations.OssuaryOfEyesLocs.CloisterGarden));
        }


        /// <summary>
        /// Starts a dialogue from a non-dialogue context.
        /// </summary>
        /// <param name="dialogueName">The filename of the dialogue to start, without extensions nor paths.</param>
        public async Task<bool> TryStartDialogueFromNonDialogue(string dialogueName)
        {
            if (_dialogueRunning)
            {
                await IOService.Output.DisplayDebugMessage("Cannot start a new dialogue while another dialogue is running.", Globals.Enums.ConsoleMessageTypes.WARNING);
                return false;
            }
            await IOService.Output.DisplayDebugMessage($"{dialogueName} dialogue starting.", AshborneGame._Core.Globals.Enums.ConsoleMessageTypes.INFO);
            await DialogueService.StartDialogue(dialogueName);
            return true;
        }

        public async void Start()
        {
            await StartGameLoop(GameContext.Player, GameContext.GameState);
        }

        // blazor version (NOT the console version, which uses StartGameLoop instead and is in Program.cs)
        public async Task StartGameLoopAsync()
        {
            _isRunning = true;
            
            // Initialise the game state
            GameContext.TimeTracker.StartTickLoop();

            
            // Description is now handled inside SetupMoveTo
        }

        public async Task StartNewGameAsnyc()
        {
            //await DialogueService.StartDialogue($"{_startingActNo}_{_startingSceneNo}_{_startingSceneSection}");

            Console.WriteLine("[GameEngine] Initial intro dialogue completed.");

            GameContext.LocationRegistry.TryGetLocationByDefinitionID(DefinitionIDs.Locations.OssuaryOfEyesLocs.WakingChamber, out var location);

            await GameContext.Player.SetupMoveTo(location, location.Scene, true);
            GameContext.GameState.SetCounter(StateKeys.Counters.Player.CurrentActNo, 1);
            await StartGameLoopAsync();
        }
        
        // console version (NOT the Blazor version, which uses ReceiveCommand instead and is in Home.razor.cs)
        public async Task StartGameLoop(Player player, GameStateManager gameState)
        {
            await DialogueService.StartDialogue($"{_startingActNo}_{_startingSceneNo}_{_startingSceneSection}");

            await DialogueService.StartDialogue($"{_startingActNo}_{_startingSceneNo}_Ossaneth_Domain_Intro");

            GameContext.LocationRegistry.TryGetLocationByDefinitionID(DefinitionIDs.Locations.Dreamspace.EyePlatform, out var location);

            await player.SetupMoveTo(location, location.Scene, true);

            GameContext.TimeTracker.StartTickLoop();
            _isRunning = true;
            while (_isRunning)
            {
                if (_dialogueRunning)
                {
                    continue;
                }

                string inputStr = await IOService.Input.GetPlayerInput();
                inputStr = inputStr.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(inputStr))
                {
                    await IOService.Output.DisplayFailMessage("You must enter a command.");
                    continue;
                }

                var splitInput = inputStr.Split(' ').ToList();
                var action = CommandManager.ExtractAction(splitInput, out List<string> args);

                bool isValidCommand = await CommandManager.TryExecute(action, args, player);

                while (!isValidCommand)
                {
                    await IOService.Output.DisplayFailMessage("Invalid command. Please try again or type 'help' for assistance.");


                    inputStr = await IOService.Input.GetPlayerInput();
                    inputStr = inputStr.Trim().ToLower();
                    if (string.IsNullOrWhiteSpace(inputStr))
                    {
                        continue;
                    }

                    splitInput = inputStr.Split(' ').ToList();
                    action = CommandManager.ExtractAction(splitInput, out var args2);

                    isValidCommand = await CommandManager.TryExecute(action, args2, player);
                }
            }
            await GameContext.TimeTracker.StopTickLoop();
        }

        public async void ReceiveCommand(string input)
        {
            if (!_isRunning)
            {
                await IOService.Output.DisplayFailMessage("Game is not running.");
                return;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                await IOService.Output.DisplayFailMessage("You must enter a command.");
                return;
            }

            var splitInput = input.Split(' ').ToList();
            var action = CommandManager.ExtractAction(splitInput, out List<string> args);

            await CommandManager.TryExecute(action, args, GameContext.Player);
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}