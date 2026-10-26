using FreelancerStudent.API.DTOs.ReponsesDTO;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface INhaTuyenDungService
    {
        //Lấy danh sách nhà tuyển dụng
        Task<List<NhaTuyenDung_ResponseDTO>> layDanhSachNhaTuyenDungAsync();

        //Lấy thông tin hồ sơ nhà tuyển dụng
        Task<NhaTuyenDungProfile_ReponseDTO?> layProfileNhaTuyenDung(int maNhaTuyenDung);
        Task<NhaTuyenDungProfile_ReponseDTO?> layProfileNhaTuyenDungTheoMaUser(int maUser);
    }
}