 
using Newtonsoft.Json;
using Pacco.Services.Availability.Api;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Events;
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

namespace Pacco.Services.Availability.Tests.Integration.Async
{
    public class AddResourceTests : IDisposable, IClassFixture<PaccoApplicationFactory<Program>>
    {
        private Task Act(AddResource command) => _rabbitMqFixture.PublishAsync(command, Exchange);
 
        [Fact]
        public async Task add_resource_command_should_add_document_with_given_id_to_database()
        {
            var command = new AddResource(Guid.NewGuid(), new[] { "tag" });

            //subskrybcja pod callback ktory przyjdzie z systemu
            //<jakie zdarzenie, co bedziemy chciecli pobrac (co sie spodziewamy ze w bazie zostanie zapisane) 
            //task jaki wykonuje, id zasobu jaki sie spodziewamy 
            var tcs = _rabbitMqFixture.SubscribeAndGet<ResourceAdded, ResourceDocument>(
                Exchange, _mongoDbFixture.GetAsync, command.ResourceId);

            //moment publisha
            await Act(command);

            //kluczowe jest to ze tcs nie ma await wiec nie czekamy
            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ResourceId);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private const string Exchange = "availability";
        private readonly MongoDbFixture<ResourceDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public AddResourceTests(PaccoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ResourceDocument, Guid>("resources");
            _rabbitMqFixture = new RabbitMqFixture();
            factory.Server.AllowSynchronousIO = true; 
        }

        public void Dispose()
        {
            //po zakonczeniu testu baza sie automatycznie usuwa
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}
