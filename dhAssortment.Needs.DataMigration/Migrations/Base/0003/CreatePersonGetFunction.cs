using System.IO;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;
using Scriban;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    /// <summary>
    /// Uses https://github.com/lunet-io/scriban
    /// </summary>
    [Tags("FF", "FB")]
    [Tags("FFFFF")]
    [CustomMigration(description: "Create the functions needed for person table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 03)]
    public class CreatePersonGetFunction : Migration
    {
        private readonly AppSettings appSettings;

        public CreatePersonGetFunction(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override void Up()
        {
            var sqlScriptFile = File.ReadAllText("Migrations\\Base\\0003\\person_get.sql");
            var template = Template.Parse(sqlScriptFile);
            var result = template.Render(new { schema_name = this.appSettings.DefaultSchemaName });

            this.Execute.Sql(result);
        }

        public override void Down()
        {
            var sqlScriptFile = File.ReadAllText("Migrations\\Base\\0003\\person_get_drop_function.sql");
            var template = Template.Parse(sqlScriptFile);
            var result = template.Render(new { schema_name = this.appSettings.DefaultSchemaName });

            this.Execute.Sql(result);
        }
    }
}
