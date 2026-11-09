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

            if (string.IsNullOrWhiteSpace(model.noiDung) && string.IsNullOrWhiteSpace(model.fileDinhKem))
            {
                return Json(new { success = false, message = "Nội dung tin nhắn hoặc tệp đính kèm không được để trống" });
            }

            string noiDungGui = string.IsNullOrWhiteSpace(model.noiDung) ? "[Tệp đính kèm]" : model.noiDung;
            string loaiTin = string.IsNullOrWhiteSpace(model.loaiTinNhan) ? "Text" : model.loaiTinNhan;

            var result = await _chatWebService.GuiTinNhanAsync(model.maPhongChat, maUser.Value, noiDungGui, model.fileDinhKem, loaiTin);
            return Json(result);
        }

        // POST: /Chat/UploadFile (AJAX call)
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn tệp" });
            }

            var result = await _chatWebService.UploadFileAsync(file);
            return Json(result);
        }

        // GET: /Chat/DanhSachFile?maPhongChat=1 (AJAX call)
        [HttpGet]
        public async Task<IActionResult> DanhSachFile(int maPhongChat)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var result = await _chatWebService.LayDanhSachFileAsync(maPhongChat);
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

        // GET: /Chat/DemChuaDoc (AJAX sync)
        [HttpGet]
        public async Task<IActionResult> DemChuaDoc()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                return Json(new { success = false, data = 0 });
            }

            var result = await _chatWebService.DemTinNhanChuaDocAsync(maUser.Value);
            return Json(new { success = result.success, data = result.data });
        }
    }
}
