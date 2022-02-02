using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Application.Exceptions
{
    internal class ResourceAlreadyExistsException : AppExceptionBase
    {
        public override string Code => "resource_already_exists";
        public Guid ResourceId { get; set; }

        public ResourceAlreadyExistsException(Guid resourceId) : 
            base($"Resource with id: {resourceId} already exists.")
        {
            ResourceId = resourceId;
        }
    }
}
