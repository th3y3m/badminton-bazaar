using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/orderdetail")]
    public class OrderDetailController : ControllerBase
    {
        private readonly OrderDetailService _orderDetailService;

        public OrderDetailController(OrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public IActionResult GetOrderDetail([FromQuery] string orderId)
        {
            var orderDetail = _orderDetailService.GetOrderDetail(orderId);
            return Ok(orderDetail);
        }
    }
}
