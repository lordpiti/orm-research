using System.Collections.Generic;
using CommandLine;

namespace dhAssortment.Needs.DataMigration
{
    public class StartupOptions
    {
        [Option('t', "tags", Required = true, HelpText = "Tags to be processed.")]
        public string Tags { get; set; }

        [Option('p', "preview", Required = true, HelpText = "Run in preview mode")]
        public string Preview { get; set; }

        [Option('r', "rollbackTo", Required = false, HelpText = "Version to rollback to")]
        public string RollbackTo { get; set; }

        [Option('d', "database", Required = true, HelpText = "use Sqlite")]
        public string Database { get; set; }
    }
}
