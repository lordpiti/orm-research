using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Colorful;
using CommandLine;
using dhAssortment.Needs.DataMigration.Configuration;
using dhAssortment.Needs.DataMigration.Migrations;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Console = Colorful.Console;

namespace dhAssortment.Needs.DataMigration
{
    public static class Program
    {
        private const string MigrationGeneratedSqlFolder = "c:\\logs\\";

        public static void Main(string[] args)
        {
            var parsedResults = Parser.Default.ParseArguments<StartupOptions>(args);

            if (parsedResults.Tag == ParserResultType.NotParsed)
            {
                DisplayUsageError();
                return;
            }

            // create service collection
            var services = new ServiceCollection();

            Parameters.RollbackToVersion = ((Parsed<StartupOptions>)parsedResults).Value.RollbackTo;
            Parameters.PreviewOnly = Convert.ToBoolean(((Parsed<StartupOptions>)parsedResults).Value.Preview, CultureInfo.CurrentCulture);
            Parameters.Tags = ((Parsed<StartupOptions>)parsedResults).Value.Tags.Split(',').ToList();
            Parameters.UseSqlite = Convert.ToBoolean(((Parsed<StartupOptions>)parsedResults).Value.Database, CultureInfo.CurrentCulture);

            ConfigureServices(services);

            Console.WriteLine(string.Empty);
            Console.WriteLine("Migrations completed. Press any key to quit.", Color.DarkTurquoise);
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddOptions();
            services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)));
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            // add app
            services.AddTransient<App>();

            var migrationGeneratedSqlFilename =
                $"migrationGeneratedSql-{DateTime.Now:yyyy-MM-ddTHHmmss}-{string.Join("-", Parameters.Tags.ToArray())}.sql";

            ShowStartMessages(MigrationGeneratedSqlFolder, migrationGeneratedSqlFilename);

            if (!Directory.Exists(MigrationGeneratedSqlFolder))
            {
                Directory.CreateDirectory(MigrationGeneratedSqlFolder);
            }

            var output = new StreamWriter($"{MigrationGeneratedSqlFolder}{migrationGeneratedSqlFilename}");
            services
                .AddSingleton<ILoggerProvider>(new SqlScriptFluentMigratorLoggerProvider(output));

            Action<IMigrationRunnerBuilder> getDatabaseRunnerConfiguration = rb =>
            {
                if (Parameters.UseSqlite)
                {
                    rb
                    .AddSQLite() // add sqlite support
                    .WithGlobalConnectionString(
                       configuration.GetSection(nameof(ConnectionStrings))[
                           "SqLite"]) // provide the connection string to database
                    .ScanIn(typeof(InitialDatabaseCreation).Assembly).For
                    .Migrations();
                }
                else
                {
                    rb
                    .AddPostgres() // add postgres support
                    .WithGlobalConnectionString(
                       configuration.GetSection(nameof(ConnectionStrings))[
                           "AdventureWorks"]) // provide the connection string to database
                    .ScanIn(typeof(InitialDatabaseCreation).Assembly).For
                    .Migrations();
                }
            };

            // create service provider
            var serviceProvider = services
                .AddFluentMigratorCore()
                .AddScoped<IMigrationInformationLoader, DefaultMigrationInformationLoader>()
                .ConfigureRunner(getDatabaseRunnerConfiguration)
                .AddLogging(builder =>
                {
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })
                .Configure<RunnerOptions>(opt =>
                {
                    opt.Tags = Parameters.Tags.Count > 0 ? Parameters.Tags.ToArray() : null;
                })
                .Configure<ProcessorOptions>(opt => opt.PreviewOnly = Parameters.PreviewOnly)
                .BuildServiceProvider(false);

            // entry to run app
            serviceProvider.GetService<App>().Run();
        }

        private static void DisplayUsageError()
        {
            Formatter[] message = new Formatter[]
            {
                new Formatter("ENTER", Color.Crimson),
            };

            Console.WriteLineFormatted("Press {0} to quit", Color.Gray, message);
            Console.ReadLine();
        }

        private static void ShowStartMessages(string migrationGeneratedSqlFolder, string migrationGeneratedSqlFilename)
        {
            Formatter[] message;
            if (!Parameters.PreviewOnly)
            {
                message = new[]
                {
                    new Formatter("UPDATE", Color.Red),
                    new Formatter($"{migrationGeneratedSqlFolder}{migrationGeneratedSqlFilename}", Color.Green)
                };
            }
            else
            {
                message = new[]
                {
                    new Formatter("PREVIEW", Color.Green),
                    new Formatter($"{migrationGeneratedSqlFolder}{migrationGeneratedSqlFilename}", Color.Green)
                };
            }

            Console.WriteLineFormatted(
                "Starting migrations in {0} mode." + Environment.NewLine + "SQL will be written to {1}", Color.White, message);

            Console.WriteLine($"Tags passed: {string.Join(",", Parameters.Tags)}");
        }
    }
}
