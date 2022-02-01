using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pacco.Services.Availability.Core.ValueObjects
{
    //struct to value objects
    //porównujemy przez wartości jego właściwości, a nie przez tożsamość, której nie posiada
    public struct Reservation : IEquatable<Reservation>
    {
        public DateTime DateTime { get; }
        public int Priority { get; }

        public Reservation(DateTime dateTime, int priority)
            => (DateTime, Priority) = (dateTime, priority);

        public bool Equals([AllowNull] Reservation other) => DateTime.Equals(other.DateTime, this.DateTime) && Priority == other.Priority;

        public override bool Equals([AllowNull] object obj) => obj is Reservation other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(DateTime, Priority);
    }
}
