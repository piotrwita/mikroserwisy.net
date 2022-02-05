using Convey.CQRS.Events;
using Pacco.Services.Availability.Application.Events;
using Pacco.Services.Availability.Application.Srervices;
using Pacco.Services.Availability.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pacco.Services.Availability.Infrastructure.Services
{
    public class EventMapper : IEventMapper
    {
        public IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events)
            => events.Select(Map);

        public IEvent Map(IDomainEvent @event)
        => @event switch
        {
            ResourceCreated e => new ResourceAdded(e.Resource.Id),
            ReservationCanceled e => new ResourceReservationCanceled(e.Resource.Id, e.Reservation.DateTime),
            ReservationAdded e => new ResourceReserved(e.Resource.Id, e.Reservation.DateTime),
            _ => null
        };
    }
}
