using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IWebHostEnvironment _env;

        public ChatController(IChatService chatService, IWebHostEnvironment env)
        {
            _chatService = chatService;
            _env = env;
        }

        // POST: api/Chat/TaoHoacLayPhong
        [HttpPost("TaoHoacLayPhong")]
        public async Task<IActionResult> TaoHoacLayPhong([FromBody] TaoPhongChat_RequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

                var phong = await _chatService.TaoHoacLayPhongChatAsync(request);
                return Ok(new { success = true, message = "Lấy phòng chat thành công", data = phong });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi mở phòng chat: " + ex.Message });
            }
        }

        // GET: api/Chat/DanhSachPhong/{maUser}
        [HttpGet("DanhSachPhong/{maUser}")]
        public async Task<IActionResult> LayDanhSachPhong(int maUser)
        {
            try
            {
                var ds = await _chatService.LayDanhSachPhongChatAsync(maUser);
                return Ok(new { success = true, message = "Lấy danh sách phòng chat thành công", data = ds });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lấy danh sách phòng: " + ex.Message });
            }
        }

        // GET: api/Chat/chua-doc?maUser=123
        [HttpGet("chua-doc")]
        public async Task<IActionResult> DemChuaDoc([FromQuery] int maUser)
        {
            try
            {
                var count = await _chatService.DemTinNhanChuaDocAsync(maUser);
                return Ok(new { success = true, message = "Lấy số tin nhắn chưa đọc thành công", data = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi đếm tin nhắn chưa đọc: " + ex.Message });
            }
        }

        // GET: api/Chat/TinNhan/{maPhongChat}?maUser=123
        [HttpGet("TinNhan/{maPhongChat}")]
        public async Task<IActionResult> LayLichSuTinNhan(int maPhongChat, [FromQuery] int maUser)
        {
            try
            {
                var tinNhans = await _chatService.LayLichSuTinNhanAsync(maPhongChat, maUser);
                return Ok(new { success = true, message = "Lấy lịch sử tin nhắn thành công", data = tinNhans });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lấy tin nhắn: " + ex.Message });
            }
        }

        // POST: api/Chat/GuiTinNhan
        [HttpPost("GuiTinNhan")]
        public async Task<IActionResult> GuiTinNhan([FromBody] GuiTinNhan_RequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

                var tinMoi = await _chatService.GuiTinNhanAsync(request);
                return Ok(new { success = true, message = "Gửi tin nhắn thành công", data = tinMoi });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi gửi tin nhắn: " + ex.Message });
            }
        }

        // POST: api/Chat/UploadFile
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Vui lòng chọn tệp đính kèm." });
                }

                // Giới hạn 20MB
                const long maxFileSize = 20 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    return BadRequest(new { success = false, message = "Dung lượng tệp không được vượt quá 20MB." });
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var imageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg" };
                var docExtensions = new HashSet<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv", ".zip", ".rar", ".7z" };

                // Kiểm tra blacklist các đuôi file nguy hiểm
                var dangerousExtensions = new HashSet<string> { ".exe", ".bat", ".cmd", ".sh", ".php", ".phtml", ".js", ".vbs", ".msi", ".dll", ".com", ".scr" };
                if (dangerousExtensions.Contains(extension))
                {
                    return BadRequest(new { success = false, message = "Định dạng tệp thực thi bị chặn vì lý do bảo mật." });
                }

                if (!imageExtensions.Contains(extension) && !docExtensions.Contains(extension))
                {
                    return BadRequest(new { success = false, message = "Định dạng tệp không được hỗ trợ. Vui lòng chọn ảnh, tài liệu hoặc file nén (PDF, Word, Excel, ZIP...)." });
                }

                string loaiTinNhan = imageExtensions.Contains(extension) ? "Image" : "File";

                // Đường dẫn thư mục uploads/chat/yyyyMM
                string subFolder = Path.Combine("uploads", "chat", DateTime.Now.ToString("yyyyMM"));
                string apiUploadPath = Path.Combine(_env.ContentRootPath, "wwwroot", subFolder);

                if (!Directory.Exists(apiUploadPath))
                {
                    Directory.CreateDirectory(apiUploadPath);
                }

                // Chuẩn hóa tên file an toàn
                string rawName = Path.GetFileNameWithoutExtension(file.FileName);
                string cleanName = string.Join("_", rawName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
                if (cleanName.Length > 50) cleanName = cleanName.Substring(0, 50);

                string guidShort = Guid.NewGuid().ToString("N")[..8];
                string uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{guidShort}_{cleanName}{extension}";
                string destinationFile = Path.Combine(apiUploadPath, uniqueFileName);

                using (var stream = new FileStream(destinationFile, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Đồng bộ sang thư mục FreelancerStudent.Web/wwwroot nếu tồn tại
                try
                {
                    string webUploadPath = Path.Combine(_env.ContentRootPath, "..", "FreelancerStudent.Web", "wwwroot", subFolder);
                    if (!Directory.Exists(webUploadPath))
                    {
                        Directory.CreateDirectory(webUploadPath);
                    }
                    string webDestination = Path.Combine(webUploadPath, uniqueFileName);
                    System.IO.File.Copy(destinationFile, webDestination, true);
                }
                catch
                {
                    // Tiếp tục nếu không thể đồng bộ sang Web
                }

                string relativeUrl = $"/{subFolder.Replace('\\', '/')}/{uniqueFileName}";

                return Ok(new
                {
                    success = true,
                    message = "Tải tệp lên thành công.",
                    data = new
                    {
                        fileUrl = relativeUrl,
                        fileName = file.FileName,
                        tenFile = file.FileName,
                        loaiTinNhan = loaiTinNhan,
                        loaiFile = loaiTinNhan
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi tải tệp lên: " + ex.Message });
            }
        }

        // GET: api/Chat/DanhSachFile/{maPhongChat}
        [HttpGet("DanhSachFile/{maPhongChat}")]
        public async Task<IActionResult> LayDanhSachFile(int maPhongChat)
        {
            try
            {
                var files = await _chatService.LayDanhSachFileTheoPhongChatAsync(maPhongChat);
                return Ok(new { success = true, message = "Lấy danh sách tệp đính kèm thành công", data = files });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lấy danh sách tệp: " + ex.Message });
            }
        }
    }
}
