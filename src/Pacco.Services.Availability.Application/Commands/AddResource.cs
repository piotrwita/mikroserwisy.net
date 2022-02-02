using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Application.Commands
{
    public class AddResource : ICommand
    {
        //queries nie moga moga miec setow 
        public Guid ResourceId { get; }    
        public IEnumerable<string> Tags { get; }

        public AddResource(Guid resourceId, IEnumerable<string> tags)
        {
            ResourceId = resourceId;
            Tags = tags;
        }
    }
}
