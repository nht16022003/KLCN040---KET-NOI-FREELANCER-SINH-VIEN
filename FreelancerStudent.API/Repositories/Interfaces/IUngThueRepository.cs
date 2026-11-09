using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IUngThueRepository
    {
        Task<UngThue> TaoYeuCauThueAsync(UngThue ungThue);
        Task<UngThue?> LayTheoMaAsync(int maYeuCau);
        Task<List<UngThue>> LayDanhSachTheoFreelancerAsync(int maFreelancerStudent);
        Task<List<UngThue>> LayDanhSachTheoNTDAsync(int maNhaTuyenDung);
        Task<bool> KiemTraYeuCauTrungAsync(int maNhaTuyenDung, int maFreelancerStudent, string? maJob);
        Task<bool> CapNhatAsync(UngThue ungThue);
    }
}

