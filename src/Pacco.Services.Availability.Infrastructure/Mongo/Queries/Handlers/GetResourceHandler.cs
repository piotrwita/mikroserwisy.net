using Convey.CQRS.Queries;
using MongoDB.Driver;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Queries;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Queries.Handlers
{
    //ukrywanie implementacji
    internal sealed class GetResourceHandler : IQueryHandler<GetResource, ResourceDto>
    {
        private readonly IMongoDatabase _database;

        public GetResourceHandler(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<ResourceDto> HandleAsync(GetResource query)
        {
            var document = await _database.GetCollection<ResourceDocument>("resources")
                .Find(r => r.Id == query.ResourceId)
                .SingleOrDefaultAsync();    

            return document?.AsDto(); //sprawdzamy czy null, jesli tak to serwer zwraca 404
        }
    }
}
