using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Pixel.IRIS5.API.Mobile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
               {
                   //webBuilder.UseKestrel();
                   webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                   webBuilder.UseIISIntegration();
                   webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                   {
                       var env = hostingContext.HostingEnvironment;

                       // find the shared folder in the parent folder
                       //var sharedFolder = Path.Combine(env.ContentRootPath, "..", "Shared");

                       //load the SharedSettings first, so that appsettings.json overrwrites it
                       //config
                       // //.AddJsonFile(Path.Combine(sharedFolder, "GlobalSettings.json"), optional: true)
                       // .AddJsonFile("GlobalSettings.json", optional: true)
                       // .AddJsonFile("appsettings.json", optional: true)
                       // .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                       config
                      //.AddJsonFile(Path.Combine(sharedFolder, "GlobalSettings.json"), optional: true)
                      .AddJsonFile("appsettings.json", optional: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                       config.AddEnvironmentVariables();
                   });

                   webBuilder.UseStartup<Startup>();
               });
    }
}
