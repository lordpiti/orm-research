using System;
using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations
{
    [CustomMigration(description: "Create the person table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 02)]
    public class CreatePersonTable : Migration
    {
        private readonly AppSettings appSettings;

        public CreatePersonTable(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override void Up()
        {
            this.Create.Table("Category").InSchema(this.appSettings.DefaultSchemaName)
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString();

            this.Create.ForeignKey("fk_category_product")
                .FromTable("Product")
                .InSchema(this.appSettings.DefaultSchemaName)
                .ForeignColumn("categoryId")
                .ToTable("Category")
                .InSchema(this.appSettings.DefaultSchemaName)
                .PrimaryColumn("id");

            this.Insert
                .IntoTable("Category")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Category 1"
                });

            this.Insert
                .IntoTable("Category")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Category 2"
                });

            this.Insert
                .IntoTable("Category")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Category 3"
                });

            this.Insert
                .IntoTable("Product")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Product 1",
                    categoryId = 1,
                    Unit_Price = 10
                });

            this.Insert
                .IntoTable("Product")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Product 2",
                    categoryId = 2,
                    Unit_Price = 20
                });

            this.Insert
                .IntoTable("Product")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Product 3",
                    categoryId = 3,
                    Unit_Price = 30
                });

            this.Insert
                .IntoTable("Product")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Product 4",
                    categoryId = 1,
                    Unit_Price = 40
                });

            this.Insert
                .IntoTable("Product")
                .InSchema(this.appSettings.DefaultSchemaName)
                .Row(
                new
                {
                    name = "Product 5",
                    categoryId = 1,
                    Unit_Price = 50
                });
        }

        public override void Down()
        {
            this.Delete.Table("Category")
                .InSchema(this.appSettings.DefaultSchemaName);
        }
    }
}
