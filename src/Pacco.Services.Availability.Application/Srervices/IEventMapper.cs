using Convey.CQRS.Events;
using Pacco.Services.Availability.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Application.Srervices
{
    //maper zdarzen domenowych na integracyjne
    public interface IEventMapper
    {
        public IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events);
        public IEvent Map(IDomainEvent @event);
    }
}
