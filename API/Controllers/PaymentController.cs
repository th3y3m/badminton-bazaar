using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
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
    }
}
