using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;

namespace AshborneGame._Core.Game
{
    public class GameEngine
    {
        private bool _isRunning;
        private DialogueService _dialogueService;

        private string _startingAct = "Act1";
        private string _startingScene = "Scene1";
        private string _startingSceneSection = "Intro_Dialogue";

        public GameEngine(IInputHandler input, IOutputHandler output)
        {
            IOService.Initialise(input, output);

            Player player = new Player("Hero");

            var gameState = new GameStateManager(player);
            var inkRunner = new InkRunner(gameState, player);
            _dialogueService = new DialogueService(inkRunner);
            GameContext.Initialise(player, gameState);

            Location startingLocation = InitialiseStartingLocation(player);
            player.SetupMoveTo(startingLocation);

            InitialiseGameWorld(player);

            IOService.Output.DisplayDebugMessage("Game engine initialised successfully.");
            IOService.Output.DisplayDebugMessage("Starting game engine...");

            StartGameLoop(player);
        }

        private Location InitialiseStartingLocation(Player player)
        {
            //Location dreamVoid = new Location("centre", "Splintered mirrors are suspended in the air, but" +
                "they reflect nothing. Shards of light slice the grey and lifeless sky. They stay still. All is still. All is silent.", "at the", "centre", "It's an endless ocean of black water frozen in time");

            //Location mirrorOfIdentity = new Location("mirror", " Your reflection stares back at you. It doesn't blink.", "standing beneath the", "mirror", "It's tall and cracked.");

            //Location knifeOfViolence = new Location("pedestal holding a knife", "It's obsidian, with a golden stand holding up a shining silver knife. Your reflection " +
                "lies on the blade.", "standing in front of the", "pedestal");

            //Location throneOfPower = new Location("emerald-laced throne ", "It's empty. Should you sit on it?", "in front of the", "throne");

            dreamVoid.AddExit("north", mirrorOfIdentity);
            dreamVoid.AddExit("west", knifeOfViolence);
            dreamVoid.AddExit("east", throneOfPower);
            mirrorOfIdentity.AddExit("south", dreamVoid);
            knifeOfViolence.AddExit("east", dreamVoid);
            throneOfPower.AddExit("west", dreamVoid);
            return dreamVoid;
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

        private void StartGameLoop(Player player)
        {
            _dialogueService.StartDialogue($"D:\\C# Projects\\AshborneCode\\AshborneGame\\_Core\\Data\\Scripts\\Act1\\Scene1\\{_startingAct}_{_startingScene}_{_startingSceneSection}.json");

            _isRunning = true;

            IOService.Output.WriteLine("Welcome to *Ashborne*.");
            IOService.Output.WriteLine("Type 'help' if you are unsure what to do.\n");
            IOService.Output.WriteLine(player.CurrentLocation.GetFullDescription(player));

            while (_isRunning)
            {
                // TODO: Add a check here to make sure that there's no conversation running right now
                string input = IOService.Input.GetPlayerInput().Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(input))
                {
                    IOService.Output.WriteLine("You must enter a command.");
                    continue;
                }

                var splitInput = input.Split(' ').ToList();
                var action = ExtractAction(splitInput, out List<string> args);

                bool isValidCommand = CommandManager.TryExecute(action, args, player);

                while (!isValidCommand)
                {
                    IOService.Output.WriteLine("Invalid command. Please try again or type 'help' for assistance.");

                    input = IOService.Input.GetPlayerInput().Trim();
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    splitInput = input.Split(' ').ToList();
                    action = ExtractAction(splitInput, out var args2);

                    isValidCommand = CommandManager.TryExecute(action, args2, player);
                }
            }
        }

        private string ExtractAction(List<string> input, out List<string> args)
        {
            if (input.Count >= 2 && (input[0] == "go" || input[0] == "talk") && input[1] == "to")
            {
                var copy = new List<string>(input);
                args = new List<string>(input);
                args.RemoveRange(0, 2);
                return string.Join(' ', copy.GetRange(0, 2));
            }
            args = new List<string>(input);
            args.RemoveAt(0);
            return input[0];
        }
    }
}