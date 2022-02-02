using Convey;
using Convey.CQRS.Queries;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Core.Repositories;
using Pacco.Services.Availability.Infrastructure.Exceptions;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Infrastructure.Mongo.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Infrastructure
{
    public static class Extensions
    {
        //coś na wzór iapplicationbuilder - opakowuje application w service collection (kontener) - rejestracja zależności
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
        {
            builder.Services.AddTransient<IResourcesRepository, ResourcesMongoRepository>(); //rejestracja adaptera

            builder.AddQueryHandlers()
                    .AddInMemoryQueryDispatcher()
                    .AddErrorHandler<ExceptionToResponseMapper>() //rejestracja w kontenerze jako singleton middleware i nasz interface jako typ t
                    .AddMongo()//dodajemy bazkę mongoDB
                    .AddMongoRepository<ResourceDocument, Guid>("resources"); //rejestrujemy mongo repo

            return builder;
        }

        //coś na wzór asp middleware - szereg kolejnych metod pozwalających na wpinanie/ setupowanie
        public static IApplicationBuilder UserInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler() //middleware ktory wpina sie w asp.net.core, gdy wpada nam request(http kontekst) jest globalny try catch, probujemy wykonac kolejny krok w middleware jezeli sie nie powiedzie, logujemy blad i zwracamy response serwera z odpowiednim bledem
                .UseConvey();

            return app;
        }
    }
}
