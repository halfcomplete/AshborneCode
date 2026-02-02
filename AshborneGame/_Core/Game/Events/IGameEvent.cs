using System.Collections.Generic;

namespace AshborneGame._Core.Game.Events
{
    public interface IGameEvent
    {
        bool OneTime { get; }
    }
}
