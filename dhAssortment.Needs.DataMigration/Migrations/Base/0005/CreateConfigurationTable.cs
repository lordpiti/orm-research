using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    [CustomMigration(description: "Create the configuration table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 05)]
    public class CreateConfigurationTable : Migration
    {
        private readonly AppSettings appSettings;

        public CreateConfigurationTable(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override void Up()
        {
            this.Create.Table("configuration").InSchema(this.appSettings.DefaultSchemaName)
                .WithColumn("configuration_id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200)
                .WithColumn("value").AsString(500)
                .WithColumn("row_guid").AsGuid()
                .WithColumn("modified_date").AsDateTime();
        }

        public override void Down()
        {
            this.Delete.Table("configuration")
                .InSchema(this.appSettings.DefaultSchemaName);
        }
    }
}