using FreelancerStudent.Web.ViewModels;
using FreelancerStudent.Web.ViewModels.Chat;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IChatWebService
    {
        Task<ApiReponse<PhongChatViewModel>> TaoHoacLayPhongChatAsync(int maUserClient, int maFreelancerStudent, string? maJob);
        Task<ApiReponse<List<PhongChatViewModel>>> LayDanhSachPhongChatAsync(int maUser);
        Task<ApiReponse<List<TinNhanViewModel>>> LayLichSuTinNhanAsync(int maPhongChat, int maUser);
        Task<ApiReponse<TinNhanViewModel>> GuiTinNhanAsync(int maPhongChat, int maNguoiGui, string noiDung, string? fileDinhKem);
    }
}
