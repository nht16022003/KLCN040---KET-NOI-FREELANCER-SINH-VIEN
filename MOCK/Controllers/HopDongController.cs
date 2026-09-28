using Microsoft.AspNetCore.Mvc;
using MOCK.Models.MockData;
using MOCK.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOCK.Controllers
{
    public class HopDongController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? status, string? keyword)
        {
            var allContracts = MockDataStore.HopDongs.Select(h => MockDataStore.GetContractDetail(h.MaHD))
                .Where(c => c != null)
                .Select(c => c!)
                .ToList();

            var query = allContracts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(c => c.Contract.TrangThai.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(c =>
                    c.Contract.MaHD.ToLower().Contains(kw) ||
                    c.Job.Tieude.ToLower().Contains(kw) ||
                    c.FreelancerUser.HotenUser.ToLower().Contains(kw) ||
                    (c.Employer.Tencongty != null && c.Employer.Tencongty.ToLower().Contains(kw)));
            }

            var list = query.ToList();

            var model = new ContractListViewModel
            {
                Contracts = list,
                ActiveContracts = allContracts.Count(c => c.Contract.TrangThai == "DangThucHien"),
                CompletedContracts = allContracts.Count(c => c.Contract.TrangThai == "DaHoanThanh"),
                TotalEscrowAmount = (decimal)allContracts.Sum(c => c.Contract.Sotienkyquy ?? 0),
                SelectedStatus = status ?? "All",
                Keyword = keyword
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(string id = "HD_2026_001")
        {
            var model = MockDataStore.GetContractDetail(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // =========================================================================
        // CUS-UC-10.02: Nghiệm thu và phản hồi sản phẩm
        // =========================================================================
        [HttpGet]
        public IActionResult NghiemThu(string id = "HD_2026_001")
        {
            var model = MockDataStore.GetNghiemThuView(id);
            if (model == null)
            {
                return NotFound("Không tìm thấy hợp đồng hoặc phiên bản bàn giao.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DuyetNghiemThu(int maBanGiao, string maHD, string? danhGia, int? soSao)
        {
            bool success = MockDataStore.DuyetNghiemThu(maBanGiao, maHD, danhGia, soSao);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã phê duyệt nghiệm thu sản phẩm thành công! Tiền ký quỹ đã được giải ngân cho Freelancer.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể duyệt nghiệm thu. Vui lòng thử lại.";
            }
            return RedirectToAction(nameof(NghiemThu), new { id = maHD });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult YeuCauChinhSua(int maBanGiao, string maHD, string lyDoYeuCau)
        {
            if (string.IsNullOrWhiteSpace(lyDoYeuCau))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập chi tiết góp ý hoặc yêu cầu chỉnh sửa.";
                return RedirectToAction(nameof(NghiemThu), new { id = maHD });
            }

            bool success = MockDataStore.YeuCauChinhSua(maBanGiao, maHD, lyDoYeuCau);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã gửi phản hồi và yêu cầu chỉnh sửa đến Freelancer!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại.";
            }
            return RedirectToAction(nameof(NghiemThu), new { id = maHD });
        }
    }
}
