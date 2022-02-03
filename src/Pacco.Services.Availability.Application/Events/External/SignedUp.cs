using System;
using Convey.CQRS.Events;
using Convey.MessageBrokers;

namespace Pacco.Services.Identity.Application.Events
{
    //tworzymy lokalny kontrakt po stronie naszego api
    //pewne zdarzenia przychodzace z zewnatrz (external)
    //typ kontraktu nie musi byc 1 do 1 - mozna wywalic jakis zbedny
    [Message("identity")]//dinding key - nadpisanie info z jakiej wymiany ta wiadomosc pochodzi
    //bierzemy te nazwe z pola exchange: name z appsettings Identity (czyli miejsca ktore wysyla nam wiadomosc)
    //ten atrybut pozwoli nam utworzyc kolejke i binding do odpowiedniej wymiany
    public class SignedUp : IEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string Role { get; }

        public SignedUp(Guid userId, string email, string role)
        {
            UserId = userId;
            Email = email;
            Role = role;
        }
    }
}