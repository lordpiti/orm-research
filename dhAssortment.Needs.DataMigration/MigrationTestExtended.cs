using FluentMigrator;

namespace dhAssortment.Needs.DataMigration
{
    public abstract class MigrationTestExtended : Migration
    {
        public new object ApplicationContext { get; set; }
    }
}
