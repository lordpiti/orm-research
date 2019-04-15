using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.DataModelResearch;
using DataAccess.Interface;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;

namespace Linq2dbTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IResearchRepository _repo;
        private readonly Func<bool, IResearchRepository> _serviceAccessor;

        public ProductsController(IResearchRepository repo, Func<bool, IResearchRepository> serviceAccessor)
        {
            _repo = repo;
            _serviceAccessor = serviceAccessor;
        }

        /// <summary>
        /// Get a list with all the products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(GroupName = "v1")]
        public IEnumerable<Product> GetAllProducts()
        {
            using (MiniProfiler.Current.Step("Get method"))
            {
                var products = _repo.LoadProductsWithCategory();
                return products;
            }
        }

        /// <summary>
        /// Find a specific product
        /// </summary>
        /// <param name="id">id of the product to find</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ApiExplorerSettings(GroupName = "v1")]
        public ActionResult<Product> FindProduct(int id)
        {
            return _repo.FindProduct(id);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="category"></param>
        [HttpPost("category")]
        [ApiExplorerSettings(GroupName = "v2")]
        public void AddCategory([FromBody] Category category)
        {
            _repo.AddCategory(category);
        }


        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product"></param>
        [HttpPost]
        [ApiExplorerSettings(GroupName = "v2")]
        public void AddProduct([FromBody] Product product)
        {
            _repo.AddProduct(product);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">id of the product to delete</param>
        [HttpPost("delete/{id}")]
        [ApiExplorerSettings(GroupName = "v2")]
        public void Delete(int id)
        {
            _repo.RemoveProduct(id);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="product">Product info to update</param>
        [HttpPost("update")]
        [ApiExplorerSettings(GroupName = "v2")]
        public void Update(Product product)
        {
            _repo.UpdateProduct(product);
        }

        [HttpGet("hidden")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<string> NotRelevant()
        {
            return "Not relevant";
        }

        /// <summary>
        /// Get all the products within a category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("category/{id}")]
        [ApiExplorerSettings(GroupName = "v1")]
        public List<Product> FindProductWithinCategory(int id)
        {
            return _repo.LoadProductsWithinCategory(id);
        }

        /// <summary>
        /// Get all the products within a category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("byCategory")]
        [ApiExplorerSettings(GroupName = "v1")]
        public List<CategoryGroup> GetProductsGrouped(int id)
        {
            _repo = _serviceAccessor(true);
            return _repo.GetProductsGrouped();
        }
    }
}
