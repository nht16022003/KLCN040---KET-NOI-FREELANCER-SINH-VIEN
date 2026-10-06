using FreelancerStudent.API.DTOs.Request;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace FreelancerStudent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiaoDichNapTienController : ControllerBase
    {
        private readonly IGiaoDichNapTienService _giaoDichNapTienService;
        private readonly IConfiguration _configuration;
        public GiaoDichNapTienController(
            IGiaoDichNapTienService giaoDichNapTienService, IConfiguration configuration)
        {
            _giaoDichNapTienService = giaoDichNapTienService;
            _configuration = configuration;
        }


        // =====================================================
        // TẠO GIAO DỊCH NẠP TIỀN
        // =====================================================
        [HttpPost("TaoGiaoDich/{maUser}")]
        public async Task<IActionResult> TaoGiaoDich(
            int maUser,
            [FromBody] TaoGiaoDichNapTien_RequestDTO request)
        {
            try
            {
                var ketQua =
                    await _giaoDichNapTienService
                        .taoGiaoDichNapTienAsync(
                            maUser,
                            request
                        );

                return Ok(new
                {
                    success = true,
                    message = "Tạo giao dịch nạp tiền thành công!",
                    data = ketQua
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // TEST HOÀN THÀNH GIAO DỊCH NẠP TIỀN
        // =====================================================
        // [HttpPost("HoanThanhGiaoDich/{maGiaoDich}")]
        // public async Task<IActionResult> HoanThanhGiaoDich(
        //     string maGiaoDich)
        // {
        //     try
        //     {
        //         var ketQua =
        //             await _giaoDichNapTienService
        //                 .hoanThanhGiaoDichNapTienAsync(
        //                     maGiaoDich
        //                 );

        //         return Ok(new
        //         {
        //             success = true,
        //             message = "Giao dịch nạp tiền đã hoàn thành!",
        //             data = ketQua
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new
        //         {
        //             success = false,
        //             message = ex.Message
        //         });
        //     }
        // }
        // =====================================================
        // KIỂM TRA TRẠNG THÁI GIAO DỊCH
        // =====================================================
        [HttpGet("TrangThai/{maGiaoDich}")]
        public async Task<IActionResult> TrangThai(
            string maGiaoDich)
        {
            try
            {
                var ketQua =
                    await _giaoDichNapTienService
                        .layTrangThaiGiaoDichAsync(
                            maGiaoDich
                        );

                return Ok(new
                {
                    success = true,
                    message = "Lấy trạng thái giao dịch thành công!",
                    data = ketQua
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                // ================================================
                // 1. LẤY SECRET KEY
                // ================================================
                string? secretKey =
                    _configuration["SePay:WebhookSecret"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    Console.WriteLine(
                        "SEPAY ERROR: Chưa cấu hình WebhookSecret"
                    );

                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Server chưa cấu hình SePay Secret Key."
                    });
                }


                // ================================================
                // 2. LẤY HEADER SEPAY
                // ================================================
                string signature =
                    Request.Headers["X-SePay-Signature"]
                        .ToString();

                string timestampString =
                    Request.Headers["X-SePay-Timestamp"]
                        .ToString();

                if (string.IsNullOrEmpty(signature) ||
                    string.IsNullOrEmpty(timestampString))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Thiếu chữ ký SePay."
                    });
                }


                // ================================================
                // 3. KIỂM TRA TIMESTAMP
                // ================================================
                if (!long.TryParse(
                        timestampString,
                        out long timestamp))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Timestamp không hợp lệ."
                    });
                }

                long currentTimestamp =
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (Math.Abs(currentTimestamp - timestamp) > 300)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Webhook đã hết hạn."
                    });
                }


                // ================================================
                // 4. ĐỌC RAW BODY
                // ================================================
                using StreamReader reader =
                    new StreamReader(
                        Request.Body,
                        Encoding.UTF8
                    );

                string rawBody =
                    await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(rawBody))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Webhook không có dữ liệu."
                    });
                }


                // ================================================
                // 5. TẠO LẠI CHỮ KÝ HMAC-SHA256
                // ================================================
                string signedData =
                    timestampString + "." + rawBody;

                using HMACSHA256 hmac =
                    new HMACSHA256(
                        Encoding.UTF8.GetBytes(secretKey)
                    );

                byte[] hash =
                    hmac.ComputeHash(
                        Encoding.UTF8.GetBytes(signedData)
                    );

                string expectedSignature =
                    "sha256=" +
                    Convert.ToHexString(hash).ToLowerInvariant();


                // ================================================
                // 6. SO SÁNH CHỮ KÝ AN TOÀN
                // ================================================
                byte[] receivedBytes =
                    Encoding.UTF8.GetBytes(signature);

                byte[] expectedBytes =
                    Encoding.UTF8.GetBytes(expectedSignature);

                bool signatureValid =
                    receivedBytes.Length ==
                    expectedBytes.Length
                    &&
                    CryptographicOperations.FixedTimeEquals(
                        receivedBytes,
                        expectedBytes
                    );

                if (!signatureValid)
                {
                    Console.WriteLine(
                        "SEPAY ERROR: Chữ ký không hợp lệ."
                    );

                    return Unauthorized(new
                    {
                        success = false,
                        message = "Chữ ký webhook không hợp lệ."
                    });
                }


                // ================================================
                // 7. CHUYỂN JSON THÀNH DTO
                // ================================================
                SePayWebhook_RequestDTO? body =
                    JsonSerializer.Deserialize
                        <SePayWebhook_RequestDTO>(
                            rawBody,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }
                        );

                if (body == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu webhook không hợp lệ."
                    });
                }


                // ================================================
                // 8. IN RA CONSOLE ĐỂ TEST
                // ================================================
                Console.WriteLine(
                    "========== SEPAY WEBHOOK =========="
                );

                Console.WriteLine(
                    "HMAC-SHA256: HỢP LỆ"
                );

                Console.WriteLine(
                    $"ID SePay: {body.id}"
                );

                Console.WriteLine(
                    $"Ngân hàng: {body.gateway}"
                );

                Console.WriteLine(
                    $"Tài khoản: {body.accountNumber}"
                );

                Console.WriteLine(
                    $"Loại giao dịch: {body.transferType}"
                );

                Console.WriteLine(
                    $"Số tiền: {body.transferAmount}"
                );

                Console.WriteLine(
                    $"Nội dung: {body.content}"
                );

                Console.WriteLine(
                    $"Mã thanh toán: {body.code}"
                );

                Console.WriteLine(
                    $"Mã tham chiếu: {body.referenceCode}"
                );

                Console.WriteLine(
                    "==================================="
                );


                // ================================================
                // 9. CHỈ XỬ LÝ GIAO DỊCH TIỀN VÀO
                // ================================================
                if (!string.Equals(
                        body.transferType,
                        "in",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Không phải giao dịch tiền vào."
                    });
                }


                // ================================================
                // 10. TÌM MÃ NAP TRONG NỘI DUNG
                // MB có thể tự loại bỏ dấu "_".
                // Ví dụ:
                // NAP_E3FDC9FECD -> NAPE3FDC9FECD
                // ================================================

                string noiDung = body.content ?? string.Empty;

                Match match = Regex.Match(
                    noiDung,
                    @"NAP_?[A-Fa-f0-9]{10}",
                    RegexOptions.IgnoreCase
                );

                if (!match.Success)
                {
                    Console.WriteLine(
                        "SEPAY: Không tìm thấy mã NAP trong nội dung."
                    );

                    return Ok(new
                    {
                        success = true,
                        message = "Không phải giao dịch nạp tiền của hệ thống."
                    });
                }


                // ================================================
                // 11. CHUẨN HÓA MÃ GIAO DỊCH
                // ================================================

                string maGiaoDich =
                    match.Value.ToUpperInvariant();

                // Nếu ngân hàng xóa "_"
                // NAPE3FDC9FECD
                // -> NAP_E3FDC9FECD
                if (!maGiaoDich.StartsWith("NAP_"))
                {
                    maGiaoDich =
                        "NAP_" + maGiaoDich.Substring(3);
                }

                Console.WriteLine(
                    $"SEPAY: Mã giao dịch chuẩn hóa = {maGiaoDich}"
                );


                // ================================================
                // 12. LẤY MÃ GIAO DỊCH NGÂN HÀNG
                // ================================================
                string maGiaoDichNganHang =
                    !string.IsNullOrWhiteSpace(body.referenceCode)
                        ? body.referenceCode
                        : $"SEPAY_{body.id}";


                // ================================================
                // 13. XỬ LÝ NẠP TIỀN
                // ================================================
                await _giaoDichNapTienService
                    .xuLyWebhookSePayAsync(
                        maGiaoDich,
                        body.transferAmount,
                        maGiaoDichNganHang
                    );


                Console.WriteLine(
                    $"SEPAY: Nạp tiền thành công cho {maGiaoDich}"
                );


                // ================================================
                // 14. TRẢ KẾT QUẢ CHO SEPAY
                // ================================================
                return Ok(new
                {
                    success = true,
                    message = "Webhook đã được xử lý thành công."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SEPAY WEBHOOK ERROR: {ex.Message}"
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi xử lý webhook."
                });
            }
        }
    }
}