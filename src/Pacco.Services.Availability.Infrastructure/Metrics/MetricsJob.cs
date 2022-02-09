using App.Metrics;
using App.Metrics.Gauge;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pacco.Services.Availability.Infrastructure.Metrics
{
    //zwykly hosting extenstion z posiomu aspnetore
    public class MetricsJob : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MetricsOptions _metricsOptions;
        private readonly ILogger<MetricsJob> _logger;

        //1 metryka - ilosc aktualnych watkow
        private readonly GaugeOptions _threads = new GaugeOptions
        {
            Name = "threads"
        };
        //2 metryka 
        private readonly GaugeOptions _workingSet = new GaugeOptions
        {
            Name = "working_set"
        };

        public MetricsJob(IServiceScopeFactory serviceScopeFactory, MetricsOptions metricsOptions,
            ILogger<MetricsJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _metricsOptions = metricsOptions;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(!_metricsOptions.Enabled)
            {
                _logger.LogInformation("Metrics are disabled.");
            }

            //czyli co 5 sekund przez caly czas dzialania aplikacji
            while(!stoppingToken.IsCancellationRequested)
            {
                //
                using(var scope = _serviceScopeFactory.CreateScope())
                {
                    //pozwala zmieniac/ mierzyc wartosci metryk
                    var metricRoot = scope.ServiceProvider.GetRequiredService<IMetricsRoot>();

                    var process = Process.GetCurrentProcess();
                    //operacja na danym typie metryki - mierz - ustaw wartosc
                    metricRoot.Measure.Gauge.SetValue(_threads, process.Threads.Count);
                    metricRoot.Measure.Gauge.SetValue(_workingSet, process.WorkingSet64);
                }


                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
