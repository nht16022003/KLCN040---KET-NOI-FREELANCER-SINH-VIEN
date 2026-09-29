using System;
using Microsoft.AspNetCore.Mvc;
using MOCK.Models.MockData;

namespace MOCK.Controllers
{
    public class TranhChapController : Controller
    {
        private int CurrentUserId => 1; // Default demo user session

        // =========================================================================
        // CUS-UC-11.02: Quản lý và theo dõi tranh chấp
        // =========================================================================
        [HttpGet]
        public IActionResult Index(string? status, string? keyword)
        {
            var model = MockDataStore.GetDanhSachTranhChap(CurrentUserId, keyword, status);
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var model = MockDataStore.GetTranhChapDetail(id);
            if (model == null)
            {
                return NotFound("Không tìm thấy thông tin vụ tranh chấp.");
            }
            return View(model);
        }

        // =========================================================================
        // CUS-UC-11.01: Tạo yêu cầu hỗ trợ tranh chấp
        // =========================================================================
        [HttpGet]
        public IActionResult Create(string? maHD)
        {
            var model = MockDataStore.GetTranhChapCreateView(CurrentUserId, maHD);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string maHD, string lyDo, string moTaChiTiet, decimal soTienTranhChap, string loaiBangChung, string? duongDanFile, string? moTaBangChung)
        {
            if (string.IsNullOrWhiteSpace(maHD) || string.IsNullOrWhiteSpace(lyDo) || string.IsNullOrWhiteSpace(moTaChiTiet))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn hợp đồng và điền đầy đủ lý do, mô tả chi tiết tranh chấp.";
                return RedirectToAction(nameof(Create), new { maHD });
            }

            int newId = MockDataStore.TaoTranhChap(CurrentUserId, maHD, lyDo, moTaChiTiet, soTienTranhChap, loaiBangChung, duongDanFile, moTaBangChung);
            TempData["SuccessMessage"] = $"Đã mở hồ sơ tranh chấp #TC_{newId:D4} thành công! Ban quản trị hệ thống sẽ tiếp nhận và phản hồi trong 24-48h.";
            return RedirectToAction(nameof(Details), new { id = newId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemBangChung(int maTranhChap, string loaiBangChung, string duongDanFile, string? moTa)
        {
            if (string.IsNullOrWhiteSpace(duongDanFile))
            {
                TempData["ErrorMessage"] = "Vui lòng cung cấp link hoặc tệp bằng chứng.";
                return RedirectToAction(nameof(Details), new { id = maTranhChap });
            }

            bool result = MockDataStore.ThemBangChungTranhChap(maTranhChap, CurrentUserId, loaiBangChung, duongDanFile, moTa);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã bổ sung tài liệu bằng chứng mới cho vụ tranh chấp!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thêm bằng chứng. Vui lòng thử lại.";
            }

            return RedirectToAction(nameof(Details), new { id = maTranhChap });
        }
    }
}
