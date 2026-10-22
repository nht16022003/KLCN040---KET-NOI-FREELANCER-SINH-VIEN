using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services
{
    public interface IDanhGiaService
    {
        Task<DanhGia_FormInfo_ReponseDTO> LayThongTinFormAsync(string maHD, int maUser);
        Task<DanhGia_ReponseDTO> GuiDanhGiaAsync(DanhGia_RequestDTO request, int maUser);
        Task<DanhGia_ReponseDTO> CapNhatDanhGiaAsync(int maDanhGia, DanhGia_RequestDTO request, int maUser);
        Task<DanhGiaThongKe_ReponseDTO> LayThongKeFreelancerAsync(int maFreelancerStudent, int trang, int kichThuocTrang);
        Task<DanhGiaThongKe_ReponseDTO> LayThongKeNhaTuyenDungAsync(int maNhaTuyenDung, int trang, int kichThuocTrang);
        Task<List<HopDongDanhGia_ReponseDTO>> LayCongViecDaThueAsync(int maUser);
    }
}
