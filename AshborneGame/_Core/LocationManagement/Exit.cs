using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.LocationManagement
{
    public sealed class Exit
    {
        public string Direction { get; }

        public DefinitionID TargetLocation { get; }

        public Func<bool>? CanTraverse { get; }

        public Action? OnTraverse { get; }

        public string? FailureMessage { get; }

        public Exit(
            DefinitionID targetLocation,
            string direction,
            Func<bool>? canTraverse = null,
            Action? onTraverse = null,
            string? failureMessage = null)
        {
            Direction = direction;
            TargetLocation = targetLocation;
            CanTraverse = canTraverse;
            OnTraverse = onTraverse;
            FailureMessage = failureMessage;
        }
    }
}
