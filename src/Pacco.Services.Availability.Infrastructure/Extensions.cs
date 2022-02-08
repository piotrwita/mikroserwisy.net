using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.Docs.Swagger;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.Mongo;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Application;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Events;
using Pacco.Services.Availability.Application.Srervices;
using Pacco.Services.Availability.Application.Srervices.Clients;
using Pacco.Services.Availability.Core.Repositories;
using Pacco.Services.Availability.Infrastructure.Decorators;
using Pacco.Services.Availability.Infrastructure.Exceptions;
using Pacco.Services.Availability.Infrastructure.Logging;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Infrastructure.Mongo.Repositories;
using Pacco.Services.Availability.Infrastructure.Services;
using Pacco.Services.Availability.Infrastructure.Services.Clients;
using Pacco.Services.Identity.Application.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

//widoczne internale dla projektu endtoend
[assembly: InternalsVisibleTo("Pacco.Services.Availability.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pacco.Services.Availability.Tests.Integration")]

namespace Pacco.Services.Availability.Infrastructure
{
    public static class Extensions
    {
        //coś na wzór iapplicationbuilder - opakowuje application w service collection (kontener) - rejestracja zależności
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
        {
            builder.Services.AddTransient<IResourcesRepository, ResourcesMongoRepository>(); //rejestracja adaptera
            builder.Services.AddTransient<IMessageBroker, MessageBroker>();
            builder.Services.AddTransient<IEventProcessor, EventProcessor>();
            builder.Services.AddSingleton<IEventMapper, EventMapper>();
            builder.Services.AddTransient<ICustomersServiceClient, CustomersServiceClient>();

            
            builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandler<>));//rejestracja dekoratora , musi byc try bo jakby nie bylo zadnej implementacji to by wywalilo, rejestracja jako open generic (czyli typeof)

            //scanowanie assembly - chcemy zeby nasze assembly zrejestrowalo wszystkie implementacje idomaineventhandler
            //zakladajac ze sa one tutaj poda w naszych wartwach
            builder.Services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            builder.AddQueryHandlers()
                    .AddInMemoryQueryDispatcher()
                    .AddErrorHandler<ExceptionToResponseMapper>() //rejestracja w kontenerze jako singleton middleware i nasz interface jako typ t
                    .AddExceptionToMessageMapper<ExceptionToMessageMapper>() //rejestracja w kontenerze jako singleton middleware i nasz interface jako typ t
                    .AddMongo()//dodajemy bazkę mongoDB
                    .AddMongoRepository<ResourceDocument, Guid>("resources") //rejestrujemy mongo repo
                    .AddRabbitMq()
                    .AddSwaggerDocs()
                    .AddWebApiSwaggerDocs()
                    .AddMessageOutbox(o => o.AddMongo()) //skonfigurowana na mongo//wpinka do obslugi przypadkow gdy siec sie zerwie a my chcemy miec pewnosc obslugi naszych wiadomosci lub tego ze nie beda one przetworzone kilkukrotnie przychodzac do nasz
                    .AddHttpClient() //rejestracja z poziomu convey
                    .AddConsul()
                    .AddFabio()
                    .AddHandlersLogging(); //wywolanie rozszerzenia logowania

            return builder;
        }

        //coś na wzór asp middleware - szereg kolejnych metod pozwalających na wpinanie/ setupowanie
        public static IApplicationBuilder UserInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler() //middleware ktory wpina sie w asp.net.core, gdy wpada nam request(http kontekst) jest globalny try catch, probujemy wykonac kolejny krok w middleware jezeli sie nie powiedzie, logujemy blad i zwracamy response serwera z odpowiednim bledem
                .UseConvey()
                .UsePublicContracts<ContractAttribute>() // wpiecie naszych konkraktow oznaczonych markerem contractattribute
                .UseSwaggerDocs() //dodanie jako middleware
                .UseRabbitMq()//zawsze na samym koncu - po tym tylko suby
                .SubscribeEvent<SignedUp>()//wywołuje event zdefiniowany na poziomie mikroserwisu//rejestracja czy subskrypcja - usluga dostepnosci jest zainteresowana wiadomoscia signup z identity
                .SubscribeCommand<AddResource>()
                .SubscribeCommand<ReserveResource>();

            return app;
        }
    }
}
