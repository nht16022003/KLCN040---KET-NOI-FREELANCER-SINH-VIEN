using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface INhaTuyenDungService
    {
        //Lấy danh sách nhà tuyển dụng
        Task<List<NhaTuyenDung_ResponseDTO>> layDanhSachNhaTuyenDungAsync();

        //Lấy thông tin hồ sơ nhà tuyển dụng
        Task<NhaTuyenDungProfile_ReponseDTO?> layProfileNhaTuyenDung(int maNhaTuyenDung);
        Task<NhaTuyenDungProfile_ReponseDTO?> layProfileNhaTuyenDungTheoMaUser(int maUser);

        //Cập nhật thông tin cơ bản Nhà tuyển dụng
        Task<bool> capNhatThongTinNTDAsync(CapNhatThongTinNhaTuyenDung_RequestDTO dto);

        //Cập nhật giới thiệu đơn vị
        Task<bool> capNhatGioiThieuNTDAsync(CapNhatGioiThieuNhaTuyenDung_RequestDTO dto);

        //Cập nhật logo doanh nghiệp
        Task<bool> capNhatLogoNTDAsync(int maUser, string logoUrl);
    }
}