using DataAccess;
using LinqToDB.Configuration;
using System.Collections.Generic;

namespace Linq2db.Tests
{

    public class SqliteDbSettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders
        {
            get { yield break; }
        }

        public string DefaultConfiguration => "Microsoft.Data.Sqlite";
        public string DefaultDataProvider => "Microsoft.Data.Sqlite";

        public string ConnectionString { get; set; }

        public SqliteDbSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "Northwind",
                        ProviderName = "Microsoft.Data.Sqlite",
                        ConnectionString = ConnectionString
                    };
            }
        }

    }
}
