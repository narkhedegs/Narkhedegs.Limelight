using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

[assembly: InternalsVisibleTo("Narkhedegs.Limelight.UnitTests")]

namespace Narkhedegs.Limelight
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        static int Main(string[] arguments)
        {
            var exitCode = 0;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                _serviceProvider = ConfigureServices();

                var commandLineOptionsParserResult =
                    Parser.Default
                        .ParseArguments<Options>(arguments);

                if (commandLineOptionsParserResult.Tag == ParserResultType.Parsed)
                {
                    commandLineOptionsParserResult.WithParsed(options =>
                    {
                        var spotlightImageSaver = _serviceProvider.GetRequiredService<ISpotlightImageSaver>();

                        Log.Information(
                            $"Saving windows spotlight images to {Constants.LimelightImageDirectoryPath} directory.");
                        spotlightImageSaver.Save();
                    });
                }
                else
                {
                    commandLineOptionsParserResult.WithNotParsed(errors => { });
                }
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Limelight terminated unexpectedly. Please contact the author.");
                exitCode = 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return exitCode;
        }

        private static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(loggingBuilder => { loggingBuilder.AddSerilog(); });
            serviceCollection.AddScoped<IFileSystem, FileSystem>();
            serviceCollection.AddScoped<IJpegFormatTester, JpegFormatTester>();
            serviceCollection.AddScoped<ISpotlightImageSaver, SpotlightImageSaver>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}