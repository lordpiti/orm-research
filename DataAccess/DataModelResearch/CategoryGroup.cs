namespace DataAccess.DataModelResearch
{
    public class CategoryGroup
    {
        public string Name { get; set; }

        public int NumberOfProducts { get; set; }

        public double AveragePrice { get; set; }

        public override bool Equals(object obj)
        {
            var categoryGroup = obj as CategoryGroup;

            return categoryGroup.Name == this.Name
                && categoryGroup.AveragePrice == this.AveragePrice
                && categoryGroup.NumberOfProducts == this.NumberOfProducts;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
