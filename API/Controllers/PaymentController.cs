using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly TokenForPayment _tokenForPayment;

        public PaymentController(PaymentService paymentService, TokenForPayment tokenForPayment)
        {
            _paymentService = paymentService;
            _tokenForPayment = tokenForPayment;
        }

        [HttpGet]
        public ActionResult GetPaginatedSizePayments(
            [FromQuery] string? searchQuery = "",
            [FromQuery] string? sortBy = "size_asc",
            [FromQuery] string? status = "",
            [FromQuery] string? orderId = "",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedPayments = _paymentService.GetPaginatedPayments(searchQuery, sortBy, status, orderId, pageIndex, pageSize);
            return Ok(paginatedPayments);
        }

        [HttpGet("{id}")]
        public ActionResult GetPaymentById(string id)
        {
            var payment = _paymentService.GetPaymentById(id);
            return Ok(payment);
        }

        [HttpPost]
        public ActionResult AddPayment(Payment payment)
        {
            _paymentService.AddPayment(payment);
            return Ok(payment);
        }

        [HttpPut]
        public ActionResult UpdatePayment(Payment payment)
        {
            _paymentService.UpdatePayment(payment);
            return Ok(payment);
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePaymentById(string id)
        {
            _paymentService.DeletePayment(id);
            return Ok();
        }

        [HttpGet("GeneratePaymentToken/{bookingId}")]
        public IActionResult GeneratePaymentToken(string bookingId)
        {
            var token = _tokenForPayment.GenerateToken(bookingId);
            return Ok(new { token });
        }

        [HttpPost("ProcessPayment")]
        public async Task<ActionResult> ProcessPayment(string role, string token)
        {
            //if (bookingId == null)
            //{
            //    return BadRequest(new ResponseModel
            //    {
            //        Status = "Error",
            //        Message = "Booking information is required."
            //    });
            //}
            var bookingId = _tokenForPayment.ValidateToken(token);
            var response = await _paymentService.ProcessBookingPayment(role, bookingId);
            return Ok(response);
        }
    }
}
