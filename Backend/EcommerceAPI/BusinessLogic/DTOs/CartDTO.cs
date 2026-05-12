namespace BusinessLogic.DTOs
{
    /// <summary>
    /// Represents a cart item as sent from the frontend.
    /// The frontend sends product and quantity separately.
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartRequest
    {
        public List<CartItem> Items { get; set; } = new();
    }
}
