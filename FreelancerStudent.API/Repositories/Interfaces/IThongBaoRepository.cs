using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IThongBaoRepository
    {
        Task<(List<ThongBao> items, int total, int unread)> LayDanhSachAsync(int maUser, int trang, int kichThuocTrang);
        Task<int> DemChuaDocAsync(int maUser);
        Task<bool> DanhDauDaDocAsync(int maThongBao, int maUser);
        Task<int> DanhDauTatCaDaDocAsync(int maUser);
    }
}
