using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
