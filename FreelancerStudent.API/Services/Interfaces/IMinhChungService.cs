using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IMinhChungService
    {
        Task<QuetTheSinhVien_ResponseDTO> QuetVaLuuTheSinhVienAsync(IFormFile file, int maUser);
        Task<bool> XacNhanVaCapNhatHoSoTheSinhVienAsync(XacNhanTheSinhVien_RequestDTO request);
        Task<List<MinhChungItem_ResponseDTO>> LayLichSuMinhChungAsync(int maUser);
        Task<List<ChuyenNganhOption_DTO>> LayDanhSachChuyenNganhAsync();
        Task<ThongTinHienTai_DTO?> LayThongTinHienTaiAsync(int maUser);
        Task<KiemTraXacThuc_ResponseDTO> KiemTraTrangThaiXacThucAsync(int maUser);
        Task<bool> PheDuyetMinhChungAsync(PheDuyetMinhChung_RequestDTO request);
        Task<List<MinhChungItem_ResponseDTO>> LayDanhSachMinhChungChoDuyetAsync();
    }
}

