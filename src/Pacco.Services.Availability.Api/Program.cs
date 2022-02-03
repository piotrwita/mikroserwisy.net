using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Convey;
using Convey.WebApi;
using Pacco.Services.Availability.Application;
using Pacco.Services.Availability.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Convey.WebApi.CQRS;
using Convey.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Queries;
using Pacco.Services.Availability.Application.DTO;
using System.Collections.Generic;

namespace Pacco.Services.Availability.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await CreateWebHostBuilder(args)
                .Build()
                .RunAsync();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddConvey() //rejestracja Convey - tworzenie ConveyBuilder, który ma przekazany Services
                    .AddWebApi() //pomaga przy definicji routingu - zbior metod pomocniczych, w ktore mozemy sobie opakowac - takie fluent api - i potem wspoldzielic miedzy mikroserwisami
                    .AddApplication() //rejestracja z warstwy aplikacji
                    .AddInfrastructure() //rejestracja z warstwy infrastruktury
                    .Build())
                //wpinka w nasz middleware
                .Configure(app => app
                    .UserInfrastructure() //rejestracja z warstwy infrastruktury
                    .UseRouting() //korzystanie z routingu
                                  //podejscie klasyczne do controllerow
                                  //.UseEndpoints(e => e.MapControllers()));  //umożliwia korzystanie z controllerów 
                                  //podejście convey do controllerow
                    .UseDispatcherEndpoints(endpoints => endpoints
                        .Get("", ctx => ctx.Response.WriteAsync(
                            ctx.RequestServices.GetService<AppOptions>().Name))
                        .Get<GetResources, IEnumerable<ResourceDto>>("resources")
                        .Get<GetResource, ResourceDto>("resources/{resourceId}")
                        .Post<AddResource>("resources", afterDispatch: (cmd, ctx) =>
                            ctx.Response.Created($"resources/{cmd.ResourceId}"))
                        .Post<ReserveResource>("resources/{resourceId}/reservations/{dateTime}")));
    }
}