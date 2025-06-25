using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Location startingLocation = InitialiseStartingLocation(player);
            player.SetupMoveTo(startingLocation);

            var session = new GameSession(player);
            var gameState = new GameStateManager(player);
            var inkRunner = new InkRunner(gameState, player);
            _dialogueService = new DialogueService(inkRunner);

            InitialiseGameWorld(player);

            IOService.Output.DisplayDebugMessage("Game engine initialised successfully.");
            IOService.Output.DisplayDebugMessage("Starting game engine...");

            StartGameLoop(player);
        }

        private Location InitialiseStartingLocation(Player player)
        {
            var location = new Location("Dusty Armoury", "The room is dark", 5);

            var chest = GameObjectFactory.CreateChest("rusted chest", "A large, rusted chest covered in cobwebs", isLocked: true);
            var dummy = NPCFactory.CreateDummy("Training Dummy", "It's a wooden training dummy used for practice", 50);
            var guard = NPCFactory.CreateNPC("guard", "stern-looking", "Halt! Who goes there?", "I am the guard of this armoury");

            var key = ItemFactory.CreateKey("rusty key", "An old rusty key that looks like it might fit a lock.", "You turn the key and hear a click.", new List<string> { chest.ID });
            player.Inventory.AddItem(key, 1);

            location.AddSublocation(new Sublocation(location, chest, "rusted chest", "It's covered in cobwebs", 3));
            location.AddSublocation(new Sublocation(location, dummy, "training dummy", "A wooden training dummy used for practice.", 2));
            location.AddSublocation(new Sublocation(location, guard, "guard", "A stern-looking guard stands watch.", 4));

            if (chest.TryGetBehaviour<IHasInventory>(out var inventoryBehaviour))
            {
                inventoryBehaviour.Inventory.AddItem(ItemFactory.CreateWeapon("iron sword", "A basic iron sword", ItemQualities.Uncommon, 2, 20));
                inventoryBehaviour.Inventory.AddItem(ItemFactory.CreateArmour("leather cap", "A basic leather cap that protects your head.", new List<string> { "head" }, ItemQualities.Common, 30, new Dictionary<PlayerStatTypes, int> { { PlayerStatTypes.Defense, 10 } }));
                inventoryBehaviour.Inventory.AddItem(ItemFactory.CreateHealthPotion("health potion", 20), 3);
            }

            return location;
        }

        private void InitialiseGameWorld(Player player)
        {
            var torch = ItemFactory.CreateLightSourceEquipment("torch", "A small torch that lights up the area.", new List<string> { "hand", "offhand" }, ItemQualities.None, -1, 32);
            var damagePotion = ItemFactory.CreateHealthPotion("damage potion", -20);
            var scroll = ItemFactory.CreateMagicScroll("mysterious scroll", "A scroll with ancient runes", "You read the scroll and feel a surge of power");

            player.Inventory.AddItem(torch, 1);
            player.EquipItem(torch, "hand");

            player.Inventory.AddItem(ItemFactory.CreateHealthPotion("health potion", 20), 5);
            player.Inventory.AddItem(damagePotion, 5);
            player.Inventory.AddItem(scroll, 1);

            IOService.Output.DisplayDebugMessage("Game world initialised.");
        }

        private void StartGameLoop(Player player)
        {
            _dialogueService.StartDialogue($"D:\\C# Projects\\AshborneCode\\AshborneGame\\_Core\\Data\\Scripts\\Act1\\Scene1\\{_startingAct}_{_startingScene}_{_startingSceneSection}.json");

            _isRunning = false;

            while (_isRunning)
            {
                IOService.Output.WriteLine("Welcome to *Ashborne*.");
                IOService.Output.WriteLine("Type 'help' if you are unsure what to do.\n");
                IOService.Output.WriteLine(player.CurrentLocation.GetFullDescription(player));
                // TODO: Add a check here to make sure that there's no conversation running right now
                string input = IOService.Input.GetPlayerInput().Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(input))
                {
                    IOService.Output.WriteLine("You must enter a command.");
                    continue;
                }

                var splitInput = input.Split(' ').ToList();
                var action = ExtractAction(ref splitInput);
                var args = splitInput;

                bool isValidCommand = CommandManager.TryExecute(action, args, player);

                while (!isValidCommand)
                {
                    IOService.Output.WriteLine("Invalid command. Please try again or type 'help' for assistance.");

                    input = IOService.Input.GetPlayerInput().Trim();
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    splitInput = input.Split(' ').ToList();
                    action = ExtractAction(ref splitInput);
                    args = splitInput;

                    isValidCommand = CommandManager.TryExecute(action, args, player);
                }
            }
        }

        private string ExtractAction(ref List<string> input)
        {
            if (input.Count >= 2 && (input[0] == "go" || input[0] == "talk") && input[1] == "to")
            {
                var newInput = new List<string>(input);
                input.RemoveRange(0, 2);
                return newInput[0];
            }
            return input[0];
        }
    }
}