using System.Security.Claims;
using FreelancerStudent.Web.Helpers;
using FreelancerStudent.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FreelancerStudent.Web.Filters
{
    /// <summary>
    /// Filter bảo vệ toàn bộ các trang và chức năng nghiệp vụ sinh viên trên Web.
    /// Ngăn chặn sinh viên hết hạn niên khóa truy cập qua URL trực tiếp, session cũ hoặc thao tác trên giao diện.
    /// Cho phép tiếp tục sử dụng các luồng cần thiết: Đăng xuất, Nộp lại thẻ sinh viên (Minh chứng).
    /// Tuyệt đối không ảnh hưởng đến Nhà tuyển dụng hoặc Ban quản trị (Admin).
    /// </summary>
    public class KiemTraNienKhoaWebFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var actionName = context.RouteData.Values["action"]?.ToString() ?? string.Empty;

            // 1. Cho phép các action thiết yếu: Đăng nhập, Đăng ký, Đăng xuất, Minh chứng thẻ SV, Admin, Nhà tuyển dụng
            if (controllerName.Equals("Account", StringComparison.OrdinalIgnoreCase))
            {
                if (actionName.Equals("DangNhap", StringComparison.OrdinalIgnoreCase) ||
                    actionName.Equals("DangKy", StringComparison.OrdinalIgnoreCase) ||
                    actionName.Equals("DangXuat", StringComparison.OrdinalIgnoreCase) ||
                    actionName.Equals("MinhChung", StringComparison.OrdinalIgnoreCase) ||
                    actionName.Equals("QuetTheSinhVien", StringComparison.OrdinalIgnoreCase) ||
                    actionName.Equals("XacNhanMinhChung", StringComparison.OrdinalIgnoreCase) ||
                    actionName.Equals("PheDuyetMinhChung", StringComparison.OrdinalIgnoreCase))
                {
                    await next();
                    return;
                }
            }

            // Cho phép các trang công khai chung
            if (controllerName.Equals("Home", StringComparison.OrdinalIgnoreCase) ||
                controllerName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                controllerName.Equals("NhaTuyenDung", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            // 2. Xác định vai trò người dùng hiện tại
            var tenRole = context.HttpContext.Session.GetString("tenRole")
                          ?? context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            // Nếu không phải FreelancerStudent (hoặc chưa đăng nhập) thì bỏ qua
            if (!string.Equals(tenRole, "FreelancerStudent", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            // 3. Lấy maUser của sinh viên
            var maUser = context.HttpContext.Session.GetInt32("maUser");
            if (!maUser.HasValue)
            {
                var claimId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(claimId) && int.TryParse(claimId, out int idFromClaim))
                {
                    maUser = idFromClaim;
                }
            }

            if (maUser.HasValue && maUser.Value > 0)
            {
                var minhChungService = context.HttpContext.RequestServices.GetRequiredService<IMinhChungWebService>();
                var xacThucRes = await minhChungService.KiemTraTrangThaiXacThucAsync(maUser.Value);

                if (xacThucRes.data != null && xacThucRes.data.HetHanNienKhoa)
                {
                    // Cập nhật cờ hết hạn vào session
                    context.HttpContext.Session.SetString("HetHanNienKhoa", "true");
                    context.HttpContext.Session.SetString("ThongBaoNienKhoa", NienKhoaHelper.ThongBaoHetHan);

                    // Nếu là AJAX request hoặc mong muốn JSON
                    var isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                                 context.HttpContext.Request.Headers.Accept.ToString().Contains("application/json");

                    if (isAjax)
                    {
                        context.Result = new JsonResult(new
                        {
                            success = false,
                            hetHanNienKhoa = true,
                            message = NienKhoaHelper.ThongBaoHetHan
                        })
                        {
                            StatusCode = 403
                        };
                        return;
                    }

                    // Nếu là điều hướng trang thông thường => Chuyển hướng về trang nộp minh chứng kèm thông báo lỗi bắt buộc
                    if (context.Controller is Controller ctrl && ctrl.TempData != null)
                    {
                        ctrl.TempData["ErrorMessage"] = NienKhoaHelper.ThongBaoHetHan;
                    }

                    context.Result = new RedirectToActionResult("MinhChung", "Account", null);
                    return;
                }
                else if (xacThucRes.data != null && xacThucRes.data.DaXacMinh)
                {
                    // Nếu đã xác minh thành công và niên khóa còn hạn, xóa cờ hết hạn
                    context.HttpContext.Session.Remove("HetHanNienKhoa");
                    context.HttpContext.Session.Remove("ThongBaoNienKhoa");
                }
            }

            await next();
        }
    }
}
