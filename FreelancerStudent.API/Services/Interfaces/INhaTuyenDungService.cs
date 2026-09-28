using FreelancerStudent.API.DTOs.ReponsesDTO;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface INhaTuyenDungService
    {
        //Lấy danh sách nhà tuyển dụng
        Task<List<NhaTuyenDung_ResponseDTO>> layDanhSachNhaTuyenDungAsync();
    }
}