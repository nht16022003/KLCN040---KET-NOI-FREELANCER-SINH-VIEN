using System;
using System.Collections.Generic;

namespace FreelancerStudent.Web.ViewModels
{
    /// <summary>
    /// ViewModel chính cho Trang Tổng quan Nhà tuyển dụng (KhachHang / Employer Dashboard)
    /// </summary>
    public class EmployerDashboardViewModel
    {
        // 1. Thông tin Nhà tuyển dụng (Users + KhachHang + NhaTuyenDung)
        public EmployerProfileSummaryViewModel EmployerInfo { get; set; } = new();

        // 2. Số dư ví (Wallet)
        public EmployerWalletSummaryViewModel Wallet { get; set; } = new();

        // 3. Danh sách từ khóa kỹ năng gợi ý tìm kiếm
        public List<string> PopularSkills { get; set; } = new()
        {
            "UI/UX", "Web Development", "Content Writing", "Design", "Marketing", "Data Entry", "Lập trình"
        };

        // 4. 4 Thẻ chỉ số KPI thống kê tổng quan
        public EmployerKpiSummaryViewModel Kpis { get; set; } = new();

        // 5. Danh sách 3 card công việc tuyển dụng gần đây (JobPost)
        public List<EmployerJobCardViewModel> RecentJobCards { get; set; } = new();

        // 6. Danh sách ứng viên mới nộp hồ sơ
        public List<EmployerNewApplicantViewModel> NewApplicants { get; set; } = new();

        // 7. Danh sách hợp đồng đang thực hiện (HopDong)
        public List<EmployerActiveContractViewModel> ActiveContracts { get; set; } = new();
    }

    public class EmployerProfileSummaryViewModel
    {
        public string UserID { get; set; } = "USR002";
        public string CusID { get; set; } = "CUS001";
        public string UserName { get; set; } = "khachhang_a";
        public string FullName { get; set; } = "khachhang_a";
        public string RoleTitle { get; set; } = "Nhà tuyển dụng";
        public string CompanyName { get; set; } = "Công ty ABC";
        public string AvatarUrl { get; set; } = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80";
        public int UnreadNotificationCount { get; set; } = 3;
        public int UnreadMessageCount { get; set; } = 2;
    }

    public class EmployerWalletSummaryViewModel
    {
        public string WalletID { get; set; } = "WAL001";
        public decimal SoDuKhadung { get; set; } = 12000000;
        public decimal SoDuDongBang { get; set; } = 0;
        public string FormattedBalance => SoDuKhadung.ToString("N0") + "đ";
    }

    public class EmployerKpiSummaryViewModel
    {
        // 1. Bài đăng tuyển dụng (JobPost WHERE cusID = @cusID)
        public int TotalJobPosts { get; set; } = 5;
        public int ActiveJobPosts { get; set; } = 5;
        public int JobPostsGrowthPercentage { get; set; } = 25; // ↑ 25%

        // 2. Freelancer ứng tuyển
        public int TotalApplicants { get; set; } = 28;
        public int ApplicantsGrowthPercentage { get; set; } = 40; // ↑ 40%
        public string ApplicantsSubtitle { get; set; } = "28 hồ sơ đã ứng tuyển";

        // 3. Công việc đang thực hiện (HopDong WHERE trangThai = 'DangThucHien')
        public int ActiveContractsCount { get; set; } = 5;
        public int ContractsGrowthPercentage { get; set; } = 25; // ↑ 25%
        public string ContractsSubtitle { get; set; } = "5 hợp đồng đang làm";

        // 4. Dự án đã hoàn thành (HopDong WHERE trangThai = 'HoanThanh')
        public int CompletedProjectsCount { get; set; } = 1;
        public int CompletedGrowthPercentage { get; set; } = 100; // ↑ 100%
        public string CompletedSubtitle { get; set; } = "1 dự án đã bàn giao";
    }

    public class EmployerJobCardViewModel
    {
        public string JobID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = "Công ty ABC";
        public string FormattedBudget { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string WorkMode { get; set; } = "Remote";
        public string ImageUrl { get; set; } = string.Empty;
        public string StatusText { get; set; } = "Đang tuyển";
        public List<string> Skills { get; set; } = new();
        public int ApplicantCount { get; set; } = 0;
    }

    public class EmployerNewApplicantViewModel
    {
        public string ApplicantID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AppliedJobID { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }

    public class EmployerActiveContractViewModel
    {
        public string ContractID { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string FreelancerName { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; } = 0;
        public string StatusText { get; set; } = "Đang làm";
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}
