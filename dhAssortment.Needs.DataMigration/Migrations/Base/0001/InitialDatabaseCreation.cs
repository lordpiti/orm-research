using System;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    [CustomMigration(description: "Create the initial database", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 01)]
    public class InitialDatabaseCreation : Migration
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<InitialDatabaseCreation> logger;

        public InitialDatabaseCreation(IOptions<AppSettings> appSettings, ILogger<InitialDatabaseCreation> logger)
        {
            this.appSettings = appSettings.Value;
            this.logger = logger;
        }

        public override void Up()
        {
            this.logger.LogDebug("Start initial database creation...");

            this.Create.Schema(this.appSettings.DefaultSchemaName);

            this.Create.Table("Product").InSchema(this.appSettings.DefaultSchemaName)
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString()
                .WithColumn("categoryId").AsInt64()
                .WithColumn("Unit_Price").AsDecimal();

            // this.Insert.IntoTable("business_entity")
            //    .InSchema(this.appSettings.DefaultSchemaName)
            //    .Row(
            //    new
            //    {
            //        row_guid = Guid.NewGuid(),
            //        modified_date = DateTime.Now
            //    });
        }

        public override void Down()
        {
            this.Delete.Table("Product")
                .InSchema(this.appSettings.DefaultSchemaName);
            this.Delete.Schema(this.appSettings.DefaultSchemaName);
        }
    }
}
