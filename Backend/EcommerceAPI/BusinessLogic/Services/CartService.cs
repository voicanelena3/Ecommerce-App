using BusinessLogic.DTOs;
using Repository.Repositories;

namespace BusinessLogic.Services
{
    public interface ICartService
    {
        decimal CalculateCartTotal(List<CartItem> cartItems);
        bool ValidateCartItems(List<CartItem> cartItems);
    }

    public class CartService : ICartService
    {
        private readonly IProductRepository _productRepository;

        public CartService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Calculate the total price of cart items based on current database prices.
        /// This ensures the backend independently calculates the total and doesn't trust frontend values.
        /// </summary>
        public decimal CalculateCartTotal(List<CartItem> cartItems)
        {
            decimal total = 0;

            foreach (var cartItem in cartItems)
            {
                var product = _productRepository.GetProductById(cartItem.ProductId);
                if (product != null)
                {
                    total += product.Price * cartItem.Quantity;
                }
            }

            return total;
        }

        /// <summary>
        /// Validate that all cart items exist and have sufficient stock.
        /// </summary>
        public bool ValidateCartItems(List<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                var product = _productRepository.GetProductById(cartItem.ProductId);
                if (product == null || product.Stock < cartItem.Quantity)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
