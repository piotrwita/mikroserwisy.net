using Convey.CQRS.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Queries;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Queries.Handlers
{
    //ukrywanie implementacji
    internal sealed class GetResourcesHandler : IQueryHandler<GetResources, IEnumerable<ResourceDto>>
    {
        //nie używamy repo, nie chcemy by nasze repo mialo info ze bedzie uzywane do wykonywania kwerend
        private readonly IMongoDatabase _database;

        public GetResourcesHandler(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<ResourceDto>> HandleAsync(GetResources query)
        {
            var collection = _database.GetCollection<ResourceDocument>("resources");

            if(query.Tags is null || !query.Tags.Any())
            {
                //zwracamy wszystkie dokumenty poniewaz nie mamy ustawionego filtra
                var allDocuments = await collection.Find(_ => true).ToListAsync();

                return allDocuments.Select(d => d.AsDto());
            }

            //jeszcze nie materializujemy zapytania
            var documents = collection.AsQueryable();
            documents = query.MatchAllTags
                ? documents.Where(d => d.Tags.All(t => d.Tags.Contains(t)))
                : documents.Where(d => d.Tags.Any(t => d.Tags.Contains(t)));

            //materializacja
            var resources = await documents.ToListAsync();

            return resources.Select(d => d.AsDto());
        }
    }
}
