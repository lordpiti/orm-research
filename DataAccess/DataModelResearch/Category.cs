using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DataModelResearch
{
    public class Category : BaseModel
    {
        public Category()
        {
            Product = new List<Product>();
        }

        public string Name { get; set; }

        public List<Product> Product { get; set; }
    }
}
