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

        [HttpGet]
        public ActionResult<PaginatedList<Order>> GetPaginatedOrders(
            [FromQuery] DateOnly? start,
            [FromQuery] DateOnly? end,
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedOrders = _orderService.GetPaginatedOrders(start, end, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedOrders);
        }
    }
}
