using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> layTheoMaFreelancerAsync(int maFreelancerStudents);
        Task<Portfolio> taoPortfolioAsync(Portfolio portfolio);
        Task<List<DuAnTrongPortfolio>> layDuAnAsync(string maPortfolio);
        Task<DuAnTrongPortfolio> themDuAnAsync(DuAnTrongPortfolio project);
        Task<DuAnTrongPortfolio?> layDuAnTheoMaAsync(string maDA);
        Task capNhatDuAnAsync(DuAnTrongPortfolio project);
        Task xoaDuAnAsync(DuAnTrongPortfolio project);
        Task capNhatDuAnNoiBatAsync(string maPortfolio, IReadOnlyCollection<string> maDAs);
    }
}