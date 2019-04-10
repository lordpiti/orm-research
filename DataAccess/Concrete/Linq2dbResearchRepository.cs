using DataAccess.Interface;
using DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DataAccess.DataModelResearch;
using LinqToDB;

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
                var q = db.Products.LoadWith(x=>x.Category);

                var productList = q.ToList();

                return productList;
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
    }
}
