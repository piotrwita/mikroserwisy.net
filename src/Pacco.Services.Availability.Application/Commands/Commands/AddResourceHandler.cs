﻿using Convey.CQRS.Commands;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Core.Entietes;
using Pacco.Services.Availability.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Application.Commands.Commands
{
    public class AddResourceHandler : ICommandHandler<AddResource>
    {
        private readonly IResourcesRepository _resourcesRepository;

        public AddResourceHandler(IResourcesRepository resourcesRepository)
        {
            _resourcesRepository = resourcesRepository;
        }

        public async Task HandleAsync(AddResource command)
        {
            //upewniamy sie resource nie istnieje
            if(await _resourcesRepository.ExistsAsync(command.ResourceId))
            {
                throw new ResourceAlreadyExistsException(command.ResourceId);
            }

            //pod spodem dodajemy nasz domenowy event (wywołanie przez static)
            var resource = Resource.Create(command.ResourceId, command.Tags);
            //zapis do bazy
            await _resourcesRepository.AddAsync(resource);
        }
    }
}
