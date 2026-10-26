using FreelancerStudent.API.DTOs.ReponseDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IKyNangGoiYService
    {
        /// <summary>
        /// Gợi ý danh sách kỹ năng chuẩn trong SQL dựa trên tiêu đề và nội dung mô tả công việc
        /// </summary>
        Task<List<KyNangGoiY_ResponseDTO>> GoiYKyNangTheoVanBanAsync(string? tieuDe, string? moTa = null);

        /// <summary>
        /// Suy luận tập hợp các kỹ năng liên quan từ văn bản phục vụ thuật toán gợi ý việc làm
        /// </summary>
        Task<HashSet<string>> SuyLuanKyNangTuVanBanAsync(string? tieuDe, string? moTa = null);

        /// <summary>
        /// Đảm bảo danh mục kỹ năng mặc định luôn có sẵn trong SQL Server (không tạo bảng mới)
        /// </summary>
        Task<bool> DamBaoKyNangMacDinhAsync();
    }
}
