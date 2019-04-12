using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DataModelResearch
{
    public class CategoryGroup
    {
        public string Name { get; set; }

        public List<Product> Products { get; set; }
    }
}
