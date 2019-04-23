using System;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    [Tags("US")]
    [Tags("Production", "Staging")]
    [CustomMigration(description: "Insert US specific data into the product_category table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 08)]
    public class InsertUsProductCategories : MigrationTestExtended
    {
        private readonly AppSettings appSettings;

        public InsertUsProductCategories(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override void Up()
        {
            this.Insert
                .IntoTable("product_category")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                    new
                    {
                        name = "Zucchini",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    })
                .Row(
                    new
                    {
                        name = "Candies",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    })
                .Row(
                    new
                    {
                        name = "Cookies",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    });
        }

        public override void Down()
        {
            ////Delete.Table("product_category");
        }
    }
}