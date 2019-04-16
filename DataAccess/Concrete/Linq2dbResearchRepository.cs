using DataAccess.DataModelResearch;
using DataAccess.Interface;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Data;

namespace DataAccess.Concrete
{
    public class Linq2dbResearchRepository : IResearchRepository
    {
        public bool AddCategory(Category category)
        {
            using (var db = new Context())
            {
                category.Id = db.InsertWithInt32Identity(category);
            }
            return true;
        }

        public bool AddProduct(Product product)
        {
            if (product.CategoryId == 0)
            {
                product.CategoryId = product.Category.Id;
            }

            using (var db = new Context())
            {
                product.Id = db.InsertWithInt32Identity(product);
            }
            return true;
        }

        //public List<test_Product> Test()
        //{
        //    using (var db = new ResearchDB())
        //    {
        //        var q = db.Products;
        //        var productList = q.ToList();

        //        return productList;

        //    }

        //}

        public List<Product> LoadProductsWithCategory()
        {
            using (var db = new Context())
            {
                var q = db.Products.LoadWith(x => x.Category);

                //query is not translated to SQL and executed until we indicate it with .tolist(), firstordefault(), etc
                var productList = q.OrderBy(x => x.Name).ToList();
                return productList;

                #region Compiled query

                //var compiledQuery = CompiledQuery.Compile((Context db2) => db2.Products.LoadWith(x => x.Category).OrderBy(x=>x.Name));
                //return compiledQuery(db).ToList();

                #endregion

            }
        }

        public Product FindProduct(int id)
        {
            using (var db = new Context())
            {
                var product = db.Products.LoadWith(x => x.Category).FirstOrDefault(x => x.Id == id);

                return product;
            }
        }

        public void RemoveProduct(int id)
        {
            using (var db = new Context())
            {
                db.Products
                    .Where(p => p.Id == id)
                    .Delete();
            }
        }

        public void UpdateProduct(Product product)
        {
            if (product.CategoryId == 0)
            {
                product.CategoryId = product.Category.Id;
            }

            using (var db = new Context())
            {
                db.Update(product);
            }
        }

        public List<Product> LoadProductsWithinCategory(int id)
        {
            using (var db = new Context())
            {
                var q = db.Products.LoadWith(x=>x.Category).Where(x => x.CategoryId == id);

                var productList = q.ToList();

                return productList;
            }
        }

        public List<CategoryGroup> GetProductsGrouped()
        {
            using (var db = new Context())
            {
                var testdata = db.Products.GroupBy(x => x.Category.Name)
                    .Select(x => new CategoryGroup
                    {
                        Name = x.Key,
                        NumberOfProducts = x.Count(),
                        AveragePrice = x.Average(y=>y.UnitPrice)
                        //Products = x.Select(product => new DataModelResearch.Product
                        //{
                        //    Name = product.Name,
                        //    Id = product.Id,
                        //    UnitPrice = product.UnitPrice
                        //}).ToList()
                    }).ToList();

                return testdata;
            }
        }

        public List<object> GetProductsWithCategoryPriceAverage()
        {
            using (var db = new Context())
            {
                var products = db.Products.LoadWith(x=>x.Category).Select(x => new
                {
                    productId = x.Id,
                    name = x.Name,
                    categoryAveragePrice = Sql.Ext.Average<decimal>(x.UnitPrice).Over().PartitionBy(x.CategoryId ).ToValue(),
                    categoryName = x.Category.Name
                }).ToList();

                return products.ToList<object>();
            }
        }
    }
}
