using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Core.Exceptions
{
    internal class MissingResourceTagsException : DomainExceptionBase
    {
        public override string Code => "missing_resource_tags";
        public MissingResourceTagsException() : base("Resource tags are missing.")
        {
        }
    }
}
