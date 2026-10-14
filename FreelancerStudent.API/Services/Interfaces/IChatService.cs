using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IChatService
    {
        Task<PhongChat_ReponseDTO> TaoHoacLayPhongChatAsync(TaoPhongChat_RequestDTO request);
        Task<List<PhongChat_ReponseDTO>> LayDanhSachPhongChatAsync(int maUser);
        Task<List<TinNhan_ReponseDTO>> LayLichSuTinNhanAsync(int maPhongChat, int maUserDangXem);
        Task<TinNhan_ReponseDTO> GuiTinNhanAsync(GuiTinNhan_RequestDTO request);
    }
}
