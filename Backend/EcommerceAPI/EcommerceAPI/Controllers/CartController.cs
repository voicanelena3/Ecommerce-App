using BusinessLogic.DTOs;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{   
    /// <summary>
    /// Represents an API controller that manages shopping cart operations such as calculating totals and validating
    /// cart items.
    /// </summary>
    /// <remarks>All endpoints require authentication. This controller provides endpoints for calculating the
    /// total price of items in a cart and for validating the contents of a cart. Requests and responses are formatted
    /// as JSON. Use this controller to integrate cart-related functionality into client applications.</remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        /// <summary>
        /// Calculates the total price for the items in the shopping cart based on the provided request.
        /// </summary>
        /// <remarks>The request model must pass validation for the calculation to proceed. This endpoint
        /// is typically used to display the total cost before checkout.</remarks>
        /// <param name="request">The request containing the list of cart items to be totaled. Must not be null and must contain valid item
        /// data.</param>
        /// <returns>An HTTP 200 response containing the calculated total price if the request is valid; otherwise, an HTTP 400
        /// response with validation errors.</returns>
        [HttpPost("calculate-total")]
        public IActionResult CalculateCartTotal([FromBody] CartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var total = _cartService.CalculateCartTotal(request.Items);
            return Ok(new { total = total });
        }
        /// <summary>
        /// Validates the items in the specified shopping cart request and returns the validation result.
        /// </summary>
        /// <param name="request">The cart request containing the items to validate. Cannot be null.</param>
        /// <returns>An IActionResult containing a JSON object with an isValid property set to true if all cart items are valid;
        /// otherwise, false. Returns a BadRequest result if the request model is invalid.</returns>
        [HttpPost("validate")]
        public IActionResult ValidateCart([FromBody] CartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool isValid = _cartService.ValidateCartItems(request.Items);
            return Ok(new { isValid = isValid });
        }
    }
}
