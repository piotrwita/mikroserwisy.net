using Pacco.Services.Availability.Core.Entietes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Core.Events
{
    public class ReservationAdded : IDomainEvent
    {
        public Resource Resource { get; }
       
        public ReservationAdded(Resource resource) => Resource = resource;
    }
}
