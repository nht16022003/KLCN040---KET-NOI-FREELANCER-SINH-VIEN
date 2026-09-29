using Microsoft.AspNetCore.Mvc;
using MOCK.Models.Entities;
using MOCK.Models.MockData;
using MOCK.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOCK.Controllers
{
    public class FreelancerController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? keyword, string? chuyenNganh, string? kyNang, string? kyNangCoBan, string? nganSach, double? gpa)
        {
            return Search(keyword, chuyenNganh, kyNang, kyNangCoBan, nganSach, gpa);
        }

        [HttpGet]
        public IActionResult Search(string? keyword, string? chuyenNganh, string? kyNang, string? kyNangCoBan, string? nganSach, double? gpa)
        {
            // Lấy dữ liệu thuần từ MockDataStore (không sửa, không thêm, không xóa)
            var query = MockDataStore.FreelancerStudents.AsEnumerable();

            // Tìm kiếm theo từ khóa (Tên, trường, chuyên ngành)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(s =>
                {
                    var u = MockDataStore.Users.FirstOrDefault(x => x.MaUser == s.MaUser);
                    var cn = MockDataStore.ChuyenNganhs.FirstOrDefault(x => x.MaChuyenNganh == s.MaChuyenNganh);
                    var skills = MockDataStore.KyNangs.Where(k => k.MaFreelancerStudents == s.MaFreelancerStudents).Select(k => k.TenKyNang.ToLower());

                    bool matchUser = u != null && u.HotenUser.ToLower().Contains(kw);
                    bool matchTruong = s.TenTruong.ToLower().Contains(kw) || s.MaTruong.ToLower().Contains(kw);
                    bool matchChuyenNganh = cn != null && (cn.TenChuyenNganh.ToLower().Contains(kw) || cn.MaChuyenNganh.ToLower().Contains(kw));
                    bool matchSkills = skills.Any(sk => sk.Contains(kw));

                    return matchUser || matchTruong || matchChuyenNganh || matchSkills;
                });
            }

            // Lọc theo Chuyên ngành
            if (!string.IsNullOrWhiteSpace(chuyenNganh))
            {
                query = query.Where(s => s.MaChuyenNganh.Equals(chuyenNganh, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo Kỹ năng chuyên ngành
            if (!string.IsNullOrWhiteSpace(kyNang))
            {
                query = query.Where(s =>
                    MockDataStore.KyNangs.Any(k => k.MaFreelancerStudents == s.MaFreelancerStudents &&
                                                   k.TenKyNang.Equals(kyNang, StringComparison.OrdinalIgnoreCase)));
            }

            // Lọc theo Kỹ năng cơ bản
            if (!string.IsNullOrWhiteSpace(kyNangCoBan))
            {
                query = query.Where(s => s.KyNangCoBan != null && s.KyNangCoBan.Contains(kyNangCoBan, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo Ngân sách
            if (!string.IsNullOrWhiteSpace(nganSach))
            {
                if (decimal.TryParse(nganSach, out decimal maxBudget))
                {
                    query = query.Where(s => s.ChiPhiTu.HasValue && s.ChiPhiTu.Value <= maxBudget);
                }
            }

            // Lọc theo GPA
            if (gpa.HasValue)
            {
                query = query.Where(s => s.GPA >= gpa.Value);
            }

            // Xây dựng danh sách Card ViewModel
            var cardList = query.Select(s =>
            {
                var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == s.MaUser) ?? new User();
                var cn = MockDataStore.ChuyenNganhs.FirstOrDefault(c => c.MaChuyenNganh == s.MaChuyenNganh);
                var skills = MockDataStore.KyNangs.Where(k => k.MaFreelancerStudents == s.MaFreelancerStudents).ToList();

                return new FreelancerCardItemViewModel
                {
                    User = user,
                    Student = s,
                    ChuyenNganh = cn,
                    KyNangs = skills
                };
            }).ToList();

            // Lấy danh sách duy nhất các kỹ năng để đưa vào dropdown filter
            var allSkills = MockDataStore.KyNangs.Select(k => k.TenKyNang).Distinct().ToList();
            var allBasicSkills = MockDataStore.FreelancerStudents
                .Where(s => !string.IsNullOrEmpty(s.KyNangCoBan))
                .SelectMany(s => s.KyNangCoBan!.Split(',', StringSplitOptions.TrimEntries))
                .Distinct()
                .ToList();

            var viewModel = new FreelancerSearchViewModel
            {
                Freelancers = cardList,
                ChuyenNganhs = MockDataStore.ChuyenNganhs,
                AllKyNangs = allSkills,
                AllKyNangCoBans = allBasicSkills,
                Keyword = keyword,
                SelectedChuyenNganh = chuyenNganh,
                SelectedKyNang = kyNang,
                SelectedKyNangCoBan = kyNangCoBan,
                SelectedNganSach = nganSach,
                SelectedGPA = gpa
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Profile(int id)
        {
            var profile = MockDataStore.GetFreelancerProfile(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        [HttpGet]
        public IActionResult DanhSachNopTuyen(string? status)
        {
            var model = MockDataStore.GetStudentAppliedJobs(1, status);
            return View(model);
        }

        [HttpGet]
        public IActionResult LoiMoiNhanViec()
        {
            var model = MockDataStore.GetStudentInvitations(1);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AcceptInvitation(int maYeuCau)
        {
            var invite = MockDataStore.YeuCauThues.FirstOrDefault(y => y.MaYeuCau == maYeuCau);
            if (invite != null)
            {
                invite.TrangThai = "DongY";

                // Khởi tạo hợp đồng mới từ lời mời
                int nextHdIndex = MockDataStore.HopDongs.Count + 1;
                var newContract = new HopDong
                {
                    MaHD = $"HD_2026_{nextHdIndex:D3}",
                    MaJob = $"JOB_DIRECT_{maYeuCau}",
                    MaFreelancerStudent = invite.MaFreelancerStudent,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(14),
                    Sotienkyquy = invite.NganSachDeNghi ?? 2000000,
                    Hinhthuclamviec = "Remote",
                    TrangThai = "DangThucHien"
                };
                MockDataStore.HopDongs.Add(newContract);

                TempData["SuccessMessage"] = "Bạn đã đồng ý nhận lời mời làm việc! Hợp đồng bảo đảm đã được khởi tạo.";
            }
            return RedirectToAction("LoiMoiNhanViec");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectInvitation(int maYeuCau, string? lyDo)
        {
            var invite = MockDataStore.YeuCauThues.FirstOrDefault(y => y.MaYeuCau == maYeuCau);
            if (invite != null)
            {
                invite.TrangThai = "TuChoi";
                invite.LyDoTuChoi = !string.IsNullOrEmpty(lyDo) ? lyDo : "Sinh viên bận lịch học không thể nhận dự án lúc này.";
                TempData["SuccessMessage"] = "Đã từ chối lời mời làm việc.";
            }
            return RedirectToAction("LoiMoiNhanViec");
        }

        [HttpGet]
        public IActionResult Portfolio()
        {
            var model = MockDataStore.GetStudentPortfolioManage(1);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProject(StudentPortfolioManageViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewTenDuAn))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tên dự án.";
                return RedirectToAction("Portfolio");
            }

            int nextIndex = MockDataStore.DuAns.Count + 1;
            var newProject = new DuAnTrongPortfolio
            {
                MaDA = $"DA_{nextIndex:D2}",
                MaPortfolio = model.Portfolio.MaPortfolio ?? "PORT_01",
                TenDuAn = model.NewTenDuAn,
                MoTa = model.NewMoTa,
                VaiTro = model.NewVaiTro,
                Congnghe = model.NewCongNghe,
                LinkGithub = model.NewLinkGithub,
                LinkDemo = model.NewLinkDemo,
                Link_file = model.NewLinkFile
            };

            MockDataStore.DuAns.Add(newProject);
            TempData["SuccessMessage"] = $"Đã thêm thành công dự án '{newProject.TenDuAn}' vào Portfolio!";
            return RedirectToAction("Portfolio");
        }

        [HttpGet]
        public IActionResult HoSo()
        {
            var model = MockDataStore.GetStudentProfileEdit(1);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HoSo(StudentProfileEditViewModel model)
        {
            var student = MockDataStore.FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == 1);
            var user = MockDataStore.Users.FirstOrDefault(u => u.MaUser == (student != null ? student.MaUser : 1));

            if (user != null)
            {
                user.HotenUser = model.HotenUser;
                user.SdtUser = model.SdtUser;
            }

            if (student != null)
            {
                student.Gioithieu = model.Gioithieu;
                student.NgonNgu = model.NgonNgu;
                student.KyNangCoBan = model.KyNangCoBan;
                student.TrangthaiNhanViec = model.TrangthaiNhanViec;
                student.ChiPhiTu = model.ChiPhiTu;

                // Cập nhật kỹ năng
                if (!string.IsNullOrWhiteSpace(model.NewSkillsInput))
                {
                    MockDataStore.KyNangs.RemoveAll(k => k.MaFreelancerStudents == 1);
                    var skillNames = model.NewSkillsInput.Split(new[] { ',', ';' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    int skillId = MockDataStore.KyNangs.Any() ? MockDataStore.KyNangs.Max(k => k.MaKyNang) + 1 : 1;
                    foreach (var s in skillNames)
                    {
                        MockDataStore.KyNangs.Add(new KynangChuyennganhFreelancerStudent
                        {
                            MaKyNang = skillId++,
                            MaFreelancerStudents = 1,
                            TenKyNang = s
                        });
                    }
                }
            }

            TempData["SuccessMessage"] = "Cập nhật hồ sơ năng lực thành công!";
            return RedirectToAction("HoSo");
        }

        // ========================================================
        // FRL-UC: QUẢN LÝ NHÀ TUYỂN DỤNG QUAN TÂM (YÊU THÍCH)
        // ========================================================
        [HttpGet]
        public IActionResult NhaTuyenDungYeuThich(string? keyword, string? linhVuc, string? sortBy)
        {
            // Mặc định Freelancer sinh viên hiện tại (MaFreelancerStudent = 1)
            int currentFreelancerId = 1;
            var model = MockDataStore.GetDanhSachNhaTuyenDungYeuThich(currentFreelancerId, keyword, linhVuc, sortBy);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaNhaTuyenDungYeuThich(int maBookMark)
        {
            bool result = MockDataStore.XoaNhaTuyenDungYeuThich(maBookMark);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã xóa Nhà tuyển dụng khỏi danh sách quan tâm.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ nhà tuyển dụng cần xóa.";
            }
            return RedirectToAction("NhaTuyenDungYeuThich");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatGhiChuNTD(int maBookMark, string ghiChu)
        {
            bool result = MockDataStore.CapNhatGhiChuNhaTuyenDungYeuThich(maBookMark, ghiChu);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã cập nhật ghi chú cho Nhà tuyển dụng thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ để cập nhật ghi chú.";
            }
            return RedirectToAction("NhaTuyenDungYeuThich");
        }

        [HttpPost]
        public IActionResult ToggleNhaTuyenDungYeuThich(int maNhaTuyenDung, string? ghiChu)
        {
            int currentFreelancerId = 1;
            bool isAdded = MockDataStore.ToggleNhaTuyenDungYeuThich(currentFreelancerId, maNhaTuyenDung, ghiChu);
            return Json(new
            {
                success = true,
                isFavorite = isAdded,
                message = isAdded ? "Đã lưu vào danh sách Nhà tuyển dụng quan tâm!" : "Đã bỏ lưu khỏi danh sách quan tâm!"
            });
        }

        // ==========================================
        // FRL-UC-06: ĐỀ XUẤT HỢP ĐỒNG & THỐNG NHẤT ĐIỀU KHOẢN
        // ==========================================
        [HttpGet]
        public IActionResult DeXuatHopDong(string? maJob, int? maUngThue)
        {
            int currentFreelancerId = 1;
            var model = MockDataStore.GetDeXuatHopDongViewModel(maJob, maUngThue, currentFreelancerId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeXuatHopDong(DeXuatHopDongViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contract = MockDataStore.CreateDeXuatHopDong(model);
            TempData["SuccessMessage"] = $"Đã gửi đề xuất hợp đồng {contract.MaHD} tới Nhà tuyển dụng thành công! Vui lòng chờ Nhà tuyển dụng ký duyệt và ký quỹ.";
            return RedirectToAction("Details", "HopDong", new { id = contract.MaHD });
        }

        // ========================================================
        // FRL: ĐĂNG BÀI TÌM VIỆC & QUẢN LÝ TÌM VIỆC (BaiDangTimViec_FreelancerStudent)
        // ========================================================
        [HttpGet]
        public IActionResult QuanLyTimViec(string? status, string? keyword)
        {
            int currentFreelancerId = 1;
            var model = MockDataStore.GetQuanLyTimViec(currentFreelancerId, status, keyword);
            return View(model);
        }

        [HttpGet]
        public IActionResult DangTinTimViec(string? id)
        {
            int currentFreelancerId = 1;
            var model = MockDataStore.GetDangTinTimViecViewModel(id, currentFreelancerId);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài đăng tìm việc tương ứng.";
                return RedirectToAction("QuanLyTimViec");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangTinTimViec(DangTinTimViecViewModel model)
        {
            int currentFreelancerId = 1;
            if (!ModelState.IsValid)
            {
                var student = MockDataStore.FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == currentFreelancerId);
                var user = student != null ? MockDataStore.Users.FirstOrDefault(u => u.MaUser == student.MaUser) : null;
                model.StudentInfo = student;
                model.UserInfo = user;
                return View(model);
            }

            var post = MockDataStore.CreateOrUpdateBaiDangTimViec(model, currentFreelancerId);
            TempData["SuccessMessage"] = model.IsEditMode 
                ? $"Đã cập nhật bài đăng tìm việc '{post.Tieude}' thành công!" 
                : $"Đã đăng bài tìm việc mới '{post.Tieude}' lên sàn thành công!";

            return RedirectToAction("QuanLyTimViec");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleTrangThaiBaiDang(string maBaiDang, string trangThai)
        {
            int currentFreelancerId = 1;
            bool success = MockDataStore.ToggleTrangThaiBaiDangTimViec(maBaiDang, trangThai, currentFreelancerId);
            if (success)
            {
                string statusText = trangThai == "DangHienThi" ? "Hiển thị" : (trangThai == "DaAn" ? "Ẩn" : "Đã nhận việc");
                TempData["SuccessMessage"] = $"Đã chuyển trạng thái bài đăng sang '{statusText}'!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thay đổi trạng thái bài đăng.";
            }
            return RedirectToAction("QuanLyTimViec");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaBaiDang(string maBaiDang)
        {
            int currentFreelancerId = 1;
            bool success = MockDataStore.DeleteBaiDangTimViec(maBaiDang, currentFreelancerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa bài đăng tìm việc thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài đăng cần xóa.";
            }
            return RedirectToAction("QuanLyTimViec");
        }
    }
}
