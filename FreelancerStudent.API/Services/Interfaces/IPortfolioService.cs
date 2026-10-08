using FreelancerStudent.API.DTOs.ReponsesDTO;

namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IPortfolioService
    {
        Task<Portfolio_ReponseDTO?> layPortfolioAsync(int maFreelancerStudents);
        Task<Portfolio_ReponseDTO?> themDuAnAsync(DuAnTrongPortfolio_RequestDTO request);
        Task<Portfolio_ReponseDTO?> suaDuAnAsync(DuAnTrongPortfolio_UpdateRequestDTO request);
        Task<Portfolio_ReponseDTO?> xoaDuAnAsync(string maDA, int maFreelancerStudents);
        Task<Portfolio_ReponseDTO?> capNhatDuAnNoiBatAsync(DuAnNoiBat_RequestDTO request);
    }
}