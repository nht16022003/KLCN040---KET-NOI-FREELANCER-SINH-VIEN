using Microsoft.AspNetCore.Mvc;
using MOCK.Models.Entities;
using MOCK.Models.MockData;
using MOCK.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOCK.Controllers
{
    public class EmployerController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? keyword, string? linhVuc, string? diaDiem, double? danhGia)
        {
            return Search(keyword, linhVuc, diaDiem, danhGia);
        }

        [HttpGet]
        public IActionResult Search(string? keyword, string? linhVuc, string? diaDiem, double? danhGia)
        {
            // Lấy dữ liệu thuần từ MockDataStore
            var query = MockDataStore.NhaTuyenDungs.AsEnumerable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(e =>
                {
                    var u = MockDataStore.Users.FirstOrDefault(x => x.MaUser == e.MaUser);
                    bool matchName = e.Tencongty != null && e.Tencongty.ToLower().Contains(kw);
                    bool matchUser = u != null && u.HotenUser.ToLower().Contains(kw);
                    bool matchLinhVuc = e.Linhvuc != null && e.Linhvuc.ToLower().Contains(kw);
                    bool matchDiaChi = e.Diachi != null && e.Diachi.ToLower().Contains(kw);

                    return matchName || matchUser || matchLinhVuc || matchDiaChi;
                });
            }

            // Lọc theo Lĩnh vực
            if (!string.IsNullOrWhiteSpace(linhVuc))
            {
                query = query.Where(e => e.Linhvuc != null && e.Linhvuc.Equals(linhVuc, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo Địa điểm
            if (!string.IsNullOrWhiteSpace(diaDiem))
            {
                query = query.Where(e => e.Diachi != null && e.Diachi.Contains(diaDiem, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo Đánh giá
            if (danhGia.HasValue)
            {
                query = query.Where(e => e.Sosaodanhgia.HasValue && e.Sosaodanhgia.Value >= danhGia.Value);
            }

            // Xây dựng danh sách Card ViewModel
            var cardList = query.Select(e =>
            {
                var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == e.MaUser) ?? new User();
                var allJobs = MockDataStore.JobPosts.Where(j => j.MaNhaTuyenDung == e.MaNhaTuyenDung).ToList();
                var activeJobs = allJobs.Where(j => j.Status == "DangTuyen").ToList();

                return new EmployerCardItemViewModel
                {
                    Employer = e,
                    User = user,
                    TotalJobs = allJobs.Count,
                    ActiveJobsCount = activeJobs.Count,
                    FeaturedJobs = activeJobs.Take(2).ToList(),
                    IsBookmarked = MockDataStore.IsNhaTuyenDungYeuThich(1, e.MaNhaTuyenDung)
                };
            }).ToList();

            var allLinhVucs = MockDataStore.NhaTuyenDungs
                .Where(e => !string.IsNullOrEmpty(e.Linhvuc))
                .Select(e => e.Linhvuc!)
                .Distinct()
                .ToList();

            var allDiaDiems = MockDataStore.NhaTuyenDungs
                .Where(e => !string.IsNullOrEmpty(e.Diachi))
                .Select(e => e.Diachi!)
                .Distinct()
                .ToList();

            var viewModel = new EmployerSearchViewModel
            {
                Employers = cardList,
                AllLinhVucs = allLinhVucs,
                AllDiaDiems = allDiaDiems,
                Keyword = keyword,
                SelectedLinhVuc = linhVuc,
                SelectedDiaDiem = diaDiem,
                SelectedRating = danhGia
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Profile(int id)
        {
            var profile = MockDataStore.GetEmployerProfile(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        [HttpGet]
        public IActionResult ManageJobs(string? status)
        {
            var model = MockDataStore.GetEmployerManageJobs(1, status);
            return View(model);
        }

        [HttpGet]
        public IActionResult Applicants(string id = "JOB_01")
        {
            var model = MockDataStore.GetJobApplicants(id);
            if (model == null)
            {
                var firstJob = MockDataStore.JobPosts.FirstOrDefault();
                if (firstJob != null)
                {
                    model = MockDataStore.GetJobApplicants(firstJob.MaJob);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AcceptApplicant(int maUngTuyen)
        {
            var app = MockDataStore.UngTuyens.FirstOrDefault(u => u.MaUngTuyen == maUngTuyen);
            if (app != null)
            {
                app.TrangThaiUngTuyen = "ChapNhan";

                // Kiểm tra xem đã có hợp đồng cho Job này chưa
                var job = MockDataStore.JobPosts.FirstOrDefault(j => j.MaJob == app.MaJob);
                if (job != null)
                {
                    job.Status = "DangThucHien";

                    var existingHD = MockDataStore.HopDongs.FirstOrDefault(h => h.MaJob == app.MaJob);
                    if (existingHD == null)
                    {
                        int nextHdIndex = MockDataStore.HopDongs.Count + 1;
                        var newContract = new HopDong
                        {
                            MaHD = $"HD_2026_{nextHdIndex:D3}",
                            MaJob = app.MaJob,
                            MaFreelancerStudent = app.MaFreelancerStudent,
                            NgayBatDau = DateTime.Now,
                            NgayKetThuc = DateTime.Now.AddDays(15),
                            Sotienkyquy = app.ThulaoDeXuat ?? job.Thulao ?? 1500000,
                            Hinhthuclamviec = "Remote",
                            TrangThai = "DangThucHien"
                        };
                        MockDataStore.HopDongs.Add(newContract);
                    }
                }

                TempData["SuccessMessage"] = "Đã chấp nhận ứng viên và tự động khởi tạo hợp đồng làm việc!";
                return RedirectToAction("Applicants", new { id = app.MaJob });
            }
            return RedirectToAction("ManageJobs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectApplicant(int maUngTuyen)
        {
            var app = MockDataStore.UngTuyens.FirstOrDefault(u => u.MaUngTuyen == maUngTuyen);
            if (app != null)
            {
                app.TrangThaiUngTuyen = "TuChoi";
                TempData["SuccessMessage"] = "Đã từ chối hồ sơ ứng viên này.";
                return RedirectToAction("Applicants", new { id = app.MaJob });
            }
            return RedirectToAction("ManageJobs");
        }

        // ==========================================
        // CUS-UC-04.01: QUẢN LÝ FREELANCER QUAN TÂM (YÊU THÍCH)
        // ==========================================
        [HttpGet]
        public IActionResult FreelancerYeuThich(string? keyword, string? chuyenNganh, string? sortBy)
        {
            // Mặc định Nhà tuyển dụng hiện tại (MaNhaTuyenDung = 1)
            int currentEmployerId = 1;
            var model = MockDataStore.GetDanhSachFreelancerYeuThich(currentEmployerId, keyword, chuyenNganh, sortBy);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaYeuThich(int maBookMark)
        {
            bool result = MockDataStore.XoaFreelancerYeuThich(maBookMark);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã xóa Freelancer khỏi danh sách quan tâm.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ yêu thích cần xóa.";
            }
            return RedirectToAction("FreelancerYeuThich");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatGhiChu(int maBookMark, string ghiChu)
        {
            bool result = MockDataStore.CapNhatGhiChuYeuThich(maBookMark, ghiChu);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã cập nhật ghi chú cho Freelancer thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ để cập nhật ghi chú.";
            }
            return RedirectToAction("FreelancerYeuThich");
        }

        [HttpPost]
        public IActionResult ToggleYeuThich(int maFreelancerStudent, string? ghiChu)
        {
            int currentEmployerId = 1;
            bool isAdded = MockDataStore.ToggleFreelancerYeuThich(currentEmployerId, maFreelancerStudent, ghiChu);
            return Json(new
            {
                success = true,
                isFavorite = isAdded,
                message = isAdded ? "Đã lưu vào danh sách Freelancer quan tâm!" : "Đã bỏ lưu khỏi danh sách quan tâm!"
            });
        }

        // ==========================================
        // CUS-UC-04: XEM CHI TIẾT HỒ SƠ FREELANCER TỪ GÓC NHÌN EMPLOYER
        // ==========================================
        [HttpGet]
        public IActionResult FreelancerDetail(int id)
        {
            int currentEmployerId = 1;
            var model = MockDataStore.GetFreelancerDetailForEmployer(id, currentEmployerId);
            if (model == null)
            {
                // Fallback nếu không tìm thấy id
                var firstStudent = MockDataStore.FreelancerStudents.FirstOrDefault();
                if (firstStudent != null)
                {
                    model = MockDataStore.GetFreelancerDetailForEmployer(firstStudent.MaFreelancerStudents, currentEmployerId);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(model);
        }

        // ==========================================
        // CUS-UC-06 / BẢNG UNGTHUE: GỬI LỜI MỜI THUÊ TRỰC TIẾP
        // ==========================================
        [HttpGet]
        public IActionResult MoiNhanViec(int freelancerId)
        {
            int currentEmployerId = 1;
            var model = MockDataStore.GetSendDirectOfferViewModel(freelancerId, currentEmployerId);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MoiNhanViec(SendDirectOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var offer = MockDataStore.CreateUngThue(model);
            TempData["SuccessMessage"] = $"Đã gửi lời mời thuê trực tiếp tới {model.TenFreelancer} thành công! Bạn có thể theo dõi phản hồi tại mục Quản lý tin / Lời mời.";
            return RedirectToAction("FreelancerDetail", new { id = model.MaFreelancerStudent });
        }
    }
}
