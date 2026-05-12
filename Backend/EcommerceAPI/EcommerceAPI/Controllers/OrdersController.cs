using BusinessLogic.DTOs;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to order management, including placing new orders and retrieving order information
    /// for authenticated users.
    /// </summary>
    /// <remarks>All endpoints require authentication. Users can only access their own orders. This controller
    /// provides actions for placing orders, retrieving a specific order by ID, and listing all orders for the current
    /// user.</remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Processes a checkout request and places a new order for the authenticated user.
        /// </summary>
        /// <remarks>The user must be authenticated for the order to be placed. Returns a BadRequest if
        /// the request is invalid or if the order cannot be completed due to issues such as out-of-stock items or
        /// invalid shipping information.</remarks>
        /// <param name="request">The checkout details, including items to purchase and shipping information. Must not be null and must
        /// satisfy all validation requirements.</param>
        /// <returns>An IActionResult containing the order confirmation if the order is placed successfully; otherwise, a
        /// BadRequest or Unauthorized result describing the failure.</returns>
        [HttpPost]
        public IActionResult PlaceOrder([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var response = _orderService.CreateOrder(userId, request);
            if (response == null)
                return BadRequest(new { message = "Checkout failed. Items may be out of stock or invalid shipping information." });

            return Ok(response);
        }

        /// <summary>
        /// Retrieves the details of a specific order by its identifier for the authenticated user.
        /// </summary>
        /// <remarks>Only the user who owns the order can access its details. Returns an error response if
        /// the order does not exist or if the user is not authorized to view it.</remarks>
        /// <param name="id">The unique identifier of the order to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the order details if found and accessible; otherwise, an
        /// appropriate error response such as Unauthorized, NotFound, or Forbid.</returns>
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound(new { message = "Order not found." });

            if (order.UserId != userId)
                return Forbid();

            return Ok(order);
        }

        /// <summary>
        /// Retrieves the list of orders associated with the currently authenticated user.
        /// </summary>
        /// <remarks>This endpoint requires the user to be authenticated. The returned result contains
        /// only the orders belonging to the current user.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the user's orders if authentication is successful; otherwise, an
        /// unauthorized result.</returns>
        [HttpGet("user")]
        public IActionResult GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var orders = _orderService.GetUserOrders(userId);
            return Ok(orders);
        }
    }
}
