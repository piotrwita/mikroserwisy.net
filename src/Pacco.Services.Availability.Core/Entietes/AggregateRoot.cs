using Pacco.Services.Availability.Core.Events;
using System.Collections.Generic;
using System.Linq;

namespace Pacco.Services.Availability.Core.Entietes
{
    /// <summary>
    /// Enkapsuluję domenę
    /// Korzeń - odpowiedzialny za trzymanie wersji , wszystkich eventów domenowych
    /// </summary>
    public abstract class AggregateRoot
    {
        //ISet by zapewnić unikalność, by nie dodać tego samego zdarzenia pod ten sam objekt
        private readonly ISet<IDomainEvent> _events = new HashSet<IDomainEvent>();

        //opakowanie _events w IEnumerable, czyli kolekcje tylko do odczytu (do HashSet można dodawać elementy) 
        public IEnumerable<IDomainEvent> Events => _events;

        public AggregateId Id { get; protected set; }
        public int Version { get; protected set; }

        //za pomocją zdarzeń domenowych możemy łatwo obsłużyć przykładową akcję - wywłaszczenie kogoś + rezerwacja zasobu
        protected void AddEvent(IDomainEvent @event)
        {
            //jeżeli nie istnieje żadne event
            if (!_events.Any())
            {
                //podbijamy jego wersję - wiemy że nastąpiła modyfikacja jego stanu
                Version++;
            }

            _events.Add(@event);
        }

        public void ClearEvent() => _events.Clear();
    }
}
