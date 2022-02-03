using Convey.CQRS.Events;
using Pacco.Services.Identity.Application.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Application.Events.External.Handlers
{
    //fakty ktore juz zaszly w sytemie czyli zdarzenia
    //wywolanie nam zapewnia podpiecie w extensions  .AddCommandHandlers() .AddEventHandlers()
    public class SignedUpHandler : IEventHandler<SignedUp>
    {
        public Task HandleAsync(SignedUp @event)
        {
            return Task.CompletedTask;
        }
    }
}
