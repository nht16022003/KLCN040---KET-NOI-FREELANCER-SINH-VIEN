using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IBaiDangTimViecService
    {
        //Tuấn
        Task<BaiDangTimViec_ReponseDTO> taoBaiDangAsync(BaiDangTimViec_RequestDTO request);

        //Tuấn
        Task<List<BaiDangTimViec_ReponseDTO>> layDanhSachTheoMaUserAsync(int maUser);

        //Tuấn
        Task<List<BaiDangTimViec_ReponseDTO>> layTatCaBaiDangKhaDungAsync();

        //Tuấn
        Task<bool> doiTrangThaiAsync(string maBaiDang, string trangthaiMoi);

        //Tuấn
        Task<bool> xoaBaiDangAsync(string maBaiDang);

        //Tuấn
        Task<bool> capNhatBaiDangAsync(string maBaiDang, BaiDangTimViec_RequestDTO request);
    }
}
