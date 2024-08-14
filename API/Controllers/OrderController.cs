using BusinessObjects;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Models;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IBackgroundJobClient _backgroundJobClient;


        public OrderController(IOrderService orderService, IBackgroundJobClient backgroundJobClient)
        {
            _orderService = orderService;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpGet("GetPaginatedOrders")]
        public async Task<IActionResult> GetPaginatedOrders(
            [FromQuery] DateOnly? start,
            [FromQuery] DateOnly? end,
            [FromQuery] string? userId = null,
            [FromQuery] string sortBy = "orderdate_asc",
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedOrders = await _orderService.GetPaginatedOrders(start, end, userId, sortBy, status, pageIndex, pageSize);
                return Ok(paginatedOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Price/{orderId}")]
        public async Task<IActionResult> TotalPrice(string orderId)
        {
            try
            {
                var totalPrice = await _orderService.TotalPrice(orderId);
                return Ok(totalPrice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var newOrder = await _orderService.AddOrder(request.UserId, request.Freight, request.Address);

                if (newOrder == null)
                {
                    return BadRequest("Failed to create order. Cart might be empty.");
                }
                newOrder.OrderDetails = null;

                _backgroundJobClient.Schedule(
                    () => _orderService.AutomaticFailedOrder(newOrder.OrderId),
                    TimeSpan.FromMinutes(15));

                return Ok(newOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteOrder/{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            try
            {
                await _orderService.CancelOrder(orderId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
