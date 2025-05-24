using InMemoryCartApi.Data;
using InMemoryCartApi.Models;
using InMemoryCartApi.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace InMemoryCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly string _mockUserId = InMemoryCartStorage.MockUserId; // Using the mock user ID

        // Inject the Cart Service
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // POST /api/cart/add
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid) // Basic model validation
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCart = _cartService.AddToCart(_mockUserId, request.ProductId, request.Quantity);
                return Ok(updatedCart);
            }
            catch (ArgumentException ex) // Catch specific exceptions from service
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) // Generic error handler
            {
                // Log the exception ex
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // POST /api/cart/remove/{productId}?quantity=1
        [HttpPost("remove/{productId}")]
        public IActionResult RemoveFromCart(int productId, [FromQuery] int quantity = 1) // Default quantity to 1 if not provided
        {
            try
            {
                var updatedCart = _cartService.RemoveFromCart(_mockUserId, productId, quantity);
                // Decide whether to return Ok(updatedCart) or NoContent() if successful but nothing changed
                return Ok(updatedCart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            // Catch KeyNotFoundException if the service throws it when item not in cart
            // catch (KeyNotFoundException ex) 
            // { 
            //     return NotFound(new { message = ex.Message }); 
            // }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // GET /api/cart
        [HttpGet]
        public IActionResult GetCart()
        {
            try
            {
                var cart = _cartService.GetCart(_mockUserId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // POST /api/cart/checkout
        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            try
            {
                var summary = _cartService.Checkout(_mockUserId);
                return Ok(summary);
            }
            catch (InvalidOperationException ex) // Specific exception for empty cart checkout
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
