using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{   
    /// <summary>
    /// Represents an API controller that provides endpoints for retrieving and searching product data.
    /// </summary>
    /// <remarks>This controller exposes RESTful endpoints for accessing product information, including
    /// retrieving all products, fetching a product by its identifier, and searching for products by name or
    /// description. All routes are prefixed with 'api/products'. The controller depends on an implementation of
    /// IProductService to perform product-related operations.</remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

    /// <summary>
    /// Retrieves a collection of all available products.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the list of products. The response has a status code of 200 (OK) with
    /// the product data if successful.</returns>
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            return Ok(products);
        }

        /// <summary>
        /// Retrieves the product with the specified identifier.
        /// </summary>
        /// <remarks>Use this method to obtain details for a single product by its identifier. If the
        /// product does not exist, a not found response is returned.</remarks>
        /// <param name="id">The unique identifier of the product to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the product data if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound(new { message = "Product not found." });

            return Ok(product);
        }

       /// <summary>
       /// Searches for products that match the specified query string.
       /// </summary>
       /// <remarks>Returns a bad request response if the search query is not provided or is empty. The
       /// search is case-insensitive and matches products based on the provided query.</remarks>
       /// <param name="q">The search query used to filter products. Cannot be null, empty, or consist only of white-space characters.</param>
       /// <returns>An <see cref="IActionResult"/> containing the list of matching products if the query is valid; otherwise, a
       /// bad request response if the query is missing or invalid.</returns>
        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Search query is required." });

            var products = _productService.SearchProducts(q);
            return Ok(products);
        }
    }
}
