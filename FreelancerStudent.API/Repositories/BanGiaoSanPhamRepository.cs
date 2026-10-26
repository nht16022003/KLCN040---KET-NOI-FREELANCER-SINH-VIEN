
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class BanGiaoSanPhamRepository
    {
        private readonly ApplicationDBContext _context;

        public BanGiaoSanPhamRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Lấy danh sách bàn giao theo hợp đồng
        public async Task<List<BanGiao_SanPham>>
            GetByMaHDAsync(string maHD)
        {
            return await _context.BanGiao_SanPhams
                .AsNoTracking()
                .Where(x => x.maHD == maHD)
                .OrderByDescending(x => x.ngayNop)
                .ThenByDescending(x => x.maBanGiao)
                .ToListAsync();
        }

        // Lấy một lần bàn giao theo mã
        public async Task<BanGiao_SanPham?>
            GetByIdAsync(int maBanGiao)
        {
            return await _context.BanGiao_SanPhams
                .FirstOrDefaultAsync(
                    x => x.maBanGiao == maBanGiao);
        }

        // Lấy lần bàn giao mới nhất
        public async Task<BanGiao_SanPham?>
            GetLatestAsync(string maHD)
        {
            return await _context.BanGiao_SanPhams
                .AsNoTracking()
                .Where(x => x.maHD == maHD)
                .OrderByDescending(x => x.maBanGiao)
                .FirstOrDefaultAsync();
        }

        // Kiểm tra hợp đồng tồn tại
        public async Task<HopDong?>
            GetHopDongAsync(string maHD)
        {
            return await _context.HopDongs
                .Include(x => x.JobPost)
                .FirstOrDefaultAsync(x => x.maHD == maHD);
        }

        // Thêm phiên bản bàn giao
        public async Task AddAsync(BanGiao_SanPham entity)
        {
            await _context.BanGiao_SanPhams.AddAsync(entity);
        }

        // Lưu thay đổi
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
