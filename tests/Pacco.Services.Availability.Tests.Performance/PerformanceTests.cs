using NBomber.CSharp;
using NBomber.Http.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pacco.Services.Availability.Tests.Performance
{
    public class PerformanceTests
    {
        [Fact]
        //wysylamy zapytanie i testujemy jak duza ilosc rzadan na sekunde przetworzy
        public void get_resources()
        {
            //w klasie pomocniczej to dac
            const string url = "http:localhost:5001";
            const string stepName = "init";
            //czas trwania testu 3 s
            const int duration = 3;
            //czekuje ze przetworzy 100 requestow na s
            const int expectedRps = 100;

            //gdzie uderzam
            var endpoint = $"{url}/resources";

            //tworzymy nowy krok - tworzymy kontext w ktorym oczekujemy ze dla metody get pod wskazana sciezke
            var step = HttpStep.Create(stepName, ctx =>
                 Task.FromResult(Http.CreateRequest("GET", endpoint)
                    .WithCheck(response => Task.FromResult(response.IsSuccessStatusCode)))); //oprocz tego chce zeby wszystkie requesty mialy taki status

            //Assert
            var assertions = new[]
            {
                Assertion.ForStep(stepName, s => s.RPS >= expectedRps), //typ statistic wyiaga roznego rodzaju dane - my rpsy sprawdzamy
                Assertion.ForStep(stepName, s => s.OkCount >= expectedRps * duration) // oczekuje ze poprawne odpowiedzi beda na co najmniej takim poziomie
            };

            //scenariusz testowy
            var scenario = ScenarioBuilder.CreateScenario("GET resources", step)
                .WithConcurrentCopies(1) //jedno wywolanie rownolegle
                .WithOutWarmUp() //dziala to tak ze przez n sekund aplikacja sie rozgrzewa 
                .WithDuration(TimeSpan.FromSeconds(duration))//czas testow
                .WithAssertions(assertions);//przekazujemy nasze asercje

            //wpiecie pod framework testowy - rejestracja
            NBomberRunner.RegisterScenarios(scenario)
                .RunTest();
        }
    }
}
