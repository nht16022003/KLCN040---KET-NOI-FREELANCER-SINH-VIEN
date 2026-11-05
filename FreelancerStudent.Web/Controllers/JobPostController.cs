using System.Security.Claims;
using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.Services.Interfaces;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Controllers
{
    public class JobPostController : Controller
    {
        private readonly IJobPostWebService _jobPostService; //Khai báo để sử dụng serviceswweb

        private readonly INhaTuyenDungWebService _nhaTuyenDungService;
        private readonly IMinhChungWebService _minhChungWebService;
        private readonly IWalletWebService _walletService;
        private readonly IYeuThichWebService _yeuThichService;

        public JobPostController(
            IJobPostWebService jobPostWebService,
            INhaTuyenDungWebService nhaTuyenDungWebService,
            IMinhChungWebService minhChungWebService,
            IWalletWebService walletService,
            IYeuThichWebService yeuThichService)
        {
            _jobPostService = jobPostWebService;
            _nhaTuyenDungService = nhaTuyenDungWebService;
            _minhChungWebService = minhChungWebService;
            _walletService = walletService;
            _yeuThichService = yeuThichService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? maNhaTuyenDung,
            string? mucLuong,
            string? kyNang,
            string? trangThai,
            double? soSaoMin,
            string? sapXep)
        {
            var ketQuaJob = await _jobPostService.layDanhSachJobPost();
            var rawList = ketQuaJob.data ?? new List<JobPostViewModel>();

            // Lấy danh sách dropdown NTD và Kỹ năng trước khi lọc
            var allNtd = rawList
                .Where(j => j.maNhaTuyenDung > 0 && !string.IsNullOrWhiteSpace(j.tenCongTy))
                .GroupBy(j => j.maNhaTuyenDung)
                .Select(g => new KeyValuePair<int, string>(g.Key, g.First().tenCongTy!))
                .OrderBy(x => x.Value)
                .ToList();

            var allSkills = rawList
                .Where(j => !string.IsNullOrWhiteSpace(j.kynangyeucau))
                .SelectMany(j => j.kynangyeucau!.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            // Áp dụng bộ lọc
            var query = rawList.AsEnumerable();

            // 1. Tìm theo từ khóa (tiêu đề, mô tả, tên công ty, kỹ năng)
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(j =>
                    (j.tieude != null && j.tieude.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (j.mota != null && j.mota.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (j.tenCongTy != null && j.tenCongTy.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (j.kynangyeucau != null && j.kynangyeucau.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            // 2. Lọc theo Nhà tuyển dụng
            if (maNhaTuyenDung.HasValue && maNhaTuyenDung.Value > 0)
            {
                query = query.Where(j => j.maNhaTuyenDung == maNhaTuyenDung.Value);
            }

            // 3. Lọc theo mức thù lao
            if (!string.IsNullOrWhiteSpace(mucLuong))
            {
                switch (mucLuong)
                {
                    case "duoi-1tr":
                        query = query.Where(j => j.thulao.HasValue && j.thulao.Value > 0 && j.thulao.Value < 1000000);
                        break;
                    case "1tr-3tr":
                        query = query.Where(j => j.thulao.HasValue && j.thulao.Value >= 1000000 && j.thulao.Value <= 3000000);
                        break;
                    case "3tr-10tr":
                        query = query.Where(j => j.thulao.HasValue && j.thulao.Value > 3000000 && j.thulao.Value <= 10000000);
                        break;
                    case "tren-10tr":
                        query = query.Where(j => j.thulao.HasValue && j.thulao.Value > 10000000);
                        break;
                    case "thoa-thuan":
                        query = query.Where(j => !j.thulao.HasValue || j.thulao.Value <= 0);
                        break;
                }
            }

            // 4. Lọc theo kỹ năng
            if (!string.IsNullOrWhiteSpace(kyNang))
            {
                var kn = kyNang.Trim();
                query = query.Where(j => j.kynangyeucau != null && j.kynangyeucau.Contains(kn, StringComparison.OrdinalIgnoreCase));
            }

            // 5. Lọc theo trạng thái công việc
            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "TatCa")
            {
                query = query.Where(j => string.Equals(j.status, trangThai, StringComparison.OrdinalIgnoreCase));
            }

            // 6. Lọc theo số sao đánh giá của Nhà tuyển dụng
            if (soSaoMin.HasValue && soSaoMin.Value > 0)
            {
                query = query.Where(j => j.soSaoDanhGiaNhaTuyenDung.HasValue && j.soSaoDanhGiaNhaTuyenDung.Value >= soSaoMin.Value);
            }

            // 7. Sắp xếp (Mặc định bắt buộc: Mới nhất trước)
            string sortOrder = string.IsNullOrWhiteSpace(sapXep) ? "moi-nhat" : sapXep;
            switch (sortOrder)
            {
                case "luong-cao":
                    query = query.OrderByDescending(j => j.thulao ?? 0).ThenByDescending(j => j.thoigiandangtuyen);
                    break;
                case "luong-thap":
                    query = query.OrderBy(j => j.thulao ?? 0).ThenByDescending(j => j.thoigiandangtuyen);
                    break;
                case "sao-cao":
                    query = query.OrderByDescending(j => j.soSaoDanhGiaNhaTuyenDung ?? 0).ThenByDescending(j => j.thoigiandangtuyen);
                    break;
                case "moi-nhat":
                default:
                    query = query.OrderByDescending(j => j.thoigiandangtuyen).ThenByDescending(j => j.maJob);
                    break;
            }

            var dsJob = query.ToList();

            // Lấy trạng thái ứng tuyển và danh sách việc đã lưu nếu là Freelancer
            var trangThaiTheoJob = new Dictionary<string, string>();
            var dsMaJobDaLuu = new HashSet<string>();
            var maUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? User.FindFirst(ClaimTypes.Role)?.Value;

            if (maUser.HasValue && role == "FreelancerStudent")
            {
                var ketQuaDon = await _jobPostService.layDSUngTuyenTheoMaUser(maUser.Value);

                if (ketQuaDon.success && ketQuaDon.data != null)
                {
                    trangThaiTheoJob = ketQuaDon.data
                        .GroupBy(x => x.maJob)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderByDescending(x => x.ngayUngTuyen)
                                .First().trangThaiUngTuyen ?? ""
                        );
                }

                var listSaved = await _yeuThichService.LayDanhSachViecDaLuuAsync(maUser.Value);
                if (listSaved != null && listSaved.Any())
                {
                    dsMaJobDaLuu = listSaved.Select(x => x.maJob).ToHashSet();
                }
            }

            ViewBag.TrangThaiUngTuyen = trangThaiTheoJob;
            ViewBag.DanhSachMaJobDaLuu = dsMaJobDaLuu;

            var model = new JobPostFilterViewModel
            {
                DanhSachJobPost = dsJob,
                TuKhoa = tuKhoa,
                MaNhaTuyenDung = maNhaTuyenDung,
                MucLuong = mucLuong,
                KyNang = kyNang,
                TrangThai = trangThai ?? "TatCa",
                SoSaoMin = soSaoMin,
                SapXep = sortOrder,
                DanhSachNhaTuyenDung = allNtd,
                DanhSachKyNang = allSkills
            };

            return View(model);
        }

        //XS
        [HttpGet]
        public async Task<IActionResult> Detail(string maJob)
        {
            if (string.IsNullOrWhiteSpace(maJob))
            {
                return NotFound();
            }

            var ketQua = await _jobPostService.layChiTietJobPost(maJob);

            if (!ketQua.success || ketQua.data == null)
            {
                if (!string.IsNullOrWhiteSpace(ketQua.message))
                {
                    TempData["Error"] = ketQua.message;
                }

                return NotFound();
            }

            var maUserDetail = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var roleDetail = HttpContext.Session.GetString("tenRole") ?? User.FindFirst(ClaimTypes.Role)?.Value;
            bool daLuu = false;
            if (maUserDetail.HasValue && roleDetail == "FreelancerStudent")
            {
                daLuu = await _yeuThichService.KiemTraLuuViecAsync(maUserDetail.Value, maJob);
            }
            ViewBag.DaLuuViec = daLuu;

            var jobDetail = ketQua.data.Job;
            ViewBag.DaTuyenDu = jobDetail?.DaTuyenDu ?? false;
            ViewBag.DaHoanThanh = string.Equals(jobDetail?.status, "DaHoanThanh", StringComparison.OrdinalIgnoreCase);

            return View(ketQua.data);
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            //Nếu chưa đăng ký thì trả về View DangKy với
            //model là DangKyViewModel
            return View(new DangKyViewModel());
            //new DangKyViewModel() tức là tạo object để truyền dữ liệu từ Controller -> View
            //Trong view tương ứng sẽ @model DangKyViewModel
            //Sau đó có thể sửa dụng @Model.hovaten
        }



        //Tuấn Anh

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Lấy phí đăng bài hiện tại từ hệ thống
            var phiRes = await _jobPostService.layPhiDangBaiHienTaiAsync();
            decimal phiDangBai = phiRes.data;

            // Lấy số dư khả dụng hiện tại của Nhà tuyển dụng
            decimal soDuKhaDung = 0;
            var viRes = await _walletService.layViTheoUser(maUser.Value);
            if (viRes.success && viRes.data != null)
            {
                soDuKhaDung = viRes.data.soDuKhaDung;
            }

            ViewBag.SoDuKhaDung = soDuKhaDung;

            var model = new JobPostViewModel
            {
                maUser = maUser.Value,
                soluongtuyen = 1,
                phiDangBai = phiDangBai
            };

            return View(model);
        }


        //Tuấn Anh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPostViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");

            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Không lấy maUser từ HTML để tránh người dùng sửa ID
            model.maUser = maUser.Value;

            // Luôn đồng bộ mức phí đăng bài hiện hành do Admin cấu hình
            var phiRes = await _jobPostService.layPhiDangBaiHienTaiAsync();
            model.phiDangBai = phiRes.data;

            // Kiểm tra số dư ví khả dụng
            decimal soDuKhaDung = 0;
            var viRes = await _walletService.layViTheoUser(maUser.Value);
            if (viRes.success && viRes.data != null)
            {
                soDuKhaDung = viRes.data.soDuKhaDung;
            }
            ViewBag.SoDuKhaDung = soDuKhaDung;

            if (model.thoigiandukienhoanthanh <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(model.thoigiandukienhoanthanh),
                    "Hạn hoàn thành phải lớn hơn ngày hiện tại."
                );
            }

            if (model.phiDangBai > 0 && soDuKhaDung < model.phiDangBai)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Số dư khả dụng trong ví ({soDuKhaDung:N0} VNĐ) không đủ để thanh toán phí đăng tin ({model.phiDangBai:N0} VNĐ). Vui lòng nạp thêm tiền vào ví trước khi đăng bài."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ketQua = await _jobPostService.taoJobPost(model);

            if (ketQua.success)
            {
                TempData["Success"] = model.phiDangBai > 0
                    ? $"Đăng tin tuyển dụng thành công! Đã thanh toán {model.phiDangBai:N0} VNĐ phí đăng bài."
                    : "Đăng tin tuyển dụng thành công!";

                return RedirectToAction("Index");
            }

            ModelState.AddModelError(
                string.Empty,
                ketQua.message ?? "Không thể đăng tin tuyển dụng."
            );

            return View(model);
        }

        //Tuấn
        [HttpGet]
        public async Task<IActionResult> DanhSachUngTuyen(string? maJob)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (!string.IsNullOrEmpty(maJob))
            {
                return RedirectToAction("DanhSachUngVien", "NhaTuyenDung", new { maJob });
            }
            return RedirectToAction("DanhSachUngVien", "NhaTuyenDung");
        }

        //Tuấn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetUngTuyen(int maUngTuyen, string trangThai, string? maJob)
        {
            var ketQua = await _jobPostService.duyetUngTuyenAsync(maUngTuyen, trangThai);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = ketQua.message;
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message;
            }

            if (!string.IsNullOrEmpty(maJob))
            {
                return RedirectToAction("DanhSachUngVien", "NhaTuyenDung", new { maJob });
            }
            return RedirectToAction("DanhSachUngVien", "NhaTuyenDung");
        }

        //Tuan
        [HttpGet]
        public async Task<IActionResult> UngTuyen(string maJob)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra trạng thái xác thực minh chứng sinh viên
            var checkXacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
            if (checkXacThuc?.data != null && !checkXacThuc.data.DaXacMinh)
            {
                TempData["ErrorMessage"] = !string.IsNullOrWhiteSpace(checkXacThuc.data.ThongBao)
                    ? checkXacThuc.data.ThongBao
                    : "Bạn cần hoàn tất xác thực minh chứng sinh viên (trạng thái 'Đã xác minh') trước khi nộp đơn ứng tuyển!";
                return RedirectToAction("MinhChung", "Account");
            }

            var ketQuaDon = await _jobPostService.layDSUngTuyenTheoMaUser(maUser.Value);

            if (ketQuaDon.success && ketQuaDon.data != null)
            {
                var donDaNop = ketQuaDon.data
                    .Where(u => u.maJob == maJob)
                    .OrderByDescending(u => u.ngayUngTuyen)
                    .FirstOrDefault();

                if (donDaNop != null &&
                    (donDaNop.trangThaiUngTuyen == "ChoDuyet" ||
                    donDaNop.trangThaiUngTuyen == "ChapNhan"))
                {
                    TempData["ErrorMessage"] =
                        "Bạn đã ứng tuyển công việc này và không thể nộp đơn trùng.";

                    return RedirectToAction("Index");
                }
            }
            //Lay thong tin bai jobpost

            var dsJob = await _jobPostService.layDanhSachJobPost();
            var job = dsJob.data?.FirstOrDefault(j => j.maJob == maJob);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc này!";
                return RedirectToAction("Index");
            }
            
            if (string.Equals(job.status, "DaHoanThanh", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Công việc này đã hoàn thành nghiệm thu dự án, không thể ứng tuyển.";
                return RedirectToAction("Detail", new { maJob });
            }

            if (job.DaTuyenDu || string.Equals(job.status, "DaTuyenDu", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Công việc này đã có ứng viên được lựa chọn (đã tuyển đủ), không thể ứng tuyển thêm.";
                return RedirectToAction("Detail", new { maJob });
            }

            // Lấy danh sách Nhà tuyển dụng để tìm người đăng bài Job này
            var dsNTD = await _nhaTuyenDungService.layDanhSachNhaTuyenDungAsync();
            var ntd = dsNTD.data?.FirstOrDefault(n => n.maNhaTuyenDung == job.maNhaTuyenDung);

            var model = new UngTuyenViewModel
            {
                maJob = job.maJob,
                maUser = maUser.Value,
                tieude = job.tieude,
                mota = job.mota,
                thulao = job.thulao,
                kynangyeucau = job.kynangyeucau,
                thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                soluongtuyen = job.soluongtuyen,

                // Điền sẵn giá gốc của bài Job vào ô đề xuất
                thulaoDeXuat = job.thulao,
                thoiGianHoanThanhDeXuat = job.thoigiandukienhoanthanh.ToString("dd/MM/yyyy"),
                // Thông tin Nhà tuyển dụng
                maNhaTuyenDung = job.maNhaTuyenDung,
                tenCongTy = !string.IsNullOrWhiteSpace(ntd?.tencongty) ? ntd.tencongty : ntd?.hotenUser,
                avatarNhaTuyenDung = ntd?.logo ?? ntd?.avatar
            };
            return View(model);
        }


        //Tuan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UngTuyen(UngTuyenViewModel model)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra trạng thái xác thực minh chứng sinh viên
            var checkXacThuc = await _minhChungWebService.KiemTraTrangThaiXacThucAsync(maUser.Value);
            if (checkXacThuc?.data != null && !checkXacThuc.data.DaXacMinh)
            {
                TempData["ErrorMessage"] = !string.IsNullOrWhiteSpace(checkXacThuc.data.ThongBao)
                    ? checkXacThuc.data.ThongBao
                    : "Bạn cần hoàn tất xác thực minh chứng sinh viên (trạng thái 'Đã xác minh') trước khi nộp đơn ứng tuyển!";
                return RedirectToAction("MinhChung", "Account");
            }

            model.maUser = maUser.Value;

            // Ràng buộc thời gian hoàn thành theo JobPost
            if (model.thoigiandukienhoanthanh != DateTime.MinValue)
            {
                model.thoiGianHoanThanhDeXuat = model.thoigiandukienhoanthanh.ToString("dd/MM/yyyy");
            }

            var ketQua = await _jobPostService.nopDonUngTuyenAsync(model);

            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Ứng tuyển thành công!";
                // Chuyển hướng về trang Danh sách bài đã nộp của sinh viên
                return RedirectToAction("DanhSachNopTuyen", "FreelancerStudent");
            }



            ModelState.AddModelError(string.Empty, ketQua.message ?? "Có lỗi xảy ra khi nộp đơn ứng tuyển.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> QuanLyTinTuyenDung()
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var ketQua = await _jobPostService.layJobCuaToiAsync(maUser.Value);
            var dsJob = ketQua?.data ?? new List<JobPostViewModel>();

            return View(dsJob);
        }

        //Tuấn
        [HttpGet]
        public async Task<IActionResult> Edit(string maJob)
        {
            var dsJob = await _jobPostService.layDanhSachJobPost();
            var job = dsJob.data?.FirstOrDefault(j => j.maJob == maJob);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài tuyển dụng!";
                return RedirectToAction("QuanLyTinTuyenDung");
            }
            return View(job);
        }
        //Tuấn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobPostViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ketQua = await _jobPostService.capNhatJobPostAsync(model);
            if (ketQua.success)
            {
                TempData["SuccessMessage"] = "Cập nhật bài tuyển dụng thành công!";
                return RedirectToAction("QuanLyTinTuyenDung");
            }

            ModelState.AddModelError(string.Empty, ketQua.message ?? "Cập nhật thất bại.");
            return View(model);
        }

        // Đổi trạng thái tin tuyển dụng (Đang tuyển <-> Đã đóng / Tạm dừng)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(string maJob, string trangThaiMoi)
        {
            var maUser = HttpContext.Session.GetInt32("maUser");
            if (maUser == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (string.IsNullOrWhiteSpace(maJob))
            {
                TempData["ErrorMessage"] = "Mã bài tuyển dụng không hợp lệ.";
                return RedirectToAction("QuanLyTinTuyenDung");
            }

            var dsJob = await _jobPostService.layDanhSachJobPost();
            var job = dsJob.data?.FirstOrDefault(j => j.maJob == maJob);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài tuyển dụng!";
                return RedirectToAction("QuanLyTinTuyenDung");
            }

            job.status = trangThaiMoi;
            var ketQua = await _jobPostService.capNhatJobPostAsync(job);

            if (ketQua.success)
            {
                TempData["SuccessMessage"] = trangThaiMoi == "DangTuyen" 
                    ? "Đã mở lại tin tuyển dụng thành công!" 
                    : "Đã đóng tin tuyển dụng!";
            }
            else
            {
                TempData["ErrorMessage"] = ketQua.message ?? "Không thể cập nhật trạng thái tin.";
            }

            return RedirectToAction("QuanLyTinTuyenDung");
        }

        [HttpGet]
        public async Task<IActionResult> GoiYKyNang(string? tieuDe, string? moTa)
        {
            var res = await _jobPostService.layGoiYKyNangTheoTieuDe(tieuDe, moTa);
            return Json(res);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLuuViec([FromBody] ToggleLuuViecRequest request)
        {
            var maUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? User.FindFirst(ClaimTypes.Role)?.Value;

            if (!maUser.HasValue || role != "FreelancerStudent")
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập với tư cách Freelancer sinh viên để lưu công việc!" });
            }

            if (string.IsNullOrWhiteSpace(request?.MaJob))
            {
                return Json(new { success = false, message = "Mã công việc không hợp lệ!" });
            }

            var daLuu = await _yeuThichService.KiemTraLuuViecAsync(maUser.Value, request.MaJob);
            if (daLuu)
            {
                var boLuu = await _yeuThichService.BoLuuViecAsync(maUser.Value, request.MaJob);
                return Json(new { success = boLuu.success, daLuu = false, message = "Đã bỏ lưu công việc thành công!" });
            }
            else
            {
                var luu = await _yeuThichService.LuuViecAsync(maUser.Value, request.MaJob, request.GhiChu);
                return Json(new { success = luu.success, daLuu = true, message = "Đã lưu công việc thành công!" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViecDaLuu()
        {
            var maUser = HttpContext.Session.GetInt32("maUser") ?? (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int uId) ? uId : (int?)null);
            var role = HttpContext.Session.GetString("tenRole") ?? User.FindFirst(ClaimTypes.Role)?.Value;

            if (!maUser.HasValue || role != "FreelancerStudent")
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách Freelancer sinh viên để xem danh sách việc đã lưu.";
                return RedirectToAction("DangNhap", "Account");
            }

            var danhSach = await _yeuThichService.LayDanhSachViecDaLuuAsync(maUser.Value);
            return View(danhSach);
        }
    }

    public class ToggleLuuViecRequest
    {
        public string MaJob { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }
}