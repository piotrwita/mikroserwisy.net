using System;

namespace Pacco.Services.Availability.Application.Exceptions
{
    internal class ResourceNotFoundException : AppException
    {
        public override string Code => "resource_not_found";
        public Guid ResourceId { get; set; }

        public ResourceNotFoundException(Guid resourceId) :
            base($"Resource with id: {resourceId} was not found.")
        {
            ResourceId = resourceId;
        }
    }
}
