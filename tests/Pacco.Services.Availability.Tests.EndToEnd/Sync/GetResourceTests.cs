 
using Newtonsoft.Json;
using Pacco.Services.Availability.Api;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Tests.Shared.Factories;
using Pacco.Services.Availability.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pacco.Services.Availability.Tests.EndToEnd.Sync
{
    public class GetResourceTests : IDisposable, IClassFixture<PaccoApplicationFactory<Program>>
    {
        //act czyli to co wywolujemy
        private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"resources/{_resourceId}");

        //resource nie znajduje sie w bazie
        public async Task get_resource_endpoint_should_retourn_not_found_status_code_if_resource_document_does_not_exist()
        {
            //nie potrzebujemy nawet arrange poniewaz baza nie zawiera id
            var response = await Act();

            response.ShouldBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        //resource nie znajduje sie w bazie
        public async Task get_resource_endpoint_should_retourn_dto_with_correct_data()
        {
            await InsertResourceAsync();

            var response = await Act();

            response.ShouldBeNull();
            //mozna dodac do helpera
            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<ResourceDto>(content);
            dto.Id.ShouldBe(_resourceId);
        }

        #region Arrange

        private readonly Guid _resourceId;
        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ResourceDocument, Guid> _mongoDbFixture;

        //zawsze chcemy miec w get nasze resource dlatego tutaj dodajemy
        private Task InsertResourceAsync()
        => _mongoDbFixture.InsertAsync(new ResourceDocument()
        {
            Id = _resourceId,
            Tags = new[] { "tag" },
            Reservations = new[] 
            {
                new ReservationDocument()
                {
                    Priority = 0,
                    TimeStamp = DateTime.UtcNow.AsDaysSinceEpoch()
                } 
            }     
        });

        //nasz setup
        public GetResourceTests(PaccoApplicationFactory<Program> factory)
        {
            _resourceId = Guid.NewGuid(); //albo jakis sztywny nr stala wawrtosc
            _mongoDbFixture = new MongoDbFixture<ResourceDocument, Guid>("resources");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        public void Dispose()
        {
            //po zakonczeniu testu baza sie automatycznie usuwa
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}
