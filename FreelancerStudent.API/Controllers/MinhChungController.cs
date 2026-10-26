using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinhChungController : ControllerBase
    {
        private readonly IMinhChungService _minhChungService;

        public MinhChungController(IMinhChungService minhChungService)
        {
            _minhChungService = minhChungService;
        }

        /// <summary>
        /// Tải lên ảnh Thẻ sinh viên, chạy OCR trích xuất thông tin và đối chiếu với hồ sơ hiện tại
        /// </summary>
        [HttpPost("quet-the-sinh-vien")]
        public async Task<IActionResult> QuetTheSinhVien([FromForm] IFormFile file, [FromQuery] int maUser)
        {
            try
            {
                if (maUser <= 0)
                {
                    return BadRequest(new { success = false, message = "Mã người dùng không hợp lệ." });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Vui lòng chọn hoặc chụp ảnh thẻ sinh viên." });
                }

                var result = await _minhChungService.QuetVaLuuTheSinhVienAsync(file, maUser);

                return Ok(new
                {
                    success = result.ThanhCong,
                    message = result.ThongBao,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi xử lý ảnh thẻ sinh viên: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Sinh viên xác nhận thông tin sau khi kiểm tra OCR, cập nhật hồ sơ và lưu minh chứng
        /// </summary>
        [HttpPost("xac-nhan-the-sinh-vien")]
        public async Task<IActionResult> XacNhanTheSinhVien([FromBody] XacNhanTheSinhVien_RequestDTO request)
        {
            try
            {
                if (request == null || request.MaUser <= 0)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu xác nhận không hợp lệ." });
                }

                if (string.IsNullOrWhiteSpace(request.FileMinhChungUrl))
                {
                    return BadRequest(new { success = false, message = "Thiếu đường dẫn file ảnh minh chứng." });
                }

                var thanhCong = await _minhChungService.XacNhanVaCapNhatHoSoTheSinhVienAsync(request);

                return Ok(new
                {
                    success = thanhCong,
                    message = "Cập nhật hồ sơ từ Thẻ sinh viên và gửi minh chứng thành công! Hồ sơ đang ở trạng thái 'Đang gửi' chờ thẩm định."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi xác nhận hồ sơ: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử các minh chứng đã nộp của sinh viên
        /// </summary>
        [HttpGet("lich-su/{maUser:int}")]
        public async Task<IActionResult> LayLichSu(int maUser)
        {
            try
            {
                if (maUser <= 0)
                {
                    return BadRequest(new { success = false, message = "Mã sinh viên không hợp lệ." });
                }

                var list = await _minhChungService.LayLichSuMinhChungAsync(maUser);

                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch sử minh chứng thành công.",
                    data = list
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy lịch sử minh chứng: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách các chuyên ngành đào tạo trong hệ thống
        /// </summary>
        [HttpGet("chuyen-nganh")]
        public async Task<IActionResult> LayDanhSachChuyenNganh()
        {
            try
            {
                var list = await _minhChungService.LayDanhSachChuyenNganhAsync();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách chuyên ngành thành công.",
                    data = list
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy danh sách chuyên ngành: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin hồ sơ hiện tại của sinh viên để đối chiếu
        /// </summary>
        [HttpGet("thong-tin-hien-tai/{maUser:int}")]
        public async Task<IActionResult> LayThongTinHienTai(int maUser)
        {
            try
            {
                if (maUser <= 0)
                {
                    return BadRequest(new { success = false, message = "Mã người dùng không hợp lệ." });
                }

                var data = await _minhChungService.LayThongTinHienTaiAsync(maUser);
                if (data == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy hồ sơ sinh viên." });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin sinh viên thành công.",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy thông tin sinh viên: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái xác thực minh chứng sinh viên
        /// </summary>
        [HttpGet("kiem-tra-xac-thuc/{maUser:int}")]
        public async Task<IActionResult> KiemTraXacThuc(int maUser)
        {
            try
            {
                var result = await _minhChungService.KiemTraTrangThaiXacThucAsync(maUser);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi kiểm tra trạng thái xác thực: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Admin lấy danh sách các minh chứng thẻ sinh viên đang chờ duyệt
        /// </summary>
        [HttpGet("danh-sach-cho-duyet")]
        public async Task<IActionResult> LayDanhSachChoDuyet()
        {
            try
            {
                var list = await _minhChungService.LayDanhSachMinhChungChoDuyetAsync();
                return Ok(new
                {
                    success = true,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi tải danh sách chờ duyệt: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Admin phê duyệt hoặc từ chối minh chứng thẻ sinh viên
        /// </summary>
        [HttpPost("phe-duyet")]
        public async Task<IActionResult> PheDuyet([FromBody] DTOs.ReponseDTOs.PheDuyetMinhChung_RequestDTO request)
        {
            try
            {
                if (request == null || request.MaMinhChung <= 0)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu phê duyệt không hợp lệ." });
                }

                var thanhCong = await _minhChungService.PheDuyetMinhChungAsync(request);
                if (!thanhCong)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy bản ghi minh chứng cần duyệt." });
                }

                return Ok(new
                {
                    success = true,
                    message = request.ChapNhan
                        ? "Đã phê duyệt minh chứng sinh viên thành công! Sinh viên đã được mở khóa toàn bộ tính năng."
                        : "Đã từ chối minh chứng sinh viên."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi phê duyệt minh chứng: " + ex.Message
                });
            }
        }
    }
}



