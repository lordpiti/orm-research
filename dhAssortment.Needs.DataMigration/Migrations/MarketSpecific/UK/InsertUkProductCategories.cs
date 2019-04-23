using System;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    [Tags("UK")]
    [Tags("Production", "Staging")]
    [CustomMigration(description: "Insert UK specific data into the product_category table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 08)]
    public class InsertUkProductCategories : Migration
    {
        private readonly AppSettings appSettings;

        public InsertUkProductCategories(IOptions<AppSettings> appSettings)
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
                        name = "Courgette",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    })
                .Row(
                    new
                    {
                        name = "Sweets",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    })
                .Row(
                    new
                    {
                        name = "Biscuits",
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