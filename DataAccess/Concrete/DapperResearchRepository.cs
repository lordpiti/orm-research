using Dapper;
using DataAccess.DataModelResearch;
using DataAccess.Interface;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StackExchange.Profiling;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccess.Concrete
{
    public class DapperResearchRepository : IResearchRepository
    {
        private string connectionString;

        public DapperResearchRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetSection("DBInfo:ConnectionString").Value.ToString();
        }

        internal IDbConnection Connection
        {
            get
            {
                var dbConnection = new NpgsqlConnection(connectionString);
                //basic configuration
                //return dbConnection;

                //Wrap the db connection into the Miniprofiler class to use it
                return new StackExchange.Profiling.Data.ProfiledDbConnection(dbConnection, MiniProfiler.Current);
            }
        }

        public bool AddCategory(Category category)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO test.\"Category\" (name) VALUES(@Name)", category);
                return true;
            }
        }

        public List<Product> LoadProductsWithCategory()
        {
            string sql = "SELECT * FROM test.\"Product\" AS A INNER JOIN test.\"Category\" AS B ON A.\"categoryId\" = B.id;";
            
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var products = dbConnection.Query<Product, Category, Product>(
                    sql,
                    (product, productCategory) =>
                    {
                        product.Category = productCategory;
                        product.CategoryId = productCategory.Id;
                        return product;
                    },
                    splitOn: "Id")
                .Distinct()
                .ToList();

                return products;
            }
        }

        public Product FindProduct(int id)
        {
            string sql = "SELECT * FROM test.\"Product\" AS A INNER JOIN test.\"Category\" AS B ON A.\"categoryId\" = B.id WHERE A.\"Id\"=@Id;";

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var productFound = dbConnection.Query<Product, Category, Product>(
                    sql,
                    (product, productCategory) =>
                    {
                        product.Category = productCategory;
                        product.CategoryId = productCategory.Id;
                        return product;
                    },
                    new { Id = id },
                    splitOn: "Id").FirstOrDefault();
                return productFound;
            }
        }

        public bool AddProduct(Product product)
        {
            if (product.CategoryId == 0)
            {
                product.CategoryId = product.Category.Id;
            }

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO test.\"Product\" (name, categoryId, unitPrice) VALUES(@Name, @CategoryId, @UnitPrice)", product);
                return true;
            }
        }

        public void RemoveProduct(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM test.\"Product\" WHERE \"Id\"=@Id", new { Id = id });
            }
        }

        public void UpdateProduct(Product product)
        {
            if (product.CategoryId == 0)
            {
                product.CategoryId = product.Category.Id;
            }          

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Query("UPDATE test.\"Product\" SET name = @Name,  \"categoryId\"=@CategoryId, \"unitPrice\"=@UnitPrice WHERE \"Id\" = @Id", product);
            }
        }

        public List<Product> LoadProductsWithinCategory(int id)
        {
            string sql = "SELECT * FROM test.\"Product\" AS A INNER JOIN test.\"Category\" AS B ON A.\"categoryId\" = B.id WHERE A.\"categoryId\" = @categoryId;";

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var products = dbConnection.Query<Product, Category, Product>(
                    sql,
                    (product, productCategory) =>
                    {
                        product.Category = productCategory;
                        product.CategoryId = productCategory.Id;
                        return product;
                    },
                    new { categoryId= id },
                    splitOn: "categoryId")
                .Distinct()
                .ToList();
                return products;
            }
        }

        public List<CategoryGroup> GetProductsGrouped()
        {

            throw new System.NotImplementedException();
        }
    }
}
