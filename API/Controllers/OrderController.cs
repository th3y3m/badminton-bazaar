using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetPaginatedOrders")]
        public ActionResult<PaginatedList<Order>> GetPaginatedOrders(
            [FromQuery] DateOnly? start,
            [FromQuery] DateOnly? end,
            [FromQuery] string sortBy = "orderdate_asc",
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedOrders = _orderService.GetPaginatedOrders(start, end, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedOrders);
        }

        [HttpGet("Price/{orderId}")]
        public ActionResult<decimal> TotalPrice(string orderId)
        {
            var totalPrice = _orderService.TotalPrice(orderId);
            return Ok(totalPrice);
        }

        [HttpGet("{orderId}")]
        public ActionResult<Order> GetOrderById(string orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            return Ok(order);
        }

        [HttpPost("CreateOrder")]
        public ActionResult<Order> CreateOrder([FromBody] string userId)
        {
            var newOrder = _orderService.AddOrder(userId);
            return Ok(newOrder);
        }

        [HttpDelete("DeleteOrder/{orderId}")]
        public ActionResult<Order> DeleteOrder(string orderId)
        {
            _orderService.CancelOrder(orderId);
            return Ok();
        }
    }
}
