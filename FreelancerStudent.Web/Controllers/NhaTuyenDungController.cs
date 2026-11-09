using System.Security.Cryptography.X509Certificates;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class NhaTuyenDungController : Controller
    {

        private readonly INhaTuyenDungWebService _nhaTuyenDungWebService;
        private readonly IJobPostWebService _jobPostWebService;
        private readonly IDanhGiaWebService _danhGiaWebService;
        private readonly IYeuThichWebService _yeuThichService;
        private readonly IUngThueWebService _ungThueWebService;
        private readonly IFreelancerStudentWebService _freelancerWebService;
        private readonly IAuthWebService _authWebService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NhaTuyenDungController(
            INhaTuyenDungWebService nhaTuyenDungWebService, 
            IJobPostWebService jobPostWebService,
            IDanhGiaWebService danhGiaWebService,
            IYeuThichWebService yeuThichService,
            IUngThueWebService ungThueWebService,
            IFreelancerStudentWebService freelancerWebService,
            IAuthWebService authWebService,
            IWebHostEnvironment webHostEnvironment)
        {
            _nhaTuyenDungWebService = nhaTuyenDungWebService;
            _jobPostWebService = jobPostWebService;
            _danhGiaWebService = danhGiaWebService;
            _yeuThichService = yeuThichService;
            _ungThueWebService = ungThueWebService;
            _freelancerWebService = freelancerWebService;
            _authWebService = authWebService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Xem hồ sơ Nhà tuyển dụng (cá nhân hoặc công khai)
        /// Hỗ trợ cả route /NhaTuyenDung/Profile và /Employer/Profile
        /// </summary>
        [HttpGet]
        [Route("NhaTuyenDung/Profile")]
        [Route("NhaTuyenDung/Profile/{id:int}")]
        [Route("Employer/Profile")]
        [Route("Employer/Profile/{id:int}")]
        public async Task<IActionResult> Profile(int? id, int? maUser, int? maNhaTuyenDung)
        {
            var currentUserId = HttpContext.Session.GetInt32("maUser");

            // Xác định đối tượng cần truy vấn
            int? targetNtdId = id.HasValue && id > 0 ? id : (maNhaTuyenDung.HasValue && maNhaTuyenDung > 0 ? maNhaTuyenDung : null);
            int? targetUserId = maUser.HasValue && maUser > 0 ? maUser : null;

            // Nếu không truyền ID nào, dùng tài khoản đang đăng nhập
            if (!targetNtdId.HasValue && !targetUserId.HasValue)
            {
                if (currentUserId.HasValue && currentUserId > 0)
                {
                    targetUserId = currentUserId.Value;
                }
                else
                {
                    return RedirectToAction("DangNhap", "Account");
                }
            }

            ApiReponse<FreelancerStudent.Web.ViewModels.NhaTuyenDungProfileViewModel>? result = null;

            if (targetNtdId.HasValue)
            {
                result = await _nhaTuyenDungWebService.layProfileNhaTuyenDungAsync(targetNtdId.Value);
            }
            else if (targetUserId.HasValue)
            {
                result = await _nhaTuyenDungWebService.layProfileNhaTuyenDungTheoMaUserAsync(targetUserId.Value);
            }

            if (result == null || !result.success || result.data == null)
            {
                TempData["Error"] = result?.message ?? "Không tìm thấy hồ sơ Nhà tuyển dụng.";
                return RedirectToAction("Search", "NhaTuyenDung");
            }

            var profile = result.data;
            ViewData["IsProfileOwner"] = currentUserId.HasValue && profile.NhaTuyenDung != null && profile.NhaTuyenDung.MaUser == currentUserId.Value;

            if (profile.NhaTuyenDung != null)
            {
                try
                {
                    var stats = await _danhGiaWebService.LayNhaTuyenDungAsync(profile.NhaTuyenDung.MaNhaTuyenDung, 1, 50);
                    if (stats?.success == true && stats.data != null)
                    {
                        profile.ThongKeDanhGia = stats.data;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NhaTuyenDungController] Lỗi khi nạp đánh giá: {ex.Message}");
                }
            }

            return View(profile);
        }

        //Tuấn
        [HttpGet]
        public async Task<IActionResult> DanhSachUngVien(string? maJob)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            // Gọi API lấy danh sách các bạn sinh viên đã nộp đơn vào Job của NTD này
            var ketQua = await _jobPostWebService.layDSUngTuyenTheoMaUser(maUser.Value);

            var dsUngTuyen = ketQua?.data ?? new List<UngTuyenViewModel>();
            // Nếu có truyền maJob -> Chỉ lọc lấy các ứng viên nộp vào bài Job này
            if (!string.IsNullOrEmpty(maJob))
            {
                dsUngTuyen = dsUngTuyen.Where(u => u.maJob == maJob).ToList();
                ViewBag.CurrentMaJob = maJob;
            }

            return View(dsUngTuyen);
        }


        //Tuấn Anh
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            //Gọi service lấy danh sách từ API
            var layDanhSachNhaTuyenDung = await _nhaTuyenDungWebService.layDanhSachNhaTuyenDungAsync();

            //  IN XEM WEB ĐÃ NHẬN ĐƯỢC BAO NHIÊU PHẦN TỬ:
            Console.WriteLine($"===> [SỐ LƯỢNG NTD NHẬN ĐƯỢC]: {layDanhSachNhaTuyenDung.data?.Count ?? 0}");


            //Truyền ds sang View
            var danhsachNTD = layDanhSachNhaTuyenDung.data ?? new List<FreelancerStudent.Web.ViewModels.NhaTuyenDungViewModel>();

            return View(danhsachNTD);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLuuFreelancer([FromBody] ToggleLuuFreelancerRequest request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!maUser.HasValue || (role != "NhaTuyenDung" && role != "Employer"))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập với tư cách Nhà tuyển dụng để lưu Freelancer yêu thích!" });
            }

            if (request == null || request.MaFreelancerStudent <= 0)
            {
                return Json(new { success = false, message = "Thông tin Freelancer không hợp lệ!" });
            }

            var daLuu = await _yeuThichService.KiemTraLuuFreelancerAsync(maUser.Value, request.MaFreelancerStudent);
            if (daLuu)
            {
                var boLuu = await _yeuThichService.BoLuuFreelancerAsync(maUser.Value, request.MaFreelancerStudent);
                return Json(new { success = boLuu.success, daLuu = false, message = "Đã bỏ lưu Freelancer khỏi danh sách yêu thích!" });
            }
            else
            {
                var luu = await _yeuThichService.LuuFreelancerAsync(maUser.Value, request.MaFreelancerStudent, request.GhiChu);
                return Json(new { success = luu.success, daLuu = true, message = "Đã lưu Freelancer vào danh sách yêu thích!" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> FreelancerYeuThich()
        {
            var maUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!maUser.HasValue || (role != "NhaTuyenDung" && role != "Employer"))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách Nhà tuyển dụng để xem danh sách Freelancer yêu thích.";
                return RedirectToAction("DangNhap", "Account");
            }

            var danhSach = await _yeuThichService.LayDanhSachFreelancerYeuThichAsync(maUser.Value);
            return View(danhSach);
        }

        // ==================== TRỰC TIẾP THUÊ FREELANCER (DIRECT HIRE) ====================

        /// <summary>
        /// Bước 1: Nhà tuyển dụng chọn JobPost mở của mình để gửi yêu cầu thuê Freelancer
        /// Hỗ trợ nhận diện linh hoạt các tham số định danh: maFreelancerStudent, maFreelancerStudents, maUser, id
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ChonJobThueFreelancer(int? maFreelancerStudent, int? maFreelancerStudents, int? maUser, int? id)
        {
            var sessionMaUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? HttpContext.Session.GetString("role") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!sessionMaUser.HasValue || (role != "NhaTuyenDung" && role != "Employer" && role != "Client"))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách Nhà tuyển dụng để thực hiện thuê ứng viên.";
                return RedirectToAction("DangNhap", "Account");
            }

            int targetId = (maFreelancerStudent.HasValue && maFreelancerStudent > 0)
                ? maFreelancerStudent.Value
                : ((maFreelancerStudents.HasValue && maFreelancerStudents > 0)
                    ? maFreelancerStudents.Value
                    : ((maUser.HasValue && maUser > 0)
                        ? maUser.Value
                        : (id.HasValue && id > 0 ? id.Value : 0)));

            if (targetId <= 0)
            {
                TempData["ErrorMessage"] = "Thông tin Freelancer không hợp lệ.";
                return RedirectToAction("Index", "FreelancerStudent");
            }

            // Lấy thông tin Freelancer đầy đủ từ danh sách hoặc chi tiết (hỗ trợ cả maFreelancerStudents và maUser)
            var listResult = await _freelancerWebService.layDanhSachFreelancerStudentAsync();
            var fl = listResult?.data?.FirstOrDefault(f => f.maFreelancerStudents == targetId || f.maUser == targetId);

            if (fl == null)
            {
                var flChiTietRes = await _freelancerWebService.layChiTietHoSoAsync(targetId);
                var flChiTiet = flChiTietRes?.data;
                if (flChiTiet != null)
                {
                    fl = new FreelacerStudentViewModel
                    {
                        maFreelancerStudents = flChiTiet.MaFreelancerStudents,
                        maUser = flChiTiet.MaUser,
                        tenUser = flChiTiet.TenUser,
                        avatar = flChiTiet.Avatar,
                        tenChuyenNganh = flChiTiet.TenChuyenNganh,
                        tenTruong = flChiTiet.TenTruong,
                        namThu = flChiTiet.NamThu,
                        GPA = flChiTiet.GPA,
                        gioithieu = flChiTiet.Gioithieu,
                        kyNangCoBan = flChiTiet.KyNangCoBan,
                        danhSachKyNang = flChiTiet.DanhSachKyNang ?? new List<string>(),
                        trangthaiNhanViec = flChiTiet.TrangthaiNhanViec,
                        chiPhiTu = flChiTiet.ChiPhiTu
                    };
                }
            }

            if (fl == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin Freelancer được chọn.";
                return RedirectToAction("Index", "FreelancerStudent");
            }

            // Chuẩn hóa mã FreelancerStudents chính xác của ứng viên
            int normalizedMaFL = fl.maFreelancerStudents > 0 ? fl.maFreelancerStudents : targetId;

            // Lấy danh sách JobPost của Nhà tuyển dụng
            var jobsRes = await _jobPostWebService.layJobCuaToiAsync(sessionMaUser.Value);
            var dsJob = jobsRes?.data?.Where(j => string.Equals(j.status, "DangTuyen", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(j.status)).ToList() ?? new List<JobPostViewModel>();

            var model = new ChonJobThueFreelancerViewModel
            {
                MaFreelancerStudent = normalizedMaFL,
                TenFreelancer = fl.tenUser ?? "Ứng viên sinh viên",
                AvatarFreelancer = fl.avatar,
                ChuyenNganh = fl.tenChuyenNganh,
                TruongDaiHoc = fl.tenTruong,
                NamThu = fl.namThu,
                GPA = fl.GPA,
                SoSaoDanhGia = fl.diemDanhGiaTrungBinh,
                SoLuotDanhGia = fl.soLuotDanhGia,
                GioiThieu = fl.gioithieu,
                TrangthaiNhanViec = fl.trangthaiNhanViec,
                ChiPhiTu = fl.chiPhiTu,
                DanhSachKyNang = fl.danhSachKyNang ?? new List<string>(),
                FreelancerCard = fl,
                DanhSachJob = dsJob
            };

            return View(model);
        }

        /// <summary>
        /// Bước 2: Xem trước và xác nhận thông tin trước khi gửi yêu cầu thuê
        /// </summary>
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> XacNhanThueFreelancer(int? maFreelancerStudent, int? maFreelancerStudents, int? maUser, int? id, string maJob)
        {
            var sessionMaUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? HttpContext.Session.GetString("role") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!sessionMaUser.HasValue || (role != "NhaTuyenDung" && role != "Employer" && role != "Client"))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách Nhà tuyển dụng.";
                return RedirectToAction("DangNhap", "Account");
            }

            int targetId = (maFreelancerStudent.HasValue && maFreelancerStudent > 0)
                ? maFreelancerStudent.Value
                : ((maFreelancerStudents.HasValue && maFreelancerStudents > 0)
                    ? maFreelancerStudents.Value
                    : ((maUser.HasValue && maUser > 0)
                        ? maUser.Value
                        : (id.HasValue && id > 0 ? id.Value : 0)));

            if (targetId <= 0 || string.IsNullOrWhiteSpace(maJob))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn Freelancer và công việc tuyển dụng.";
                return RedirectToAction("ChonJobThueFreelancer", new { maFreelancerStudent = targetId > 0 ? targetId : (int?)null });
            }

            // Lấy thông tin Freelancer
            var flRes = await _freelancerWebService.layChiTietHoSoAsync(targetId);
            var fl = flRes?.data;
            int normalizedMaFL = (fl != null && fl.MaFreelancerStudents > 0) ? fl.MaFreelancerStudents : targetId;

            // Lấy chi tiết JobPost
            var jobRes = await _jobPostWebService.layChiTietJobPost(maJob);
            var job = jobRes?.data?.Job;

            if (job == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin bài đăng công việc.";
                return RedirectToAction("ChonJobThueFreelancer", new { maFreelancerStudent = normalizedMaFL });
            }

            var model = new XacNhanThueFreelancerViewModel
            {
                MaFreelancerStudent = normalizedMaFL,
                TenFreelancer = fl?.TenUser ?? "Ứng viên sinh viên",
                AvatarFreelancer = fl?.Avatar,
                ChuyenNganh = fl?.TenChuyenNganh,
                TruongDaiHoc = fl?.TenTruong,
                MaJob = maJob,
                TieuDeJob = job.tieude,
                ThuLaoJob = job.thulao,
                ThoiHanHoanThanhJob = job.thoigiandukienhoanthanh,
                MoTaJob = job.mota,
                FormModel = new GuiYeuCauThueRequestModel
                {
                    maFreelancerStudent = normalizedMaFL,
                    maJob = maJob,
                    tieuDeCongViec = job.tieude,
                    moTaCongViec = job.mota ?? string.Empty,
                    nganSachDeNghi = job.thulao,
                    thoiHanDuKien = job.thoigiandukienhoanthanh != DateTime.MinValue ? job.thoigiandukienhoanthanh.ToString("yyyy-MM-dd") : null
                }
            };

            return View(model);
        }

        /// <summary>
        /// Bước 3: Gửi yêu cầu thuê chính thức sang API UngThue
        /// Hỗ trợ cả model binding trực tiếp và prefix "FormModel" từ View
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiYeuCauThue(
            GuiYeuCauThueRequestModel model,
            [Microsoft.AspNetCore.Mvc.Bind(Prefix = "FormModel")] GuiYeuCauThueRequestModel? formModel)
        {
            var sessionMaUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? HttpContext.Session.GetString("role") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!sessionMaUser.HasValue || (role != "NhaTuyenDung" && role != "Employer" && role != "Client"))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách Nhà tuyển dụng.";
                return RedirectToAction("DangNhap", "Account");
            }

            // Hợp nhất dữ liệu giữa model trực tiếp và formModel có tiền tố
            var targetModel = new GuiYeuCauThueRequestModel
            {
                maFreelancerStudent = (formModel != null && formModel.maFreelancerStudent > 0) ? formModel.maFreelancerStudent : model.maFreelancerStudent,
                maJob = (!string.IsNullOrWhiteSpace(formModel?.maJob)) ? formModel.maJob : model.maJob,
                tieuDeCongViec = (!string.IsNullOrWhiteSpace(formModel?.tieuDeCongViec)) ? formModel.tieuDeCongViec : model.tieuDeCongViec,
                moTaCongViec = (!string.IsNullOrWhiteSpace(formModel?.moTaCongViec)) ? formModel.moTaCongViec : model.moTaCongViec,
                nganSachDeNghi = formModel?.nganSachDeNghi ?? model.nganSachDeNghi,
                thoiHanDuKien = (!string.IsNullOrWhiteSpace(formModel?.thoiHanDuKien)) ? formModel.thoiHanDuKien : model.thoiHanDuKien
            };

            if (targetModel.maFreelancerStudent <= 0 || string.IsNullOrWhiteSpace(targetModel.maJob) || string.IsNullOrWhiteSpace(targetModel.tieuDeCongViec))
            {
                TempData["ErrorMessage"] = "Dữ liệu gửi yêu cầu thuê không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction("XacNhanThueFreelancer", new { maFreelancerStudent = targetModel.maFreelancerStudent, maJob = targetModel.maJob });
            }

            var result = await _ungThueWebService.GuiYeuCauThueAsync(targetModel, sessionMaUser.Value);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Gửi yêu cầu thuê Freelancer thành công! Yêu cầu đang chờ sinh viên phản hồi.";
                return RedirectToAction("QuanLyYeuCauThue");
            }

            TempData["ErrorMessage"] = result.message ?? "Không thể gửi yêu cầu thuê Freelancer.";
            return RedirectToAction("XacNhanThueFreelancer", new { maFreelancerStudent = targetModel.maFreelancerStudent, maJob = targetModel.maJob });
        }

        /// <summary>
        /// Trang Quản lý danh sách các yêu cầu thuê đã gửi từ Nhà tuyển dụng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> QuanLyYeuCauThue()
        {
            var maUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? HttpContext.Session.GetString("role") ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!maUser.HasValue || (role != "NhaTuyenDung" && role != "Employer" && role != "Client"))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách Nhà tuyển dụng để xem các yêu cầu thuê.";
                return RedirectToAction("DangNhap", "Account");
            }

            var res = await _ungThueWebService.LayDanhSachYeuCauThueTheoNhaTuyenDungAsync(maUser.Value);
            var list = res?.data ?? new List<YeuCauThueViewModel>();

            return View(list);
        }

        /// <summary>
        /// Cập nhật thông tin cơ bản của Nhà tuyển dụng từ modal trên Profile
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThongTinCoBan(CapNhatThongTinNhaTuyenDungViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("DangNhap", "Account");
            }

            model.MaUser = maUser.Value;

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = !string.IsNullOrWhiteSpace(errors) ? errors : "Thông tin nhập vào không hợp lệ.";
                return RedirectToAction(nameof(Profile));
            }

            var res = await _nhaTuyenDungWebService.capNhatThongTinNTDAsync(model);
            if (res.success)
            {
                if (!string.IsNullOrWhiteSpace(model.Tencongty))
                {
                    HttpContext.Session.SetString("hovaten", model.Tencongty);
                }
                TempData["SuccessMessage"] = "Cập nhật thông tin doanh nghiệp thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể cập nhật thông tin doanh nghiệp.";
            }

            return RedirectToAction(nameof(Profile));
        }

        /// <summary>
        /// Cập nhật giới thiệu về doanh nghiệp / đơn vị từ modal trên Profile
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatGioiThieu(string? gioiThieu)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("DangNhap", "Account");
            }

            var model = new CapNhatGioiThieuNhaTuyenDungViewModel
            {
                MaUser = maUser.Value,
                Gioithieu = gioiThieu
            };

            var res = await _nhaTuyenDungWebService.capNhatGioiThieuNTDAsync(model);
            if (res.success)
            {
                TempData["SuccessMessage"] = "Cập nhật giới thiệu đơn vị thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = res.message ?? "Không thể cập nhật giới thiệu đơn vị.";
            }

            return RedirectToAction(nameof(Profile));
        }

        /// <summary>
        /// Cập nhật ảnh đại diện / Logo nhà tuyển dụng có preview trực tiếp
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatAvatar(IFormFile avatar)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("DangNhap", "Account");
            }

            if (avatar == null || avatar.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn tệp ảnh đại diện / logo.";
                return RedirectToAction(nameof(Profile));
            }

            // Giới hạn 5MB
            if (avatar.Length > 5 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "Dung lượng ảnh tối đa là 5MB.";
                return RedirectToAction(nameof(Profile));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Định dạng ảnh không hợp lệ. Vui lòng chọn file .jpg, .jpeg, .png, .webp hoặc .gif.";
                return RedirectToAction(nameof(Profile));
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"ntd_logo_{maUser.Value}_{DateTime.UtcNow.Ticks}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            var avatarUrl = $"/uploads/avatars/{fileName}";

            // 1. Cập nhật bảng Users (avatarUrl)
            var authRes = await _authWebService.CapNhatAvatarAsync(maUser.Value, avatarUrl);

            // 2. Cập nhật bảng NhaTuyenDung (logo)
            var ntdRes = await _nhaTuyenDungWebService.capNhatLogoNTDAsync(maUser.Value, avatarUrl);

            if (!authRes.success && !ntdRes.success)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                TempData["ErrorMessage"] = authRes.message ?? ntdRes.message ?? "Cập nhật ảnh đại diện thất bại.";
                return RedirectToAction(nameof(Profile));
            }

            // Đồng bộ Session
            HttpContext.Session.SetString("UserAvatar", avatarUrl);
            TempData["SuccessMessage"] = "Cập nhật ảnh đại diện / Logo đơn vị thành công!";
            return RedirectToAction(nameof(Profile));
        }
    }

    public class ToggleLuuFreelancerRequest
    {
        public int MaFreelancerStudent { get; set; }
        public string? GhiChu { get; set; }
    }
}