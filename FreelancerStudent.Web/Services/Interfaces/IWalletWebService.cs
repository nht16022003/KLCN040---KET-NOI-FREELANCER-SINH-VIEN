using FreelancerStudent.Web.ViewModels;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IWalletWebService
    {
        Task<ApiReponse<WalletViewModel>> layViTheoUser(int maUser);
    }
}