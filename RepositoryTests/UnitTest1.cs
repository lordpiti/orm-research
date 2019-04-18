using DataAccess.Concrete;
using DataAccess.DataModelResearch;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using LinqToDB.Mapping;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var testObject = new Product();

            var context = new Mock<Context>();
            var dbSetMock = new Mock<ITable<Product>>();
            context.Setup(x => x.Products).Returns(dbSetMock.Object);
            context.Setup(x => x.InsertWithInt32Identity(It.IsAny<Product>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(1);
            //dbSetMock.Setup(x => x.InsertWithInt32Identity(It.IsAny<Product>())).Returns(testObject);

            // Act
            var repository = new Linq2dbResearchRepository();
            repository.AddProduct(testObject);

            //Assert
            context.Verify(x => x.InsertWithInt32Identity(testObject, null, null, null));



            Assert.Pass();
        }
    }
}