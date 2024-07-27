using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetPaginatedOrders")]
        public async Task<IActionResult> GetPaginatedOrders(
            [FromQuery] DateOnly? start,
            [FromQuery] DateOnly? end,
            [FromQuery] string sortBy = "orderdate_asc",
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedOrders = await _orderService.GetPaginatedOrders(start, end, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedOrders);
        }

        [HttpGet("Price/{orderId}")]
        public async Task<IActionResult> TotalPrice(string orderId)
        {
            var totalPrice = await _orderService.TotalPrice(orderId);
            return Ok(totalPrice);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            return Ok(order);
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] string userId)
        {
            var newOrder = await _orderService.AddOrder(userId);
            return Ok(newOrder);
        }

        [HttpDelete("DeleteOrder/{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            await _orderService.CancelOrder(orderId);
            return Ok();
        }
    }
}
