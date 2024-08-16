using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedPayments(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "paymentdate_asc",
            [FromQuery] string status = "",
            [FromQuery] string orderId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedPayments = await _paymentService.GetPaginatedPayments(searchQuery, sortBy, status, orderId, pageIndex, pageSize);
                return Ok(paginatedPayments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(string id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentById(id);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetPaymentByOrderId/{id}")]
        public async Task<IActionResult> GetPaymentByOrderId(string id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByOrderId(id);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(Payment payment)
        {
            try
            {
                await _paymentService.AddPayment(payment);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayment(Payment payment)
        {
            try
            {
                await _paymentService.UpdatePayment(payment);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentById(string id)
        {
            try
            {
                await _paymentService.DeletePayment(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GeneratePaymentToken/{bookingId}")]
        public IActionResult GeneratePaymentToken(string bookingId)
        {
            try
            {
                var token = TokenForPayment.GenerateToken(bookingId);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(string role, string token)
        {
            try
            {
                var bookingId = TokenForPayment.ValidateToken(token);
                var response = await _paymentService.ProcessBookingPayment(role, bookingId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("ProcessPaymentMoMo")]
        public async Task<IActionResult> ProcessPaymentMoMo(string role, string token)
        {
            try
            {
                var bookingId = TokenForPayment.ValidateToken(token);
                var response = await _paymentService.ProcessBookingPaymentMoMo(role, bookingId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
