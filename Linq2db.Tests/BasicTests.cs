using DataAccess.Concrete;
using DataAccess.DataModelResearch;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Linq2db.Tests
{
    public class BasicTests : IClassFixture<TestDbFixture>
    {
        private readonly TestDbSetup _setup;

        //this is to display some output about the counts
        private readonly ITestOutputHelper _output;

        public BasicTests(ITestOutputHelper output, TestDbFixture fixture)
        {
            _setup = fixture.Setup;
            _output = output;
        }

        [Fact]
        public void CanGetProductsViaLinq()
        {
            using (var db = _setup.GetDbTest())
            {
                var query = db.Products.OrderByDescending(o => o.Name)
                    .ToList();
                _output.WriteLine("Products: {0}", query.Count);
                Assert.True(query.Any());
            }
        }

        [Fact]
        public void CanGetProductsViaLinq2()
        {
            using (var db = _setup.GetDbTest())
            {
                var query = db.Products.Where(o => o.UnitPrice > 20).OrderByDescending(o => o.Name)
                    .ToList();
                Assert.True(query.Any());
            }
        }


        [Fact]
        public void CanLoadProductsWithCategory()
        {
            var db = _setup.GetDbTest();

            var repo = new Linq2dbResearchRepository(db);

            var query = repo.LoadProductsWithCategory();

            Assert.True(query.Count == 5);
        }

        [Fact]
        public void CanAddProduct()
        {
            var db = _setup.GetDbTest();

            var repo = new Linq2dbResearchRepository(db);

            var mockProduct = new Product()
            {
                Name = "testproductha",
                CategoryId = 1,
                UnitPrice = 765
            };

            var query = repo.AddProduct(mockProduct);

            Assert.True(query);
        }

        [Fact]
        public void GetProductsGrouped()
        {
            var db = _setup.GetDbTest();

            var repo = new Linq2dbResearchRepository(db);

            var query = repo.GetProductsGrouped();

            var expected = new List<CategoryGroup>()
            {
                new CategoryGroup()
                {
                    Name = "Category 1",
                    NumberOfProducts = 3,
                    AveragePrice = 33.333333333333336,
                },
                new CategoryGroup()
                {
                    Name = "Category 2",
                    NumberOfProducts = 1,
                    AveragePrice = 20
                    
                },
                new CategoryGroup()
                {
                    Name = "Category 3",
                    NumberOfProducts = 1,
                    AveragePrice = 30
                },
            };

            Assert.Equal(query[0], expected[0]);
            Assert.Equal(query[1], expected[1]);
            Assert.Equal(query[2], expected[2]);
        }
    }
}
