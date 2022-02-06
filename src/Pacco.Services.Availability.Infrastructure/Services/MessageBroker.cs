using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using Pacco.Services.Availability.Application.Srervices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Services
{
    internal sealed class MessageBroker : IMessageBroker
    {
        private readonly IBusPublisher _busPublisher;
        private readonly IMessageOutbox _outbox;
        private readonly OutboxOptions _outboxOptions;
        private readonly IMessagePropertiesAccessor _messagePropertiesAccessor;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public MessageBroker(IBusPublisher busPublisher, IMessageOutbox outbox, OutboxOptions outboxOptions,
            IMessagePropertiesAccessor messagePropertiesAccessor, ICorrelationContextAccessor correlationContextAccessor)
        {
            _busPublisher = busPublisher;
            _outbox = outbox;
            _outboxOptions = outboxOptions;
            _messagePropertiesAccessor = messagePropertiesAccessor;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public Task PublishAsync(params IEvent[] events) => PublishAsync(events?.AsEnumerable());

        public async Task PublishAsync(IEnumerable<IEvent> events)
        {
            if (events is null) return;

            //kluczowe dane zeby flow w calym przeplywie async bylo pospinane
            //async robimy tylko ciezkie zlozone operacje
            //reszta prostych operacji synchronicznie zeby zachwoac prostolinijnosc dla przebiegu
            var messageProperties = _messagePropertiesAccessor.MessageProperties;
            var originatedMessageId = messageProperties?.MessageId; //? nullcheck //id wiadomosoci w rabbicie
            var correlationId = messageProperties?.CorrelationId;//id calego flow w rabbicie
            var correlationContext = _correlationContextAccessor.CorrelationContext; //kontekst w obrebie wiadomosci ktora w danym momencie przetwarzamy

            foreach (var @event in events)
            {
                if (@event is null) continue;

                var messageId = Guid.NewGuid().ToString("N"); //ten format to guid bez myslnikow

                if(_outbox.Enabled)
                {
                    await _outbox.SendAsync(@event, originatedMessageId, messageId, correlationId, messageContext: correlationContext);
                    continue;
                }

                await _busPublisher.PublishAsync(@event, messageId, correlationId, messageContext: correlationContext);
            }

        }
    }
}
