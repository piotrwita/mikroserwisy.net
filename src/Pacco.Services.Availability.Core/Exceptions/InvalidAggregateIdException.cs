using System;

namespace Pacco.Services.Availability.Core.Exceptions
{
    internal class InvalidAggregateIdException : DomainException
    {
        public override string Code => "invalid_aggregate_id";

        public InvalidAggregateIdException(Guid id) : base($"Invalid aggregate id: {id}.")
        {
        }
    }
}
