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
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICartService> _mockCartService;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCartService = new Mock<ICartService>();

            _orderService = new OrderService(
                _mockOrderRepository.Object,
                _mockProductRepository.Object,
                _mockCartService.Object
            );
        }

        [Fact]
        public void CreateOrder_WithValidInput_CreatesOrderSuccessfully()
        {
            var userId = 1;
            var cartItem = new CartItem { ProductId = 1, Quantity = 2 };
            var request = new CheckoutRequest
            {
                ShippingAddress = "123 Main St",
                ShippingCity = "New York",
                ShippingState = "NY",
                ShippingZip = "10001",
                CartItems = new List<CartItem> { cartItem }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 50m,
                Stock = 10,
                Description = "Test"
            };

            _mockCartService
                .Setup(c => c.ValidateCartItems(request.CartItems))
                .Returns(true);

            _mockCartService
                .Setup(c => c.CalculateCartTotal(request.CartItems))
                .Returns(100m);

            _mockOrderRepository
                .Setup(r => r.CreateOrder(It.IsAny<Order>()))
                .Returns(1);

            _mockProductRepository
                .Setup(r => r.GetProductById(1))
                .Returns(product);

           
            var response = _orderService.CreateOrder(userId, request);

            Assert.NotNull(response);
            Assert.Equal(1, response.Id);
            Assert.Equal(100m, response.TotalPrice);
            _mockOrderRepository.Verify(r => r.CreateOrder(It.IsAny<Order>()), Times.Once);
            _mockOrderRepository.Verify(r => r.CreateOrderItem(It.IsAny<OrderItem>()), Times.Once);
            _mockProductRepository.Verify(r => r.UpdateProductStock(1, 2), Times.Once);
        }

        [Fact]
        public void CreateOrder_WithInvalidShippingAddress_ReturnsNull()
        {
            var userId = 1;
            var request = new CheckoutRequest
            {
                ShippingAddress = "",
                ShippingCity = "New York",
                ShippingState = "NY",
                ShippingZip = "10001",
                CartItems = new List<CartItem>()
            };

            var response = _orderService.CreateOrder(userId, request);

           
            Assert.Null(response);
        }

        [Fact]
        public void CreateOrder_WithEmptyCart_ReturnsNull()
        {
            var userId = 1;
            var request = new CheckoutRequest
            {
                ShippingAddress = "123 Main St",
                ShippingCity = "New York",
                ShippingState = "NY",
                ShippingZip = "10001",
                CartItems = new List<CartItem>()
            };

            var response = _orderService.CreateOrder(userId, request);

            Assert.Null(response);
        }

        [Fact]
        public void CreateOrder_WithInvalidCartItems_ReturnsNull()
        {
            var userId = 1;
            var cartItem = new CartItem { ProductId = 1, Quantity = 2 };
            var request = new CheckoutRequest
            {
                ShippingAddress = "123 Main St",
                ShippingCity = "New York",
                ShippingState = "NY",
                ShippingZip = "10001",
                CartItems = new List<CartItem> { cartItem }
            };

            _mockCartService
                .Setup(c => c.ValidateCartItems(request.CartItems))
                .Returns(false);

            var response = _orderService.CreateOrder(userId, request);

            Assert.Null(response);
        }

        [Fact]
        public void GetOrderById_WithValidId_ReturnsOrder()
        {
            var orderId = 1;
            var order = new Order
            {
                Id = 1,
                UserId = 1,
                TotalPrice = 100m,
                ShippingAddress = "123 Main St",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            _mockOrderRepository
                .Setup(r => r.GetOrderById(orderId))
                .Returns(order);

            var result = _orderService.GetOrderById(orderId);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("123 Main St", result.ShippingAddress);
        }

        [Fact]
        public void GetOrderById_WithInvalidId_ReturnsNull()
        {
            var orderId = 999;
            _mockOrderRepository
                .Setup(r => r.GetOrderById(orderId))
                .Returns((Order)null!);

            var result = _orderService.GetOrderById(orderId);

            Assert.Null(result);
        }

        [Fact]
        public void GetUserOrders_WithValidUserId_ReturnsOrderList()
        {
            var userId = 1;
            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    UserId = 1,
                    TotalPrice = 100m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                },
                new Order
                {
                    Id = 2,
                    UserId = 1,
                    TotalPrice = 200m,
                    Status = "Delivered",
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                }
            };

            _mockOrderRepository
                .Setup(r => r.GetOrdersByUserId(userId))
                .Returns(orders);

            var result = _orderService.GetUserOrders(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(100m, result[0].TotalPrice);
            Assert.Equal(200m, result[1].TotalPrice);
        }
    }
}
