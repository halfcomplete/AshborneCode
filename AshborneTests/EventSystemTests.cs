using AshborneGame._Core.Game.Events;

namespace AshborneTests
{
    /// <summary>
    /// Unit tests for the EventBus, EventToken, and CompositeEventToken classes.
    /// Tests cover subscription, publishing, unsubscription, and thread-safety scenarios.
    /// </summary>
    [Collection("AshborneTests")]
    public class EventSystemTests : IDisposable
    {
        public EventSystemTests()
        {
            // Ensure clean state before each test
            EventBus.Clear();
        }

        public void Dispose()
        {
            // Clean up after each test
            EventBus.Clear();
            GC.SuppressFinalize(this);
        }

        #region Test Events

        /// <summary>
        /// A simple test event with a message payload.
        /// </summary>
        private sealed record TestEvent(string Message) : IGameEvent;

        /// <summary>
        /// A one-time event that should remove all subscribers after publishing.
        /// </summary>
        private sealed record OneTimeTestEvent(string Message) : IGameEvent
        {
            public bool OneTime => true;
        }

        /// <summary>
        /// Another event type to test type isolation.
        /// </summary>
        private sealed record OtherTestEvent(int Value) : IGameEvent;

        /// <summary>
        /// An event with no payload.
        /// </summary>
        private sealed record EmptyTestEvent() : IGameEvent;

        #endregion

        #region Subscribe Tests

        [Fact]
        public void Subscribe_ShouldReturnValidToken_WhenGivenValidCallback()
        {
            // Arrange
            var callCount = 0;

            // Act
            var token = EventBus.Subscribe<TestEvent>(e => callCount++);

            // Assert
            Assert.NotNull(token);
            Assert.False(token.IsDisposed);
            Assert.Equal(typeof(TestEvent), token.EventType);
            Assert.False(token.IsAsync);
        }

        [Fact]
        public void Subscribe_ShouldIncrementSubscriberCount_WhenCalled()
        {
            // Arrange & Act
            EventBus.Subscribe<TestEvent>(e => { });
            EventBus.Subscribe<TestEvent>(e => { });

            // Assert
            Assert.Equal(2, EventBus.GetSubscriberCount<TestEvent>());
        }

        [Fact]
        public void Subscribe_ShouldThrowArgumentNullException_WhenCallbackIsNull()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => EventBus.Subscribe<TestEvent>(null!));
        }

        [Fact]
        public void SubscribeAsync_ShouldReturnAsyncToken_WhenGivenAsyncCallback()
        {
            // Arrange & Act
            var token = EventBus.SubscribeAsync<TestEvent>(async e => await Task.Delay(1));

            // Assert
            Assert.NotNull(token);
            Assert.True(token.IsAsync);
            Assert.Equal(typeof(TestEvent), token.EventType);
        }

        [Fact]
        public void SubscribeAsync_ShouldThrowArgumentNullException_WhenCallbackIsNull()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => EventBus.SubscribeAsync<TestEvent>(null!));
        }

        #endregion

        #region Publish Tests

        [Fact]
        public void Publish_ShouldInvokeSubscriber_WhenEventIsPublished()
        {
            // Arrange
            var receivedMessage = string.Empty;
            EventBus.Subscribe<TestEvent>(e => receivedMessage = e.Message);

            // Act
            EventBus.Publish(new TestEvent("Hello, World!"));

            // Assert
            Assert.Equal("Hello, World!", receivedMessage);
        }

        [Fact]
        public void Publish_ShouldInvokeAllSubscribers_WhenMultipleSubscribersExist()
        {
            // Arrange
            var callCount = 0;
            EventBus.Subscribe<TestEvent>(e => callCount++);
            EventBus.Subscribe<TestEvent>(e => callCount++);
            EventBus.Subscribe<TestEvent>(e => callCount++);

            // Act
            EventBus.Publish(new TestEvent("Test"));

            // Assert
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void Publish_ShouldNotInvokeOtherEventSubscribers_WhenDifferentEventTypeIsPublished()
        {
            // Arrange
            var testEventCalled = false;
            var otherEventCalled = false;
            EventBus.Subscribe<TestEvent>(e => testEventCalled = true);
            EventBus.Subscribe<OtherTestEvent>(e => otherEventCalled = true);

            // Act
            EventBus.Publish(new OtherTestEvent(42));

            // Assert
            Assert.False(testEventCalled);
            Assert.True(otherEventCalled);
        }

        [Fact]
        public void Publish_ShouldPassCorrectEventData_ToSubscriber()
        {
            // Arrange
            TestEvent? receivedEvent = null;
            EventBus.Subscribe<TestEvent>(e => receivedEvent = e);

            // Act
            var publishedEvent = new TestEvent("Test Message");
            EventBus.Publish(publishedEvent);

            // Assert
            Assert.NotNull(receivedEvent);
            Assert.Equal("Test Message", receivedEvent.Message);
            Assert.Equal(publishedEvent, receivedEvent);
        }

        [Fact]
        public void Publish_ShouldDoNothing_WhenNoSubscribersExist()
        {
            // Arrange & Act & Assert (no exception should be thrown)
            var exception = Record.Exception(() => EventBus.Publish(new TestEvent("No subscribers")));
            Assert.Null(exception);
        }

        [Fact]
        public void Publish_ShouldThrowArgumentNullException_WhenEventIsNull()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => EventBus.Publish<TestEvent>(null!));
        }

        [Fact]
        public void Publish_ShouldContinueInvokingHandlers_WhenOneHandlerThrowsException()
        {
            // Arrange
            var handlersCalled = new List<int>();
            EventBus.Subscribe<TestEvent>(e => handlersCalled.Add(1));
            EventBus.Subscribe<TestEvent>(e => throw new InvalidOperationException("Test exception"));
            EventBus.Subscribe<TestEvent>(e => handlersCalled.Add(3));

            // Act
            EventBus.Publish(new TestEvent("Test"));

            // Assert - handlers 1 and 3 should still be called despite handler 2 throwing
            Assert.Contains(1, handlersCalled);
            Assert.Contains(3, handlersCalled);
        }

        #endregion

        #region PublishAsync Tests

        [Fact]
        public async Task PublishAsync_ShouldInvokeAsyncSubscriber_WhenEventIsPublished()
        {
            // Arrange
            var receivedMessage = string.Empty;
            EventBus.SubscribeAsync<TestEvent>(async e =>
            {
                await Task.Delay(10);
                receivedMessage = e.Message;
            });

            // Act
            await EventBus.PublishAsync(new TestEvent("Async Test"));

            // Assert
            Assert.Equal("Async Test", receivedMessage);
        }

        [Fact]
        public async Task PublishAsync_ShouldInvokeBothSyncAndAsyncSubscribers_WhenMixed()
        {
            // Arrange
            var syncCalled = false;
            var asyncCalled = false;
            EventBus.Subscribe<TestEvent>(e => syncCalled = true);
            EventBus.SubscribeAsync<TestEvent>(async e =>
            {
                await Task.Delay(10);
                asyncCalled = true;
            });

            // Act
            await EventBus.PublishAsync(new TestEvent("Mixed Test"));

            // Assert
            Assert.True(syncCalled);
            Assert.True(asyncCalled);
        }

        [Fact]
        public async Task PublishAsync_ShouldAwaitAllHandlers_BeforeReturning()
        {
            // Arrange
            var completionOrder = new List<int>();
            EventBus.SubscribeAsync<TestEvent>(async e =>
            {
                await Task.Delay(50);
                completionOrder.Add(1);
            });
            EventBus.SubscribeAsync<TestEvent>(async e =>
            {
                await Task.Delay(10);
                completionOrder.Add(2);
            });

            // Act
            await EventBus.PublishAsync(new TestEvent("Await Test"));

            // Assert - both should complete (order may vary based on sequential execution)
            Assert.Equal(2, completionOrder.Count);
        }

        #endregion

        #region OneTime Event Tests

        [Fact]
        public void Publish_ShouldRemoveAllSubscribers_WhenOneTimeEventIsPublished()
        {
            // Arrange
            var callCount = 0;
            EventBus.Subscribe<OneTimeTestEvent>(e => callCount++);
            EventBus.Subscribe<OneTimeTestEvent>(e => callCount++);

            // Act
            EventBus.Publish(new OneTimeTestEvent("First"));
            EventBus.Publish(new OneTimeTestEvent("Second"));

            // Assert - only the first publish should invoke handlers
            Assert.Equal(2, callCount); // 2 handlers called once each
            Assert.Equal(0, EventBus.GetSubscriberCount<OneTimeTestEvent>());
        }

        [Fact]
        public void Publish_ShouldStillInvokeAllHandlers_BeforeRemovingThemForOneTimeEvent()
        {
            // Arrange
            var messages = new List<string>();
            EventBus.Subscribe<OneTimeTestEvent>(e => messages.Add("Handler1: " + e.Message));
            EventBus.Subscribe<OneTimeTestEvent>(e => messages.Add("Handler2: " + e.Message));

            // Act
            EventBus.Publish(new OneTimeTestEvent("OneTime"));

            // Assert
            Assert.Equal(2, messages.Count);
            Assert.Contains("Handler1: OneTime", messages);
            Assert.Contains("Handler2: OneTime", messages);
        }

        #endregion

        #region Unsubscribe / Token Tests

        [Fact]
        public void TokenDispose_ShouldUnsubscribeFromEvent_WhenDisposed()
        {
            // Arrange
            var callCount = 0;
            var token = EventBus.Subscribe<TestEvent>(e => callCount++);

            // Act
            token.Dispose();
            EventBus.Publish(new TestEvent("After dispose"));

            // Assert
            Assert.Equal(0, callCount);
            Assert.True(token.IsDisposed);
        }

        [Fact]
        public void TokenDispose_ShouldBeIdempotent_WhenCalledMultipleTimes()
        {
            // Arrange
            var token = EventBus.Subscribe<TestEvent>(e => { });

            // Act
            token.Dispose();
            token.Dispose();
            token.Dispose();

            // Assert - no exception should be thrown
            Assert.True(token.IsDisposed);
        }

        [Fact]
        public void TokenDispose_ShouldOnlyUnsubscribeSelf_WhenMultipleSubscribersExist()
        {
            // Arrange
            var callCount = 0;
            var token1 = EventBus.Subscribe<TestEvent>(e => callCount++);
            var token2 = EventBus.Subscribe<TestEvent>(e => callCount++);

            // Act
            token1.Dispose();
            EventBus.Publish(new TestEvent("Test"));

            // Assert
            Assert.Equal(1, callCount); // Only token2's handler should be called
            Assert.Equal(1, EventBus.GetSubscriberCount<TestEvent>());
        }

        [Fact]
        public void UsingStatement_ShouldAutoDisposeToken_WhenScopeEnds()
        {
            // Arrange
            var callCount = 0;

            // Act
            using (var token = EventBus.Subscribe<TestEvent>(e => callCount++))
            {
                EventBus.Publish(new TestEvent("Inside scope"));
            }
            EventBus.Publish(new TestEvent("Outside scope"));

            // Assert
            Assert.Equal(1, callCount);
        }

        #endregion

        #region CompositeEventToken Tests

        [Fact]
        public void CompositeEventToken_ShouldDisposeAllTokens_WhenDisposed()
        {
            // Arrange
            var callCount = 0;
            var composite = new CompositeEventToken();
            composite.Add(EventBus.Subscribe<TestEvent>(e => callCount++));
            composite.Add(EventBus.Subscribe<TestEvent>(e => callCount++));
            composite.Add(EventBus.Subscribe<OtherTestEvent>(e => callCount++));

            // Act
            composite.Dispose();
            EventBus.Publish(new TestEvent("Test"));
            EventBus.Publish(new OtherTestEvent(42));

            // Assert
            Assert.Equal(0, callCount);
            Assert.Equal(0, EventBus.GetSubscriberCount<TestEvent>());
            Assert.Equal(0, EventBus.GetSubscriberCount<OtherTestEvent>());
        }

        [Fact]
        public void CompositeEventToken_ShouldDisposeNewTokensImmediately_WhenAddedAfterDisposal()
        {
            // Arrange
            var composite = new CompositeEventToken();
            composite.Dispose();

            // Act
            var token = EventBus.Subscribe<TestEvent>(e => { });
            composite.Add(token);

            // Assert - token should be disposed immediately when added to disposed composite
            Assert.True(token.IsDisposed);
        }

        [Fact]
        public void CompositeEventToken_ShouldBeIdempotent_WhenDisposedMultipleTimes()
        {
            // Arrange
            var composite = new CompositeEventToken();
            composite.Add(EventBus.Subscribe<TestEvent>(e => { }));

            // Act
            composite.Dispose();
            composite.Dispose();
            composite.Dispose();

            // Assert - no exception
            Assert.Equal(0, EventBus.GetSubscriberCount<TestEvent>());
        }

        #endregion

        #region Utility Method Tests

        [Fact]
        public void Clear_ShouldRemoveAllSubscribers_WhenCalled()
        {
            // Arrange
            EventBus.Subscribe<TestEvent>(e => { });
            EventBus.Subscribe<OtherTestEvent>(e => { });

            // Act
            EventBus.Clear();

            // Assert
            Assert.Equal(0, EventBus.GetSubscriberCount<TestEvent>());
            Assert.Equal(0, EventBus.GetSubscriberCount<OtherTestEvent>());
        }

        [Fact]
        public void GetSubscriberCount_ShouldReturnZero_WhenNoSubscribersExist()
        {
            // Arrange & Act & Assert
            Assert.Equal(0, EventBus.GetSubscriberCount<TestEvent>());
        }

        [Fact]
        public void HasSubscribers_ShouldReturnTrue_WhenSubscribersExist()
        {
            // Arrange
            EventBus.Subscribe<TestEvent>(e => { });

            // Act & Assert
            Assert.True(EventBus.HasSubscribers<TestEvent>());
        }

        [Fact]
        public void HasSubscribers_ShouldReturnFalse_WhenNoSubscribersExist()
        {
            // Arrange & Act & Assert
            Assert.False(EventBus.HasSubscribers<TestEvent>());
        }

        #endregion

        #region Extension Method Tests

        [Fact]
        public void EventPublishExtension_ShouldPublishEvent_WhenCalledOnEventInstance()
        {
            // Arrange
            var received = false;
            EventBus.Subscribe<TestEvent>(e => received = true);

            // Act
            new TestEvent("Extension Test").Publish();

            // Assert
            Assert.True(received);
        }

        [Fact]
        public async Task EventPublishAsyncExtension_ShouldPublishEventAsync_WhenCalledOnEventInstance()
        {
            // Arrange
            var received = false;
            EventBus.SubscribeAsync<TestEvent>(async e =>
            {
                await Task.Delay(1);
                received = true;
            });

            // Act
            await new TestEvent("Async Extension Test").PublishAsync();

            // Assert
            Assert.True(received);
        }

        #endregion

        #region Thread Safety Tests

        [Fact]
        public async Task EventBus_ShouldHandleConcurrentSubscriptions_WithoutErrors()
        {
            // Arrange
            var tasks = new List<Task>();
            var tokenCount = 0;

            // Act - subscribe concurrently from multiple threads
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var token = EventBus.Subscribe<TestEvent>(e => { });
                    Interlocked.Increment(ref tokenCount);
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(100, tokenCount);
            Assert.Equal(100, EventBus.GetSubscriberCount<TestEvent>());
        }

        [Fact]
        public async Task EventBus_ShouldHandleConcurrentPublishing_WithoutErrors()
        {
            // Arrange
            var callCount = 0;
            EventBus.Subscribe<TestEvent>(e => Interlocked.Increment(ref callCount));
            var tasks = new List<Task>();

            // Act - publish concurrently from multiple threads
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() => EventBus.Publish(new TestEvent($"Message {i}"))));
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(100, callCount);
        }

        #endregion
    }
}
