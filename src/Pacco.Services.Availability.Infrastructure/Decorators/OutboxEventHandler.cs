using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using System; 
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Decorators
{
    //wzorzec dekoratora
    internal sealed class OutboxEventHandler<T> : IEventHandler<T> where T : class, IEvent
    {
        private readonly IEventHandler<T> _handler;
        private readonly IMessageOutbox _outbox;
        private readonly string _messageId;
        private readonly bool _enabled;

        public OutboxEventHandler(IEventHandler<T> handler, IMessageOutbox outbox, 
            OutboxOptions outboxOptions, IMessagePropertiesAccessor messagePropertiesAccessor)
        {
            _handler = handler;
            _outbox = outbox;
            _enabled = outboxOptions.Enabled;

            //upewniamy sie ze przekazujemy message z rabbita, jezeli z api jakies to sobie sami tworzymy
            var messageProperties = messagePropertiesAccessor.MessageProperties;
            _messageId = string.IsNullOrWhiteSpace(messageProperties?.MessageId) 
                ? Guid.NewGuid().ToString("N")
                : messageProperties.MessageId;
        }

        //poniewaaz to wzorzec dekoratora to tylko metoda handleasync
        public Task HandleAsync(T @event)
        => _enabled
            ? _outbox.HandleAsync(_messageId, () => _handler.HandleAsync(@event)) //wywolujemy z outboxa
            : _handler.HandleAsync(@event);
    }
}
