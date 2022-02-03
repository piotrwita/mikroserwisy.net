using Pacco.Services.Availability.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pacco.Services.Availability.Core.Entietes
{
    public class AggregateId : IEquatable<AggregateId>
    {
        public Guid Value { get; }

        public AggregateId() : this(Guid.NewGuid())
        { 
        }

        public AggregateId(Guid value)
        {
            if (value == Guid.Empty)
                throw new InvalidAggregateIdException(value);

            Value = value;
        }

        public bool Equals([AllowNull] AggregateId other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Value.Equals(other.Value);
        }

        public override bool Equals([AllowNull] object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AggregateId)obj);    
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //imlicit nam pozwala bez jawnego castowania w nawiasach przechodzenie z jednego typu na drugi - w tym przypadku guid mozemy przypisywac do AggreateId
        public static implicit operator Guid(AggregateId id) => id.Value;
        //i vice versa
        public static implicit operator AggregateId(Guid aggregateId) => new AggregateId(aggregateId);

        //informacja na poziomie logowania naszej wartosci guid
        public override string ToString() => Value.ToString();
    }
}
