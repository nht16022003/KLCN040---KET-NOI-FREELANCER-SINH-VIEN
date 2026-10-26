
using System.Security.Claims;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BanGiaoSanPhamController : ControllerBase
    {
        private readonly BanGiaoSanPhamService _service;

        public BanGiaoSanPhamController(
            BanGiaoSanPhamService service)
        {
            _service = service;
        }

        // Lấy mã người dùng từ phiên xác thực
        private int LayMaUser()
        {
            var giaTri = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(giaTri, out int maUser))
                throw new UnauthorizedAccessException(
                    "Phiên đăng nhập không hợp lệ.");

            return maUser;
        }

        // Xử lý lỗi nghiệp vụ
        private IActionResult XuLyLoi(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException =>
                    NotFound(new { message = ex.Message }),

                UnauthorizedAccessException =>
                    StatusCode(403,
                        new { message = ex.Message }),

                ArgumentException =>
                    BadRequest(new { message = ex.Message }),

                InvalidOperationException =>
                    Conflict(new { message = ex.Message }),

                _ => StatusCode(500,
                    new { message = "Lỗi hệ thống." })
            };
        }

        // 1. Freelancer bàn giao sản phẩm
        [HttpPost]
        public async Task<IActionResult> TaoBanGiao(
            [FromBody] TaoBanGiaoRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ketQua = await _service.TaoBanGiaoAsync(
                    request, LayMaUser());

                return Ok(new
                {
                    success = true,
                    message = "Bàn giao sản phẩm thành công.",
                    data = ketQua
                });
            }
            catch (Exception ex)
            {
                return XuLyLoi(ex);
            }
        }

        // 2. Xem lịch sử bàn giao
        [HttpGet("hopdong/{maHD}")]
        public async Task<IActionResult> LayDanhSach(
            string maHD)
        {
            try
            {
                var ketQua = await _service.LayDanhSachAsync(
                    maHD, LayMaUser());

                return Ok(new
                {
                    success = true,
                    data = ketQua
                });
            }
            catch (Exception ex)
            {
                return XuLyLoi(ex);
            }
        }

        // 3. Khách hàng gửi yêu cầu chỉnh sửa
        [HttpPost("yeucauchinhsua")]
        public async Task<IActionResult> GuiYeuCau(
            [FromBody] TaoYeuCauChinhSuaRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ketQua = await _service.GuiYeuCauAsync(
                    request, LayMaUser());

                return Ok(new
                {
                    success = true,
                    message = "Gửi yêu cầu chỉnh sửa thành công.",
                    data = ketQua
                });
            }
            catch (Exception ex)
            {
                return XuLyLoi(ex);
            }
        }

        // 4. Xem yêu cầu chỉnh sửa theo hợp đồng
        [HttpGet("yeucauchinhsua/{maHD}")]
        public async Task<IActionResult> LayYeuCau(
            string maHD)
        {
            try
            {
                var ketQua = await _service.LayYeuCauAsync(
                    maHD, LayMaUser());

                return Ok(new
                {
                    success = true,
                    data = ketQua
                });
            }
            catch (Exception ex)
            {
                return XuLyLoi(ex);
            }
        }

        // 5. Khách hàng nghiệm thu
        [HttpPut("nghiemthu/{maBanGiao:int}")]
        public async Task<IActionResult> NghiemThu(
            int maBanGiao)
        {
            try
            {
                var ketQua = await _service.NghiemThuAsync(
                    maBanGiao, LayMaUser());

                return Ok(new
                {
                    success = true,
                    message = "Nghiệm thu sản phẩm thành công.",
                    data = ketQua
                });
            }
            catch (Exception ex)
            {
                return XuLyLoi(ex);
            }
        }
    }
}
