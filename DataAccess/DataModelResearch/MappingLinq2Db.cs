using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DataModelResearch
{
    public static class MappingLinq2Db
    {
        static MappingSchema _schema = null;
        public static MappingSchema Do()
        {
            if (_schema == null)
            {
                _schema = MappingSchema.Default;
                var mapper = _schema.GetFluentMappingBuilder();
                mapper
                    .Entity<Product>()
                    .HasSchemaName("test")
                    .HasTableName("Product")
                    .Property(x => x.Id).HasColumnName("Id")
                        .IsIdentity().IsPrimaryKey()
                    .Property(x => x.Name).HasColumnName("name")
                    .Property(x => x.CategoryId).HasColumnName("categoryId")
                    .Association(product => product.Category, (product, category) => product.CategoryId == category.Id);

                mapper
                    .Entity<Category>()
                    .HasSchemaName("test")
                    .HasTableName("Category")
                    .Property(x => x.Id).HasColumnName("id")
                        .IsIdentity().IsPrimaryKey()
                    .Property(x => x.Name).HasColumnName("name");

            }

            return _schema;
        }

    }
}
