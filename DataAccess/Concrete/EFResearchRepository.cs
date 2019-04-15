using DataAccess.EFModels;
using DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Concrete
{
    public class EFResearchRepository : IResearchRepository
    {
        private readonly researchContext _context;

        public EFResearchRepository(researchContext context)
        {
            _context = context;
        }

        public bool AddCategory(DataModelResearch.Category category)
        {

            var newCategory = new EFModels.Category()
            {
                Name = category.Name
            };

            _context.Category.Add(newCategory);

            _context.SaveChanges();

            return true;
        }

        public bool AddProduct(DataModelResearch.Product product)
        {
            var newProduct = new EFModels.Product()
            {
                Name = product.Name,
                CategoryId = product.CategoryId,
                UnitPrice = product.UnitPrice
            };

            _context.Product.Add(newProduct);
            _context.SaveChanges();

            return true;
        }

        public DataModelResearch.Product FindProduct(int id)
        {
            var foundProduct = _context.Product.FirstOrDefault(x => x.Id == id);

            return new DataModelResearch.Product()
            {
                Name = foundProduct.Name,
                UnitPrice = foundProduct.UnitPrice,
                Category = new DataModelResearch.Category()
                {
                    Name = foundProduct.Category.Name,
                    Id = foundProduct.Category.Id,
                }
            };
        }

        public List<DataModelResearch.Product> LoadProductsWithCategory()
        {
            var productsFromDb = _context.Product.OrderBy(x => x.Name);

            #region Compiled query

            ////compiled queries not fully supported in EF core 2
            //// https://dzone.com/articles/compiled-queries-in-entity-framework-core-20
            //var compiledQuery = EF.CompileQuery((researchContext context) => context.Product.Include(x=>x.Category));
            //var productsFromDb = compiledQuery(_context).OrderBy(x => x.Name);

            #endregion

            var products = productsFromDb.Select(x => new DataModelResearch.Product()
            {
                Name = x.Name,
                Id = x.Id,
                UnitPrice = x.UnitPrice,
                Category = new DataModelResearch.Category()
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name
                }
            }).ToList();

            return products;
        }

        public List<DataModelResearch.Product> LoadProductsWithinCategory(int id)
        {
            var products = _context.Product
                .Where(x => x.CategoryId == id).Include(x => x.Category)
                .Select(x => new DataModelResearch.Product()
                {
                    Name = x.Name,
                    Id = x.Id,
                    UnitPrice = x.UnitPrice,
                    Category = new DataModelResearch.Category()
                    {
                        Id = x.Category.Id,
                        Name = x.Category.Name
                    }
                }).ToList();

            //var products = _context.Category.Include(x=>x.Product)
            //    .FirstOrDefault(x => x.Id == id).Product
            //    .Select(x => new DataModelResearch.Product()
            //    {
            //        Name = x.Name,
            //        Id = x.Id,
            //        Category = new DataModelResearch.Category()
            //        {
            //            Id = x.Category.Id,
            //            Name = x.Category.Name
            //        }
            //    }).ToList();

            return products;
        }

        public void RemoveProduct(int id)
        {
            var productToRemove = _context.Product.FirstOrDefault(x => x.Id == id);

            _context.Product.Remove(productToRemove);

            _context.SaveChanges();
        }

        public void UpdateProduct(DataModelResearch.Product product)
        {
            var productToUpdate = _context.Product.FirstOrDefault(x => x.Id == product.Id);

            productToUpdate.Name = product.Name;
            productToUpdate.UnitPrice = product.UnitPrice;

            var newCategory = _context.Category.FirstOrDefault(x => x.Id == product.CategoryId);

            //productToUpdate.CategoryId = product.CategoryId;
            productToUpdate.Category = newCategory;

            _context.SaveChanges();
        }

        public List<DataModelResearch.CategoryGroup> GetProductsGrouped()
        {
            var testdata = _context.Product.GroupBy(x => x.Category.Name)
                .Select(x => new DataModelResearch.CategoryGroup
                {
                    Name = x.Key,
                    NumberOfProducts = x.Count(),
                    AveragePrice = x.Average(y=>y.UnitPrice)
                    //Products = x.Select(product => new DataModelResearch.Product
                    //    {
                    //        Name = product.Name,
                    //        Id = product.Id,
                    //        UnitPrice = product.UnitPrice
                    //    }).ToList()
                }).ToList();

            return testdata;
        }
    }
}

