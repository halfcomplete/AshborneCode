using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;

namespace AshborneGame._Core.Game
{
    public class GameEngine
    {
        private bool _isRunning;
        private bool _dialogueRunning { get; set; }
        private DialogueService _dialogueService;

        private string _startingAct = "Act1";
        private string _startingScene = "Scene1";
        private string _startingSceneSection = "Intro_Dialogue";

        public GameEngine(IInputHandler input, IOutputHandler output, AppEnvironment appEnvironment)
        {
            IOService.Initialise(input, output);

            Player player = new Player("Hero");
            var gameState = new GameStateManager(player);
            var inkRunner = new InkRunner(gameState, player, appEnvironment);
            _dialogueService = new DialogueService(inkRunner);

            GameContext.Initialise(player, gameState, _dialogueService, this);
            gameState.InitialiseMasks(MaskInitialiser.InitialiseMasks());

            (Location startingLocation, LocationGroup startingLocationGroup) = InitialiseStartingLocation(player);
            player.SetupMoveTo(startingLocation, startingLocationGroup);
            _dialogueService.DialogueStart += async () =>
            {
                _dialogueRunning = true;
            };
            _dialogueService.DialogueComplete += async () =>
            {
                _dialogueRunning = false;
            };
            InitialiseGameWorld(player);
        }

        public void Start()
        {
            StartGameLoop(GameContext.Player, GameContext.GameState);
        }

        public async Task StartGameLoopAsync()
        {
            _isRunning = true;
            
            // Initialize the game state
            GameContext.GameState.StartTickLoop();

            await _dialogueService.StartDialogue($"{_startingAct}_{_startingScene}_{_startingSceneSection}");

            await _dialogueService.StartDialogue($"{_startingAct}_{_startingScene}_Ossaneth_Domain_Intro");

            // Display initial location description
            ILocation location = (ILocation)GameContext.Player.CurrentLocation;
            IOService.Output.WriteLine(location.GetDescription(GameContext.Player, GameContext.GameState));
        }

        private (Location, LocationGroup) InitialiseStartingLocation(Player player)
        {
            var eyePlatformDesc = new LocationDescriptor("the centre", "the", "stand on");
            var eyePlatformNarr = new LocationNarrativeProfile { FirstTimeDescription = "On all sides, an endless ocean of black sand stretches away..." };
            var eyePlatform = new Location(eyePlatformDesc, eyePlatformNarr, System.Guid.NewGuid().ToString());

            var mirrorDesc = new LocationDescriptor("the mirror", "the", "standing beneath the");
            var mirrorNarr = new LocationNarrativeProfile { FirstTimeDescription = "Your reflection stares back at you. It doesn't blink. It's tall and cracked." };
            var mirrorOfIdentity = new Location(mirrorDesc, mirrorNarr, System.Guid.NewGuid().ToString());

            var knifeDesc = new LocationDescriptor("the pedestal", "the", "standing in front of the");
            var knifeNarr = new LocationNarrativeProfile { FirstTimeDescription = "It's obsidian, with a golden stand holding up a shining silver knife. Your reflection lies on the blade." };
            var knifeOfViolence = new Location(knifeDesc, knifeNarr, System.Guid.NewGuid().ToString());

            var throneDesc = new LocationDescriptor("the throne", "the", "in front of the");
            var throneNarr = new LocationNarrativeProfile { FirstTimeDescription = "It's emerald-laced and empty. Nothing reflects off of it. Should you sit on it?" };
            var throneOfPower = new Location(throneDesc, throneNarr, System.Guid.NewGuid().ToString());

            var slopeDesc = new LocationDescriptor("the slope", "the", "on top of");
            var slopeNarr = new LocationNarrativeProfile { FirstTimeDescription = "You descend down the sand slope, until you reach the bottom. A chained figure kneels there..." };
            var slope = new Location(slopeDesc, slopeNarr, System.Guid.NewGuid().ToString());

            // Example NPC as GameObject for sublocation
            var chainedFigure = new GameObject("Chained Prisoner", "A chained prisoner");
            var prisonerDesc = new LocationDescriptor("prisoner");
            var prisonerNarr = new LocationNarrativeProfile { FirstTimeDescription = "A chained prisoner" };
            var chainedFigureSublocation = new Sublocation(slope, chainedFigure, prisonerDesc, prisonerNarr, System.Guid.NewGuid().ToString());
            slope.AddSublocation(chainedFigureSublocation);

            // Example custom command for a location
            eyePlatform.AddCustomCommand(new List<string> { "pray" },
                () => "You kneel and pray. The silence is overwhelming.",
                () => { /* effect logic here */ });

            // Example custom command for a sublocation
            chainedFigureSublocation.AddCustomCommand(new List<string> { "free prisoner" },
                () => "You attempt to free the prisoner, but the chains are too strong.",
                () => { /* effect logic here */ });

            // Add exits to the locations
            eyePlatform.Exits.Add("north", mirrorOfIdentity);
            eyePlatform.Exits.Add("east", knifeOfViolence);
            eyePlatform.Exits.Add("west", throneOfPower);
            eyePlatform.Exits.Add("south", slope);

            mirrorOfIdentity.Exits.Add("south", eyePlatform);
            knifeOfViolence.Exits.Add("west", eyePlatform);
            throneOfPower.Exits.Add("east", eyePlatform);
            slope.Exits.Add("north", eyePlatform);

            var locations = new List<Location> { eyePlatform, mirrorOfIdentity, knifeOfViolence, throneOfPower, slope };
            var ossanethDomain = new LocationGroup("Ossaneth's Domain", "Ossaneth's Domain");
            foreach (var loc in locations) ossanethDomain.AddLocation(loc);

            return (eyePlatform, ossanethDomain);
        }


        private void InitialiseGameWorld(Player player)
        {
            var torch = ItemFactory.CreateLightSourceEquipment("torch", "A small torch that lights up the area.", new List<string> { "hand", "offhand" }, ItemQualities.None, -1, 32);
            var damagePotion = ItemFactory.CreateHealthPotion("damage potion", -20);
            var scroll = ItemFactory.CreateMagicScroll("mysterious scroll", "A scroll with ancient runes", "You read the scroll and feel a surge of power");

            player.Inventory.AddItem(torch, 1);

            player.Inventory.AddItem(ItemFactory.CreateHealthPotion("health potion", 20), 5);
            player.Inventory.AddItem(damagePotion, 5);
            player.Inventory.AddItem(scroll, 1);

            IOService.Output.DisplayDebugMessage("Game world initialised.");
        }

        public async Task StartGameLoop(Player player, GameStateManager gameState)
        {
            await _dialogueService.StartDialogue($"{_startingAct}_{_startingScene}_{_startingSceneSection}");

            _isRunning = true;

            await _dialogueService.StartDialogue($"{_startingAct}_{_startingScene}_Ossaneth_Domain_Intro");
            IOService.Output.WriteLine(player.CurrentLocation.GetDescription(player, gameState));

            gameState.StartTickLoop();
            while (_isRunning)
            {
                if (_dialogueRunning) continue;

                string input = IOService.Input.GetPlayerInput().Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(input))
                {
                    IOService.Output.DisplayFailMessage("You must enter a command.");
                    continue;
                }

                var splitInput = input.Split(' ').ToList();
                var action = CommandManager.ExtractAction(splitInput, out List<string> args);

                bool isValidCommand = CommandManager.TryExecute(action, args, player);

                while (!isValidCommand)
                {
                    IOService.Output.DisplayFailMessage("Invalid command. Please try again or type 'help' for assistance.");

                    input = IOService.Input.GetPlayerInput().Trim();
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    splitInput = input.Split(' ').ToList();
                    action = CommandManager.ExtractAction(splitInput, out var args2);

                    isValidCommand = CommandManager.TryExecute(action, args2, player);
                }
            }
            gameState.StopTickLoop();
        }

        public void ReceiveCommand(string input)
        {
            if (!_isRunning)
            {
                IOService.Output.DisplayFailMessage("Game is not running.");
                return;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                IOService.Output.DisplayFailMessage("You must enter a command.");
                return;
            }

            var splitInput = input.Split(' ').ToList();
            var action = CommandManager.ExtractAction(splitInput, out List<string> args);

            bool isValidCommand = CommandManager.TryExecute(action, args, GameContext.Player);

            if (!isValidCommand)
            {
                IOService.Output.DisplayFailMessage("Invalid command. Please try again or type 'help' for assistance.");
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}