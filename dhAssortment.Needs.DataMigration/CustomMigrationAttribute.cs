namespace dhAssortment.Needs.DataMigration
{
    public class CustomMigrationAttribute : FluentMigrator.MigrationAttribute
    {
        public CustomMigrationAttribute(int branchNumber, int year, int month, int day, int hour, int minute, string description)
            : base(CalculateValue(branchNumber, year, month, day, hour, minute), description)
        {
        }

        private static long CalculateValue(int branchNumber, int year, int month, int day, int hour, int minute)
        {
            return branchNumber * 1000000000000L + year * 100000000L + month * 1000000L + day * 10000L + hour * 100L + minute;
        }
    }
}