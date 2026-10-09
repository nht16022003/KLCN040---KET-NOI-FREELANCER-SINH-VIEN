using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _repository;

        public PortfolioService(IPortfolioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Portfolio_ReponseDTO?> layPortfolioAsync(int maFreelancerStudents)
        {
            var portfolio = await _repository.layTheoMaFreelancerAsync(maFreelancerStudents);
            if (portfolio == null)
            {
                return null;
            }

            var result = Map(portfolio);
            result.projects = (await _repository.layDuAnAsync(portfolio.maPortfolio))
                .Select(MapProject)
                .ToList();
            return result;
        }

        public async Task<Portfolio_ReponseDTO?> capNhatPortfolioAsync(Portfolio_UpdateRequestDTO request)
        {
            var portfolio = await _repository.layTheoMaFreelancerAsync(request.maFreelancerStudents);
            if (portfolio == null) return null;

            portfolio.moTaBanThan = request.moTaBanThan;
            portfolio.url_video = request.url_video;
            await _repository.capNhatPortfolioAsync(portfolio);

            return await layPortfolioAsync(request.maFreelancerStudents);
        }

        public async Task<Portfolio_ReponseDTO?> themDuAnAsync(DuAnTrongPortfolio_RequestDTO request)
        {
            var portfolio = await _repository.layTheoMaFreelancerAsync(request.maFreelancerStudents);
            if (portfolio == null)
            {
                portfolio = await _repository.taoPortfolioAsync(new Portfolio
                {
                    maPortfolio = $"PF{Guid.NewGuid():N}"[..20],
                    maFreelancerStudents = request.maFreelancerStudents
                });
            }

            await _repository.themDuAnAsync(new DuAnTrongPortfolio
            {
                maDA = $"DA{Guid.NewGuid():N}"[..20],
                maPortfolio = portfolio.maPortfolio,
                tenDuAn = request.tenDuAn,
                vaiTro = request.vaiTro,
                moTa = request.moTa,
                congnghe = request.congnghe,
                linkGithub = request.linkGithub,
                linkDemo = request.linkDemo,
                link_file = request.link_file
            });

            return await layPortfolioAsync(request.maFreelancerStudents);
        }

        public async Task<Portfolio_ReponseDTO?> suaDuAnAsync(DuAnTrongPortfolio_UpdateRequestDTO request)
        {
            var project = await _repository.layDuAnTheoMaAsync(request.maDA);
            if (project == null) return null;

            var portfolio = await _repository.layTheoMaFreelancerAsync(request.maFreelancerStudents);
            if (portfolio == null || project.maPortfolio != portfolio.maPortfolio) return null;

            project.tenDuAn = request.tenDuAn;
            project.vaiTro = request.vaiTro;
            project.moTa = request.moTa;
            project.congnghe = request.congnghe;
            project.linkGithub = request.linkGithub;
            project.linkDemo = request.linkDemo;
            project.link_file = request.link_file;
            await _repository.capNhatDuAnAsync(project);
            return await layPortfolioAsync(request.maFreelancerStudents);
        }

        public async Task<Portfolio_ReponseDTO?> xoaDuAnAsync(string maDA, int maFreelancerStudents)
        {
            var project = await _repository.layDuAnTheoMaAsync(maDA);
            var portfolio = await _repository.layTheoMaFreelancerAsync(maFreelancerStudents);
            if (project == null || portfolio == null || project.maPortfolio != portfolio.maPortfolio) return null;

            await _repository.xoaDuAnAsync(project);
            return await layPortfolioAsync(maFreelancerStudents);
        }

        public async Task<Portfolio_ReponseDTO?> capNhatDuAnNoiBatAsync(DuAnNoiBat_RequestDTO request)
        {
            var portfolio = await _repository.layTheoMaFreelancerAsync(request.maFreelancerStudents);
            if (portfolio == null) return null;

            await _repository.capNhatDuAnNoiBatAsync(portfolio.maPortfolio, request.maDAs);
            return await layPortfolioAsync(request.maFreelancerStudents);
        }

        private static Portfolio_ReponseDTO Map(Portfolio portfolio)
        {
            return new Portfolio_ReponseDTO
            {
                maPortfolio = portfolio.maPortfolio,
                maFreelancerStudents = portfolio.maFreelancerStudents,
                moTaBanThan = portfolio.moTaBanThan,
                url_video = portfolio.url_video,
                projects = new List<DuAnTrongPortfolio_ReponseDTO>()
            };
        }

        private static DuAnTrongPortfolio_ReponseDTO MapProject(DuAnTrongPortfolio project)
        {
            return new DuAnTrongPortfolio_ReponseDTO
            {
                maDA = project.maDA,
                maPortfolio = project.maPortfolio,
                tenDuAn = project.tenDuAn,
                vaiTro = project.vaiTro,
                moTa = project.moTa,
                congnghe = project.congnghe,
                linkGithub = project.linkGithub,
                linkDemo = project.linkDemo,
                link_file = project.link_file,
                laDuAnNoiBat = project.laDuAnNoiBat
            };
        }
    }
}