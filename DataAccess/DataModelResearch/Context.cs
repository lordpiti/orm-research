using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DataModelResearch
{
    public class Context : DataConnection
    {
        public Context()
        {
            if (mappingSchema == null)
                mappingSchema = MappingLinq2Db.Do();
        }

        public ITable<Product> Products { get { return this.GetTable<Product>(); } }

        public ITable<Category> Category { get { return this.GetTable<Category>(); } }

        private static MappingSchema mappingSchema;

    }
}
