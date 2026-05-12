namespace BusinessLogic.DTOs
{
    /// <summary>
    /// Checkout request from frontend with shipping information and cart items.
    /// </summary>
    public class CheckoutRequest
    {
        [System.Text.Json.Serialization.JsonPropertyName("cartItems")]
        public List<CartItem> CartItems { get; set; } = new();

        [System.Text.Json.Serialization.JsonPropertyName("shippingAddress")]
        public string ShippingAddress { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("shippingCity")]
        public string ShippingCity { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("shippingState")]
        public string ShippingState { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("shippingZip")]
        public string ShippingZip { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response returned after successful checkout.
    /// </summary>
    public class CheckoutResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Order details returned to frontend.
    /// </summary>
    public class OrderDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("userId")]
        public int UserId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("shippingAddress")]
        public string ShippingAddress { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("shippingCity")]
        public string ShippingCity { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("shippingState")]
        public string ShippingState { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("shippingZip")]
        public string ShippingZip { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("items")]
        public List<OrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// Individual item in an order.
    /// </summary>
    public class OrderItemDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
