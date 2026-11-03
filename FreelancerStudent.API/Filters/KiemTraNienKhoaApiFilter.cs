using System.Security.Claims;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FreelancerStudent.API.Filters
{
    /// <summary>
    /// Filter tập trung kiểm tra thời hạn niên khóa cho toàn bộ API nghiệp vụ sinh viên.
    /// Ngăn chặn gọi trực tiếp API bằng token cũ, cookie cũ hoặc URL trực tiếp khi niên khóa đã hết hạn.
    /// Không ảnh hưởng đến tài khoản Nhà tuyển dụng và Quản trị viên (Admin).
    /// </summary>
    public class KiemTraNienKhoaApiFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var actionName = context.RouteData.Values["action"]?.ToString() ?? string.Empty;

            // 1. Danh sách các Controller / Action được miễn trừ (Công khai, Đăng nhập, Duyệt minh chứng, Nộp minh chứng lại, Admin, Nhà tuyển dụng)
            if (controllerName.Equals("Auth", StringComparison.OrdinalIgnoreCase) ||
                controllerName.Equals("MinhChung", StringComparison.OrdinalIgnoreCase) ||
                controllerName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                controllerName.Equals("NhaTuyenDung", StringComparison.OrdinalIgnoreCase) ||
                controllerName.Equals("WeatherForecast", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            // Các action xem tin tuyển dụng công khai cho mọi đối tượng
            if (controllerName.Equals("JobPost", StringComparison.OrdinalIgnoreCase) &&
                (actionName.Equals("layDanhSachJobPost", StringComparison.OrdinalIgnoreCase) ||
                 actionName.Equals("ChiTietJobPost", StringComparison.OrdinalIgnoreCase)))
            {
                await next();
                return;
            }

            // 2. Trích xuất maUser từ nhiều nguồn (Claims, Route, Query, Action Arguments)
            int maUser = 0;

            // Nguồn 1: Claims từ Cookie Authentication
            var claimId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(claimId) && int.TryParse(claimId, out int idFromClaim))
            {
                maUser = idFromClaim;
            }

            // Nguồn 2: RouteData hoặc Query parameter
            if (maUser <= 0)
            {
                if (context.RouteData.Values.TryGetValue("maUser", out var routeVal) && routeVal != null)
                {
                    int.TryParse(routeVal.ToString(), out maUser);
                }
                else if (context.HttpContext.Request.Query.TryGetValue("maUser", out var queryVal))
                {
                    int.TryParse(queryVal.ToString(), out maUser);
                }
            }

            // Nguồn 3: Action Arguments (Body DTO có trường maUser)
            if (maUser <= 0)
            {
                foreach (var arg in context.ActionArguments.Values)
                {
                    if (arg == null) continue;
                    if (arg is int intVal && intVal > 0)
                    {
                        maUser = intVal;
                        break;
                    }
                    var prop = arg.GetType().GetProperty("maUser") ?? arg.GetType().GetProperty("MaUser");
                    if (prop != null)
                    {
                        var propVal = prop.GetValue(arg);
                        if (propVal is int pInt && pInt > 0)
                        {
                            maUser = pInt;
                            break;
                        }
                    }
                }
            }

            // 3. Nếu tìm thấy mã người dùng, kiểm tra trạng thái niên khóa sinh viên
            if (maUser > 0)
            {
                var minhChungService = context.HttpContext.RequestServices.GetRequiredService<IMinhChungService>();
                var check = await minhChungService.KiemTraTrangThaiXacThucAsync(maUser);

                // Nếu là tài khoản khác (Nhà tuyển dụng / Admin: TrangThai == "KhongApDung") => Cho phép tiếp tục
                if (check.TrangThai == "KhongApDung")
                {
                    await next();
                    return;
                }

                // Nếu là FreelancerStudent và niên khóa đã hết hạn => Chặn với HTTP 403 Forbidden và thông báo bắt buộc
                if (check.HetHanNienKhoa)
                {
                    context.Result = new ObjectResult(new
                    {
                        success = false,
                        hetHanNienKhoa = true,
                        message = NienKhoaHelper.ThongBaoHetHan,
                        data = check
                    })
                    {
                        StatusCode = 403
                    };
                    return;
                }
            }

            await next();
        }
    }
}
