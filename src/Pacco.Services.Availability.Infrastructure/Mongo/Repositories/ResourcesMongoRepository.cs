using Convey.Persistence.MongoDB;
using MongoDB.Driver;
using Pacco.Services.Availability.Core.Entietes;
using Pacco.Services.Availability.Core.Repositories;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using System;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Repositories
{
    internal sealed class ResourcesMongoRepository : IResourcesRepository
    {
        private readonly IMongoRepository<ResourceDocument, Guid> _repository;

        public ResourcesMongoRepository(IMongoRepository<ResourceDocument, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<Resource> GetAsync(AggregateId id)
        {
            var document = await _repository.GetAsync(r => r.Id == id);
            return document?.AsEntity();//jezeli jest nullem to null, jezeli nie to mapujemy naszą encje;
        }

        public Task<bool> ExistsAsync(AggregateId id) 
            => _repository.ExistsAsync(r => r.Id == id);

        public Task AddAsync(Resource resource)
            => _repository.AddAsync(resource.AsDocument());

        //na poziomie mongo operacje na poziomie pojedynczego modelu są atomowe
        public Task UpdateAsync(Resource resource)
            => _repository.Collection.ReplaceOneAsync(r => r.Id == resource.Id &&
                r.Version < resource.Version,//upewnienie czy wersja ktora nadpisujemy jest najswiezszą
                resource.AsDocument());

        public Task DeleteAsync(AggregateId id)
            => _repository.DeleteAsync(id);    
    }
}
