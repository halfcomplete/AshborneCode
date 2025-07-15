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

            (ILocation startingLocation, LocationGroup startingLocationGroup) = InitialiseStartingLocation(player);
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
            IOService.Output.WriteLine(GameContext.Player.CurrentLocation.GetFullDescription(GameContext.Player));
        }

        private (ILocation, LocationGroup) InitialiseStartingLocation(Player player)
        {

            ILocation eyePlatform = new Location(
                "the centre", 
                "On all sides, an endless ocean of black sand stretches away. Splintered glass suspend themselves in the air, each " +
                "one clearer and cleaner than the next. Shards of light slice the grey and lifeless sky. They stay still. All is still. All is silent.\nTo your north lies a mirror. To your east lies a throne." +
                " To your west lies a pedestal. To your south lies a downward slope - you cannot see what's at the bottom.", 
                "stand on", 
                "centre");

            ILocation mirrorOfIdentity = new Location("the mirror", "Your reflection stares back at you. It doesn't blink.", "standing beneath the", "mirror", "It's tall and cracked.");

            ILocation knifeOfViolence = new Location("the pedestal", "It's obsidian, with a golden stand holding up a shining silver knife. Your reflection " +
                "lies on the blade.", "standing in front of the", "pedestal");

            ILocation throneOfPower = new Location("the throne", "It's emerald-laced and empty. Nothing reflects off of it. Should you sit on it?", "in front of the", "throne");

            ILocation slope = new Location("the slope", "You descend down the sand slope, until you reach the bottom. A chained figure kneels there, his hands bound above his head and his feet stuck to the ground." +
                " The figure's head is turned down, and his ragged black hair is almost invisible against the darkness around him.", "on top of", "the slope");

            NPC chainedFigure = new NPC("Chained Prisoner", null, "Act1_Scene1_Prisoner_Dialogue");
            ILocation chainedFigureSublocation = new Sublocation((Location)slope, chainedFigure, "prisoner", "A chained prisoner", 5);
            ((Location)slope).AddSublocation((Sublocation)chainedFigureSublocation);
            var locations = new List<ILocation> { eyePlatform, mirrorOfIdentity, knifeOfViolence, throneOfPower, slope };
            LocationGroup ossanethDomain = new LocationGroup("Ossaneth's Domain", locations);

            ((Location)eyePlatform).AddExit("north", (Location)mirrorOfIdentity);
            ((Location)eyePlatform).AddExit("west", (Location)knifeOfViolence);
            ((Location)eyePlatform).AddExit("east", (Location)throneOfPower);
            ((Location)eyePlatform).AddExit("south", (Location)slope);
            ((Location)mirrorOfIdentity).AddExit("south", (Location)eyePlatform);
            ((Location)knifeOfViolence).AddExit("east", (Location)eyePlatform);
            ((Location)throneOfPower).AddExit("west", (Location)eyePlatform);
            ((Location)slope).AddExit("north", (Location)eyePlatform);

            throneOfPower.AddCustomCommand(
            new List<string> { "sit on throne", "get on throne", "sit on the throne", "get on the throne" },
                () =>
                {
                    if (!GameContext.GameState.TryGetFlag("player.actions.sat_on_throne", out var value))
                    {
                        return "You sit on the throne. It's smooth and eerily comfortable, as though it's shaping itself to your body. You feel uneasy, and get off.";
                    }
                    return "You sit once again. The throne remains silent, and so does Ossaneth.";
                },
                () =>
                {
                    GameContext.GameState.SetFlag("player.actions.sat_on_throne", true);
                    EventBus.Call(new GameEvent("player.actions.sat_on_throne", new Dictionary<string, object> { { "location", "the throne" } }));
                }
            );

            knifeOfViolence.AddCustomCommand(["touch it", "touch the knife", "touch knife"],
                () =>
                {
                    return "You reach your hand out to feel the knife. As you slide your fingeres across it, a sting of pain suddenly bursts from your hand. Your hand is bleeding. But there's no blood on the knife.";
                },
                () =>
                {
                    GameContext.GameState.SetFlag("player.actions.touched_knife", true);
                    EventBus.Call(new GameEvent("player.actions.touched_knife", new Dictionary<string, object> { { "location", "the pedestal" } }));
                }
            );
            
            mirrorOfIdentity.AddCustomCommand(["wait"],
                () =>
                {
                    return "You stand before the mirror. You blink. Your reflection doesn't.";
                }, 
            
            () =>
            {
                if (!GameContext.GameState.TryIncrementCounter("player.actions.times_waited_at_mirror"))
                {
                    GameContext.GameState.SetCounter("player.actions.times_waited_at_mirror", 1);
                }
            });
            
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
            IOService.Output.WriteLine(player.CurrentLocation.GetFullDescription(player));

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