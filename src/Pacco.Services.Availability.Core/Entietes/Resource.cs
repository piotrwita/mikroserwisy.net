using Pacco.Services.Availability.Core.Events;
using Pacco.Services.Availability.Core.Exceptions;
using Pacco.Services.Availability.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pacco.Services.Availability.Core.Entietes
{
    /// <summary>
    /// Model domenowy - zasób wykorzystywany do rezerwacji
    /// </summary>
    public class Resource : AggregateRoot
    {
        //unikalny zbiór tagów
        //np czy to auto czy konwój
        private ISet<string> _tags = new HashSet<string>();

        //kolekcja rezerwacja w danym czasie
        private ISet<Reservation> _reservations = new HashSet<Reservation>();

        public IEnumerable<string> Tags
        {
            get => _tags;
            private set => _tags = new HashSet<string>(value);
        }

        public IEnumerable<Reservation> Reservations
        {
            get => _reservations;
            private set => _reservations = new HashSet<Reservation>(value);
        }

        public Resource(AggregateId id, IEnumerable<string> tags, IEnumerable<Reservation> reservations = null, int version = 0)
        {
            ValidateTags(tags);

            Id = id;
            Tags = tags;
            Reservations = reservations ?? Enumerable.Empty<Reservation>();
            Version = version;
        }

        private static void ValidateTags(IEnumerable<string> tags)
        {
            if(tags is null || !tags.Any())
            {
                throw new MissingResourceTagsException();
            }
             
            if (tags.Any(string.IsNullOrWhiteSpace))
            {
                throw new InvalidResourceTagsException();
            }            
        }
        
        //static factory method
        //utworzenie resource + zdarzenie domenowe
        public static Resource Create(AggregateId id, IEnumerable<string> tags, IEnumerable<Reservation> reservations = null)
        {
            var resource = new Resource(id, tags, reservations);
            resource.AddEvent(new ResourceCreated(resource));

            return resource;
        }
    }
}
