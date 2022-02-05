using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Application.Events
{
    [Contract] //prosta automatyczna dokumentacja naszych kontraktow
    public class ResourceReservationCanceled : IEvent
    {
        //zeby domena nie wyciekala 
        //w zdarzeniu domenowym przekazywalismy caly obiekt, tutaj wystaczy tylko id
        public Guid ResourceId { get; }
        public DateTime DateTime { get; }

        public ResourceReservationCanceled(Guid resourceId, DateTime dateTime)
        {
            ResourceId = resourceId;
            DateTime = dateTime;
        }
    }
}
