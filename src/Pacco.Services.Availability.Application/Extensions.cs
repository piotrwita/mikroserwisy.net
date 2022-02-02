using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Application
{
    public static class Extensions
    {
        //coś na wzór iapplicationbuilder - opakowuje application w service collection (kontener) - rejestracja zależności
        public static IConveyBuilder AddApplication(this IConveyBuilder builder)
            => builder
                .AddCommandHandlers() //skanuje kod w poszukiwaniu wszystkich typów które implementują ICommandHandler
                .AddEventHandlers() //skanuje kod w poszukiwaniu wszystkich typów które implementują IEventHandler
                .AddInMemoryCommandDispatcher() // obsługa/wysyłka command w pamięci
                .AddInMemoryEventDispatcher(); // obsługa/wysyłka event w pamięci
    }
}
