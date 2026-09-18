using System;
using System.Collections.Generic;

namespace FreelancerStudent.API.DTOs
{
    public class EmployerDashboardDto
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        // 1. Hồ sơ
        public string UserID { get; set; } = string.Empty;
        public string CusID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleTitle { get; set; } = "Nhà tuyển dụng";
        public string CompanyName { get; set; } = "Công ty ABC";

        // 2. Ví tiền
        public decimal SoDuKhadung { get; set; } = 0;
        public decimal SoDuDongBang { get; set; } = 0;

        // 3. KPI thống kê
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

        // 4. Danh sách công việc gần đây
        public List<ApiJobCardDto> RecentJobCards { get; set; } = new();

        // 5. Danh sách hợp đồng đang thực hiện
        public List<ApiActiveContractDto> ActiveContracts { get; set; } = new();

        // 6. Danh sách ứng viên mới
        public List<ApiNewApplicantDto> NewApplicants { get; set; } = new();
    }

    public class ApiJobCardDto
    {
        public string JobID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = "Công ty ABC";
        public decimal? Thulao { get; set; }
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
