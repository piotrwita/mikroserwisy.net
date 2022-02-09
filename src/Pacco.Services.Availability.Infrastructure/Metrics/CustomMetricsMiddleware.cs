using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Metrics
{
    //sciezki ktore chcemy trackowac, request ktory znajduje sie na liscie i do nas przychodzi - podbijamy jego counter
    public class CustomMetricsMiddleware : IMiddleware
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly bool _enabled;

        //slownik sciezka - faktyczna instancja danej metryki
        private readonly IDictionary<string, CounterOptions> _metrics = new Dictionary<string, CounterOptions>
        {
            //pobranie i wyslaniem komendy dodaj zasob
            [GetKey("GET", "/resources")] = Query(typeof(GetResources).Name),
            [GetKey("POST", "/resources")] = Query(typeof(AddResource).Name)
        };

        public CustomMetricsMiddleware(IServiceScopeFactory serviceScopeFactory, MetricsOptions metricsOptions)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _enabled = metricsOptions.Enabled;
        }

        //sytuacja ze mam metryke typu komendy i ile razy hednler wykonujacy komende mi wpadl
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if(!_enabled)
            {
                //wychodzimy - wywolujemy kolejny krok w naszym mieddleware
                return next(context);
            }

            //sprawdzamy czy nasz request wpasowuje sie w klucz ktory chcemy implementowac
            var request = context.Request;
            //sprawdzamy czy nasza metryka wystepuje
            if(!_metrics.TryGetValue(GetKey(request.Method,request.Path.ToString()), out var metrics))
            {
                //wychodzimy - przechodzimy dalej z middleware
                return next(context);
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var metricsRoot = scope.ServiceProvider.GetRequiredService<IMetricsRoot>();
            metricsRoot.Measure.Counter.Increment(metrics);

            return next(context);
        }

        //klucz to typ metody http oraz sciezka
        private static string GetKey(string method, string path) => $"{method}:{path}"; 

        //licznik wywolan danej kwerendy /komendy 
        private static CounterOptions Command(string command)
        => new CounterOptions
        {
            Name = "commands",
            Tags = new MetricTags("command", command)
        };
        private static CounterOptions Query(string query)
        => new CounterOptions
        {
            Name = "queries",
            Tags = new MetricTags("query", query)
        };
    }
}
