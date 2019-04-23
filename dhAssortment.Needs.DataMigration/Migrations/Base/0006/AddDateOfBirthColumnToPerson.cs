using dhAssortment.Needs.DataMigration.Configuration;
using FluentMigrator;
using Microsoft.Extensions.Options;

namespace dhAssortment.Needs.DataMigration.Migrations.Base
{
    [CustomMigration(description: "Add column to person table", branchNumber: 1, year: 2019, month: 4, day: 1, hour: 14, minute: 06)]
    public class AddDateOfBirthColumnToPerson : Migration
    {
        private readonly AppSettings appSettings;

        public AddDateOfBirthColumnToPerson(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override void Up()
        {
            this.Alter.Table("person")
                .InSchema(this.appSettings.DefaultSchemaName)
                .AddColumn("date_of_birth").AsDateTime().Nullable();
        }

        public override void Down()
        {
            this.Delete.Column("date_of_birth").FromTable("person").InSchema(this.appSettings.DefaultSchemaName);
        }
    }
}
