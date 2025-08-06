using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
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

        private string _startingActNo = "Act1";
        private string _startingSceneNo = "Scene1";
        private string _startingSceneSection = "Intro_Dialogue";

        private Location _firstLocation;
        private Scene _firstScene;

        public GameEngine(IInputHandler input, IOutputHandler output, AppEnvironment appEnvironment)
        {
            IOService.Initialise(input, output);

            Player player = new Player("Hero");
            var gameState = new GameStateManager(player);
            gameState.SetCounter(GameStateKeyConstants.Counters.Player.CurrentActNo, 1);
            var inkRunner = new InkRunner(gameState, player, appEnvironment);
            _dialogueService = new DialogueService(inkRunner);

            GameContext.Initialise(player, gameState, _dialogueService, inkRunner, this);
            gameState.InitialiseMasks(MaskInitialiser.InitialiseMasks());

            ((Location startingLocation, Scene startingLocationGroup), (_firstLocation, _firstScene)) = InitialiseStartingLocation(player);
            player.SetupMoveTo(startingLocation, startingLocationGroup);
            _dialogueService.DialogueStart += async () =>
            {
                _dialogueRunning = true;
            };
            _dialogueService.DialogueComplete += async () =>
            {
                _dialogueRunning = false;
            };
            
            // Subscribe to dreamspace outro event
            EventBus.Subscribe("player.dreamspace.outro.triggered", OnDreamspaceOutroTriggered);
            
            InitialiseGameWorld(player);
        }

        private void OnDreamspaceOutroTriggered(GameEvent evt)
        {
            // Start the outro dialogue
            _dialogueService.StartDialogue("Act1_Scene1_Ossaneth_Domain_Outro");
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

            await _dialogueService.StartDialogue($"{_startingActNo}_{_startingSceneNo}_{_startingSceneSection}");

            await _dialogueService.StartDialogue($"{_startingActNo}_{_startingSceneNo}_Ossaneth_Domain_Intro");

            GameContext.Player.SetupMoveTo(_firstLocation, _firstScene);
            // Description is now handled inside SetupMoveTo
        }

        private ((Location, Scene), (Location, Scene)) InitialiseStartingLocation(Player player)
        {
            Location eyePlatform = LocationFactory.CreateLocation(
                new Location(
                    new LocationIdentifier("Eye Platform"),
                    "Eye Platform"),
                new LookDescription(
                    "You glance around uneasily. The eye you stand on is unblinking and unmoving. Black clouds cover the sky, and the occasional lightning flashes are bright white against an otherwise dull and dark background.",
                    "You look around once more. Nothing changes — but are the shards sharper now?"),
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

            Location hallOfMirrors = LocationFactory.CreateLocation(
                new Location(
                    new LocationIdentifier("Hall of Mirrors"),
                    "Hall of Mirrors"),
                new LookDescription(
                    "You look around the hall. Everywhere, your reflection stares right back at you, each mirror containing an infinite universe of you's.",
                    "You look around the hall again. The mirrors remain ever so still, ever so silent."
                    ),
                new FadingDescription(
                    "You enter the Hall of Mirrors. In front of you is a long, stretching hallway that seems to go on forever, the wall, floor, and ceilings covered in mirrors. As you walk by, some reflections lag behind and others move before you. Some never blink while others walk with their eyes closed. " +
                    "You are surprised to see that the Mask that was forced on to you just before is no longer on your face — instead, it leaves blank, featureless skin. Your identity. Gone. You know you should scream, but you cannot. You are just thankful, but you do not know why.",
                    "You enter the Hall of Mirrors again. Nothing seems to have changed, but you think that the reflections are diverging further and further away from your real self.",
                    "For the fourth time, you enter the Hall of Mirrors. The reflections are increasingly clearer in some mirrors, while gone in others. For the first time, there are cracked mirrors dotted along the silver-lined hallway."),
                new SensoryDescription(),
                new AmbientDescription(
                    new Dictionary<TimeSpan, string>() { { TimeSpan.FromSeconds(12), "You stand still. Your reflections do not." } }));

            Item mirrorShard = ItemFactory.CreateMagicScroll(
                "mirror shard",
                "A small shard of a mirror from Ossaneth's Domain that seems to reflect deeper than a normal mirror.",
                "You hold the shard up to your face. You feel a strange emptiness inside you.",
                32);

            Sublocation mirrorShardSublocation = new Sublocation(hallOfMirrors, mirrorShard, new LocationIdentifier("mirror shard", new List<string> { "shard", "mirror shard", "shard of glass", "shard of mirror", "shard of a mirror" }),
                new DescriptionComposer(
                    new LookDescription("You look at the shard. It is a small piece of a broken mirror, but it seems to reflect deeper than a normal mirror. You can see your reflection, but it feels... empty.",
                        "You look at the shard again. It still feels empty, but you can't shake the feeling that it is important."),
                    new FadingDescription("You walk up to the shard of mirror. It is small and broken, but it seems to reflect deeper than a normal mirror can. Perhaps storing it for later will be beneficial.\n",
                        "You walk up to the shard again. It still feels empty, but you can't shake the feeling that it is important.",
                        "You go to the shard again. It still feels empty, but you can't shake the feeling that it is important."),
                    new SensoryDescription("The shard is cold to the touch.", "It is eerily quiet here.")),
                "mirror shard",
                OutputConstants.ShortenedAtMiddle,
                "a shard of a mirror, lying on the floor");

            hallOfMirrors.AddCustomCommand(new List<string> { "pick up shard", "pick shard", "take shard", "take shard of mirror", "pick up mirror shard", "take mirror shard", "pick up shard of mirror" },
                () => $"You cannot do that from here. Try moving closer to the shard.",
                () => { });

            mirrorShardSublocation.AddCustomCommand(new List<string> { "take it", "pick it up", "pick up shard", "pick shard", "take shard", "take shard of mirror", "pick up mirror shard", "take mirror shard", "pick up shard of mirror" },
                () => $"\nYou pick up the {mirrorShard.Name}. It is sharp and cold against your hand. Maybe it could be useful later.",
                () =>
                {
                    GameContext.Player.Inventory.AddItem(mirrorShard, 1);
                    hallOfMirrors.RemoveSublocation(mirrorShardSublocation);
                });

            hallOfMirrors.AddSublocation(mirrorShardSublocation);

            Location chamberOfCycles = LocationFactory.CreateLocation(
                new Location(
                    new LocationIdentifier("Chamber of Cycles"),
                    "Chamber of Cycles"),
                new LookDescription(
                    "You look around the chamber, inspecting the clocks further. The clocks are broken — a tick forward is immediately followed by a jump back. Time isn't moving forward. Not anymore.",
                    "You look around the chamber again. Still, the clocks are broken. Time is frozen. Tick. Tock. Tick. Tock."),
                new FadingDescription(
                    "You enter the Chamber of Cycles. It is a circular room covered top to bottom with clocks of every century, each ticking to a separate rhythm. In the centre lies a single, looming hourglass. It's almost finished. The clocks talk to you. Tick. Tock. Tick. Tock.",
                    "You are back in the Chamber of Cycles. The clocks seem to be ticking faster now, with just a little sand left in the massive hourglass. ",
                    "For the fourth time, you enter the Chamber of Cycles. Some clocks are shattered now. The hourglass is cracking. Is time... breaking?"),
                new SensoryDescription(
                    "Dust covers every inch of the chamber.",
                    "The ticks form a strange, horrible cacophony. Tick. Tick. Tock. Tock."
                    ),
                new AmbientDescription(new Dictionary<TimeSpan, string>() { { TimeSpan.FromSeconds(10), "Tick. Tock. Tick. Tock." } }));

            Location templeOfTheBound = LocationFactory.CreateLocation(
                new Location(
                    new LocationIdentifier("Temple of the Bound One"),
                    "Temple of the Bound"),
                new LookDescription(
                    "You look around the temple. The strange, alien symbols covering the walls are each etched deep into rough bricks the size of a person. You cannot read them, but as your eyes jump from one symbol to another, they glow ever so slightly against the dark. The characters are beautiful, each line a river of the unknown.",
                    "You look around the temple again. The prisoner shuffles slightly.",
                    "Turning your head, you peel your eyes for anything that has changed. Nothing has."),
                new FadingDescription(
                    "You walk inside the Temple of the Bound to a massive, dark area. Unknown inscriptions on the wall glow faintly. The air is damp.",
                    "You return to the Temple of the Bound. One of the candles in the middle have gone out.",
                    "You go back to the Temple of the Bound. The symbols on the wall are stronger, denser, more... powerful. You feel your hair bristle. You shouldn't be here."),
                new SensoryDescription(
                    "The vaulted ceiling is covered in hanging moss and leaves.",
                    "Whispers of magic and memory dance in the air. Is that coming from the runes?"));

            NPC boundOne = NPCFactory.CreateTalkableNPC("Bound One", "It's a sad, chained man with messy, overgrown hair and bloodred eyes.", "Act1_Scene1_Prisoner_Dialogue", 
                new List<string> { "the prisoner", "prisoner", "the chained prisoner", "chained prisoner" });
            templeOfTheBound.AddSublocation(new Sublocation(templeOfTheBound, boundOne, new LocationIdentifier("circle of candles", new List<string>() { "circle", "candles", "candle circle" }), new DescriptionComposer(
                new LookDescription("You look closer at the circle. It's made up of 12 white wax candles placed precisely around the prisoner. Beneath the candles and the prisoner is a scrawled red 12-pointed star, with the centre circle formed by the intersecting lines slightly aflame. Shadows dance on the floor. You shiver. The fire is not warm.",
                "You look at the circle again. The shadows seem to have grown larger and sharper. Or is the eerie holiness of the temple finally getting to your mind."),
                new FadingDescription("You walk up to the circle of candles. The prisoner is a sorry sight—crippled, ragged, and chained to the ground, they are surrounded by the circle of candles, as though a ritual will take place soon. Or has it already finished? You wouldn't be surprised if that were the case—but maybe you should talk to them.",
                "You walk up to the circle again. The candles dim ever so slightly as your movement shakes the otherwise still air.",
                "You go to the circle again. The candles and shadows tire of your presence. And perhaps the prisoner does too."),
                new SensoryDescription("The flames are red and wild — almost as wild as the prisoner.",
                "It is quiet save for the soft crackling of the candles.")), "circle of candles", OutputConstants.ShortenedCentre, "a circle of dimly lit candles surrounding a chained prisoner"));
            LocationFactory.AddMutualExits(eyePlatform, hallOfMirrors, DirectionConstants.South);
            LocationFactory.AddMutualExits(eyePlatform, chamberOfCycles, DirectionConstants.West);
            LocationFactory.AddMutualExits(eyePlatform, templeOfTheBound, DirectionConstants.North);

            var ossanethDomain = LocationFactory.CreateScene("Ossaneth's Domain", "Ossaneth's Domain", new List<Location> { eyePlatform, hallOfMirrors });

            var prologueLocation = LocationFactory.CreateLocation(new Location(), new LookDescription(), new FadingDescription(), new SensoryDescription());
            var prologue = new Scene("Prologue", "Prologue");

            prologue.AddLocation(prologueLocation);
            GameContext.GameState.SetCounter(GameStateKeyConstants.Counters.Player.CurrentSceneNo, 0);

            return ((prologueLocation, prologue), (eyePlatform, ossanethDomain));
        }


        private void InitialiseGameWorld(Player player)
        {
            //var torch = ItemFactory.CreateLightSourceEquipment("torch", "A small torch that lights up the area.", new List<string> { "hand", "offhand" }, ItemQualities.None, -1, 32);
            //var damagePotion = ItemFactory.CreateHealthPotion("damage potion", -20);
            //var scroll = ItemFactory.CreateMagicScroll("mysterious scroll", "A scroll with ancient runes", "You read the scroll and feel a surge of power");

            //player.Inventory.AddItem(torch, 1);

            //player.Inventory.AddItem(ItemFactory.CreateHealthPotion("health potion", 20), 5);
            //player.Inventory.AddItem(damagePotion, 5);
            //player.Inventory.AddItem(scroll, 1);

            IOService.Output.DisplayDebugMessage("Game world initialised.");
        }

        public async Task StartGameLoop(Player player, GameStateManager gameState)
        {
            await _dialogueService.StartDialogue($"{_startingActNo}_{_startingSceneNo}_{_startingSceneSection}");

            await _dialogueService.StartDialogue($"{_startingActNo}_{_startingSceneNo}_Ossaneth_Domain_Intro");

            IOService.Output.WriteLine(player.CurrentLocation.GetDescription(player, gameState));

            gameState.StartTickLoop();

            _isRunning = true;
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