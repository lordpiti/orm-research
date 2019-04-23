using System;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    [CustomMigration(description: "Create the product_category table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 04)]
    public class CreateProductCategoryTable : Migration
    {
        private readonly AppSettings appSettings;

        public CreateProductCategoryTable(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override void Up()
        {
            this.Create.Table("product_category").InSchema(this.appSettings.DefaultSchemaName)
                .WithColumn("product_category_id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200)
                .WithColumn("row_guid").AsGuid()
                .WithColumn("modified_date").AsDateTime();

            // Insert common product categories into product_category table
            this.Insert
                .IntoTable("product_category")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                    new
                    {
                        name = "Pasta",
                        row_guid = Guid.NewGuid(),
                        modified_date = DateTime.Now
                    });
        }

        public override void Down()
        {
            this.Delete.Table("product_category")
                .InSchema(this.appSettings.DefaultSchemaName);
        }
    }
}