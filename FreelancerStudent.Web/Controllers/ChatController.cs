using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels.Chat;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatWebService _chatWebService;

        public ChatController(IChatWebService chatWebService)
        {
            _chatWebService = chatWebService;
        }

        // GET: /Chat?maPhongChat=1&maFreelancerStudent=2&maUserNTD=3&maJob=JOB001
        [HttpGet]
        public async Task<IActionResult> Index(int? maPhongChat, int? maFreelancerStudent, int? maUserNTD, string? maJob)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            int currentUserId = maUser.Value;
            var role = HttpContext.Session.GetString("role") ?? "Client";

            int selectedPhongId = maPhongChat ?? 0;

            // 1. Trường hợp NTD muốn chat với Freelancer
            if (maFreelancerStudent.HasValue && maFreelancerStudent.Value > 0)
            {
                var taoPhongResult = await _chatWebService.TaoHoacLayPhongChatAsync(currentUserId, maFreelancerStudent.Value, maJob);
                if (taoPhongResult.success && taoPhongResult.data != null)
                {
                    selectedPhongId = taoPhongResult.data.maPhongChat;
                }
            }
            // 2. Trường hợp Freelancer muốn chat về một bài JobPost cụ thể
            else if (!string.IsNullOrEmpty(maJob))
            {
                var taoPhongResult = await _chatWebService.TaoHoacLayPhongChatAsync(maUserNTD ?? 0, currentUserId, maJob);
                if (taoPhongResult.success && taoPhongResult.data != null)
                {
                    selectedPhongId = taoPhongResult.data.maPhongChat;
                }
            }
            // 3. Trường hợp Freelancer muốn chat với Nhà tuyển dụng từ trang NhaTuyenDung/Search
            else if (maUserNTD.HasValue && maUserNTD.Value > 0)
            {
                var taoPhongResult = await _chatWebService.TaoHoacLayPhongChatAsync(maUserNTD.Value, currentUserId, maJob);
                if (taoPhongResult.success && taoPhongResult.data != null)
                {
                    selectedPhongId = taoPhongResult.data.maPhongChat;
                }
            }

            // 2. Lấy danh sách toàn bộ phòng chat của user
            var dsPhongResult = await _chatWebService.LayDanhSachPhongChatAsync(currentUserId);
            var dsPhong = dsPhongResult?.data ?? new List<PhongChatViewModel>();

            // Nếu chưa chỉ định phòng và danh sách có phòng, chọn phòng đầu tiên
            if (selectedPhongId == 0 && dsPhong.Count > 0)
            {
                selectedPhongId = dsPhong[0].maPhongChat;
            }

            // 3. Lấy thông tin phòng hiện tại và lịch sử tin nhắn
            PhongChatViewModel? phongHienTai = dsPhong.FirstOrDefault(p => p.maPhongChat == selectedPhongId);
            var dsTinNhan = new List<TinNhanViewModel>();

            if (selectedPhongId > 0)
            {
                var tinNhanResult = await _chatWebService.LayLichSuTinNhanAsync(selectedPhongId, currentUserId);
                dsTinNhan = tinNhanResult?.data ?? new List<TinNhanViewModel>();
            }

            var model = new ChatIndexViewModel
            {
                DanhSachPhong = dsPhong,
                PhongHienTai = phongHienTai,
                LichSuTinNhan = dsTinNhan,
                CurrentUserId = currentUserId,
                CurrentUserRole = role
            };

            return View(model);
        }

        // POST: /Chat/GuiTinNhan (AJAX call)
        [HttpPost]
        public async Task<IActionResult> GuiTinNhan([FromBody] GuiTinNhanWebModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (string.IsNullOrWhiteSpace(model.noiDung))
            {
                return Json(new { success = false, message = "Nội dung tin nhắn không được để trống" });
            }

            var result = await _chatWebService.GuiTinNhanAsync(model.maPhongChat, maUser.Value, model.noiDung, model.fileDinhKem);
            return Json(result);
        }

        // GET: /Chat/LayTinNhan?maPhongChat=1 (AJAX polling)
        [HttpGet]
        public async Task<IActionResult> LayTinNhan(int maPhongChat)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var result = await _chatWebService.LayLichSuTinNhanAsync(maPhongChat, maUser.Value);
            return Json(result);
        }
    }
}
