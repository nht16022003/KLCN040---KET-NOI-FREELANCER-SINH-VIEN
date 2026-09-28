using Microsoft.AspNetCore.Mvc;

namespace MOCK.Controllers
{
    public class MinhChungController : Controller
    {
        [HttpGet]
        public IActionResult Index(bool? isNew, int? userId)
        {
            return RedirectToAction("MinhChung", "Account", new { isNew, userId });
        }
    }
}
