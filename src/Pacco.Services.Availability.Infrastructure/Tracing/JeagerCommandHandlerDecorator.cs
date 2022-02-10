using Convey.CQRS.Commands;
using OpenTracing;
using OpenTracing.Tag;
using System;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Tracing
{
    //dekorujemy wszystkie nasze command handlery   
    public class JeagerCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly ITracer _tracer;

        public JeagerCommandHandlerDecorator(ICommandHandler<T> handler, ITracer tracer)
        {
            _handler = handler;
            _tracer = tracer;
        }

        public async Task HandleAsync(T command)
        {
            var commandName = command.GetType().Name;

            //cos co nasz span uruchomi
            using var scope = BuildScope(commandName);

            var span = scope.Span;

            try
            {
                span.Log($"Handling a message: {commandName}");
                await _handler.HandleAsync(command);
                span.Log($"Handled a message: {commandName}");
            }
            catch (Exception ex)
            {
                span.Log(ex.Message);
                span.SetTag(Tags.Error, true);
                throw;
            }
        }

        //metoda pomocnicza budująca scope
        private IScope BuildScope(string commandName)
        {
            var scope = _tracer
                .BuildSpan($"handling-{commandName}")//nazwa komendy ktora aktualnie przetwarzamy
                .WithTag($"message-name", commandName); //otagowanie

            //sprawdzenie czy aktualnie posiadamy aktywny spam zey ew sie do niedo dolaczyc
            if (_tracer.ActiveSpan is { })//is null
            {
                scope.AddReference(References.ChildOf, _tracer.ActiveSpan.Context);
            }

            //wystartuj aktywny span
            return scope.StartActive(true);
        }
    }
}
