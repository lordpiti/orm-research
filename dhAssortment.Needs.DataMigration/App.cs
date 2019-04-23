using System;
using System.Drawing;
using System.Globalization;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Console = Colorful.Console;

namespace dhAssortment.Needs.DataMigration
{
    public class App
    {
        private const string DbName = "adventureworks";

        private readonly ConnectionStrings connectionStrings;
        private readonly IServiceProvider serviceProvider;

        public App(IOptions<ConnectionStrings> connectionStrings, IServiceProvider serviceProvider)
        {
            this.connectionStrings = connectionStrings.Value;
            this.serviceProvider = serviceProvider;
        }

        public void Run()
        {
            // Ensure that the database exists, if it doesn't then create it:
            if (!Parameters.UseSqlite)
            {
                this.EnsureDbExists();
            }

            // If a rollback version was provided from the command line:
            if (!string.IsNullOrEmpty(Parameters.RollbackToVersion))
            {
                Console.WriteLine($"{Environment.NewLine}Reverting to version {Parameters.RollbackToVersion}...{Environment.NewLine}", Color.Red);
                Console.WriteLine($"Press ENTER to continue", Color.DarkTurquoise);
                Console.ReadLine();
                this.RollbackDatabase();
                return;
            }

            // Put the database update into a scope to ensure that all resources will be disposed.
            using (var scope = this.serviceProvider.CreateScope())
            {
                this.UpdateDatabase();
            }
        }

        private void UpdateDatabase()
        {
            // Instantiate the runner
            var runner = this.serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }

        private void RollbackDatabase()
        {
            var version = Convert.ToInt64(Parameters.RollbackToVersion, CultureInfo.CurrentCulture);
            MigrationRunner runner = (MigrationRunner)this.serviceProvider.GetRequiredService<IMigrationRunner>();

            // Validate the version number that has been passed in
            if (!runner.VersionLoader.VersionInfo.HasAppliedMigration(Convert.ToInt64(Parameters.RollbackToVersion, CultureInfo.CurrentCulture)))
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"Version {Parameters.RollbackToVersion} does not exist. Exiting...", Color.Red);
                Console.WriteLine(string.Empty);
                return;
            }

            runner.MigrateDown(version);
        }

        /// <summary>
        /// Creates the database if it doesn't exist
        /// </summary>
        private void EnsureDbExists()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(this.connectionStrings.Postgres))
            {
                string sql = $"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{DbName}'";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        var i = command.ExecuteScalar();
                        conn.Close();

                        if (i != null && i.ToString().Equals(DbName, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Database {DbName} already exists. Press any key to continue...", Color.DarkTurquoise);
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine($"Database {DbName} does not exist. Creating...");
                            sql = $"CREATE DATABASE {DbName} WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";
                            using (NpgsqlCommand createDatabaseCommand = new NpgsqlCommand(sql, conn))
                            {
                                conn.Open();
                                createDatabaseCommand.ExecuteNonQuery();
                                conn.Close();
                            }

                            Console.WriteLine($"Database {DbName} created! Press any key to continue...", Color.DarkTurquoise);
                            Console.ReadKey();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
