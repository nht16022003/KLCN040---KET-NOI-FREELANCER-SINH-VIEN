using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FreelancerStudent.Web.ViewModels;
using System.Collections.Generic;

namespace FreelancerStudent.Web.Services
{
    public interface ICustomerApiService
    {
        Task<EmployerDashboardViewModel> GetDashboardDataAsync(string? userId, string? cusId);
    }

    public class CustomerApiService : ICustomerApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomerApiService> _logger;

        public CustomerApiService(HttpClient httpClient, IConfiguration configuration, ILogger<CustomerApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7172/";
            if (!_httpClient.BaseAddress?.ToString().StartsWith("http") ?? true)
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<EmployerDashboardViewModel> GetDashboardDataAsync(string? userId, string? cusId)
        {
            try
            {
                var url = $"api/Customer/dashboard?userId={userId}&cusId={cusId}";
                _logger.LogInformation("Web: Đang gửi yêu cầu lấy dữ liệu Dashboard sang API: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = await response.Content.ReadFromJsonAsync<EmployerDashboardApiResponse>();
                    if (apiResult != null && apiResult.IsSuccess)
                    {
                        return MapToViewModel(apiResult);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể kết nối đến API lấy Dashboard. Đang sử dụng dữ liệu mặc định.");
            }

            return GetFallbackViewModel(userId, cusId);
        }

        private EmployerDashboardViewModel MapToViewModel(EmployerDashboardApiResponse api)
        {
            var vm = new EmployerDashboardViewModel
            {
                EmployerInfo = new EmployerProfileSummaryViewModel
                {
                    UserID = api.UserID,
                    CusID = api.CusID,
                    UserName = api.UserName,
                    FullName = api.FullName,
                    RoleTitle = api.RoleTitle,
                    CompanyName = api.CompanyName,
                    AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80",
                    UnreadNotificationCount = 3,
                    UnreadMessageCount = 2
                },
                Wallet = new EmployerWalletSummaryViewModel
                {
                    WalletID = "WAL001",
                    SoDuKhadung = api.SoDuKhadung,
                    SoDuDongBang = api.SoDuDongBang
                },
                Kpis = new EmployerKpiSummaryViewModel
                {
                    TotalJobPosts = api.TotalJobPosts,
                    ActiveJobPosts = api.ActiveJobPosts,
                    JobPostsGrowthPercentage = api.JobPostsGrowthPercentage,
                    TotalApplicants = api.TotalApplicants,
                    ApplicantsGrowthPercentage = api.ApplicantsGrowthPercentage,
                    ApplicantsSubtitle = api.ApplicantsSubtitle,
                    ActiveContractsCount = api.ActiveContractsCount,
                    ContractsGrowthPercentage = api.ContractsGrowthPercentage,
                    ContractsSubtitle = api.ContractsSubtitle,
                    CompletedProjectsCount = api.CompletedProjectsCount,
                    CompletedGrowthPercentage = api.CompletedGrowthPercentage,
                    CompletedSubtitle = api.CompletedSubtitle
                }
            };

            foreach (var j in api.RecentJobCards)
            {
                vm.RecentJobCards.Add(new EmployerJobCardViewModel
                {
                    JobID = j.JobID,
                    Title = j.Title,
                    CompanyName = j.CompanyName,
                    FormattedBudget = j.FormattedBudget,
                    Duration = j.Duration,
                    WorkMode = j.WorkMode,
                    StatusText = j.StatusText,
                    ImageUrl = j.ImageUrl,
                    Skills = j.Skills ?? new List<string>(),
                    ApplicantCount = j.ApplicantCount
                });
            }

            foreach (var a in api.NewApplicants)
            {
                vm.NewApplicants.Add(new EmployerNewApplicantViewModel
                {
                    ApplicantID = a.ApplicantID,
                    FullName = a.FullName,
                    AppliedJobID = a.AppliedJobID,
                    TimeAgo = a.TimeAgo,
                    AvatarUrl = a.AvatarUrl
                });
            }

            foreach (var c in api.ActiveContracts)
            {
                vm.ActiveContracts.Add(new EmployerActiveContractViewModel
                {
                    ContractID = c.ContractID,
                    ProjectTitle = c.ProjectTitle,
                    FreelancerName = c.FreelancerName,
                    ProgressPercentage = c.ProgressPercentage,
                    StatusText = c.StatusText,
                    ThumbnailUrl = c.ThumbnailUrl
                });
            }

            return vm;
        }

        private EmployerDashboardViewModel GetFallbackViewModel(string? userId, string? cusId)
        {
            return new EmployerDashboardViewModel
            {
                EmployerInfo = new EmployerProfileSummaryViewModel
                {
                    UserID = userId ?? "USR002",
                    CusID = cusId ?? "CUS001",
                    UserName = "khachhang_a",
                    FullName = "khachhang_a",
                    RoleTitle = "Nhà tuyển dụng",
                    CompanyName = "Công ty ABC",
                    AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80",
                    UnreadNotificationCount = 3,
                    UnreadMessageCount = 2
                },
                Wallet = new EmployerWalletSummaryViewModel
                {
                    WalletID = "WAL001",
                    SoDuKhadung = 12000000,
                    SoDuDongBang = 0
                },
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
                RecentJobCards = new List<EmployerJobCardViewModel>
                {
                    new EmployerJobCardViewModel
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
                    new EmployerJobCardViewModel
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
                    new EmployerJobCardViewModel
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
                },
                NewApplicants = new List<EmployerNewApplicantViewModel>
                {
                    new EmployerNewApplicantViewModel
                    {
                        ApplicantID = "APP001",
                        FullName = "Nguyễn Văn Sơn",
                        AppliedJobID = "JOB001",
                        TimeAgo = "2 giờ trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&w=120&q=80"
                    },
                    new EmployerNewApplicantViewModel
                    {
                        ApplicantID = "APP002",
                        FullName = "Trần Thị Mai",
                        AppliedJobID = "JOB004",
                        TimeAgo = "5 giờ trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=120&q=80"
                    },
                    new EmployerNewApplicantViewModel
                    {
                        ApplicantID = "APP003",
                        FullName = "Lê Minh Khang",
                        AppliedJobID = "JOB002",
                        TimeAgo = "8 giờ trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=120&q=80"
                    },
                    new EmployerNewApplicantViewModel
                    {
                        ApplicantID = "APP004",
                        FullName = "Phạm Ngọc An",
                        AppliedJobID = "JOB003",
                        TimeAgo = "1 ngày trước",
                        AvatarUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=120&q=80"
                    }
                },
                ActiveContracts = new List<EmployerActiveContractViewModel>
                {
                    new EmployerActiveContractViewModel
                    {
                        ContractID = "HD001",
                        ProjectTitle = "Xây dựng website giới thiệu",
                        FreelancerName = "Nguyễn Văn Sơn",
                        ProgressPercentage = 75,
                        StatusText = "Đang làm",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=120&q=80"
                    },
                    new EmployerActiveContractViewModel
                    {
                        ContractID = "HD002",
                        ProjectTitle = "Thiết kế hệ thống nhận diện",
                        FreelancerName = "Trần Thị Mai",
                        ProgressPercentage = 40,
                        StatusText = "Đang làm",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=120&q=80"
                    }
                }
            };
        }
    }

    public class EmployerDashboardApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string CusID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleTitle { get; set; } = "Nhà tuyển dụng";
        public string CompanyName { get; set; } = "Công ty ABC";
        public decimal SoDuKhadung { get; set; } = 0;
        public decimal SoDuDongBang { get; set; } = 0;
        public int TotalJobPosts { get; set; } = 0;
        public int ActiveJobPosts { get; set; } = 0;
        public int JobPostsGrowthPercentage { get; set; } = 25;
        public int TotalApplicants { get; set; } = 28;
        public int ApplicantsGrowthPercentage { get; set; } = 40;
        public string ApplicantsSubtitle { get; set; } = "28 hồ sơ đã ứng tuyển";
        public int ActiveContractsCount { get; set; } = 0;
        public int ContractsGrowthPercentage { get; set; } = 25;
        public string ContractsSubtitle { get; set; } = "5 hợp đồng đang làm";
        public int CompletedProjectsCount { get; set; } = 0;
        public int CompletedGrowthPercentage { get; set; } = 100;
        public string CompletedSubtitle { get; set; } = "1 dự án đã bàn giao";
        public List<ApiJobCardDto> RecentJobCards { get; set; } = new();
        public List<ApiActiveContractDto> ActiveContracts { get; set; } = new();
        public List<ApiNewApplicantDto> NewApplicants { get; set; } = new();
    }

    public class ApiJobCardDto
    {
        public string JobID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = "Công ty ABC";
        public string FormattedBudget { get; set; } = string.Empty;
        public string Duration { get; set; } = "2 - 4 tuần";
        public string WorkMode { get; set; } = "Remote";
        public string StatusText { get; set; } = "Đang tuyển";
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
        public int ApplicantCount { get; set; } = 6;
    }

    public class ApiActiveContractDto
    {
        public string ContractID { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string FreelancerName { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; } = 0;
        public string StatusText { get; set; } = "Đang làm";
        public string ThumbnailUrl { get; set; } = string.Empty;
    }

    public class ApiNewApplicantDto
    {
        public string ApplicantID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AppliedJobID { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
