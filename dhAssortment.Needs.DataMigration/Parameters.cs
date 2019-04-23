using System.Collections.Generic;

namespace dhAssortment.Needs.DataMigration
{
    public static class Parameters
    {
        public static bool PreviewOnly { get; set; }

        public static string RollbackToVersion { get; set; }

        public static List<string> Tags { get; set; }

        public static bool UseSqlite { get; set; }

        ////public static string DefaultSchemaName => "person";
    }
}