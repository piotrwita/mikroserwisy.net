using Convey.MessageBrokers.RabbitMQ;
using Convey.WebApi.Exceptions;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Events.Rejected;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Core.Exceptions;
using System;
using System.Net;

namespace Pacco.Services.Availability.Infrastructure.Exceptions
{
    //mapowanie z wyjatkow na wiadomosci
    internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        //na wejsciu mamy wyjatek jak i message czyli z jakiego handlera ten wyjatek polecial
        //poniewaz sam exception to za malo bysmy mogli wiedziec skad dany wyjatek polecial

        public object Map(Exception exception, object message)
            => exception switch
            {
                //obsluga bledow domenowych
                MissingResourceTagsException ex => new AddResourceRejected(Guid.Empty, ex.Message, ex.Code),
                InvalidResourceTagsException ex => new AddResourceRejected(Guid.Empty, ex.Message, ex.Code),
                CannotExpropriateReservationException ex => new ResourceReservedRejected(ex.ResourceId, ex.Message, ex.Code),
                //obsluga bledow aplikacyjnych
                ResourceAlreadyExistsException ex => new AddResourceRejected(ex.ResourceId, ex.Message, ex.Code),
                ResourceNotFoundException ex => message switch
                {
                    ReserveResource cmd => new ResourceReservedRejected(ex.ResourceId, ex.Message, ex.Code),
                    _ => null
                },
                _ => null
            };
    }
}