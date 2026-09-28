using System;
using Microsoft.AspNetCore.Mvc;
using MOCK.Models.MockData;

namespace MOCK.Controllers
{
    public class NhanTinController : Controller
    {
        private int CurrentUserId => 1; // Default session demo user (Freelancer Nguyễn Văn An)

        // GET: /NhanTin  — Phòng chat chính
        [HttpGet]
        public IActionResult Index(string? chatId, int? userId, string? jobId)
        {
            var model = MockDataStore.GetChatView(CurrentUserId, chatId, userId, jobId);
            return View(model);
        }

        // GET: /NhanTin/CuocTroChuyen/{id}  — Mở phòng chat theo Job ID hoặc Chat ID
        [HttpGet]
        public IActionResult CuocTroChuyen(string id = "JOB_01")
        {
            if (id.StartsWith("CHAT_"))
            {
                return RedirectToAction(nameof(Index), new { chatId = id });
            }
            return RedirectToAction(nameof(Index), new { jobId = id });
        }

        // POST: /NhanTin/GuiTinNhan — Gửi tin nhắn
        [HttpPost]
        public IActionResult GuiTinNhan(string chatId, string noiDung, string? fileUrl, string? tenFile, string? loaiTinNhan, string? maDuAnPortfolio)
        {
            if (string.IsNullOrWhiteSpace(noiDung) && string.IsNullOrWhiteSpace(fileUrl) && string.IsNullOrWhiteSpace(maDuAnPortfolio))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Nội dung tin nhắn không được để trống." });
                }
                return RedirectToAction(nameof(Index), new { chatId });
            }

            var newMsg = MockDataStore.GuiTinNhan(CurrentUserId, chatId, noiDung, fileUrl, tenFile, loaiTinNhan ?? "VanBan", maDuAnPortfolio);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    messageId = newMsg.MaTinNhan,
                    chatId = newMsg.MaPhongChat,
                    noiDung = newMsg.NoiDung,
                    loaiTinNhan = newMsg.LoaiTinNhan,
                    time = newMsg.NgayGui.ToString("HH:mm"),
                    isMine = true
                });
            }

            return RedirectToAction(nameof(Index), new { chatId });
        }
    }
}
