using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.QuestManagement;

namespace AshborneTests
{
    /// <summary>
    /// Unit tests for the Quest system including Quest, QuestCriteria, QuestProgressTracker, and QuestFactory.
    /// </summary>
    [Collection("AshborneTests")]
    public class QuestSystemTests : IDisposable
    {
        private readonly Player _player;
        private readonly GameStateManager _gameState;

        public QuestSystemTests()
        {
            _player = new Player();
            _gameState = new GameStateManager(_player);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Quest Tests

        [Fact]
        public void Quest_ShouldHaveInProgressStatus_WhenCreated()
        {
            // Arrange & Act
            var quest = QuestFactory.CreateQuest(
                "Test Quest",
                "A test quest",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria().If(_ => false).ThenProgressThisQuest());

            // Assert
            Assert.Equal(QuestStatus.InProgress, quest.Status);
        }

        [Fact]
        public void Quest_ShouldHaveGeneratedId_WhenCreated()
        {
            // Arrange & Act
            var quest = QuestFactory.CreateQuest(
                "Test Quest",
                "A test quest",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria().If(_ => true).ThenProgressThisQuest());

            // Assert
            Assert.NotNull(quest.ID);
            Assert.NotEmpty(quest.ID);
            Assert.Contains("quest", quest.ID);
        }

        [Fact]
        public void Quest_ShouldStoreNameAndDescription_WhenCreated()
        {
            // Arrange & Act
            var quest = QuestFactory.CreateQuest(
                "My Quest Name",
                "My Quest Description",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria().If(_ => true).ThenProgressThisQuest());

            // Assert
            Assert.Equal("My Quest Name", quest.Name);
            Assert.Equal("My Quest Description", quest.Description);
        }

        [Fact]
        public void QuestTick_ShouldChangeStatusToCompleted_WhenCompletionCriteriaMet()
        {
            // Arrange
            var criteriaEvaluated = false;
            var quest = QuestFactory.CreateQuest(
                "Completion Quest",
                "Test completion",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria()
                    .If(_ =>
                    {
                        criteriaEvaluated = true;
                        return true;
                    })
                    .ThenProgressThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert
            Assert.True(criteriaEvaluated);
            Assert.Equal(QuestStatus.Completed, quest.Status);
        }

        [Fact]
        public void QuestTick_ShouldChangeStatusToFailed_WhenFailureCriteriaMet()
        {
            // Arrange
            var quest = QuestFactory.CreateQuest(
                "Fail Quest",
                "Test failure",
                onComplete: _ => { },
                onFail: _ => { },
                new QuestCriteria().If(_ => false).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => true).ThenFailThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert
            Assert.Equal(QuestStatus.Failed, quest.Status);
        }

        [Fact]
        public void QuestTick_ShouldInvokeOnCompleteCallback_WhenCompleted()
        {
            // Arrange
            var onCompleteCalled = false;
            var quest = QuestFactory.CreateQuest(
                "Callback Quest",
                "Test callback",
                onComplete: _ => onCompleteCalled = true,
                onFail: null,
                new QuestCriteria().If(_ => true).ThenProgressThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert
            Assert.True(onCompleteCalled);
        }

        [Fact]
        public void QuestTick_ShouldInvokeOnFailCallback_WhenFailed()
        {
            // Arrange
            var onFailCalled = false;
            var quest = QuestFactory.CreateQuest(
                "Fail Callback Quest",
                "Test fail callback",
                onComplete: _ => { },
                onFail: _ => onFailCalled = true,
                new QuestCriteria().If(_ => false).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => true).ThenFailThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert
            Assert.True(onFailCalled);
        }

        [Fact]
        public void QuestTick_ShouldNotReEvaluateCriteria_WhenAlreadyCompleted()
        {
            // Arrange
            var evaluationCount = 0;
            var quest = QuestFactory.CreateQuest(
                "No Re-evaluation Quest",
                "Test",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria()
                    .If(_ =>
                    {
                        evaluationCount++;
                        return true;
                    })
                    .ThenProgressThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);
            quest.TickQuestTime(1, _gameState);
            quest.TickQuestTime(1, _gameState);

            // Assert - should only evaluate once before completing
            Assert.Equal(1, evaluationCount);
        }

        [Fact]
        public void QuestTick_ShouldCheckFailureCriteriaBeforeCompletion_WhenBothExist()
        {
            // Arrange - both criteria are true, failure should take precedence
            var quest = QuestFactory.CreateQuest(
                "Priority Quest",
                "Test priority",
                onComplete: _ => { },
                onFail: _ => { },
                new QuestCriteria().If(_ => true).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => true).ThenFailThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert - failure should be checked first
            Assert.Equal(QuestStatus.Failed, quest.Status);
        }

        #endregion

        #region QuestCriteria Tests

        [Fact]
        public void QuestCriteria_ShouldEvaluateTrue_WhenIfConditionIsTrue()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .If(_ => true)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void QuestCriteria_ShouldEvaluateFalse_WhenIfConditionIsFalse()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .If(_ => false)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void QuestCriteria_ShouldEvaluateTrue_WhenIfNotConditionIsFalse()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .IfNot(_ => false)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void QuestCriteria_ShouldEvaluateFalse_WhenIfNotConditionIsTrue()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .IfNot(_ => true)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void QuestCriteria_ShouldEvaluateTrue_WhenAllAndConditionsAreTrue()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .If(_ => true)
                .AndIf(_ => true)
                .AndIf(_ => true)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void QuestCriteria_ShouldEvaluateFalse_WhenAnyAndConditionIsFalse()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .If(_ => true)
                .AndIf(_ => true)
                .AndIf(_ => false)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void QuestCriteria_ShouldEvaluateTrue_WhenAnyOrConditionIsTrue()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .If(_ => false)
                .OrIf(_ => false)
                .OrIf(_ => true)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void QuestCriteria_ShouldEvaluateFalse_WhenAllOrConditionsAreFalse()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .If(_ => false)
                .OrIf(_ => false)
                .OrIf(_ => false)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void QuestCriteria_ShouldBeCompletionCriteria_WhenThenProgressThisQuestIsCalled()
        {
            // Arrange & Act
            var criteria = new QuestCriteria()
                .If(_ => true)
                .ThenProgressThisQuest();

            // Assert
            Assert.True(criteria.IsCompletionCriteria);
        }

        [Fact]
        public void QuestCriteria_ShouldBeFailureCriteria_WhenThenFailThisQuestIsCalled()
        {
            // Arrange & Act
            var criteria = new QuestCriteria()
                .If(_ => true)
                .ThenFailThisQuest();

            // Assert
            Assert.False(criteria.IsCompletionCriteria);
        }

        [Fact]
        public void QuestCriteria_ShouldAccessGameState_WhenEvaluating()
        {
            // Arrange
            var flagKey = new GameStateKey<bool>("test.flag");
            _gameState.SetFlag(flagKey, true);

            var criteria = new QuestCriteria()
                .If(gs => gs.TryGetFlag(flagKey, out var value) && value)
                .ThenProgressThisQuest();

            // Act
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void QuestCriteria_ShouldTrackTimePassed_WhenTicked()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .IfTimeHasPassed(5)
                .ThenProgressThisQuest();

            // Act - tick for 3 hours (should not complete)
            criteria.TickTimePassed(3);
            var resultBefore = criteria.Evaluate(_gameState, 0);

            // Act - tick for 3 more hours (should complete)
            criteria.TickTimePassed(3);
            var resultAfter = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.False(resultBefore);
            Assert.True(resultAfter);
        }

        [Fact]
        public void QuestCriteria_ShouldResetTimePassed_WhenResetCalled()
        {
            // Arrange
            var criteria = new QuestCriteria()
                .IfTimeHasPassed(5)
                .ThenProgressThisQuest();

            criteria.TickTimePassed(10);

            // Act
            criteria.ResetTimePassed();
            var result = criteria.Evaluate(_gameState, 0);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region QuestFactory Tests

        [Fact]
        public void QuestFactory_ShouldCreateQuest_WithValidCriteria()
        {
            // Arrange & Act
            var quest = QuestFactory.CreateQuest(
                "Factory Quest",
                "Created by factory",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria().If(_ => true).ThenProgressThisQuest());

            // Assert
            Assert.NotNull(quest);
            Assert.Equal("Factory Quest", quest.Name);
        }

        [Fact]
        public void QuestFactory_ShouldThrowException_WhenCriteriaHasNoFunction()
        {
            // Arrange & Act & Assert
            Assert.Throws<InvalidOperationException>(() => QuestFactory.CreateQuest(
                "Invalid Quest",
                "Should throw",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria().ThenProgressThisQuest()));
        }

        [Fact]
        public void QuestFactory_ShouldAcceptMultipleCriteria_WhenProvided()
        {
            // Arrange
            var criteriaEvaluationCount = 0;

            // Act
            var quest = QuestFactory.CreateQuest(
                "Multi Criteria Quest",
                "Multiple criteria",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria().If(_ => { criteriaEvaluationCount++; return true; }).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => { criteriaEvaluationCount++; return true; }).ThenProgressThisQuest());

            quest.TickQuestTime(1, _gameState);

            // Assert - both criteria should be evaluated
            Assert.Equal(2, criteriaEvaluationCount);
        }

        [Fact]
        public void QuestFactory_ShouldSeparateCompletionAndFailureCriteria_WhenBothProvided()
        {
            // Arrange
            var completionEvaluated = false;
            var failureEvaluated = false;

            var quest = QuestFactory.CreateQuest(
                "Separated Criteria Quest",
                "Test separation",
                onComplete: _ => { },
                onFail: _ => { },
                new QuestCriteria().If(_ => { completionEvaluated = true; return false; }).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => { failureEvaluated = true; return false; }).ThenFailThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert
            Assert.True(completionEvaluated);
            Assert.True(failureEvaluated);
            Assert.Equal(QuestStatus.InProgress, quest.Status);
        }

        #endregion

        #region QuestProgressTracker Tests

        [Fact]
        public void QuestProgressTracker_ShouldReturnTrue_WhenAllCompletionCriteriaAreMet()
        {
            // Arrange
            var criteria = new List<QuestCriteria>
            {
                new QuestCriteria().If(_ => true).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => true).ThenProgressThisQuest()
            };
            var tracker = new QuestProgressTracker(criteria);

            // Act
            var isComplete = tracker.IsQuestComplete(0, _gameState);

            // Assert
            Assert.True(isComplete);
        }

        [Fact]
        public void QuestProgressTracker_ShouldReturnFalse_WhenAnyCompletionCriteriaIsNotMet()
        {
            // Arrange
            var criteria = new List<QuestCriteria>
            {
                new QuestCriteria().If(_ => true).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => false).ThenProgressThisQuest()
            };
            var tracker = new QuestProgressTracker(criteria);

            // Act
            var isComplete = tracker.IsQuestComplete(0, _gameState);

            // Assert
            Assert.False(isComplete);
        }

        [Fact]
        public void QuestProgressTracker_ShouldReturnTrue_WhenAnyFailureCriteriaIsMet()
        {
            // Arrange
            var completionCriteria = new List<QuestCriteria>
            {
                new QuestCriteria().If(_ => false).ThenProgressThisQuest()
            };
            var failureCriteria = new List<QuestCriteria>
            {
                new QuestCriteria().If(_ => false).ThenFailThisQuest(),
                new QuestCriteria().If(_ => true).ThenFailThisQuest()
            };
            var tracker = new QuestProgressTracker(completionCriteria, failureCriteria);

            // Act
            var isFailed = tracker.IsQuestFailed(0, _gameState);

            // Assert
            Assert.True(isFailed);
        }

        [Fact]
        public void QuestProgressTracker_ShouldReturnFalse_WhenNoFailureCriteriaExists()
        {
            // Arrange
            var criteria = new List<QuestCriteria>
            {
                new QuestCriteria().If(_ => true).ThenProgressThisQuest()
            };
            var tracker = new QuestProgressTracker(criteria, null);

            // Act
            var isFailed = tracker.IsQuestFailed(0, _gameState);

            // Assert
            Assert.False(isFailed);
        }

        [Fact]
        public void QuestProgressTracker_ShouldTickAllCriteria_WhenTickCriteriaCalled()
        {
            // Arrange
            var criteria1 = new QuestCriteria().IfTimeHasPassed(5).ThenProgressThisQuest();
            var criteria2 = new QuestCriteria().IfTimeHasPassed(3).ThenProgressThisQuest();
            var tracker = new QuestProgressTracker(new List<QuestCriteria> { criteria1, criteria2 });

            // Act - tick for 4 hours
            tracker.TickCriteria(4, _gameState);

            // Assert
            Assert.False(criteria1.Evaluate(_gameState, 0)); // Needs 5 hours
            Assert.True(criteria2.Evaluate(_gameState, 0));  // Needs 3 hours
        }

        #endregion
    }

    /// <summary>
    /// Integration tests that verify the Event and Quest systems work together.
    /// </summary>
    [Collection("AshborneTests")]
    public class EventQuestIntegrationTests : IDisposable
    {
        private readonly Player _player;
        private readonly GameStateManager _gameState;
        private readonly QuestTracker _questTracker;
        private readonly TimeTracker _timeTracker = new TimeTracker(new QuestTracker());

        /// <summary>
        /// Event raised when a quest is completed.
        /// </summary>
        private sealed record TestQuestCompletedEvent(int CurrentTotalHours, string QuestId, string QuestName) : IGameEvent;

        /// <summary>
        /// Event raised when a quest fails.
        /// </summary>
        private sealed record TestQuestFailedEvent(int CurrentTotalHours, string QuestId, string QuestName) : IGameEvent;

        /// <summary>
        /// Event that triggers quest completion.
        /// </summary>
        private sealed record TestTriggerEvent(int CurrentTotalHours, string TriggerData) : IGameEvent;

        public EventQuestIntegrationTests()
        {
            EventBus.Clear();
            _player = new Player();
            _gameState = new GameStateManager(_player);
            _questTracker = new QuestTracker();
        }

        public void Dispose()
        {
            EventBus.Clear();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void QuestCompletion_ShouldPublishEvent_WhenQuestIsCompleted()
        {
            // Arrange
            TestQuestCompletedEvent? receivedEvent = null;
            EventBus.Subscribe<TestQuestCompletedEvent>(e => receivedEvent = e);

            var quest = QuestFactory.CreateQuest(
                "Event Quest",
                "Publishes event on completion",
                onComplete: _ => EventBus.Publish(new TestQuestCompletedEvent(0, "quest-1", "Event Quest")),
                onFail: null,
                new QuestCriteria().If(_ => true).ThenProgressThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("quest-1", receivedEvent.QuestId);
            Assert.Equal("Event Quest", receivedEvent.QuestName);
        }

        [Fact]
        public void QuestFailure_ShouldPublishEvent_WhenQuestFails()
        {
            // Arrange
            TestQuestFailedEvent? receivedEvent = null;
            EventBus.Subscribe<TestQuestFailedEvent>(e => receivedEvent = e);

            var quest = QuestFactory.CreateQuest(
                "Fail Event Quest",
                "Publishes event on failure",
                onComplete: _ => { },
                onFail: _ => EventBus.Publish(new TestQuestFailedEvent(0, "quest-2", "Fail Event Quest")),
                new QuestCriteria().If(_ => false).ThenProgressThisQuest(),
                new QuestCriteria().If(_ => true).ThenFailThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("quest-2", receivedEvent.QuestId);
        }

        [Fact]
        public void Event_ShouldTriggerQuestCompletionViaSateChange_WhenPublished()
        {
            // Arrange
            var flagKey = new GameStateKey<bool>("trigger.received");

            // Subscribe to event that sets a flag
            EventBus.Subscribe<TestTriggerEvent>(e =>
            {
                _gameState.SetFlag(flagKey, true);
            });

            // Create quest that completes when the flag is set
            var quest = QuestFactory.CreateQuest(
                "Triggered Quest",
                "Completes when trigger event is received",
                onComplete: _ => { },
                onFail: null,
                new QuestCriteria()
                    .If(gs => gs.TryGetFlag(flagKey, out var value) && value)
                    .ThenProgressThisQuest());

            // Assert - quest should be in progress before event
            Assert.Equal(QuestStatus.InProgress, quest.Status);
            quest.TickQuestTime(1, _gameState);
            Assert.Equal(QuestStatus.InProgress, quest.Status);

            // Act - publish the trigger event
            EventBus.Publish(new TestTriggerEvent(0, "trigger!"));

            // Now tick the quest
            quest.TickQuestTime(1, _gameState);

            // Assert - quest should now be completed
            Assert.Equal(QuestStatus.Completed, quest.Status);
        }

        [Fact]
        public void MultipleQuests_ShouldRespondToSameEvent_WhenSubscribed()
        {
            // Arrange
            var flagKey = new GameStateKey<bool>("shared.trigger");
            var completedQuests = new List<string>();

            EventBus.Subscribe<TestTriggerEvent>(_ => _gameState.SetFlag(flagKey, true));

            var quest1 = QuestFactory.CreateQuest(
                "Quest 1",
                "First quest",
                onComplete: _ => completedQuests.Add("Quest 1"),
                onFail: null,
                new QuestCriteria()
                    .If(gs => gs.TryGetFlag(flagKey, out var value) && value)
                    .ThenProgressThisQuest());

            var quest2 = QuestFactory.CreateQuest(
                "Quest 2",
                "Second quest",
                onComplete: _ => completedQuests.Add("Quest 2"),
                onFail: null,
                new QuestCriteria()
                    .If(gs => gs.TryGetFlag(flagKey, out var value) && value)
                    .ThenProgressThisQuest());

            // Act
            EventBus.Publish(new TestTriggerEvent(0, "shared trigger"));
            quest1.TickQuestTime(1, _gameState);
            quest2.TickQuestTime(1, _gameState);

            // Assert
            Assert.Equal(2, completedQuests.Count);
            Assert.Contains("Quest 1", completedQuests);
            Assert.Contains("Quest 2", completedQuests);
        }

        [Fact]
        public void GameStateManager_ShouldTickQuests_AndProcessEvents()
        {
            // Arrange
            var eventReceived = false;
            EventBus.Subscribe<TestQuestCompletedEvent>(_ => eventReceived = true);

            var quest = QuestFactory.CreateQuest(
                "GameState Quest",
                "Added to GameStateManager",
                onComplete: _ => EventBus.Publish(new TestQuestCompletedEvent(0, "gs-quest", "GameState Quest")),
                onFail: null,
                new QuestCriteria().If(_ => true).ThenProgressThisQuest());

            _questTracker.AddQuest(quest);

            // Act - simulate an hour passing
            _questTracker.TickQuestTimeTracking(1);
            

            // Assert
            Assert.True(eventReceived);
            Assert.Equal(QuestStatus.Completed, quest.Status);
        }

        [Fact]
        public async Task AsyncEventHandler_ShouldWorkWithQuestSystem_WhenUsedTogether()
        {
            // Arrange
            var asyncOperationCompleted = false;
            var questCompleted = false;

            EventBus.SubscribeAsync<TestQuestCompletedEvent>(async e =>
            {
                await Task.Delay(50);
                asyncOperationCompleted = true;
            });

            var quest = QuestFactory.CreateQuest(
                "Async Quest",
                "Uses async event handler",
                onComplete: _ =>
                {
                    questCompleted = true;
                    EventBus.Publish(new TestQuestCompletedEvent(0, "async-quest", "Async Quest"));
                },
                onFail: null,
                new QuestCriteria().If(_ => true).ThenProgressThisQuest());

            // Act
            quest.TickQuestTime(1, _gameState);

            // Wait for async handler to complete
            await Task.Delay(100);

            // Assert
            Assert.True(questCompleted);
            Assert.True(asyncOperationCompleted);
        }
    }
}
