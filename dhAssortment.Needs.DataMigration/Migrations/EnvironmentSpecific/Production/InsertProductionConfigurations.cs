using System;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    [Tags("US", "UK")]
    [Tags("Production")]
    [CustomMigration(description: "Insert Production specific data into the configuration table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 07)]
    public class InsertProductionConfigurations : Migration
    {
        private readonly AppSettings appSettings;

        public InsertProductionConfigurations(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override void Up()
        {
            this.Insert
                .IntoTable("configuration")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                    new
                    {
                        name = "ShopUrl",
                        value = "https://production.dunnhumby.com/shop",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    })
                .Row(
                    new
                    {
                        name = "ShelfReviewUrl",
                        value = "https://production.dunnhumby.com/shelfreview",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    });
        }

        public override void Down()
        {
            this.Delete
                .FromTable("configuration")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                    new
                    {
                        name = "ShopUrl",
                        value = "https://production.dunnhumby.com/shop"
                    });
            this.Delete
                .FromTable("configuration")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                    new
                    {
                        name = "ShelfReviewUrl",
                        value = "https://production.dunnhumby.com/shelfreview"
                    });
        }
    }
}