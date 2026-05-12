using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using BusinessLogic.Services;
using BusinessLogic.DTOs;
using Repository.Models;
using Repository.Repositories;

namespace EcommerceAPI.Tests.Services
{
    public class CartServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _cartService = new CartService(_mockProductRepository.Object);
        }

        [Fact]
        public void CalculateCartTotal_WithValidItems_CalculatesCorrectTotal()
        {
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2 },
                new CartItem { ProductId = 2, Quantity = 1 }
            };

            var product1 = new Product { Id = 1, Name = "Product 1", Price = 50m, Stock = 10, Description = "Test" };
            var product2 = new Product { Id = 2, Name = "Product 2", Price = 100m, Stock = 5, Description = "Test" };

            _mockProductRepository
                .Setup(r => r.GetProductById(1))
                .Returns(product1);

            _mockProductRepository
                .Setup(r => r.GetProductById(2))
                .Returns(product2);

            var total = _cartService.CalculateCartTotal(cartItems);

            Assert.Equal(200m, total); // (50 * 2) + (100 * 1) = 200
        }

        [Fact]
        public void CalculateCartTotal_WithNonexistentProduct_SkipsProduct()
        {
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2 },
                new CartItem { ProductId = 999, Quantity = 1 }
            };

            var product1 = new Product { Id = 1, Name = "Product 1", Price = 50m, Stock = 10, Description = "Test" };

            _mockProductRepository
                .Setup(r => r.GetProductById(1))
                .Returns(product1);

            _mockProductRepository
                .Setup(r => r.GetProductById(999))
                .Returns((Product)null!);

            var total = _cartService.CalculateCartTotal(cartItems);

            Assert.Equal(100m, total); 
        }

        [Fact]
        public void CalculateCartTotal_WithEmptyCart_ReturnsZero()
        {
            var cartItems = new List<CartItem>();

            var total = _cartService.CalculateCartTotal(cartItems);

            Assert.Equal(0m, total);
        }

        [Fact]
        public void ValidateCartItems_WithValidItems_ReturnsTrue()
        {
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2 },
                new CartItem { ProductId = 2, Quantity = 1 }
            };

            var product1 = new Product { Id = 1, Name = "Product 1", Price = 50m, Stock = 10, Description = "Test" };
            var product2 = new Product { Id = 2, Name = "Product 2", Price = 100m, Stock = 5, Description = "Test" };

            _mockProductRepository
                .Setup(r => r.GetProductById(1))
                .Returns(product1);

            _mockProductRepository
                .Setup(r => r.GetProductById(2))
                .Returns(product2);

            var result = _cartService.ValidateCartItems(cartItems);

            Assert.True(result);
        }

        [Fact]
        public void ValidateCartItems_WithInsufficientStock_ReturnsFalse()
        {
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 20 }
            };

            var product1 = new Product { Id = 1, Name = "Product 1", Price = 50m, Stock = 5, Description = "Test" };

            _mockProductRepository
                .Setup(r => r.GetProductById(1))
                .Returns(product1);

         
            var result = _cartService.ValidateCartItems(cartItems);

            Assert.False(result);
        }

        [Fact]
        public void ValidateCartItems_WithNonexistentProduct_ReturnsFalse()
        {
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 999, Quantity = 1 }
            };

            _mockProductRepository
                .Setup(r => r.GetProductById(999))
                .Returns((Product)null!);

            var result = _cartService.ValidateCartItems(cartItems);

            Assert.False(result);
        }

        [Fact]
        public void ValidateCartItems_WithEmptyCart_ReturnsTrue()
        {
            var cartItems = new List<CartItem>();

            var result = _cartService.ValidateCartItems(cartItems);

           
            Assert.True(result);
        }
    }
}
