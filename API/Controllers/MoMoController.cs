using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [ApiController]
    public class MoMoController : Controller
    {
        private readonly IMoMoService _moMoService;

        public MoMoController(IMoMoService moMoService)
        {
            _moMoService = moMoService;
        }
        //[HttpPost("ProcessPaymentMoMo")]
        //public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        //{
        //    var response = await _moMoService.CreatePaymentRequest(request);

        //    if (response.ResultCode == 0)
        //    {
        //        return Redirect(response.PayUrl);
        //    }
        //    else
        //    {
        //        return BadRequest(new { message = response.Message });
        //    }
        //}
        //[HttpGet("payment-result")]
        //public IActionResult PaymentResult([FromQuery] MoMoResponse response)
        //{
        //    if (response.ResultCode == 0)
        //    {
        //        return Ok("Payment successful.");
        //    }
        //    else
        //    {
        //        return BadRequest(new { message = response.Message });
        //    }
        //}


        //[HttpPost]
        //[Route("TransactionProcess")]
        //public async Task<IActionResult> Payment(decimal amount, string orderinfor)
        //{
        //    try
        //    {
        //        string paymentUrl = await _moMoService.GenerateMomoUrl(orderinfor, amount);
        //        return Redirect(paymentUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //[HttpGet]
        //[Route("Payment/MomoResult")]
        //public async Task<IActionResult> PaymentConfirm()
        //{
        //    try
        //    {
        //        if (Request.QueryString.HasValue)
        //        {
        //            string queryString = Request.QueryString.Value;
        //            var validationResult = await _moMoService.ValidatePaymentResponse(queryString);

        //            if (validationResult.IsSuccessful)
        //            {

        //                return Redirect(validationResult.RedirectUrl);
        //            }
        //            else
        //            {
        //                return Redirect(validationResult.RedirectUrl);
        //            }
        //        }
        //        return Redirect("success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
        [HttpGet]
        [Route("/MomoAPI/")]
        public async Task<IActionResult> Payment(decimal amount, string infor, string orderinfor)
        {
            try
            {
                string paymentUrl = await _moMoService.CreatePaymentUrl(amount, infor, orderinfor);
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("MomoAPI/paymentconfirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            try
            {
                if (Request.QueryString.HasValue)
                {
                    string queryString = Request.QueryString.Value;
                    var validationResult = await _moMoService.ValidatePaymentResponse(queryString);

                    if (validationResult.IsSuccessful)
                    {

                        return Redirect(validationResult.RedirectUrl);
                    }
                    else
                    {
                        return Redirect(validationResult.RedirectUrl);
                    }
                }
                return Redirect("success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("MomoAPI/ipn")]
        public async Task<IActionResult> IPN([FromBody] IDictionary<string, string> parameters)
        {
            var queryString = string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
            var paymentStatus = await _moMoService.ValidatePaymentResponse(queryString);
            if (paymentStatus.IsSuccessful)
            {
                return Ok(new { message = "Payment processed successfully" });
            }
            else
            {
                return BadRequest(new { message = "Payment processing failed" });
            }
        }
    }
}
