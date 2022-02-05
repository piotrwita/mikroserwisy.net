using Convey.CQRS.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pacco.Services.Availability.Application.Events;
using Pacco.Services.Availability.Application.Srervices;
using Pacco.Services.Availability.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Services
{
    public class EventProcessor : IEventProcessor
    { 
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<IEventProcessor> _logger;

        public EventProcessor(IMessageBroker messageBroker, IEventMapper eventMapper,
            IServiceScopeFactory serviceScopeFactory,ILogger<IEventProcessor> logger)
        {
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        public async Task ProcessAsync(IEnumerable<IDomainEvent> events)
        {
            if (events is null) return;

            //przetworz nasze zdarzenia domenowe + zmapuj (eventy wejsciowe)
            var integrationEvents = await HandleDomainEventsAsync(events);
            if (!integrationEvents.Any()) return;

            //jezeli sa jakiekolwiek eventy to publikujemy je
            await _messageBroker.PublishAsync(integrationEvents);
        }

        //obluzenie zdarzen domenowych
        private async Task<List<IEvent>> HandleDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            var integrationEvents = new List<IEvent>();
            //poprawny scope zeby cykl zyia obiektu byl poprawnie obsluzony
            //zeby nie bylo sytuacje gdzie mam singleton a wstrzykne scope transient
            //cykl zycia ustalony przez rodzica zawsze
            //wychodzimy od cyklu najbardziej ogolnego do najkrotszego
            using var scope = _serviceScopeFactory.CreateScope();
            foreach(var domainEvent in domainEvents)
            {
                var domainEventType = domainEvent.GetType();
                //logowanie info ze obsluguje zdarzenie domenowe
                _logger.LogTrace($"Handling a domain event: {domainEventType.Name}.");
                //probujemy wyszukac handler ktory jest w stanie zdarzenie domenowe obsluzyc
                //to co tu robimy robimy dlatego ze nie mozemy przekazac jako generic type stricte jakiegos type
                //musimy uzyc refleksji, czyli make generictype
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEventType);
                dynamic handlers = scope.ServiceProvider.GetService(handlerType);
                //foreach(var handler in handlers)
                //{
                //    await handler.HandlerAsync((dynamic)domainEvent);
                //}
                //czyli jezeli sa jakies handlery dla zdarzen domenowych to tu powyzej zostaly one wlasnie obluzone 

                var integrationEvent = _eventMapper.Map(domainEvent);

                if (integrationEvent is null) continue;

                integrationEvents.Add(integrationEvent);
            }

            return integrationEvents;
        }
    }
}
