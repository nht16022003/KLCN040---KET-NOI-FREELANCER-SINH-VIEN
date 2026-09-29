using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IWalletService
    {
        //Lấy ví theo mã Users
        Task<Wallet_ReponseDTO> layViTheoMaUsersAsync(Wallet_RequestDTO request);
    }
}