using Pacco.Services.Availability.Core.Entietes;
using Pacco.Services.Availability.Core.Events;
using Pacco.Services.Availability.Core.Exceptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Pacco.Services.Availability.Tests.Unit.Core.Entities
{
    public class CreateResourceTests
    {
        //arrange - przygotowanie calego srodowiska testowego
        //setup ewentualnie stworzenie objektow

        //act - kluczowa faza - wywolywanie logiki do przetestowania

        //assert - czy wynik dzialania fazy act jest zgodny z naczymi oczekiwaniami

        //przyjmuje dwie rzeczy ktore potrzebujemy do utworzenia konktretnego zasobu
        private Resource Act(AggregateId id, IEnumerable<string> tags) => Resource.Create(id, tags);

        [Fact] //że test
        public void given_valid_id_and_tags_resource_should_be_created()
        {
            //Arrange
            var id = new AggregateId();
            var tags = new[] { "tag" };

            //Act
            var resource = Act(id, tags);

            //Assert
            resource.ShouldNotBeNull();
            resource.Id.ShouldBe(id);
            resource.Tags.ShouldBe(tags);

            //sprawdzenie czy event sie dodal
            resource.Events.Count().ShouldBe(1);

            var @event = resource.Events.Single();
            @event.ShouldBeOfType<ResourceCreated>();
        }

        [Fact]
        public void given_empty_tags_resource_should_throw_an_exception()
        {
            //Arrange
            var id = new AggregateId();
            var tags = Enumerable.Empty<string>();

            //Act
            //oczekujemy wyjątku
            var exception = Record.Exception(() => Act(id, tags));

            //Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<MissingResourceTagsException>();
        }
    }
}
