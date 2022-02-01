using Pacco.Services.Availability.Core.Entietes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Core.Events
{
    public class ResourceCreated : IDomainEvent
    {
        public Resource Resource { get; }
       
        public ResourceCreated(Resource resource) => Resource = resource;
    }
}
