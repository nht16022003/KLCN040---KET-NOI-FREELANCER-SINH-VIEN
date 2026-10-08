using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;

        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public Task<Portfolio?> layTheoMaFreelancerAsync(int maFreelancerStudents)
        {
            return _context.Portfolios
                .FirstOrDefaultAsync(x => x.maFreelancerStudents == maFreelancerStudents);
        }

        public async Task<Portfolio> taoPortfolioAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public Task<List<DuAnTrongPortfolio>> layDuAnAsync(string maPortfolio)
        {
            return _context.DuAnTrongPortfolios
                .Where(x => x.maPortfolio == maPortfolio)
                .ToListAsync();
        }

        public async Task<DuAnTrongPortfolio> themDuAnAsync(DuAnTrongPortfolio project)
        {
            await _context.DuAnTrongPortfolios.AddAsync(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public Task<DuAnTrongPortfolio?> layDuAnTheoMaAsync(string maDA)
        {
            return _context.DuAnTrongPortfolios.FirstOrDefaultAsync(x => x.maDA == maDA);
        }

        public async Task capNhatDuAnAsync(DuAnTrongPortfolio project)
        {
            _context.DuAnTrongPortfolios.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task xoaDuAnAsync(DuAnTrongPortfolio project)
        {
            _context.DuAnTrongPortfolios.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task capNhatDuAnNoiBatAsync(string maPortfolio, IReadOnlyCollection<string> maDAs)
        {
            var projects = await _context.DuAnTrongPortfolios
                .Where(x => x.maPortfolio == maPortfolio)
                .ToListAsync();
            var selected = maDAs.ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var project in projects)
            {
                project.laDuAnNoiBat = selected.Contains(project.maDA);
            }
            await _context.SaveChangesAsync();
        }
    }
}