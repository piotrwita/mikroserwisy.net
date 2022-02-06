using NSubstitute;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Commands.Commands;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Application.Srervices;
using Pacco.Services.Availability.Application.Srervices.Clients;
using Pacco.Services.Availability.Core.Entietes;
using Pacco.Services.Availability.Core.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pacco.Services.Availability.Tests.Unit.Application.Handlers
{
    public class ReseveResourceHandlerTests
    {
        private Task Act(ReserveResource command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_invalid_resource_id_reserve_resource_should_throw_an_exception()
        {
            var command = new ReserveResource(Guid.NewGuid(),DateTime.UtcNow,0,Guid.NewGuid());

            var exception = await Record.ExceptionAsync(async() => await Act(command));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ResourceNotFoundException>(); 
        }

        [Fact]
        public async Task given_valid_resource_id_for_valid_customer_reserve_resource_should_succeed()
        {
            var command = new ReserveResource(Guid.NewGuid(), DateTime.UtcNow, 0, Guid.NewGuid());
            var resource = Resource.Create(new AggregateId(), new[] { "tag" });
            
            //w przypadku wywolania operacji get async powinien zwrocic nam ten obiekt resource
            _resourcesRepository.GetAsync(command.ResourceId).Returns(resource);

            var customerState = new CustomerStateDto
            {
                State = "valid"
            };

            _customersServiceClient.GetStateAsync(command.CustomerId).Returns(customerState);

            await Act(command);

            //otrzymalo
            //jezeli bym nie mial resource w tym tescie to robie arg
            //_resourcesRepository.Received().UpdateAsync(Arg.Is<Resource>(r => r.Id == command.ResourceId);
            await _resourcesRepository.Received().UpdateAsync(resource);

            //upewniamy sie ze nasz eventprocessor otrzymal na wejscie operacje processasync dla naszych resource events
            await _eventProcessor.Received().ProcessAsync(resource.Events);
        }
            
        // w xunit konstruktor i dispose jako typ czyszczenia
        #region Arrange

        private readonly ReseveResourceHandler _handler;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly ICustomersServiceClient _customersServiceClient;
        private readonly IEventProcessor _eventProcessor;

        public ReseveResourceHandlerTests()
        {
            //dzieki temu ze tworzymy je przez substitute mozemy modelowac ich zachowanie i sterowac przeplywem flow w ramach wywolania handlera
            _resourcesRepository = Substitute.For<IResourcesRepository>();
            _customersServiceClient = Substitute.For<ICustomersServiceClient>();
            _eventProcessor = Substitute.For<IEventProcessor>();

            _handler = new ReseveResourceHandler(_resourcesRepository, _customersServiceClient, _eventProcessor);
        }


        #endregion
    }
}
