﻿using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly OrderDetailService _orderDetailService;

        public OrderDetailController(OrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet("GetOrderDetail")]
        public IActionResult GetOrderDetail([FromQuery] string orderId)
        {
            var orderDetail = _orderDetailService.GetOrderDetail(orderId);
            return Ok(orderDetail);
        }
    }
}
