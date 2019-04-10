using DataAccess.DataModelResearch;
using DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interface
{
    public interface IResearchRepository
    {
        //List<test_Product> Test();

        List<Product> LoadProductsWithCategory();

        Product FindProduct(int id);

        bool AddCategory(Category category);

        bool AddProduct(Product product);

        void RemoveProduct(int id);

        void UpdateProduct(Product product);
    }
}
