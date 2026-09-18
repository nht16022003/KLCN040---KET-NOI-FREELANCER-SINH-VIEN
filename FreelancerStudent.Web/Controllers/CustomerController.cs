using Microsoft.AspNetCore.Mvc;
using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.Services;
using System.Threading.Tasks;

namespace FreelancerStudent.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerApiService customerApiService, ILogger<CustomerController> logger)
        {
            _customerApiService = customerApiService;
            _logger = logger;
        }

        // GET: /Customer/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetString("UserID") ?? "USR002";
            var cusId = HttpContext.Session.GetString("CusID") ?? "CUS001";
            var userName = HttpContext.Session.GetString("UserName") ?? "khachhang_a";

            _logger.LogInformation("CustomerController: Đang nạp dữ liệu Dashboard từ API cho User: {UserName} (UserID: {UserID})", userName, userId);

            // GỌI SANG API ĐỂ LẤY TOÀN BỘ DỮ LIỆU TỪ CƠ SỞ DỮ LIỆU THẬT
            var model = await _customerApiService.GetDashboardDataAsync(userId, cusId);

            return View(model);
        }
    }
}
