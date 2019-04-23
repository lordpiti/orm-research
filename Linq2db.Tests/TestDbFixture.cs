using DataAccess.DataModelResearch;
using System;

namespace Linq2db.Tests
{
    public class TestDbFixture : IDisposable
    {
        public TestDbSetup Setup { get; }

        public TestDbFixture()
        {
            const string scriptFileName = "migrationGeneratedSql-2019-04-24T153851-US-Production.sql"; // "testdb.sql"
            //DataSource=:memory: makes Sqlite use in memory
            TestDbStartup.Init(new SqliteDbSettings("Data Source=:memory:;"));
            //Let's run this which will create our database and change our mapping names
            Setup = new TestDbSetup(scriptFileName, "DataAccess.DataModelResearch");
        }

        public void Dispose()
        {
        }
    }
}
