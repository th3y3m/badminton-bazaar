using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : Controller
    {
        private readonly ILogger<VnPayController> _logger;
        private readonly IVnpayService _vnpayService;

        public VnPayController(IVnpayService vnpayService, ILogger<VnPayController> logger)
        {
            _vnpayService = vnpayService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/VNPayAPI/")]
        public ActionResult Payment(decimal amount, string infor, string orderinfor)
        {
            try
            {
                string paymentUrl = _vnpayService.CreatePaymentUrl(amount, infor, orderinfor);
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Payment method");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("/VNpayAPI/paymentconfirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            try
            {
                if (Request.QueryString.HasValue)
                {
                    string queryString = Request.QueryString.Value;
                    var validationResult = await _vnpayService.ValidatePaymentResponse(queryString);

                    if (validationResult.IsSuccessful)
                    {

                        return Redirect(validationResult.RedirectUrl);
                    }
                    else
                    {
                        return Redirect(validationResult.RedirectUrl);
                    }
                }
                _logger.LogWarning("Invalid query string in PaymentConfirm");
                return Redirect("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PaymentConfirm method");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
