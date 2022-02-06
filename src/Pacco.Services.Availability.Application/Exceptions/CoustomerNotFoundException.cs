using System;

namespace Pacco.Services.Availability.Application.Exceptions
{
    public class CoustomerNotFoundException : AppException
    {
        public override string Code => "customer_not_found";
        public Guid CustomerId { get; set; }

        public CoustomerNotFoundException(Guid customerId) :
            base($"Customer with id: {customerId} was not found.")
        {
            CustomerId = customerId;
        }
    }
}
