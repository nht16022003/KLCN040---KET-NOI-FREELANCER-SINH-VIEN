using FreelancerStudent.API.DTOs.ReponseDTOs;

namespace FreelancerStudent.API.Services
{
    public interface IThongBaoService
    {
        Task<ThongBaoDanhSach_ReponseDTO> LayDanhSachAsync(int maUser, int trang, int kichThuocTrang);
        Task<int> DemChuaDocAsync(int maUser);
        Task<bool> DanhDauDaDocAsync(int maThongBao, int maUser);
        Task<int> DanhDauTatCaDaDocAsync(int maUser);
    }
}
