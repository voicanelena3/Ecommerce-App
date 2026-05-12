using BusinessLogic.DTOs;
using Repository.Models;
using Repository.Repositories;

namespace BusinessLogic.Services
{
    public interface IOrderService
    {
        CheckoutResponse? CreateOrder(int userId, CheckoutRequest request);
        OrderDto? GetOrderById(int orderId);
        List<OrderDto> GetUserOrders(int userId);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartService _cartService;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _cartService = cartService;
        }

        public CheckoutResponse? CreateOrder(int userId, CheckoutRequest request)
        {
            
            if (string.IsNullOrWhiteSpace(request.ShippingAddress) ||
                string.IsNullOrWhiteSpace(request.ShippingCity) ||
                string.IsNullOrWhiteSpace(request.ShippingZip) ||
                request.CartItems == null || request.CartItems.Count == 0)
            {
                return null;
            }

            
            if (!_cartService.ValidateCartItems(request.CartItems))
                return null;

            
            decimal totalPrice = _cartService.CalculateCartTotal(request.CartItems);

           
            var order = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                ShippingAddress = request.ShippingAddress,
                City = request.ShippingCity,
                State = request.ShippingState,
                ZipCode = request.ShippingZip,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            int orderId = _orderRepository.CreateOrder(order);
            if (orderId <= 0)
                return null;

            foreach (var cartItem in request.CartItems)
            {
                var product = _productRepository.GetProductById(cartItem.ProductId);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = orderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = product.Price * cartItem.Quantity
                    };

                    _orderRepository.CreateOrderItem(orderItem);
                    _productRepository.UpdateProductStock(cartItem.ProductId, cartItem.Quantity);
                }
            }

            return new CheckoutResponse
            {
                Id = orderId,
                TotalPrice = totalPrice,
                Message = $"Order created successfully. Order ID: {orderId}"
            };
        }

        public OrderDto? GetOrderById(int orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            return order != null ? MapToDto(order) : null;
        }

        public List<OrderDto> GetUserOrders(int userId)
        {
            var orders = _orderRepository.GetOrdersByUserId(userId);
            return orders.Select(MapToDto).ToList();
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.City,
                ShippingState = order.State,
                ShippingZip = order.ZipCode,
                Status = order.Status,
                OrderDate = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
