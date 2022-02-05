using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using System; 
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Decorators
{
    //wzorzec dekoratora
    internal sealed class OutboxCommandHandler<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly IMessageOutbox _outbox;
        private readonly string _messageId;
        private readonly bool _enabled;

        public OutboxCommandHandler(ICommandHandler<T> handler, IMessageOutbox outbox, 
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
        public Task HandleAsync(T command)
        => _enabled
            ? _outbox.HandleAsync(_messageId, () => _handler.HandleAsync(command)) //wywolujemy z outboxa
            : _handler.HandleAsync(command);
    }
}
