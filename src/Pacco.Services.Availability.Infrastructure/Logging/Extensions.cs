using Convey;
using Convey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Application.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Infrastructure.Logging
{
    //klasa powstala zeby nie robic syfu logami w ogolnym Extensions na poziomie infrastruktury 
    internal static class Extensions
    {
        public static IConveyBuilder AddHandlersLogging(this IConveyBuilder builder)
        {
            //znajdz assembly w ktorym znajduje sie addresource
            var assembly = typeof(AddResource).Assembly;

            //dodaj singleton dla 
            builder.Services.AddSingleton<IMessageToLogTemplateMapper, MessageToLogTemplateMapper>();

            return builder
                .AddCommandHandlersLogging(assembly)
                .AddEventHandlersLogging(assembly);
        }
    }
}
