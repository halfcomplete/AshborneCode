using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Data.Definitions
{
    // TODO: make more general
    public record ExitDefinition(DefinitionID from, DefinitionID to, string direction)
    {
        public Exit From()
        {
            return new Exit(to, direction);
        }

        public Exit To()
        {
            return new Exit(from, DirectionConstants.CardinalDirectionOppositesMap[direction]);
        }
    }
}