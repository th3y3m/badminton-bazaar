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
        [ResponseCache(Duration = 60)]
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
        [ResponseCache(Duration = 60)]
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
        [ResponseCache(Duration = 60)]
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

        [HttpGet("GetTotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            try
            {
                var totalRevenue = await _paymentService.GetTotalRevenue();
                return Ok(totalRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTodayRevenue")]
        public async Task<IActionResult> GetTodayRevenue()
        {
            try
            {
                var todayRevenue = await _paymentService.GetTodayRevenue();
                return Ok(todayRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetThisWeekRevenue")]
        public async Task<IActionResult> GetThisWeekRevenue()
        {
            try
            {
                var thisWeekRevenue = await _paymentService.GetThisWeekRevenue();
                return Ok(thisWeekRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetThisMonthRevenue")]
        public async Task<IActionResult> GetThisMonthRevenue()
        {
            try
            {
                var thisMonthRevenue = await _paymentService.GetThisMonthRevenue();
                return Ok(thisMonthRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetThisYearRevenue")]
        public async Task<IActionResult> GetThisYearRevenue()
        {
            try
            {
                var thisYearRevenue = await _paymentService.GetThisYearRevenue();
                return Ok(thisYearRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetRevenueFromStartOfWeek")]
        public async Task<IActionResult> GetRevenueFromStartOfWeek()
        {
            try
            {
                var revenueFromStartOfWeek = await _paymentService.GetRevenueFromStartOfWeek();
                return Ok(revenueFromStartOfWeek);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetRevenueFromStartOfMonth")]
        public async Task<IActionResult> GetRevenueFromStartOfMonth()
        {
            try
            {
                var revenueFromStartOfMonth = await _paymentService.GetRevenueFromStartOfMonth();
                return Ok(revenueFromStartOfMonth);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetRevenueFromStartOfYear")]
        public async Task<IActionResult> GetRevenueFromStartOfYear()
        {
            try
            {
                var revenueFromStartOfYear = await _paymentService.GetRevenueFromStartOfYear();
                return Ok(revenueFromStartOfYear);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("PredictNextDayRevenue")]
        public async Task<IActionResult> PredictNextDayRevenue()
        {
            try
            {
                var predictedRevenue = await _paymentService.PredictNextDayRevenue();
                return Ok(predictedRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("PredictNextMonthRevenue")]
        public async Task<IActionResult> PredictNextMonthRevenue()
        {
            try
            {
                var predictedRevenue = await _paymentService.PredictNextMonthRevenue();
                return Ok(predictedRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("PredictNextYearRevenue")]
        public async Task<IActionResult> PredictNextYearRevenue()
        {
            try
            {
                var predictedRevenue = await _paymentService.PredictNextYearRevenue();
                return Ok(predictedRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
