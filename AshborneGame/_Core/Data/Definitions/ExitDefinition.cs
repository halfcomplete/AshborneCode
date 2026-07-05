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
        /// <summary>
        /// Returns a new Exit object from the "from" location to the "to" location.
        /// </summary>
        public Exit FromFrom()
        {
            return new Exit(to, direction);
        }
        
        /// <summary>
        /// Returns a new Exit object from the "to" location to the "from" location.
        /// </summary>
        public Exit FromTo()
        {
            return new Exit(from, DirectionConstants.CardinalDirectionOppositesMap[direction]);
        }
    }
}