using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Application.DTO
{
    public class CustomerStateDto
    {
        public string State { get; set; }
        public bool IsValid => State.Equals("valid", StringComparison.InvariantCultureIgnoreCase);//niezalezny od wielkosci znakow
    }
}
