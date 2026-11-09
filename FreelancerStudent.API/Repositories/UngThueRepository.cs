using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class UngThueRepository : IUngThueRepository
    {
        private readonly ApplicationDBContext _context;

        public UngThueRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<UngThue> TaoYeuCauThueAsync(UngThue ungThue)
        {
            await _context.UngThues.AddAsync(ungThue);
            await _context.SaveChangesAsync();
            return await LayTheoMaAsync(ungThue.maYeuCau) ?? ungThue;
        }

        public async Task<UngThue?> LayTheoMaAsync(int maYeuCau)
        {
            return await _context.UngThues
                .Include(u => u.NhaTuyenDung).ThenInclude(n => n!.User)
                .Include(u => u.FreelancerStudent).ThenInclude(f => f!.User)
                .Include(u => u.JobPost)
                .FirstOrDefaultAsync(u => u.maYeuCau == maYeuCau);
        }

        public async Task<List<UngThue>> LayDanhSachTheoFreelancerAsync(int maFreelancerStudent)
        {
            return await _context.UngThues
                .Include(u => u.NhaTuyenDung).ThenInclude(n => n!.User)
                .Include(u => u.FreelancerStudent).ThenInclude(f => f!.User)
                .Include(u => u.JobPost)
                .Where(u => u.maFreelancerStudent == maFreelancerStudent)
                .OrderByDescending(u => u.ngayGui)
                .ToListAsync();
        }

        public async Task<List<UngThue>> LayDanhSachTheoNTDAsync(int maNhaTuyenDung)
        {
            return await _context.UngThues
                .Include(u => u.NhaTuyenDung).ThenInclude(n => n!.User)
                .Include(u => u.FreelancerStudent).ThenInclude(f => f!.User)
                .Include(u => u.JobPost)
                .Where(u => u.maNhaTuyenDung == maNhaTuyenDung)
                .OrderByDescending(u => u.ngayGui)
                .ToListAsync();
        }

        public async Task<bool> KiemTraYeuCauTrungAsync(int maNhaTuyenDung, int maFreelancerStudent, string? maJob)
        {
            // Kiểm tra xem đã có yêu cầu thuê đang chờ xử lý hoặc đã chấp nhận với cùng Job và Freelancer này chưa
            return await _context.UngThues.AnyAsync(u =>
                u.maNhaTuyenDung == maNhaTuyenDung &&
                u.maFreelancerStudent == maFreelancerStudent &&
                (string.IsNullOrEmpty(maJob) ? u.maJob == null : u.maJob == maJob) &&
                (u.trangThai == "ChoPhanHoi" || u.trangThai == "DaChapNhan"));
        }

        public async Task<bool> CapNhatAsync(UngThue ungThue)
        {
            _context.UngThues.Update(ungThue);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

