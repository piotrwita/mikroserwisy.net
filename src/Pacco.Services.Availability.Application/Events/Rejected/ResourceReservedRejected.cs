using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Application.Events.Rejected
{
    [Contract]
    //eventy mowiace otym ze cos sie nie powiodlo
    public class ResourceReservedRejected : IRejectedEvent //rozszerzony o tresci bledow
    {
        public Guid ResourceId { get; }
        public string Reason { get; }
        public string Code { get; }

        public ResourceReservedRejected(Guid resourceId, string reason, string code)
        {
            ResourceId = resourceId;
            Reason = reason;
            Code = code;
        }
    }
}
