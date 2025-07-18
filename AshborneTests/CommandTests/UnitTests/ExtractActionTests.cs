using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using Moq;

namespace AshborneTests.CommandTests
{
    [Collection("AshborneTests")]
    public class ExtractActionTests
    {
        public static IEnumerable<object[]> DictionaryTestData =>
        new List<object[]>
        {
            new object[] { "give 3 gold coin", "give", new List<string> { "3", "gold", "coin" } },
            new object[] { "go to tower", "go to", new List<string> { "tower" } },
            new object[] { "talk to guard", "talk to", new List<string> { "guard" } },
            new object[] { $"go {DirectionConstants.North}", "go", new List<string> { DirectionConstants.North } }
        };


        Mock<GameEngine> gameEngine = new Mock<GameEngine>(new Mock<IInputHandler>().Object, new Mock<IOutputHandler>().Object);

        [Theory]
        [MemberData(nameof(DictionaryTestData))]
        public void ExtractAction_Succeeds_With_Good_Input(string input, string expectedAction, List<string> expectedArgs)
        {
            var inputList = input.Split(' ').ToList();

            var action = CommandManager.ExtractAction(inputList, out var args);

            Assert.Equal(expectedAction, action);
            Assert.Equal(expectedArgs, args);
        }
    }
}
