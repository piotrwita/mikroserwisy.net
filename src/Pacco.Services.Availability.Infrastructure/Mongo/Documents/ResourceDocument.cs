using Convey.Types;
using System;
using System.Collections.Generic;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Documents
{
    //sealed poniewaz wewnetrzna klasa infrastruktury
    internal sealed class ResourceDocument : IIdentifiable<Guid> //dodane by spieło się z mongo repo
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<ReservationDocument> Reservations { get; set; }

    }
}
