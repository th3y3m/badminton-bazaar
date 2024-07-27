using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
using Services.Interface;

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
        public async Task<IActionResult> GetPaginatedSizePayments(
            [FromQuery] string? searchQuery = "",
            [FromQuery] string? sortBy = "size_asc",
            [FromQuery] string? status = "",
            [FromQuery] string? orderId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedPayments = await _paymentService.GetPaginatedPayments(searchQuery, sortBy, status, orderId, pageIndex, pageSize);
            return Ok(paginatedPayments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(string id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(Payment payment)
        {
            await _paymentService.AddPayment(payment);
            return Ok(payment);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayment(Payment payment)
        {
            await _paymentService.UpdatePayment(payment);
            return Ok(payment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentById(string id)
        {
            await _paymentService.DeletePayment(id);
            return Ok();
        }

        [HttpGet("GeneratePaymentToken/{bookingId}")]
        public async Task<IActionResult> GeneratePaymentToken(string bookingId)
        {
            var token = TokenForPayment.GenerateToken(bookingId);
            return Ok(new { token });
        }

        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(string role, string token)
        {
            //if (bookingId == null)
            //{
            //    return BadRequest(new ResponseModel
            //    {
            //        Status = "Error",
            //        Message = "Booking information is required."
            //    });
            //}
            var bookingId = TokenForPayment.ValidateToken(token);
            var response = await _paymentService.ProcessBookingPayment(role, bookingId);
            return Ok(response);
        }
    }
}
