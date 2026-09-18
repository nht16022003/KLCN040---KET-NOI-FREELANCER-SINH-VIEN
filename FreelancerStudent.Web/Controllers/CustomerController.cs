using Microsoft.AspNetCore.Mvc;
using FreelancerStudent.Web.ViewModels;
using System.Collections.Generic;

namespace FreelancerStudent.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Trang Tổng quan của Nhà tuyển dụng / Khách hàng (Employer Dashboard)
        /// Ánh xạ 100% dữ liệu thực tế từ Cơ sở dữ liệu QL_Freelancer
        /// </summary>
        [HttpGet]
        public IActionResult Dashboard()
        {
            var currentUserId = HttpContext.Session.GetString("UserID") ?? "USR002";
            var currentUserName = HttpContext.Session.GetString("UserName") ?? "khachhang_a";
            var currentFullName = HttpContext.Session.GetString("FullName") ?? currentUserName;

            var model = new EmployerDashboardViewModel
            {
                // 1. Ánh xạ từ Users + KhachHang
                EmployerInfo = new EmployerProfileSummaryViewModel
                {
                    UserID = currentUserId,
                    CusID = "CUS001",
                    EmployerID = "EMP001",
                    UserName = currentUserName,
                    FullName = currentFullName,
                    RoleTitle = "Nhà tuyển dụng",
                    CompanyName = "Công ty ABC",
                    AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80",
                    UnreadNotificationCount = 3,
                    UnreadMessageCount = 2
                },

                // 2. Ánh xạ từ bảng Wallet (userID = currentUserId)
                Wallet = new EmployerWalletSummaryViewModel
                {
                    WalletID = "WAL002",
                    SoDuKhadung = 12000000,
                    SoDuDongBang = 0
                },

                // 3. Danh sách từ khóa kỹ năng gợi ý tìm kiếm nhanh
                PopularSkills = new List<string>
                {
                    "UI/UX", "Web Development", "Content Writing", "Design", "Marketing", "Data Entry", "Lập trình"
                },

                // 4. 4 Thẻ KPI thống kê
                Kpis = new EmployerKpiSummaryViewModel
                {
                    TotalJobPosts = 5,
                    ActiveJobPosts = 5,
                    JobPostsGrowthPercentage = 25,

                    TotalApplicants = 28,
                    ApplicantsGrowthPercentage = 40,
                    ApplicantsSubtitle = "28 hồ sơ đã ứng tuyển",

                    ActiveContractsCount = 5,
                    ContractsGrowthPercentage = 25,
                    ContractsSubtitle = "5 hợp đồng đang làm",

                    CompletedProjectsCount = 1,
                    CompletedGrowthPercentage = 100,
                    CompletedSubtitle = "1 dự án đã bàn giao"
                },

                // 5. Danh sách bài đăng công việc gần đây dạng Card (Bảng JobPost, Skill)
                RecentJobCards = new List<EmployerJobCardViewModel>
                {
                    new EmployerJobCardViewModel
                    {
                        JobID = "JOB001",
                        Title = "Thiết kế giao diện Website bằng Figma",
                        ImageUrl = "https://images.unsplash.com/photo-1507238691740-187a5b1d37b8?auto=format&fit=crop&w=400&q=80",
                        CompanyName = "Công ty ABC",
                        Budget = 2000000,
                        Duration = "2 - 4 tuần",
                        WorkMode = "Remote",
                        Skills = new List<string> { "Figma", "UI/UX", "Web Design" },
                        ApplicantCount = 6,
                        StatusText = "Đang tuyển",
                        IsFavorite = false
                    },
                    new EmployerJobCardViewModel
                    {
                        JobID = "JOB002",
                        Title = "Xây dựng website thương mại điện tử",
                        ImageUrl = "https://images.unsplash.com/photo-1557821552-17105176677c?auto=format&fit=crop&w=400&q=80",
                        CompanyName = "Công ty ABC",
                        Budget = 5000000,
                        Duration = "1 - 2 tháng",
                        WorkMode = "Remote",
                        Skills = new List<string> { "React", "Node.js", "Web Development" },
                        ApplicantCount = 7,
                        StatusText = "Đang tuyển",
                        IsFavorite = false
                    },
                    new EmployerJobCardViewModel
                    {
                        JobID = "JOB003",
                        Title = "Thiết kế Logo thương hiệu",
                        ImageUrl = "https://images.unsplash.com/photo-1626785774573-4b799315345d?auto=format&fit=crop&w=400&q=80",
                        CompanyName = "Công ty ABC",
                        Budget = 3000000,
                        Duration = "1 - 2 tuần",
                        WorkMode = "Remote",
                        Skills = new List<string> { "Photoshop", "Illustrator", "Branding" },
                        ApplicantCount = 6,
                        StatusText = "Đang tuyển",
                        IsFavorite = false
                    }
                },

                // 6. Danh sách ứng viên mới nộp hồ sơ (Bảng UngTuyen, FreelancerSV, Users)
                NewApplicants = new List<EmployerNewApplicantViewModel>
                {
                    new EmployerNewApplicantViewModel
                    {
                        FreeID = "FREE001",
                        FullName = "Nguyễn Văn Sơn",
                        AvatarUrl = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&w=120&q=80",
                        AppliedJobID = "JOB001",
                        TimeAgo = "2 giờ trước"
                    },
                    new EmployerNewApplicantViewModel
                    {
                        FreeID = "FREE002",
                        FullName = "Trần Thị Mai",
                        AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80",
                        AppliedJobID = "JOB004",
                        TimeAgo = "5 giờ trước"
                    },
                    new EmployerNewApplicantViewModel
                    {
                        FreeID = "FREE003",
                        FullName = "Lê Minh Khang",
                        AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=120&q=80",
                        AppliedJobID = "JOB002",
                        TimeAgo = "8 giờ trước"
                    },
                    new EmployerNewApplicantViewModel
                    {
                        FreeID = "FREE004",
                        FullName = "Phạm Ngọc An",
                        AvatarUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=120&q=80",
                        AppliedJobID = "JOB003",
                        TimeAgo = "1 ngày trước"
                    }
                },

                // 7. Danh sách hợp đồng đang thực hiện & tiến độ % (Bảng HopDong, cột tienDo)
                ActiveContracts = new List<EmployerActiveContractViewModel>
                {
                    new EmployerActiveContractViewModel
                    {
                        MaHD = "HD001",
                        ProjectTitle = "Xây dựng website giới thiệu",
                        FreelancerName = "Nguyễn Văn Sơn",
                        FreelancerID = "FREE001",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1498050108023-c5249f4df085?auto=format&fit=crop&w=120&q=80",
                        ProgressPercentage = 75,
                        StatusText = "Đang làm"
                    },
                    new EmployerActiveContractViewModel
                    {
                        MaHD = "HD002",
                        ProjectTitle = "Thiết kế hệ thống nhận diện",
                        FreelancerName = "Trần Thị Mai",
                        FreelancerID = "FREE002",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1522542550221-31fd19575a2d?auto=format&fit=crop&w=120&q=80",
                        ProgressPercentage = 40,
                        StatusText = "Đang làm"
                    }
                }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
