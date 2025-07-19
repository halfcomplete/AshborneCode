using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;
using System.Threading;
using System.Runtime.InteropServices;

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

        private AmbientTimerManager? _eyePlatformAmbientManager;
        private Location? _eyePlatform;
        private bool _inputPausedForAmbient = false;

        public GameEngine(IInputHandler input, IOutputHandler output, AppEnvironment appEnvironment)
        {
            IOService.Initialise(input, output);

            Player player = new Player("Hero");
            var gameState = new GameStateManager(player);
            var inkRunner = new InkRunner(gameState, player, appEnvironment);
            _dialogueService = new DialogueService(inkRunner);

            GameContext.Initialise(player, gameState, _dialogueService, inkRunner, this);
            gameState.InitialiseMasks(MaskInitialiser.InitialiseMasks());

            (Location startingLocation, Scene startingLocationGroup) = InitialiseStartingLocation(player);
            player.SetupMoveTo(startingLocation, startingLocationGroup);

            // Find the eyePlatform location and set up the ambient manager
            _eyePlatform = startingLocation;
            if (_eyePlatform.DescriptionComposer.Ambient != null)
            {
                _eyePlatformAmbientManager = new AmbientTimerManager(_eyePlatform);
                if (OperatingSystem.IsBrowser())
                {
                    // Web/Blazor: use async output
                    _eyePlatformAmbientManager.OnAmbientDescriptionTriggeredAsync += async desc =>
                    {
                        _inputPausedForAmbient = true;
                        // Try to use WriteLineAsync if available
                        var webOutput = output as dynamic;
                        if (webOutput != null && webOutput.WriteLineAsync != null)
                        {
                            await webOutput.WriteLineAsync(desc);
                            await webOutput.WriteLineAsync(""); // Just a new line for spacing
                        }
                        else
                        {
                            // fallback to sync
                            output.WriteLine(desc);
                            output.WriteLine("");
                        }
                        // Do NOT output '> ' in web
                        _eyePlatformAmbientManager.OnAmbientDescriptionComplete();
                    };
                }
                else
                {
                    // Console: remove '> ', output ambient, then restore prompt
                    _eyePlatformAmbientManager.OnAmbientDescriptionTriggered += desc =>
                    {
                        _inputPausedForAmbient = true;
                        // Remove last two characters (the prompt)
                        try
                        {
                            int left = Console.CursorLeft;
                            int top = Console.CursorTop;
                            if (left >= 2)
                            {
                                Console.SetCursorPosition(left - 2, top);
                                Console.Write("  ");
                                Console.SetCursorPosition(left - 2, top);
                            }
                        }
                        catch { /* ignore if not in a real console */ }
                        output.WriteLine(desc);
                        output.WriteLine("");
                        output.Write("> ");
                        _eyePlatformAmbientManager.OnAmbientDescriptionComplete();
                    };
                }
                _eyePlatformAmbientManager.OnInputPaused += () => { _inputPausedForAmbient = true; };
                _eyePlatformAmbientManager.OnInputResumed += () => { _inputPausedForAmbient = false; };
            }
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
            
            // Initialise the game state
            GameContext.GameState.StartTickLoop();

            await _dialogueService.StartDialogue($"{_startingAct}_{_startingScene}_{_startingSceneSection}");

            await _dialogueService.StartDialogue($"{_startingAct}_{_startingScene}_Ossaneth_Domain_Intro");

            // Display initial location description
            ILocation location = GameContext.Player.CurrentLocation;
            IOService.Output.WriteLine(location.GetDescription(GameContext.Player, GameContext.GameState));
        }

        private (Location, Scene) InitialiseStartingLocation(Player player)
        {
            Location eyePlatform = LocationFactory.CreateLocation(
                new Location(
                    new LocationIdentifier("Eye Platform"),
                    "Eye Platform"),
                "You glance around uneasily. The eye you stand on is unblinking and unmoving. Black clouds cover the sky, and the occasional lightning flashes are bright white against an otherwise dull and dark background.",
                new FadingDescription(
                    "You feel sick and disoriented. It takes you a few moments to stabilise. Glancing around, you notice that you're standing on an eye-shaped platform overlooking a vast, swirling abyss. The air is thick with an otherworldly energy as mirrors and shards of glass spin wildly around you.",
                    "You are back on the platform. The eye beneath seems stronger now, the pupil having enlarged, as though it wants to see more. The abyss feels darker, heavier.",
                    "For the fourth time, you stand overlooking the mess of glass and mirrors. You almost grow tired of it. The vortex is at its strongest now. The void is at its darkest, deepest, and the mirrors reflect your ragged face. It is unrecognisable now.",
                    "You are once again on the eye platform. It remains unchanged. The vortex belows continues swirling, and the eye continues staring."),
                new SensoryDescription(
                    "The platform beneath is an alien stone, black and white patterns etched into every part of the eye.",
                    "It's eerily quiet despite the chaos above and below. As though the eye is remembering, and commanding everything to be silent."),
                new AmbientDescription(new Dictionary<TimeSpan, string>() { { TimeSpan.FromSeconds(5), "The glass keeps on spinning around you. The eye does not blink." } }),
                ConditionalDescription.Create()
                .When((player, gameState) =>
                {
                    if (gameState.TryGetFlag(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedHallwayOfMirrors, out bool value) &&
                    player.CurrentLocation.VisitCount == 1 || player.CurrentLocation.VisitCount == 2 || player.CurrentLocation.VisitCount == 4)
                    {
                        return value;
                    }
                    return false;
                })
                .Show("And the mirrors reflect even deeper now, each questioning your very identity.")
                .Once()
                .When((player, gameState) =>
                {
                    if (gameState.TryGetFlag(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedHallwayOfMirrors, out bool value) &&
                    player.CurrentLocation.VisitCount == 3 || player.CurrentLocation.VisitCount > 4)
                    {
                        return value;
                    }
                    return false;
                })
                .Show("However, the mirrors seem to reflect even deeper into you now, each questioning your very identity.")
                .Once()
                .When((player, gameState) =>
                {
                    bool visitedTemple = gameState.TryGetFlag(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound, out bool v1);
                    bool talkedToBound = gameState.TryGetFlag(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_TalkedToBoundOne, out bool v2);
                    int visits = player.CurrentLocation.VisitCount;

                    if (visitedTemple &&
                        talkedToBound &&
                        (visits == 1 || visits == 2 || visits == 4))
                    {
                        return v1 && v2;
                    }
                    return false;
                })
                .Show("The swirl almost reminds you of the Bound One — chaotic, unnerving and unpredictable. You shiver. Maybe it's best not to think about him.")
                .Once()
                .When((player, gameState) =>
                {
                    bool visitedTemple = gameState.TryGetFlag(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound, out bool v1);
                    bool talkedToBound = gameState.TryGetFlag(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_TalkedToBoundOne, out bool v2);
                    int visits = player.CurrentLocation.VisitCount;

                    if (visitedTemple && talkedToBound && (visits == 3 || visits > 4))
                    {
                        return v1 && v2;
                    }
                    return false;
                })
                .Show("However, now the swirl almost reminds you of the Bound One — chaotic, unnerving and unpredictable. You shiver. Maybe it's best not to think about him.")
                .Once()
            );

            var locations = new List<Location> { eyePlatform };
            var ossanethDomain = new Scene("Ossaneth's Domain", "Ossaneth's Domain");
            foreach (var loc in locations) ossanethDomain.AddLocation(loc);

            var prologueLocation = LocationFactory.CreateLocation(new Location(), "This is a prologue location.", new FadingDescription(), new SensoryDescription());
            var prologue = new Scene("Prologue", "Prologue");

            prologue.AddLocation(prologueLocation);
            GameContext.GameState.SetCounter("player.current_scene_no", 1);

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

            await _dialogueService.StartDialogue($"{_startingAct}_{_startingScene}_Ossaneth_Domain_Intro");

            IOService.Output.WriteLine(player.CurrentLocation.GetDescription(player, gameState));

            if (player.CurrentLocation == _eyePlatform && _eyePlatformAmbientManager != null)
            {
                _eyePlatformAmbientManager.OnEnterEyePlatform();
            }

            gameState.StartTickLoop();
            _isRunning = true;
            while (_isRunning)
            {
                if (_dialogueRunning) continue;

                // Wait if input is paused for ambient
                while (_inputPausedForAmbient)
                {
                    Thread.Sleep(100);
                }

                string inputStr = IOService.Input.GetPlayerInput().Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(inputStr))
                {
                    IOService.Output.DisplayFailMessage("You must enter a command.");
                    continue;
                }

                // If on eyePlatform, reset ambient timers
                if (player.CurrentLocation == _eyePlatform && _eyePlatformAmbientManager != null)
                {
                    _eyePlatformAmbientManager.OnPlayerCommandInput();
                }

                var splitInput = inputStr.Split(' ').ToList();
                var action = CommandManager.ExtractAction(splitInput, out List<string> args);

                bool isValidCommand = CommandManager.TryExecute(action, args, player);

                while (!isValidCommand)
                {
                    IOService.Output.DisplayFailMessage("Invalid command. Please try again or type 'help' for assistance.");

                    // Wait if input is paused for ambient
                    while (_inputPausedForAmbient)
                    {
                        Thread.Sleep(100);
                    }

                    inputStr = IOService.Input.GetPlayerInput().Trim();
                    if (string.IsNullOrWhiteSpace(inputStr)) continue;

                    // If on eyePlatform, reset ambient timers
                    if (player.CurrentLocation == _eyePlatform && _eyePlatformAmbientManager != null)
                    {
                        _eyePlatformAmbientManager.OnPlayerCommandInput();
                    }

                    splitInput = inputStr.Split(' ').ToList();
                    action = CommandManager.ExtractAction(splitInput, out var args2);

                    isValidCommand = CommandManager.TryExecute(action, args2, player);
                }

                // If the player moved to a new location, handle ambient manager
                if (player.CurrentLocation != _eyePlatform && _eyePlatformAmbientManager != null)
                {
                    _eyePlatformAmbientManager.OnExitEyePlatform();
                }
                else if (player.CurrentLocation == _eyePlatform && _eyePlatformAmbientManager != null)
                {
                    _eyePlatformAmbientManager.OnEnterEyePlatform();
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