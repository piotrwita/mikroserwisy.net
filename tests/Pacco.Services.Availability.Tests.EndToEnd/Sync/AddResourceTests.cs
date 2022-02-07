 
using Newtonsoft.Json;
using Pacco.Services.Availability.Api;
using Pacco.Services.Availability.Application.Commands;
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
    public class AddResourceTests : IDisposable, IClassFixture<PaccoApplicationFactory<Program>>
    {
        //dzieki temu typowi mozemy zwrocic status kodu
        private Task<HttpResponseMessage> Act(AddResource command) => _httpClient.PostAsync("resources", GetContent(command));

        [Fact]
        public async Task add_resource_endpoint_should_return_http_status_code_created()
        {
            var command = new AddResource(Guid.NewGuid(), new[] { "tag" });

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task add_resource_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddResource(Guid.NewGuid(), new[] { "tag" });

            //nie musze dawac response bo wiem ze sie nie wywali - sprawdza to test poprzedni
            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.ResourceId);

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ResourceId);
            document.Tags.ShouldBe(command.Tags);
        }


        #region Arrange

        private readonly HttpClient _httpClient;

        private readonly MongoDbFixture<ResourceDocument, Guid> _mongoDbFixture;
        //nasz setup
        public AddResourceTests(PaccoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ResourceDocument, Guid>("resources");

            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        public void Dispose()
        {
            //po zakonczeniu testu baza sie automatycznie usuwa
            _mongoDbFixture.Dispose();
        }

        //mozna dodac do helpera
        private static StringContent GetContent(object value)
            => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

        #endregion
    }
}
