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

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
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
    }
}
