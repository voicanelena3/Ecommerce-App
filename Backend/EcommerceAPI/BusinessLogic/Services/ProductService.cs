using BusinessLogic.DTOs;
using Repository.Models;
using Repository.Repositories;

namespace BusinessLogic.Services
{
    public interface IProductService
    {
        List<ProductDto> GetAllProducts();
        ProductDto? GetProductById(int productId);
        List<ProductDto> SearchProducts(string query);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<ProductDto> GetAllProducts()
        {
            var products = _productRepository.GetAllProducts();
            return products.Select(MapToDto).ToList();
        }

        public ProductDto? GetProductById(int productId)
        {
            var product = _productRepository.GetProductById(productId);
            return product != null ? MapToDto(product) : null;
        }

        public List<ProductDto> SearchProducts(string query)
        {
            var products = _productRepository.SearchProducts(query);
            return products.Select(MapToDto).ToList();
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                ImageUrl = product.ImageUrl
            };
        }
    }
}
