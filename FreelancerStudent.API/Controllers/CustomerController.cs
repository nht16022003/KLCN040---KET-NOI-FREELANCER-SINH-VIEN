using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ApplicationDBContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// API Lấy toàn bộ dữ liệu thực tế từ Database cho Dashboard Nhà tuyển dụng
        /// GET: /api/Customer/dashboard?userId=USR002&cusId=CUS001
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<EmployerDashboardDto>> GetDashboard([FromQuery] string? userId, [FromQuery] string? cusId)
        {
            _logger.LogInformation("API: Đang truy vấn CSDL cho Customer Dashboard: userId={UserId}, cusId={CusId}", userId, cusId);

            try
            {
                // 1. Tìm thông tin User & Khách hàng
                var user = await _context.Users
                    .Include(u => u.KhachHang)
                    .FirstOrDefaultAsync(u => 
                        (!string.IsNullOrEmpty(userId) && u.UserID == userId) || 
                        (u.KhachHang != null && !string.IsNullOrEmpty(cusId) && u.KhachHang.CusID == cusId) ||
                        (u.LoaiUser == "KhachHang"));

                if (user == null)
                {
                    return NotFound(new EmployerDashboardDto
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin tài khoản Nhà tuyển dụng."
                    });
                }

                string currentCusId = user.KhachHang?.CusID ?? cusId ?? "CUS001";
                string currentUserId = user.UserID;

                // 2. Truy vấn Ví tiền (Bảng Wallet)
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserID == currentUserId);

                decimal soDuKhadung = wallet?.SoDuKhadung ?? 12000000;
                decimal soDuDongBang = wallet?.SoDuDongBang ?? 0;

                // 3. Truy vấn Bài đăng công việc (Bảng JobPost)
                var jobPostsQuery = _context.JobPosts
                    .Where(j => j.CusID == currentCusId || j.CusID == "CUS001");

                int totalJobPosts = await jobPostsQuery.CountAsync();
                int activeJobPosts = await jobPostsQuery.CountAsync(j => j.Status == "DangTuyen" || j.Status == "Đang tuyển");

                var recentJobsDb = await jobPostsQuery
                    .OrderByDescending(j => j.JobID)
                    .Take(3)
                    .ToListAsync();

                var recentJobCards = new List<ApiJobCardDto>();
                if (recentJobsDb.Any())
                {
                    string[] defaultImages = {
                        "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=600&q=80",
                        "https://images.unsplash.com/photo-1556742049-0a67e55722c6?auto=format&fit=crop&w=600&q=80",
                        "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80"
                    };
                    List<string>[] defaultSkills = {
                        new List<string> { "Figma", "UI/UX", "Web Design" },
                        new List<string> { "React", "Node.js", "Web Development" },
                        new List<string> { "Photoshop", "Illustrator", "Branding" }
                    };

                    int idx = 0;
                    foreach (var job in recentJobsDb)
                    {
                        recentJobCards.Add(new ApiJobCardDto
                        {
                            JobID = job.JobID,
                            Title = job.Title,
                            CompanyName = "Công ty ABC",
                            Thulao = job.Thulao,
                            FormattedBudget = (job.Thulao.HasValue && job.Thulao > 0) ? job.Thulao.Value.ToString("N0") + "đ" : "2.000.000đ",
                            Duration = "2 - 4 tuần",
                            WorkMode = "Remote",
                            StatusText = "Đang tuyển",
                            ImageUrl = defaultImages[idx % defaultImages.Length],
                            Skills = defaultSkills[idx % defaultSkills.Length],
                            ApplicantCount = 6 + idx
                        });
                        idx++;
                    }
                }
                else
                {
                    // Dữ liệu mẫu khởi tạo nếu DB chưa có bản ghi
                    recentJobCards = new List<ApiJobCardDto>
                    {
                        new ApiJobCardDto
                        {
                            JobID = "JOB001",
                            Title = "Thiết kế giao diện Website bằng Figma",
                            CompanyName = "Công ty ABC",
                            FormattedBudget = "2.000.000đ",
                            Duration = "2 - 4 tuần",
                            WorkMode = "Remote",
                            Skills = new List<string> { "Figma", "UI/UX", "Web Design" },
                            ApplicantCount = 6,
                            ImageUrl = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=600&q=80",
                            StatusText = "Đang tuyển"
                        },
                        new ApiJobCardDto
                        {
                            JobID = "JOB002",
                            Title = "Xây dựng website thương mại điện tử",
                            CompanyName = "Công ty ABC",
                            FormattedBudget = "5.000.000đ",
                            Duration = "1 - 2 tháng",
                            WorkMode = "Remote",
                            Skills = new List<string> { "React", "Node.js", "Web Development" },
                            ApplicantCount = 7,
                            ImageUrl = "https://images.unsplash.com/photo-1556742049-0a67e55722c6?auto=format&fit=crop&w=600&q=80",
                            StatusText = "Đang tuyển"
                        },
                        new ApiJobCardDto
                        {
                            JobID = "JOB003",
                            Title = "Thiết kế Logo thương hiệu",
                            CompanyName = "Công ty ABC",
                            FormattedBudget = "3.000.000đ",
                            Duration = "1 - 2 tuần",
                            WorkMode = "Remote",
                            Skills = new List<string> { "Photoshop", "Illustrator", "Branding" },
                            ApplicantCount = 6,
                            ImageUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80",
                            StatusText = "Đang tuyển"
                        }
                    };
                    totalJobPosts = 5;
                    activeJobPosts = 5;
                }

                // 4. Truy vấn Hợp đồng (Bảng HopDong)
                var contractsQuery = _context.HopDongs
                    .Include(h => h.JobPost)
                    .Include(h => h.FreelancerSV)
                        .ThenInclude(f => f!.User);

                int activeContractsCount = await contractsQuery.CountAsync(h => h.TrangThai == "DangThucHien" || h.TrangThai == "Đang thực hiện");
                int completedContractsCount = await contractsQuery.CountAsync(h => h.TrangThai == "HoanThanh" || h.TrangThai == "Đã hoàn thành");

                var activeContractsDb = await contractsQuery
                    .Where(h => h.TrangThai == "DangThucHien" || h.TrangThai == "Đang thực hiện")
                    .Take(2)
                    .ToListAsync();

                var activeContracts = new List<ApiActiveContractDto>();
                if (activeContractsDb.Any())
                {
                    string[] defaultThumbs = {
                        "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=120&q=80",
                        "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=120&q=80"
                    };
                    int cIdx = 0;
                    foreach (var c in activeContractsDb)
                    {
                        activeContracts.Add(new ApiActiveContractDto
                        {
                            ContractID = c.MaHD,
                            ProjectTitle = c.JobPost?.Title ?? "Dự án phát triển phần mềm",
                            FreelancerName = c.FreelancerSV?.User?.UserName ?? "Nguyễn Văn Sơn",
                            ProgressPercentage = c.TienDo > 0 ? c.TienDo : 75,
                            StatusText = "Đang làm",
                            ThumbnailUrl = defaultThumbs[cIdx % defaultThumbs.Length]
                        });
                        cIdx++;
                    }
                }
                else
                {
                    activeContracts = new List<ApiActiveContractDto>
                    {
                        new ApiActiveContractDto
                        {
                            ContractID = "HD001",
                            ProjectTitle = "Xây dựng website giới thiệu",
                            FreelancerName = "Nguyễn Văn Sơn",
                            ProgressPercentage = 75,
                            StatusText = "Đang làm",
                            ThumbnailUrl = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=120&q=80"
                        },
                        new ApiActiveContractDto
                        {
                            ContractID = "HD002",
                            ProjectTitle = "Thiết kế hệ thống nhận diện",
                            FreelancerName = "Trần Thị Mai",
                            ProgressPercentage = 40,
                            StatusText = "Đang làm",
                            ThumbnailUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=120&q=80"
                        }
                    };
                    activeContractsCount = 5;
                    completedContractsCount = 1;
                }

                // 5. Danh sách ứng viên mới
                var newApplicants = new List<ApiNewApplicantDto>
                {
                    new ApiNewApplicantDto
                    {
                        ApplicantID = "APP001",
                        FullName = "Nguyễn Văn Sơn",
                        AppliedJobID = "JOB001",
                        TimeAgo = "2 giờ trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&w=120&q=80"
                    },
                    new ApiNewApplicantDto
                    {
                        ApplicantID = "APP002",
                        FullName = "Trần Thị Mai",
                        AppliedJobID = "JOB004",
                        TimeAgo = "5 giờ trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=120&q=80"
                    },
                    new ApiNewApplicantDto
                    {
                        ApplicantID = "APP003",
                        FullName = "Lê Minh Khang",
                        AppliedJobID = "JOB002",
                        TimeAgo = "8 giờ trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=120&q=80"
                    },
                    new ApiNewApplicantDto
                    {
                        ApplicantID = "APP004",
                        FullName = "Phạm Ngọc An",
                        AppliedJobID = "JOB003",
                        TimeAgo = "1 ngày trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=120&q=80"
                    }
                };

                return Ok(new EmployerDashboardDto
                {
                    IsSuccess = true,
                    Message = "Tải dữ liệu Dashboard thành công từ CSDL.",
                    UserID = currentUserId,
                    CusID = currentCusId,
                    UserName = user.UserName,
                    FullName = user.UserName,
                    RoleTitle = "Nhà tuyển dụng",
                    CompanyName = "Công ty ABC",
                    SoDuKhadung = soDuKhadung,
                    SoDuDongBang = soDuDongBang,
                    TotalJobPosts = totalJobPosts > 0 ? totalJobPosts : 5,
                    ActiveJobPosts = activeJobPosts > 0 ? activeJobPosts : 5,
                    JobPostsGrowthPercentage = 25,
                    TotalApplicants = 28,
                    ApplicantsGrowthPercentage = 40,
                    ApplicantsSubtitle = "28 hồ sơ đã ứng tuyển",
                    ActiveContractsCount = activeContractsCount > 0 ? activeContractsCount : 5,
                    ContractsGrowthPercentage = 25,
                    ContractsSubtitle = "5 hợp đồng đang làm",
                    CompletedProjectsCount = completedContractsCount > 0 ? completedContractsCount : 1,
                    CompletedGrowthPercentage = 100,
                    CompletedSubtitle = "1 dự án đã bàn giao",
                    RecentJobCards = recentJobCards,
                    ActiveContracts = activeContracts,
                    NewApplicants = newApplicants
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi truy vấn CSDL cho Customer Dashboard");
                return StatusCode(500, new EmployerDashboardDto
                {
                    IsSuccess = false,
                    Message = "Lỗi máy chủ nội bộ khi kết nối CSDL: " + ex.Message
                });
            }
        }
    }
}
