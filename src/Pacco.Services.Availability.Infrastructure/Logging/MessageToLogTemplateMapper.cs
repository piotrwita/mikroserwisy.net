using Convey.Logging.CQRS;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pacco.Services.Availability.Infrastructure.Logging
{
    internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
    {
        //sprawdzenie  czy w slowniku istnieje wartosc , pobieramy po kluczu i sprawdzamy czy wartosc istnieje
        //jezeli istnieje to zwracamy jezeli nie to null
        public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
            => Templates.TryGetValue(message.GetType(), out var template) ? template : null;

        //prywatny słownik - dla jakiej komendy eventu i co chcemy zalogowac
        private static IReadOnlyDictionary<Type, HandlerLogTemplate> Templates = 
            new Dictionary<Type, HandlerLogTemplate>    
            {
                //klucz - wartosc
                [typeof(AddResource)] = new HandlerLogTemplate
                {
                    Before = "Adding a resource with id: {ResourceId}.",
                    After = "Added a resource with id: {ResourceId}.",
                    OnError = new Dictionary<Type, string> //wewnetrzny slownik
                    {
                        [typeof(ResourceAlreadyExistsException)] = "Resource with id: {ResourceId} already exists."
                    }
                }
            };
    }
}
