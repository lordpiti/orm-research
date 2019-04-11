using DataAccess.DataModelResearch;
using DataAccess.EFModels;
using Microsoft.EntityFrameworkCore;
using DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
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

        public bool AddCategory(Category category)
        {
            _context.Category.Add(category);

            _context.SaveChanges();

            return true;
        }

        public bool AddProduct(Product product)
        {
            _context.Product.Add(product);

            _context.SaveChanges();

            return true;
        }

        public Product FindProduct(int id)
        {
            return _context.Product.FirstOrDefault(x => x.Id == id);
        }

        public List<Product> LoadProductsWithCategory()
        {
            var products = _context.Product.ToList();

            return products;
        }

        public void RemoveProduct(int id)
        {
            var productToRemove = _context.Product.FirstOrDefault(x => x.Id == id);

            _context.Product.Remove(productToRemove);

            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            var productToUpdate = _context.Product.FirstOrDefault(x => x.Id == product.Id);

            productToUpdate.Name = product.Name;

            var newCategory = _context.Category.FirstOrDefault(x => x.Id == product.CategoryId);

            //productToUpdate.CategoryId = product.CategoryId;
            productToUpdate.Category = newCategory;

            _context.SaveChanges();
        }
    }
}

