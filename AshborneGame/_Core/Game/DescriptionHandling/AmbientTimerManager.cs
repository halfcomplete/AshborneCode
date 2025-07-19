using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshborneGame._Core.SceneManagement;
using AshborneGame._Core.Game.DescriptionHandling;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    /// <summary>
    /// Manages ambient timers for a single location (eyePlatform) as an example.
    /// </summary>
    public class AmbientTimerManager : IDisposable
    {
        private readonly Location _eyePlatformLocation;
        private readonly AmbientDescription _ambientDescription;
        private readonly Dictionary<TimeSpan, Timer> _timers = new();
        private bool _isPaused = false;
        private bool _isTypewriterActive = false;
        private bool _isInputPaused = false;
        private readonly object _lock = new object();

        public event Action<string>? OnAmbientDescriptionTriggered;
        public event Func<string, Task>? OnAmbientDescriptionTriggeredAsync; // For web/async output
        public event Action? OnInputPaused;
        public event Action? OnInputResumed;

        public AmbientTimerManager(Location eyePlatformLocation)
        {
            _eyePlatformLocation = eyePlatformLocation;
            _ambientDescription = eyePlatformLocation.DescriptionComposer.Ambient!;
        }

        /// <summary>
        /// Call this when the player enters the eyePlatform location.
        /// </summary>
        public void OnEnterEyePlatform()
        {
            lock (_lock)
            {
                StopAllTimers();
                if (!_isTypewriterActive && !_isInputPaused)
                {
                    StartAllTimers();
                }
            }
        }

        /// <summary>
        /// Call this when the player leaves the eyePlatform location.
        /// </summary>
        public void OnExitEyePlatform()
        {
            lock (_lock)
            {
                StopAllTimers();
            }
        }

        /// <summary>
        /// Call this when the player inputs a command while on the eyePlatform.
        /// </summary>
        public void OnPlayerCommandInput()
        {
            lock (_lock)
            {
                ResetAllTimers();
            }
        }

        /// <summary>
        /// Call this when typewriter output starts.
        /// </summary>
        public void OnTypewriterStart()
        {
            lock (_lock)
            {
                _isTypewriterActive = true;
                PauseAllTimers();
            }
        }

        /// <summary>
        /// Call this when typewriter output completes.
        /// </summary>
        public void OnTypewriterComplete()
        {
            lock (_lock)
            {
                _isTypewriterActive = false;
                if (!_isInputPaused)
                {
                    StartAllTimers();
                }
            }
        }

        private void StartAllTimers()
        {
            foreach (var kvp in _ambientDescription.FromDuration)
            {
                if (_timers.ContainsKey(kvp.Key))
                    continue;
                var timer = new Timer(OnTimerElapsed, kvp.Key, kvp.Key, Timeout.InfiniteTimeSpan);
                _timers[kvp.Key] = timer;
            }
        }

        private void ResetAllTimers()
        {
            StopAllTimers();
            if (!_isTypewriterActive && !_isInputPaused)
            {
                StartAllTimers();
            }
        }

        private void PauseAllTimers()
        {
            foreach (var timer in _timers.Values)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void StopAllTimers()
        {
            foreach (var timer in _timers.Values)
            {
                timer.Dispose();
            }
            _timers.Clear();
        }

        private void OnTimerElapsed(object? state)
        {
            lock (_lock)
            {
                if (_isInputPaused) return;
                _isInputPaused = true;
                PauseAllTimers();
                var duration = (TimeSpan)state!;
                if (_ambientDescription.FromDuration.TryGetValue(duration, out var desc))
                {
                    // Prefer async event if set (web), else use sync event (console)
                    if (OnAmbientDescriptionTriggeredAsync != null)
                    {
                        // Fire and forget, GameEngine will await this
                        _ = OnAmbientDescriptionTriggeredAsync(desc);
                    }
                    else
                    {
                        OnAmbientDescriptionTriggered?.Invoke(desc);
                    }
                }
                OnInputPaused?.Invoke();
            }
        }

        /// <summary>
        /// Call this when ambient description output is complete.
        /// </summary>
        public void OnAmbientDescriptionComplete()
        {
            lock (_lock)
            {
                _isInputPaused = false;
                OnInputResumed?.Invoke();
                if (!_isTypewriterActive)
                {
                    StartAllTimers();
                }
            }
        }

        public void Dispose()
        {
            StopAllTimers();
        }
    }
} 