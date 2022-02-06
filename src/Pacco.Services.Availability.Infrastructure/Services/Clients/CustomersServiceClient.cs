using Convey.HTTP;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Srervices.Clients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Services.Clients
{
    internal sealed class CustomersServiceClient : ICustomersServiceClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _url;

        //abstrakcja nad httpclientem
        public CustomersServiceClient(IHttpClient httpClient, HttpClientOptions options)
        {
            _httpClient = httpClient;
            _url = options.Services["customers"];
        }

        public Task<CustomerStateDto> GetStateAsync(Guid customerId)
        => _httpClient.GetAsync<CustomerStateDto>($"{_url}/customers/{customerId}/state");//dokladna sciezke endpointa pobieram z projektu customers
    }
}
