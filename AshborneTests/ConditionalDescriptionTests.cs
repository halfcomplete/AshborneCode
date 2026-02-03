using Xunit;
using System;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using Moq;
using AshborneTests;

namespace AshborneTests;

[Collection("AshborneTests")]
public class ConditionalDescriptionTests : IsolatedTestBase
{
    [Fact]
    public void BuilderMethodIf_AddsPredicate_WhenCalledWithValidPredicate()
    {
        // Arrange & Act
        Func<Player, GameStateManager, bool> predicate = (player, gameState) => true;
        var conditionalDescription = ConditionalDescription.StartNew().If(predicate);
        var player = new Player();
        var gameState = new GameStateManager(player);

        // Assert
        Assert.True(conditionalDescription.Predicate!.Invoke(player, gameState));
    }

    [Fact]
    public void BuilderMethodIf_ThrowsException_WhenCalledTwiceWithoutOperator()
    {
            // Arrange
            var conditionalDescription = ConditionalDescription.StartNew();
            Func<Player, GameStateManager, bool> predicate = (player, gameState) => true;
            conditionalDescription.If(predicate);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => conditionalDescription.If(predicate));
    }

    [Fact]
    public void BuilderMethodIfNot_AddsNegatedPredicate_WhenCalledWithValidPredicate()
    {
        // Arrange & Act
        Func<Player, GameStateManager, bool> predicate = (player, gameState) => true;
        var conditionalDescription = ConditionalDescription.StartNew().IfNot(predicate);
        var player = new Player();
        var gameState = new GameStateManager(player);

        // Assert
        Assert.False(conditionalDescription.Predicate!.Invoke(player, gameState));
    }

    [Fact]
    public void BuilderMethodIfNot_ThrowsException_WhenCalledTwiceWithoutOperator()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew();
        Func<Player, GameStateManager, bool> predicate = (player, gameState) => true;
        conditionalDescription.IfNot(predicate);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => conditionalDescription.IfNot(predicate));
    }

    [Fact]
    public void BuilderMethodAnd_ThrowsException_WhenCalledBeforeStartingPredicate()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => conditionalDescription.AndIf((player, gameState) => true));
    }

    [Fact]
    public void BuilderMethodOr_ThrowsException_WhenCalledBeforeStartingPredicate()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => conditionalDescription.OrIf((player, gameState) => true));
    }

    [Fact]
    public void BuilderMethodAndIfAll_CombinesPredicatesCorrectly_WhenCalledWithValidConditionalDescription()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew();
        Func<Player, GameStateManager, bool> predicate1 = (player, gameState) => true;
        Func<Player, GameStateManager, bool> predicate2 = (player, gameState) => false;
        Func<Player, GameStateManager, bool> predicate3 = (player, gameState) => true;

        // Act
        conditionalDescription = conditionalDescription.If(predicate1).AndIfAll(
            ConditionalDescription.StartNew().IfNot(predicate2).AndIf(predicate3)
        );
    
        // Assert
        Assert.True(conditionalDescription.Predicate!.Invoke(GameContext.Player, GameContext.GameState));
    }

    [Fact]
    public void BuilderMethodAndIfAll_ThrowsException_WhenCalledBeforeStartingPredicate()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew();
        var groupDescription = ConditionalDescription.StartNew().If((p, g) => true);

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() => conditionalDescription.AndIfAll(groupDescription));
    }

    [Fact]
    public void BuilderMethodAnd_CombinesPredicatesCorrectly_WhenCalledAfterAndBeforeIF()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew();
        Func<Player, GameStateManager, bool> predicate1 = (player, gameState) => true;
        Func<Player, GameStateManager, bool> predicate2 = (player, gameState) => false;

        // Act
        conditionalDescription = conditionalDescription.If(predicate1).AndIf(predicate2);

        // Assert
        Assert.False(conditionalDescription.Predicate!.Invoke(GameContext.Player, GameContext.GameState));
    }

    [Fact]
    public void BuilderMethodOr_CombinesPredicatesCorrectly_WhenCalledAfterAndBeforeIF()
    {
        // Arrange & Act
        Func<Player, GameStateManager, bool> predicate1 = (player, gameState) => false;
        Func<Player, GameStateManager, bool> predicate2 = (player, gameState) => true;
        var conditionalDescription = ConditionalDescription.StartNew().If(predicate1).OrIf(predicate2);

        // Assert
        Assert.True(conditionalDescription.Predicate!.Invoke(GameContext.Player, GameContext.GameState));
    }

    [Fact]
    public void BuilderMethodOrIfAll_AddsGroupedPredicate_WhenCalledWithValidConditionalDescription()
    {
        // Arrange
        Func<Player, GameStateManager, bool> predicate1 = (player, gameState) => false;
        Func<Player, GameStateManager, bool> predicate2 = (player, gameState) => true;

        // Act
        var conditionalDescription = ConditionalDescription.StartNew()
            .If(predicate1)
            .OrIfAll(ConditionalDescription.StartNew().If(predicate2));

        // Assert
        Assert.True(conditionalDescription.Predicate!.Invoke(GameContext.Player, GameContext.GameState));
    }

    [Fact]
    public void BuilderMethodAndIfAll_ThrowsArgumentNullException_WhenCalledWithNull()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew().If((p, g) => true);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => conditionalDescription.AndIfAll(null!));
    }

    [Fact]
    public void BuilderMethodOrIfAll_ThrowsArgumentNullException_WhenCalledWithNull()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew().If((p, g) => true);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => conditionalDescription.OrIfAll(null!));
    }

    [Fact]
    public void BuilderMethodThenShow_SetsMessageAndGetDescriptionReturnsMessage_WhenPredicateTrue()
    {
        // Arrange
        var conditionalDescription = ConditionalDescription.StartNew().If((p, g) => true).ThenShow("hello world");

        // Act
        var result = conditionalDescription.GetDescription(out var oneTime);

        // Assert
        Assert.Equal("hello world", result);
        Assert.False(oneTime);
    }

    [Fact]
    public void BuilderMethodThenShow_ThrowsArgumentException_WhenCalledWithNullOrWhiteSpace()
    {
        // Arrange
        var conditional = ConditionalDescription.StartNew().If((p, g) => true);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => conditional.ThenShow(string.Empty));
        Assert.Throws<ArgumentException>(() => conditional.ThenShow("   "));
    }

    [Fact]
    public void BuilderMethodsOnlyOnceAndEverytime_Throw_WhenCalledBeforeThenShow()
    {
        // Arrange
        var conditional = ConditionalDescription.StartNew().If((p, g) => true);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => conditional.OnlyOnce());
        Assert.Throws<InvalidOperationException>(() => conditional.Everytime());
    }

    [Fact]
    public void BuilderMethodOnlyOnce_SetsOneTimeTrue_WhenCalledAfterThenShow()
    {
        // Arrange
        var conditional = ConditionalDescription.StartNew().If((p, g) => true).ThenShow("once").OnlyOnce();

        // Act
        var result = conditional.GetDescription(out var oneTime);

        // Assert
        Assert.Equal("once", result);
        Assert.True(oneTime);
    }

    [Fact]
    public void BuilderMethodEverytime_SetsOneTimeFalse_WhenCalledAfterThenShow()
    {
        // Arrange
        var conditional = ConditionalDescription.StartNew().If((p, g) => true).ThenShow("always").Everytime();

        // Act
        var result = conditional.GetDescription(out var oneTime);

        // Assert
        Assert.Equal("always", result);
        Assert.False(oneTime);
    }

    [Fact]
    public void GetDescription_ReturnsEmptyAndOneTimeTrue_WhenMessageNotAssigned()
    {
        // Arrange
        var conditional = ConditionalDescription.StartNew();

        // Act
        var result = conditional.GetDescription(out var oneTime);

        // Assert
        Assert.Equal(string.Empty, result);
        Assert.True(oneTime);
    }

    [Fact]
    public void GetDescription_ReturnsEmptyAndOneTimeFalse_WhenPredicateFalse()
    {
        // Arrange
        var conditional = ConditionalDescription.StartNew().If((p, g) => false).ThenShow("hidden");

        // Act
        var result = conditional.GetDescription(out var oneTime);

        // Assert
        Assert.Equal(string.Empty, result);
        Assert.False(oneTime);
    }
}