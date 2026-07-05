using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Game.DescriptionHandling;

namespace AshborneGame._Core.LocationManagement
{
    public class LocationBuilder
    {
        private LookDescription? _lookDescription;
        private VisitDescription? _visitDescription;
        private SensoryDescription? _sensoryDescription;
        private AmbientDescription? _ambientDescription;
        private ConditionalDescription? _conditionalDescription;

        public LocationBuilder AddLookBased(string firstLook, string? secondLook = null, string? repeatLook = null)
        {
            _lookDescription = new(firstLook, secondLook, repeatLook);

            return this;
        }

        public LocationBuilder AddVisitBased(string firstTime, string secondTime, string fourthTime, string? unchangedTime = null)
        {
            _visitDescription = new(firstTime, secondTime, fourthTime, unchangedTime);

            return this;
        }

        public LocationBuilder AddSensory(string visual, string auditory, string? tactile = null, string? olfactory = null, string? gustatory = null)
        {
            _sensoryDescription = new(visual, auditory, tactile, olfactory, gustatory);

            return this;
        }

        public LocationBuilder AddTimeBasedDescription(int ticks, string description)
        {
            if (_ambientDescription == null)
            {
                _ambientDescription = new();
            }
            _ambientDescription.AddTimeBased(ticks, description);

            return this;
        }

        public LocationBuilder AddRandomDescription(string description)
        {
            if (_ambientDescription == null)
            {
                _ambientDescription = new();
            }
            _ambientDescription.AddRandom(description);

            return this;
        }

        public LocationBuilder AddConditional(ConditionalDescription conditionalDescription)
        {
            _conditionalDescription = conditionalDescription;

            return this;
        }
    }
}