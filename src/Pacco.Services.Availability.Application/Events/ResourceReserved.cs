using Convey.CQRS.Events;
using System;

namespace Pacco.Services.Availability.Application.Events
{
    [Contract]
    public class ResourceReserved : IEvent
    {
        //zeby domena nie wyciekala 
        //w zdarzeniu domenowym przekazywalismy caly obiekt, tutaj wystaczy tylko id
        public Guid ResourceId { get; }
        public DateTime DateTime { get; }

        public ResourceReserved(Guid resourceId, DateTime dateTime)
        {
            ResourceId = resourceId;
            DateTime = dateTime;
        }
    }
}
