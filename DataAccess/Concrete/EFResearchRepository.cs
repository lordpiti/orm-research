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
                CategoryId = product.CategoryId
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
                Name = foundProduct.Name, Category = new DataModelResearch.Category()
                {
                    Name = foundProduct.Category.Name,
                    Id = foundProduct.Category.Id
                }
            };
        }

        public List<DataModelResearch.Product> LoadProductsWithCategory()
        {
            var products = _context.Product.Include(x=>x.Category).Select(x => new DataModelResearch.Product()
            {
                Name = x.Name,
                Id = x.Id,
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

            var newCategory = _context.Category.FirstOrDefault(x => x.Id == product.CategoryId);

            //productToUpdate.CategoryId = product.CategoryId;
            productToUpdate.Category = newCategory;

            _context.SaveChanges();
        }

        public List<object> GetProductsGrouped()
        {
            var testdata = _context.Product.GroupBy(x => x.Category.Name)
                .Select(x => new {
                    Name = x.Key,
                    Products = x.Select(product => new DataModelResearch.Product
                        {
                            Name = product.Name,
                            Id = product.Id
                        })
                    }).ToList();

            return testdata.ToList<object>();
        }
    }
}

