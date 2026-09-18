using System;
using System.Collections.Generic;

namespace FreelancerStudent.Web.ViewModels
{
    /// <summary>
    /// ViewModel cho Trang Tổng quan của Nhà tuyển dụng (Employer Dashboard)
    /// /// </summary>
    public class EmployerDashboardViewModel
    {
        // 1. Thông tin tài khoản & hồ sơ nhà tuyển dụng (Users, KhachHang, NhaTuyenDung)
        public EmployerProfileSummaryViewModel EmployerInfo { get; set; } = new();

        // 2. Thông tin ví & số dư (Bảng Wallet)
        public EmployerWalletSummaryViewModel Wallet { get; set; } = new();

        // 3. Danh sách từ khóa kỹ năng gợi ý tìm kiếm nhanh trên Banner
        public List<string> PopularSkills { get; set; } = new()
        {
            "UI/UX", "Web Development", "Content Writing", "Design", "Marketing", "Data Entry", "Lập trình"
        };

        // 4. 4 Thẻ chỉ số KPI thống kê tổng quan (JobPost, HopDong, UngTuyen)
        public EmployerKpiSummaryViewModel Kpis { get; set; } = new();

        // 5. Danh sách các bài đăng công việc gần đây dạng Card (JobPost, Skill)
        public List<EmployerJobCardViewModel> RecentJobCards { get; set; } = new();

        // 6. Danh sách ứng viên sinh viên mới nộp hồ sơ (UngTuyen, FreelancerSV, Users)
        public List<EmployerNewApplicantViewModel> NewApplicants { get; set; } = new();

        // 7. Danh sách hợp đồng đang thực hiện & tiến độ % (HopDong, FreelancerSV, Users)
        public List<EmployerActiveContractViewModel> ActiveContracts { get; set; } = new();
    }

    /// <summary>
    /// Thông tin cá nhân & công ty của Nhà tuyển dụng
    /// Bảng: Users, KhachHang, NhaTuyenDung
    /// </summary>
    public class EmployerProfileSummaryViewModel
    {
        public string UserID { get; set; } = "USR002";
        public string CusID { get; set; } = "CUS001";
        public string EmployerID { get; set; } = "EMP001";
        public string UserName { get; set; } = "khachhang_a";
        public string FullName { get; set; } = "khachhang_a";
        public string RoleTitle { get; set; } = "Nhà tuyển dụng";
        public string CompanyName { get; set; } = "Công ty ABC";
        public string AvatarUrl { get; set; } = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80";
        public int UnreadNotificationCount { get; set; } = 3;
        public int UnreadMessageCount { get; set; } = 2;
    }

    /// <summary>
    /// Số dư ví nhà tuyển dụng
    /// Bảng: Wallet (userID = 'USR002')
    /// </summary>
    public class EmployerWalletSummaryViewModel
    {
        public string WalletID { get; set; } = "WAL002";
        public decimal SoDuKhadung { get; set; } = 12000000;
        public decimal SoDuDongBang { get; set; } = 0;
        public string FormattedBalance => SoDuKhadung.ToString("N0") + "đ";
    }

    /// <summary>
    /// 4 Thẻ chỉ số KPI thống kê trên Dashboard
    /// </summary>
    public class EmployerKpiSummaryViewModel
    {
        // 1. Bài đăng tuyển dụng (JobPost WHERE cusID = @cusID)
        public int TotalJobPosts { get; set; } = 5;
        public int ActiveJobPosts { get; set; } = 5;
        public int JobPostsGrowthPercentage { get; set; } = 25; // ↑ 25%

        // 2. Freelancer ứng tuyển (UngTuyen)
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

    /// <summary>
    /// Thẻ bài đăng công việc tuyển dụng gần đây
    /// Bảng: JobPost, NhaTuyenDung, Skill
    /// </summary>
    public class EmployerJobCardViewModel
    {
        public string JobID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CompanyName { get; set; } = "Công ty ABC";
        public decimal Budget { get; set; }
        public string FormattedBudget => Budget.ToString("N0") + "đ";
        public string Duration { get; set; } = "2 - 4 tuần";
        public string WorkMode { get; set; } = "Remote"; // Remote, Onsite, Hybrid
        public List<string> Skills { get; set; } = new();
        public int ApplicantCount { get; set; }
        public string StatusText { get; set; } = "Đang tuyển";
        public bool IsFavorite { get; set; } = false;
    }

    /// <summary>
    /// Thông tin ứng viên sinh viên mới ứng tuyển vào bài đăng
    /// Bảng: UngTuyen JOIN FreelancerSV JOIN Users
    /// </summary>
    public class EmployerNewApplicantViewModel
    {
        public string FreeID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string AppliedJobID { get; set; } = string.Empty;
        public string AppliedJobTitle { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
    }

    /// <summary>
    /// Thông tin hợp đồng đang thực hiện kèm tiến độ %
    /// Bảng: HopDong JOIN FreelancerSV JOIN Users (cột tienDo INT 0-100)
    /// </summary>
    public class EmployerActiveContractViewModel
    {
        public string MaHD { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string FreelancerName { get; set; } = string.Empty;
        public string FreelancerID { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; } = 0; // tienDo: 75%, 40%...
        public string StatusText { get; set; } = "Đang làm";
    }
}
