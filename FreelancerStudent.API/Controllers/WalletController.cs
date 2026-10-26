using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // ==========================================
        // 1. LẤY THÔNG TIN VÍ
        // ==========================================
        [HttpGet("LayViTheoUser/{maUser}")]
        public async Task<IActionResult> LayViTheoUser(int maUser)
        {
            try
            {
                var request = new Wallet_RequestDTO { maUser = maUser };
                var ketquads = await _walletService.layViTheoMaUsersAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Lấy ví thành công!",
                    data = ketquads
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy ví: " + ex.Message
                });
            }
        }

        // ==========================================
        // 2. LẤY LỊCH SỬ GIAO DỊCH
        // ==========================================
        [HttpGet("LichSuGiaoDich/{maUser}")]
        public async Task<IActionResult> LayLichSuGiaoDich(int maUser)
        {
            try
            {
                var data = await _walletService.layLichSuGiaoDichAsync(maUser);
                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch sử giao dịch thành công!",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy lịch sử giao dịch: " + ex.Message
                });
            }
        }

        // ==========================================
        // 3. TẠO YÊU CẦU RÚT TIỀN MÔ PHỎNG
        // ==========================================
        [HttpPost("TaoYeuCauRutTien/{maUser}")]
        public async Task<IActionResult> TaoYeuCauRutTien(int maUser, [FromBody] TaoYeuCauRutTien_RequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var result = await _walletService.taoYeuCauRutTienAsync(maUser, request);
                return Ok(new
                {
                    success = true,
                    message = "Đã tạo yêu cầu rút tiền mô phỏng thành công! Số tiền đang được đóng băng chờ xác nhận.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tạo yêu cầu rút tiền: " + ex.Message });
            }
        }

        // ==========================================
        // 4. DANH SÁCH YÊU CẦU RÚT TIỀN
        // ==========================================
        [HttpGet("DanhSachRutTien/{maUser}")]
        public async Task<IActionResult> LayDanhSachRutTien(int maUser)
        {
            try
            {
                var data = await _walletService.layDanhSachRutTienAsync(maUser);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách yêu cầu rút tiền thành công!",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy danh sách yêu cầu rút tiền: " + ex.Message
                });
            }
        }

        // ==========================================
        // 5. XÁC NHẬN DUYỆT HOẶC HỦY RÚT TIỀN MÔ PHỎNG
        // ==========================================
        [HttpPost("XacNhanRutTienMoPhong/{maRutTien}")]
        public async Task<IActionResult> XacNhanRutTienMoPhong(
            int maRutTien,
            [FromQuery] int maUser,
            [FromQuery] bool chapNhan = true,
            [FromQuery] string? lyDo = null)
        {
            try
            {
                var result = await _walletService.xacNhanRutTienMoPhongAsync(maRutTien, maUser, chapNhan, lyDo);
                return Ok(new
                {
                    success = true,
                    message = chapNhan
                        ? "Đã xác nhận duyệt yêu cầu rút tiền mô phỏng thành công!"
                        : "Đã hủy/từ chối yêu cầu rút tiền và hoàn tiền về số dư khả dụng."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi xử lý yêu cầu rút tiền: " + ex.Message });
            }
        }
    }
}