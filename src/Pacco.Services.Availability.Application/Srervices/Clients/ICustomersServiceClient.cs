using Pacco.Services.Availability.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Application.Srervices.Clients
{
    //tworzymy nowy port w serwisach dla customers
    public interface ICustomersServiceClient
    {
        //trzeba pod tym katem zrobic dto
        //sytuacja analogiczna jak kontrakty eventow czy komend , dto bedzie lokalna kopia
        Task<CustomerStateDto> GetStateAsync(Guid customerId);
    }
}
