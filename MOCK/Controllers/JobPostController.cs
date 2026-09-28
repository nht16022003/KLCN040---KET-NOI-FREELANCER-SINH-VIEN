using Microsoft.AspNetCore.Mvc;
using MOCK.Models.MockData;
using MOCK.Models.ViewModels;
using System.Linq;

namespace MOCK.Controllers
{
    public class JobPostController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? keyword, string? status)
        {
            var jobs = MockDataStore.JobPosts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                jobs = jobs.Where(j => j.Tieude.ToLower().Contains(kw) ||
                                       (j.Mota != null && j.Mota.ToLower().Contains(kw)) ||
                                       (j.Kynangyeucau != null && j.Kynangyeucau.ToLower().Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                jobs = jobs.Where(j => j.Status == status);
            }

            return View(jobs.ToList());
        }

        [HttpGet]
        public IActionResult Details(string id = "JOB_01")
        {
            var model = MockDataStore.GetJobDetail(id);
            if (model == null)
            {
                // Fallback nếu không tìm thấy ID chính xác
                var firstJob = MockDataStore.JobPosts.FirstOrDefault();
                if (firstJob != null)
                {
                    model = MockDataStore.GetJobDetail(firstJob.MaJob);
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new JobCreateViewModel
            {
                MaNhaTuyenDung = 1,
                Thoigiandukienhoanthanh = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd"),
                Soluongtuyen = 1
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JobCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tạo mã Job mới (VD: JOB_04)
            int nextIndex = MockDataStore.JobPosts.Count + 1;
            string newMaJob = $"JOB_{nextIndex:D2}";

            var newJob = new Models.Entities.JobPost
            {
                MaJob = newMaJob,
                MaNhaTuyenDung = model.MaNhaTuyenDung > 0 ? model.MaNhaTuyenDung : 1,
                Tieude = model.Tieude,
                Mota = model.Mota,
                Kynangyeucau = model.Kynangyeucau,
                Thulao = model.Thulao,
                Thoigiandangtuyen = DateTime.Now,
                Thoigiandukienhoanthanh = model.Thoigiandukienhoanthanh,
                Status = "DangTuyen",
                Soluongtuyen = model.Soluongtuyen
            };

            MockDataStore.JobPosts.Insert(0, newJob);

            TempData["SuccessMessage"] = $"Đăng tin tuyển dụng '{newJob.Tieude}' thành công! Tin của bạn đã sẵn sàng nhận hồ sơ ứng tuyển.";
            return RedirectToAction("ManageJobs", "Employer");
        }

        // ==========================================
        // CUS-UC-06: CHỈNH SỬA BÀI ĐĂNG TUYỂN DỤNG (Edit)
        // ==========================================
        [HttpGet]
        public IActionResult Edit(string id = "JOB_01")
        {
            var model = MockDataStore.GetJobEditViewModel(id);
            if (model == null)
            {
                var firstJob = MockDataStore.JobPosts.FirstOrDefault();
                if (firstJob != null)
                {
                    model = MockDataStore.GetJobEditViewModel(firstJob.MaJob);
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
        public IActionResult Edit(JobEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = MockDataStore.UpdateJobPost(model);
            if (result)
            {
                TempData["SuccessMessage"] = $"Đã cập nhật bài đăng '{model.Tieude}' thành công!";
                return RedirectToAction("ManageJobs", "Employer");
            }

            ModelState.AddModelError(string.Empty, "Không thể cập nhật bài đăng. Vui lòng thử lại.");
            return View(model);
        }
    }
}
