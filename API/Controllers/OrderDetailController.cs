using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet("GetOrderDetailByOrderId/{orderId}")]
        public async Task<IActionResult> GetOrderDetailByOrderId(string orderId)
        {
            var orderDetail = await _orderDetailService.GetOrderDetail(orderId);
            return Ok(orderDetail);
        }
    }
}
