using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.DataModelResearch
{
    public class Product : BaseModel
    {
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public int UnitPrice { get; set; }

        public Category Category { get; set; }
    }
}
