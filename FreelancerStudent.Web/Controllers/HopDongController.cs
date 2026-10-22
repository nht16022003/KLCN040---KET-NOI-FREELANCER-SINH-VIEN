
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    [Authorize]
    public class HopDongController : Controller
    {
        private readonly IHopDongWebService _hopDongService;
        private readonly INhaTuyenDungWebService _ntdService;
        private readonly IFreelancerStudentWebService _freelancerService;
        private readonly IJobPostWebService _jobPostService;
        private readonly IWalletWebService _walletWebService;
        private readonly ITaskCongViecWebService _taskWebService;
        private readonly IDanhGiaWebService _danhGiaWebService;
        private readonly ILogger<HopDongController> _logger;

        public HopDongController(
            IHopDongWebService hopDongService,
            INhaTuyenDungWebService ntdService,
            IFreelancerStudentWebService freelancerService,
            IJobPostWebService jobPostService,
            IWalletWebService walletWebService,
            ITaskCongViecWebService taskWebService,
            IDanhGiaWebService danhGiaWebService,
            ILogger<HopDongController> logger)
        {
            _hopDongService = hopDongService;
            _ntdService = ntdService;
            _freelancerService = freelancerService;
            _jobPostService = jobPostService;
            _walletWebService = walletWebService;
            _taskWebService = taskWebService;
            _danhGiaWebService = danhGiaWebService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var role = HttpContext.Session.GetString("tenRole");

            var model = new DanhSachHopDongViewModel
            {
                TuKhoa = tuKhoa,
                TrangThai = trangThai
            };

            try
            {
                if (role == "FreelancerStudent")
                {
                    model.LaFreelancer = true;

                    // Tìm Freelancer thuộc User đăng nhập
                    var freeResponse = await _freelancerService
                        .layDanhSachFreelancerStudentAsync();

                    var freelancer = freeResponse.data?
                        .FirstOrDefault(x => x.maUser == maUser.Value);

                    if (freelancer == null)
                    {
                        model.Loi = "Chưa tìm thấy thông tin hồ sơ Freelancer của bạn.";
                        return View(model);
                    }

                    // Gọi API lấy danh sách hợp đồng của Freelancer
                    model.HopDongs = await _hopDongService
                        .LayDanhSachTheoFreelancerAsync(
                            freelancer.maFreelancerStudents,
                            tuKhoa,
                            trangThai);
                }
                else
                {
                    model.LaFreelancer = false;

                    // Tìm nhà tuyển dụng thuộc User đăng nhập
                    var response = await _ntdService
                        .layDanhSachNhaTuyenDungAsync();

                    if (!response.success || response.data == null)
                    {
                        model.Loi = "Không lấy được thông tin nhà tuyển dụng.";
                        return View(model);
                    }

                    var nhaTuyenDung = response.data
                        .FirstOrDefault(x => x.maUser == maUser.Value);

                    if (nhaTuyenDung == null)
                    {
                        // Kiểm tra dự phòng nếu tài khoản là FreelancerStudent
                        var freeResponse = await _freelancerService
                            .layDanhSachFreelancerStudentAsync();
                        var freelancer = freeResponse.data?
                            .FirstOrDefault(x => x.maUser == maUser.Value);

                        if (freelancer != null)
                        {
                            model.LaFreelancer = true;
                            model.HopDongs = await _hopDongService
                                .LayDanhSachTheoFreelancerAsync(
                                    freelancer.maFreelancerStudents,
                                    tuKhoa,
                                    trangThai);
                            return View(model);
                        }

                        model.Loi = "Tài khoản không thuộc nhà tuyển dụng hoặc freelancer hợp lệ.";
                        return View(model);
                    }

                    // Gọi API lấy danh sách hợp đồng của Nhà tuyển dụng
                    model.HopDongs = await _hopDongService
                        .LayDanhSachAsync(
                            nhaTuyenDung.maNhaTuyenDung,
                            tuKhoa,
                            trangThai);
                }
            }
            catch (HttpRequestException)
            {
                model.Loi = "Không thể kết nối đến API hợp đồng.";
            }
            catch (TaskCanceledException)
            {
                model.Loi = "Yêu cầu quá thời gian xử lý.";
            }
            catch (Exception)
            {
                model.Loi = "Có lỗi xảy ra khi tải hợp đồng.";
            }

            return View(model);
        }

        // GET: /HopDong/TaoHopDong?maJob=...&maFreelancerStudent=...&maUngTuyen=...
        [HttpGet]
        public async Task<IActionResult> TaoHopDong(string maJob, int maFreelancerStudent, int? maUngTuyen)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (string.IsNullOrWhiteSpace(maJob) || maFreelancerStudent <= 0)
            {
                TempData["ErrorMessage"] = "Thông tin bài tuyển dụng hoặc Freelancer không hợp lệ.";
                return RedirectToAction("Index", "NhaTuyenDung");
            }

            // 1. Kiểm tra xem Job này đã có hợp đồng chưa
            var hopDongHienCo = await _hopDongService.LayChiTietTheoJobAsync(maJob);
            if (hopDongHienCo != null && hopDongHienCo.trangThai != "DaHuy")
            {
                TempData["ErrorMessage"] = $"Công việc này đã có hợp đồng ({hopDongHienCo.maHD}) ở trạng thái '{hopDongHienCo.trangThai}'. Không thể tạo thêm hợp đồng mới.";
                return RedirectToAction("Index");
            }

            var model = new TaoHopDongViewModel
            {
                maJob = maJob,
                maFreelancerStudent = maFreelancerStudent,
                maUngTuyen = maUngTuyen,
                ngayBatDau = DateTime.Now,
                ngayKetThuc = DateTime.Now.AddDays(14),
                hinhthuclamviec = "Remote"
            };

            // 2. Lấy thông tin ứng tuyển từ NTD
            UngTuyenViewModel? ungTuyen = null;
            try
            {
                var ungTuyenRes = await _jobPostService.layDSUngTuyenTheoMaUser(maUser.Value);
                if (ungTuyenRes.success && ungTuyenRes.data != null)
                {
                    ungTuyen = ungTuyenRes.data.FirstOrDefault(u =>
                        u.maJob == maJob &&
                        u.maFreelancerStudents == maFreelancerStudent &&
                        (!maUngTuyen.HasValue || u.maUngTuyen == maUngTuyen.Value));
                }
            }
            catch { }

            // 3. Lấy thông tin chi tiết JobPost
            try
            {
                var jobRes = await _jobPostService.layChiTietJobPost(maJob);
                if (jobRes.success && jobRes.data != null && jobRes.data.Job != null)
                {
                    var job = jobRes.data.Job;
                    model.tieuDeJob = job.tieude ?? string.Empty;
                    model.moTaJob = job.mota ?? string.Empty;
                    model.nganSachGoc = job.thulao;
                    if (job.thoigiandukienhoanthanh > DateTime.Now)
                    {
                        model.ngayKetThuc = job.thoigiandukienhoanthanh;
                    }
                }
            }
            catch { }

            // Nếu JobDetail rỗng nhưng có ungTuyen
            if (string.IsNullOrEmpty(model.tieuDeJob) && ungTuyen != null)
            {
                model.tieuDeJob = ungTuyen.tieude ?? string.Empty;
                model.moTaJob = ungTuyen.mota ?? string.Empty;
                model.nganSachGoc = ungTuyen.thulao;
            }

            // 4. Lấy thông tin Freelancer
            if (ungTuyen != null)
            {
                model.tenFreelancer = ungTuyen.tenUser ?? "Freelancer";
                model.emailFreelancer = ungTuyen.emailUser;
                model.truongFreelancer = ungTuyen.tenTruong;
                model.chuyenNganhFreelancer = ungTuyen.tenChuyenNganh;
                model.avatarFreelancer = ungTuyen.avatar;
                model.thulaoDeXuat = ungTuyen.thulaoDeXuat;
                model.camKetThoiGian = ungTuyen.thoiGianHoanThanhDeXuat;
            }
            else
            {
                try
                {
                    var flRes = await _freelancerService.layDanhSachFreelancerStudentAsync();
                    var fl = flRes.data?.FirstOrDefault(f => f.maFreelancerStudents == maFreelancerStudent);
                    if (fl != null)
                    {
                        model.tenFreelancer = fl.tenUser ?? "Freelancer";
                        model.truongFreelancer = fl.tenTruong;
                        model.chuyenNganhFreelancer = fl.tenChuyenNganh;
                    }
                }
                catch { }
            }

            // Thiết lập số tiền thù lao thỏa thuận mặc định
            if (model.thulaoDeXuat.HasValue && model.thulaoDeXuat.Value > 0)
            {
                model.sotienkyquy = model.thulaoDeXuat.Value;
            }
            else if (model.nganSachGoc.HasValue && model.nganSachGoc.Value > 0)
            {
                model.sotienkyquy = model.nganSachGoc.Value;
            }
            else
            {
                model.sotienkyquy = 1000000;
            }

            // Điền sẵn các điều khoản giao kết cụ thể mẫu chuyên nghiệp
            model.dieuKhoan = @"ĐIỀU KHOẢN HỢP ĐỒNG GIAO KẾT DỊCH VỤ FREELANCE

Điều 1. PHẠM VI VÀ YÊU CẦU CÔNG VIỆC:
- Freelancer cam kết hoàn thành đúng và đầy đủ các yêu cầu công việc được nêu trong bài đăng tuyển dụng và các mốc bàn giao thỏa thuận.
- Sản phẩm bàn giao phải đảm bảo chất lượng, tính ứng dụng thực tế và không vi phạm bản quyền hay sao chép trái phép.

Điều 2. QUYỀN VÀ NGHĨA VỤ CỦA KHÁCH HÀNG:
- Cung cấp kịp thời đầy đủ tài liệu, yêu cầu chi tiết và hỗ trợ giải đáp thắc mắc chuyên môn trong quá trình thực hiện.
- Nghiệm thu sản phẩm đúng hạn khi Freelancer bàn giao theo từng mốc giai đoạn hoặc bàn giao tổng kết.
- Giải ngân và thanh toán đúng số tiền thù lao đã ký kết khi nghiệm thu đạt yêu cầu.

Điều 3. QUYỀN VÀ NGHĨA VỤ CỦA FREELANCER:
- Bàn giao sản phẩm đầy đủ và đúng thời hạn cam kết (deadline).
- Chịu trách nhiệm sửa đổi, bảo hành hoặc khắc phục các lỗi phát sinh trong phạm vi thỏa thuận ban đầu.
- Giữ bí mật toàn bộ thông tin kinh doanh, dữ liệu kỹ thuật và mã nguồn của Khách hàng.

Điều 4. QUYỀN SỞ HỮU TRÍ TUỆ VÀ BẢO MẬT:
- Sau khi thanh toán toàn bộ thù lao, toàn bộ mã nguồn, tài liệu thiết kế và quyền sở hữu sản phẩm hoàn toàn thuộc về Khách hàng.
- Freelancer không được tự ý tái sử dụng hoặc chia sẻ sản phẩm này cho bất kỳ bên thứ ba nào khi chưa có sự đồng ý bằng văn bản của Khách hàng.";

            // Thiết lập mốc bàn giao mặc định ban đầu
            model.giaiDoans.Add(new TaoGiaiDoanHopDongViewModel
            {
                tenGiaiDoan = "Giai đoạn 1: Bàn giao toàn bộ sản phẩm và nghiệm thu",
                moTa = "Hoàn thành và bàn giao sản phẩm hoàn chỉnh theo đúng yêu cầu đã mô tả.",
                hanChot = model.ngayKetThuc,
                tyLeGiaiNgan = 100,
                soTien = model.sotienkyquy
            });

            return View(model);
        }

        // POST: /HopDong/TaoHopDong
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoHopDong(TaoHopDongViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi WebService tạo dự thảo hợp đồng
            var response = await _hopDongService.TaoDuThaoHopDongAsync(model, maUser.Value);

            if (response.success && response.data != null)
            {
                TempData["SuccessMessage"] = $"Khởi tạo dự thảo hợp đồng {response.data.maHD} thành công! Trạng thái: Chờ Freelancer xác nhận. Hệ thống đã gửi thông báo đến Freelancer.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.message ?? "Có lỗi xảy ra khi tạo hợp đồng. Vui lòng thử lại.");
            return View(model);
        }

        // GET: /HopDong/ChiTiet?maHD=...
        [HttpGet]
        public async Task<IActionResult> ChiTiet(string maHD)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (string.IsNullOrWhiteSpace(maHD))
            {
                TempData["ErrorMessage"] = "Mã hợp đồng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var hopDong = await _hopDongService.LayChiTietTheoMaAsync(maHD);
            if (hopDong == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy thông tin hợp đồng {maHD}.";
                return RedirectToAction("Index");
            }

            // Kiểm tra xem user hiện tại có phải là Freelancer của hợp đồng này không
            if (hopDong.maUserFreelancer.HasValue && hopDong.maUserFreelancer.Value == maUser.Value)
            {
                hopDong.LaFreelancerCuaHopDong = true;
                // Nếu đang chờ Freelancer xác nhận thì Freelancer có quyền quyết định Xác nhận hoặc Từ chối
                if (hopDong.trangThai == "ChoXacNhan")
                {
                    hopDong.CoQuyenXacNhan = true;
                }
            }
            else
            {
                // Kiểm tra dự phòng nếu tài khoản đăng nhập là Freelancer
                var role = HttpContext.Session.GetString("tenRole");
                if (role == "FreelancerStudent")
                {
                    try
                    {
                        var flRes = await _freelancerService.layDanhSachFreelancerStudentAsync();
                        var fl = flRes.data?.FirstOrDefault(f => f.maUser == maUser.Value);
                        if (fl != null && fl.maFreelancerStudents == hopDong.maFreelancerStudent)
                        {
                            hopDong.LaFreelancerCuaHopDong = true;
                            if (hopDong.trangThai == "ChoXacNhan")
                            {
                                hopDong.CoQuyenXacNhan = true;
                            }
                        }
                    }
                    catch { }
                }
            }

            // Kiểm tra quyền sở hữu hợp đồng của Nhà tuyển dụng
            bool laChuHopDong =
                hopDong.maUserNhaTuyenDung.HasValue &&
                hopDong.maUserNhaTuyenDung.Value == maUser.Value;

            // Chỉ nhà tuyển dụng sở hữu hợp đồng mới có quyền ký quỹ
            hopDong.LaNhaTuyenDungCuaHopDong = laChuHopDong;

            hopDong.CoQuyenKyQuy =
                laChuHopDong &&
                hopDong.trangThai == "ChoKyQuy";
            if (!hopDong.LaFreelancerCuaHopDong &&
                !hopDong.LaNhaTuyenDungCuaHopDong)
            {
                TempData["ErrorMessage"] =
                    "Bạn không có quyền xem hợp đồng này.";

                return RedirectToAction("Index");
            }

            if (hopDong.trangThai is "DaThanhToan" or "DaHuyHoanTien")
            {
                var danhGia = await _danhGiaWebService.LayFormAsync(maHD, maUser.Value);
                if (danhGia.success && danhGia.data != null)
                {
                    hopDong.trangThaiDanhGia = danhGia.data.trangThaiDanhGia;
                    hopDong.maDanhGia = danhGia.data.danhGiaHienTai?.maDanhGia;
                    hopDong.coTheDanhGia = danhGia.data.coTheDanhGia;
                    hopDong.coTheChinhSuaDanhGia = danhGia.data.danhGiaHienTai?.coTheChinhSua == true;
                }
            }

            // và hợp đồng đang chờ ký quỹ
            if (hopDong.CoQuyenKyQuy &&
                hopDong.maUserNhaTuyenDung == maUser.Value &&
                hopDong.trangThai == "ChoKyQuy")
            {
                var ketQuaVi = await _walletWebService
                    .layViTheoUser(maUser.Value);

                if (ketQuaVi != null &&
                    ketQuaVi.success &&
                    ketQuaVi.data != null)
                {
                    hopDong.SoDuKhaDung =
                        ketQuaVi.data.soDuKhaDung;

                    hopDong.DaTaiThongTinVi = true;
                }
            }

            // Tải thông tin tiến độ dự án
            try
            {
                var tienDoRes = await _taskWebService.LayTienDoDuAnAsync(maHD, maUser.Value);
                if (tienDoRes.success && tienDoRes.data != null)
                {
                    hopDong.TienDoDuAn = tienDoRes.data;
                }
            }
            catch { }

            return View(hopDong);
        }

        // POST: /HopDong/XacNhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(string maHD, bool dongY, string? lyDoTuChoi)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (string.IsNullOrWhiteSpace(maHD))
            {
                TempData["ErrorMessage"] = "Mã hợp đồng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var response = await _hopDongService.XacNhanHopDongAsync(maHD, dongY, lyDoTuChoi, maUser.Value);

            if (response.success)
            {
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                TempData["ErrorMessage"] = response.message;
            }

            return RedirectToAction("ChiTiet", new { maHD });
        }

        // POST: /HopDong/KyQuy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KyQuy(string maHD)
        {
            // 1. Kiểm tra đăng nhập
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // 2. Kiểm tra mã hợp đồng
            if (string.IsNullOrWhiteSpace(maHD))
            {
                TempData["ErrorMessage"] = "Mã hợp đồng không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                // 3. Lấy thông tin hợp đồng
                var hopDong = await _hopDongService
                    .LayChiTietTheoMaAsync(maHD);

                if (hopDong == null)
                {
                    TempData["ErrorMessage"] =
                        "Không tìm thấy hợp đồng.";

                    return RedirectToAction("Index");
                }

                // 4. Kiểm tra quyền sở hữu hợp đồng
                if (!hopDong.maUserNhaTuyenDung.HasValue ||
                    hopDong.maUserNhaTuyenDung.Value != maUser.Value)
                {
                    TempData["ErrorMessage"] =
                        "Bạn không có quyền ký quỹ hợp đồng này.";

                    return RedirectToAction("Index");
                }

                // 5. Kiểm tra trạng thái hợp đồng
                if (hopDong.trangThai != "ChoKyQuy")
                {
                    TempData["ErrorMessage"] =
                        "Hợp đồng không ở trạng thái chờ ký quỹ.";

                    return RedirectToAction("ChiTiet", new { maHD });
                }

                // 6. Gọi Service để thực hiện ký quỹ
                var response = await _hopDongService
                    .KyQuyHopDongAsync(maHD, maUser.Value);

                // 7. Xử lý kết quả
                if (response != null && response.success)
                {
                    TempData["SuccessMessage"] =
                        string.IsNullOrWhiteSpace(response.message)
                            ? "Ký quỹ hợp đồng thành công."
                            : response.message;
                }
                else
                {
                    TempData["ErrorMessage"] =
                        response?.message
                        ?? "Ký quỹ thất bại. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý ký quỹ hợp đồng {MaHopDong} cho tài khoản {MaUser}.", maHD, maUser.Value);
                // Không hiển thị chi tiết lỗi hệ thống cho người dùng
                TempData["ErrorMessage"] =
                    "Không thể xử lý yêu cầu ký quỹ. " +
                    "Vui lòng kiểm tra lại trạng thái hợp đồng và số dư ví.";
            }

            // 8. Quay lại trang chi tiết hợp đồng
            return RedirectToAction("ChiTiet", new { maHD });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NghiemThu(string maHD)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            if (HttpContext.Session.GetString("tenRole") != "NhaTuyenDung")
                return Forbid();

            var result = await _hopDongService.NghiemThuAsync(maHD, maUser.Value);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction("ChiTiet", new { maHD });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGiaGiaiDoan(string maHD, int? maGiaiDoan, string trangThai)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            if (HttpContext.Session.GetString("tenRole") != "NhaTuyenDung")
                return Forbid();
            if (string.IsNullOrWhiteSpace(maHD) || trangThai is not ("Dat" or "ChuaDat"))
            {
                TempData["ErrorMessage"] = "Thông tin nghiệm thu giai đoạn không hợp lệ.";
                return RedirectToAction("ChiTiet", new { maHD });
            }

            var result = await _taskWebService.DanhGiaGiaiDoanAsync(maHD, maGiaiDoan, trangThai, maUser.Value);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction("ChiTiet", new { maHD });
        }

        [HttpGet]
        public async Task<IActionResult> ThanhToan(string maHD)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            if (HttpContext.Session.GetString("tenRole") != "NhaTuyenDung")
                return Forbid();

            var result = await _hopDongService.LayThongTinThanhToanAsync(maHD, maUser.Value);
            if (!result.success || result.data == null)
            {
                TempData["ErrorMessage"] = result.message;
                return RedirectToAction("ChiTiet", new { maHD });
            }
            return View(result.data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(string maHD)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
                return RedirectToAction("DangNhap", "Account");
            if (HttpContext.Session.GetString("tenRole") != "NhaTuyenDung")
                return Forbid();

            var result = await _hopDongService.ThanhToanAsync(maHD, maUser.Value);
            if (!result.success && result.message?.Contains("Số dư không đủ để giải ngân", StringComparison.OrdinalIgnoreCase) == true)
            {
                TempData["ErrorMessage"] = result.message;
                return RedirectToAction("Index", "Wallet");
            }
            if (result.success)
            {
                TempData["MoPopupDanhGia"] = maHD;
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
            }
            return RedirectToAction("ChiTiet", new { maHD });
        }

        // ================= Quản lý Tiến độ Dự án (FRL-UC-09) =================

        // POST: /HopDong/ThemTask
        [HttpPost]
        public async Task<IActionResult> ThemTask([FromBody] TaoTaskCongViecViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
            }

            if (!ModelState.IsValid)
            {
                var errorMsg = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errorMsg });
            }

            var result = await _taskWebService.TaoTaskAsync(model, maUser.Value);
            return Json(result);
        }

        // POST: /HopDong/CapNhatTask
        [HttpPost]
        public async Task<IActionResult> CapNhatTask([FromBody] CapNhatTaskCongViecViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
            }

            if (!ModelState.IsValid)
            {
                var errorMsg = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errorMsg });
            }

            var result = await _taskWebService.CapNhatTaskAsync(model, maUser.Value);
            return Json(result);
        }

        // POST: /HopDong/CapNhatTrangThaiTask
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThaiTask([FromBody] CapNhatTrangThaiTaskViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
            }

            if (!ModelState.IsValid)
            {
                var errorMsg = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errorMsg });
            }

            var result = await _taskWebService.CapNhatTrangThaiTaskAsync(model, maUser.Value);
            return Json(result);
        }

        // POST: /HopDong/XoaTask
        [HttpPost]
        public async Task<IActionResult> XoaTask([FromBody] int maTask)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
            }

            var result = await _taskWebService.XoaTaskAsync(maTask, maUser.Value);
            return Json(result);
        }

        // GET: /HopDong/LayTienDoDuAn?maHD=...
        [HttpGet]
        public async Task<IActionResult> LayTienDoDuAn(string maHD)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
            }

            var result = await _taskWebService.LayTienDoDuAnAsync(maHD, maUser.Value);
            return Json(result);
        }
    }
}