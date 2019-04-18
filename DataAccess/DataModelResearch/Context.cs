using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using Npgsql;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess.DataModelResearch
{
    public class Context : DataConnection
    {
        #region Basic setup

        //public Context()
        //{
        //    if (mappingSchema == null)
        //        mappingSchema = MappingLinq2Db.Do();
        //}

        #endregion

        #region Setup to work with Miniprofiler

        public Context() : base(GetDataProvider(), GetConnection()) {
            if (mappingSchema == null)
                mappingSchema = MappingLinq2Db.Do();
        }

        private static IDataProvider GetDataProvider()
        {
            // you can move this line to other place, but it should be
            // always set before LINQ to DB provider instance creation
            LinqToDB.Common.Configuration.AvoidSpecificDataProviderAPI = true;

            return new LinqToDB.DataProvider.PostgreSQL.PostgreSQLDataProvider("", LinqToDB.DataProvider.PostgreSQL.PostgreSQLVersion.v95);
        }

        private static IDbConnection GetConnection()
        {
            var dbConnection = new NpgsqlConnection(DataConnection.DefaultSettings.ConnectionStrings.FirstOrDefault().ConnectionString);
            return new StackExchange.Profiling.Data.ProfiledDbConnection(dbConnection, MiniProfiler.Current);
        }

        #endregion

        public virtual ITable<Product> Products { get { return this.GetTable<Product>(); } }

        public ITable<Category> Category { get { return this.GetTable<Category>(); } }

        private static MappingSchema mappingSchema;

    }
}
