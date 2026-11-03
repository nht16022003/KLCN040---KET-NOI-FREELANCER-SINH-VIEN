using System;
using System.Threading.Tasks;
using FreelancerStudent.API.DTOs.AdminDTOs;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ==========================================
        // 1. DASHBOARD & GIÁM SÁT
        // ==========================================
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var data = await _adminService.LayDashboardAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 2. QUẢN LÝ NGƯỜI DÙNG (AD-UC-01)
        // ==========================================
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? keyword, [FromQuery] int? role, [FromQuery] string? status)
        {
            try
            {
                var data = await _adminService.LayDanhSachUsersAsync(keyword, role, status);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 3. KHÓA / MỞ KHÓA TÀI KHOẢN (AD-UC-02)
        // ==========================================
        [HttpPost("XuLyTrangThaiUser")]
        public async Task<IActionResult> XuLyTrangThaiUser([FromBody] AdminKhoaMoUserRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var result = await _adminService.XuLyTrangThaiUserAsync(request);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy người dùng cần xử lý" });
                }

                return Ok(new { 
                    success = true, 
                    message = request.HanhDong == "KHOA_TAI_KHOAN" 
                        ? $"Đã khóa tài khoản #{request.MaUser} thành công!" 
                        : $"Đã mở khóa tài khoản #{request.MaUser} thành công!" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("LichSuXuLyUser")]
        public async Task<IActionResult> GetLichSuXuLyUser([FromQuery] int? maUser)
        {
            try
            {
                var data = await _adminService.LayLichSuXuLyUserAsync(maUser);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 4. ĐIỀU CHỈNH SỐ DƯ VÍ (AD-UC-04)
        // ==========================================
        [HttpGet("Wallets")]
        public async Task<IActionResult> GetWallets()
        {
            try
            {
                var data = await _adminService.LayDanhSachWalletsAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DieuChinhSoDu")]
        public async Task<IActionResult> DieuChinhSoDu([FromBody] AdminDieuChinhViRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var result = await _adminService.DieuChinhSoDuViAsync(request);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy ví cần điều chỉnh" });
                }

                return Ok(new { 
                    success = true, 
                    message = $"Điều chỉnh số dư ví #{request.MaWallet} thành công ({(request.LoaiDieuChinh == "CongTien" ? "+" : "-")}{request.SoTien:N0} VNĐ)!" 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("LichSuDieuChinhVi")]
        public async Task<IActionResult> GetLichSuDieuChinhVi([FromQuery] int? maWallet)
        {
            try
            {
                var data = await _adminService.LayLichSuDieuChinhViAsync(maWallet);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 5. QUẢN LÝ HỢP ĐỒNG (AD-UC-10)
        // ==========================================
        [HttpGet("HopDongs")]
        public async Task<IActionResult> GetHopDongs()
        {
            try
            {
                var data = await _adminService.LayDanhSachHopDongAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("HopDongs/{maHD}")]
        public async Task<IActionResult> GetHopDongDetail(string maHD)
        {
            try
            {
                var data = await _adminService.LayChiTietHopDongAsync(maHD);
                if (data == null) return NotFound(new { success = false, message = "Không tìm thấy hợp đồng" });
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("MauHopDong")]
        public async Task<IActionResult> GetMauHopDong()
        {
            try
            {
                var data = await _adminService.LayMauHopDongAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("CapNhatMauHopDong")]
        public async Task<IActionResult> CapNhatMauHopDong([FromBody] AdminMauHopDongDTO model)
        {
            try
            {
                var result = await _adminService.CapNhatMauHopDongAsync(model);
                return Ok(new { success = result, message = "Cập nhật mẫu hợp đồng thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 6. QUẢN LÝ & DUYỆT RÚT TIỀN (AD-UC-05)
        // ==========================================
        [HttpGet("YeuCauRutTien")]
        public async Task<IActionResult> GetDanhSachYeuCauRutTien([FromQuery] string? trangThai)
        {
            try
            {
                var data = await _adminService.LayDanhSachYeuCauRutTienAsync(trangThai);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("XuLyRutTien")]
        public async Task<IActionResult> XuLyRutTien([FromBody] AdminXuLyRutTienRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var result = await _adminService.XuLyYeuCauRutTienAsync(request);
                return Ok(new
                {
                    success = result,
                    message = request.ChapNhan
                        ? $"Duyệt giải ngân yêu cầu rút tiền #{request.MaRutTien} thành công!"
                        : $"Đã từ chối yêu cầu rút tiền #{request.MaRutTien} và hoàn trả tiền về ví người dùng!"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 7. CẤU HÌNH PHÍ SÀN & PHÍ ĐĂNG TIN
        // ==========================================
        [HttpGet("CauHinhPhi")]
        public async Task<IActionResult> GetCauHinhPhi()
        {
            try
            {
                var data = await _adminService.LayDanhSachCauHinhPhiAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy cấu hình phí: " + ex.Message });
            }
        }

        [HttpPost("CapNhatPhiDangBai")]
        public async Task<IActionResult> CapNhatPhiDangBai([FromBody] AdminCapNhatPhiDangBaiRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var result = await _adminService.CapNhatPhiDangBaiAsync(request);
                return Ok(new
                {
                    success = result,
                    message = $"Cập nhật mức phí đăng bài thành công: {request.PhiDangBai:N0} VNĐ!"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật phí đăng bài: " + ex.Message });
            }
        }
    }
}
