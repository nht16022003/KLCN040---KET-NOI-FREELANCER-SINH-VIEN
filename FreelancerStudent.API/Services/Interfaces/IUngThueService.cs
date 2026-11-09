using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IUngThueService
    {
        Task<YeuCauThue_ReponseDTO> GuiYeuCauThueAsync(GuiYeuCauThue_RequestDTO request);
        Task<List<YeuCauThue_ReponseDTO>> LayDanhSachTheoFreelancerAsync(int maUserFreelancer);
        Task<List<YeuCauThue_ReponseDTO>> LayDanhSachTheoNTDAsync(int maUserNTD);
        Task<YeuCauThue_ReponseDTO> PhanHoiYeuCauThueAsync(PhanHoiYeuCauThue_RequestDTO request);
    }
}

