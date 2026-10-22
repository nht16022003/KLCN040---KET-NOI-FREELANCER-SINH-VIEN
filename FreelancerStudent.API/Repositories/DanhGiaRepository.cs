using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class DanhGiaRepository : IDanhGiaRepository
    {
        private readonly ApplicationDBContext _context;

        public DanhGiaRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public Task<HopDong?> LayHopDongAsync(string maHD)
            => _context.HopDongs.AsNoTracking()
                .Include(h => h.JobPost).ThenInclude(j => j!.NhaTuyenDung).ThenInclude(n => n!.User)
                .Include(h => h.FreelancerStudent).ThenInclude(f => f!.User)
                .FirstOrDefaultAsync(h => h.maHD == maHD);

        public Task<DanhGia_NhanXet?> LayTheoHopDongNguoiDanhGiaAsync(string maHD, int maUser)
            => _context.DanhGia_NhanXets.AsNoTracking()
                .FirstOrDefaultAsync(d => d.maHD == maHD && d.maNguoiDanhGia == maUser);

        public Task<DanhGia_NhanXet?> LayTheoMaAsync(int maDanhGia)
            => _context.DanhGia_NhanXets
                .Include(d => d.HopDong).ThenInclude(h => h!.JobPost).ThenInclude(j => j!.NhaTuyenDung).ThenInclude(n => n!.User)
                .Include(d => d.HopDong).ThenInclude(h => h!.FreelancerStudent).ThenInclude(f => f!.User)
                .FirstOrDefaultAsync(d => d.maDanhGia == maDanhGia);

        public IQueryable<DanhGia_NhanXet> TruyVanDanhGiaNguoiDuocDanhGia(int maUser)
            => _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => d.maNguoiDuocDanhGia == maUser)
                .Include(d => d.NguoiDanhGia).ThenInclude(u => u!.NhaTuyenDung)
                .Include(d => d.NguoiDanhGia).ThenInclude(u => u!.FreelancerStudents)
                .Include(d => d.NguoiDanhGia).ThenInclude(u => u!.FreelancerStudents).ThenInclude(f => f!.User)
                .Include(d => d.HopDong).ThenInclude(h => h!.JobPost);

        public async Task CapNhatDiemDanhGiaAsync(int maUser)
        {
            var sao = await _context.DanhGia_NhanXets
                .Where(d => d.maNguoiDuocDanhGia == maUser)
                .Select(d => d.soSao)
                .ToListAsync();
            var diem = sao.Count == 0 ? 0m : Math.Round((decimal)sao.Average(), 1);

            var freelancer = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maUser == maUser);
            if (freelancer != null)
            {
                freelancer.diemDanhGiaTrungBinh = diem;
                freelancer.soLuotDanhGia = sao.Count;
            }

            var nhaTuyenDung = await _context.NhaTuyenDungs.FirstOrDefaultAsync(n => n.maUser == maUser);
            if (nhaTuyenDung != null)
            {
                nhaTuyenDung.sosaodanhgia = sao.Count == 0 ? 0 : (double)diem;
                nhaTuyenDung.soLuotDanhGia = sao.Count;
            }
        }
    }
}
