using LinqToDB.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class MySettings : ILinqToDBSettings
    {
        private ConnectionStringSettings _connectionStringSettings;

        public MySettings(ConnectionStringSettings options)
        {
            _connectionStringSettings = options;
        }

        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "PostgreSQL";
        public string DefaultDataProvider => "PostgreSQL";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    _connectionStringSettings;
            }
        }
    }
}
