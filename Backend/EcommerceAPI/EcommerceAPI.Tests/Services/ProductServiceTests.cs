using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using BusinessLogic.Services;
using Repository.Models;
using Repository.Repositories;

namespace EcommerceAPI.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _productService = new ProductService(_mockProductRepository.Object);
        }

        [Fact]
        public void GetAllProducts_ReturnsAllProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 50m, Stock = 10, Description = "Test 1" },
                new Product { Id = 2, Name = "Product 2", Price = 100m, Stock = 5, Description = "Test 2" },
                new Product { Id = 3, Name = "Product 3", Price = 150m, Stock = 0, Description = "Test 3" }
            };

            _mockProductRepository
                .Setup(r => r.GetAllProducts())
                .Returns(products);

            var result = _productService.GetAllProducts();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Product 1", result[0].Name);
            Assert.Equal("Product 2", result[1].Name);
            Assert.Equal("Product 3", result[2].Name);
        }

        [Fact]
        public void GetAllProducts_WithEmptyDatabase_ReturnsEmptyList()
        {
            _mockProductRepository
                .Setup(r => r.GetAllProducts())
                .Returns(new List<Product>());

            var result = _productService.GetAllProducts();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProductById_WithValidId_ReturnsProduct()
        {
            var productId = 1;
            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                Price = 50m,
                Stock = 10,
                Description = "Test Product"
            };

            _mockProductRepository
                .Setup(r => r.GetProductById(productId))
                .Returns(product);

            var result = _productService.GetProductById(productId);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Product 1", result.Name);
            Assert.Equal(50m, result.Price);
        }

        [Fact]
        public void GetProductById_WithInvalidId_ReturnsNull()
        {
            var productId = 999;
            _mockProductRepository
                .Setup(r => r.GetProductById(productId))
                .Returns((Product)null!);

            var result = _productService.GetProductById(productId);

           
            Assert.Null(result);
        }

        [Fact]
        public void SearchProducts_WithValidKeyword_ReturnsMatchingProducts()
        {
            var keyword = "keyboard";
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Mechanical Keyboard", Price = 149.99m, Stock = 10, Description = "RGB Keyboard" },
                new Product { Id = 2, Name = "Wireless Keyboard", Price = 79.99m, Stock = 5, Description = "Quiet Keyboard" }
            };

            _mockProductRepository
                .Setup(r => r.SearchProducts(keyword))
                .Returns(products);

            var result = _productService.SearchProducts(keyword);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Contains("Keyboard", p.Name));
        }

        [Fact]
        public void SearchProducts_WithNoMatches_ReturnsEmptyList()
        {
            var keyword = "nonexistent";
            _mockProductRepository
                .Setup(r => r.SearchProducts(keyword))
                .Returns(new List<Product>());

            var result = _productService.SearchProducts(keyword);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void SearchProducts_WithEmptyKeyword_ReturnsAll()
        {
            var keyword = "";
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 50m, Stock = 10, Description = "Test 1" },
                new Product { Id = 2, Name = "Product 2", Price = 100m, Stock = 5, Description = "Test 2" }
            };

            _mockProductRepository
                .Setup(r => r.SearchProducts(keyword))
                .Returns(products);

            var result = _productService.SearchProducts(keyword);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Theory]
        [InlineData("keyboard")]
        [InlineData("KEYBOARD")]
        [InlineData("KeYbOaRd")]
        public void SearchProducts_IsCaseInsensitive(string keyword)
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Mechanical Keyboard", Price = 149.99m, Stock = 10, Description = "RGB Keyboard" }
            };

            _mockProductRepository
                .Setup(r => r.SearchProducts(keyword))
                .Returns(products);

            var result = _productService.SearchProducts(keyword);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetProductById_WithZeroStock_ReturnsProduct()
        {
            var productId = 1;
            var product = new Product
            {
                Id = 1,
                Name = "Out of Stock Product",
                Price = 50m,
                Stock = 0,
                Description = "Test"
            };

            _mockProductRepository
                .Setup(r => r.GetProductById(productId))
                .Returns(product);

            var result = _productService.GetProductById(productId);

            Assert.NotNull(result);
            Assert.Equal(0, result.Stock);
        }
    }
}
