
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class YeuCauChinhSuaRepository
    {
        private readonly ApplicationDBContext _context;

        public YeuCauChinhSuaRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Lấy yêu cầu chỉnh sửa theo lần bàn giao
        public async Task<List<YeuCauChinhSua>>
            GetByMaBanGiaoAsync(int maBanGiao)
        {
            return await _context.YeuCauChinhSuas
                .AsNoTracking()
                .Where(x => x.maBanGiao == maBanGiao)
                .OrderByDescending(x => x.ngayYeuCau)
                .ToListAsync();
        }

        // Lấy yêu cầu chỉnh sửa theo mã
        public async Task<YeuCauChinhSua?>
            GetByIdAsync(int maYeuCau)
        {
            return await _context.YeuCauChinhSuas
                .FirstOrDefaultAsync(
                    x => x.maYeuCauChinhSua == maYeuCau);
        }

        // Lấy tất cả yêu cầu theo hợp đồng
        public async Task<List<YeuCauChinhSua>>
            GetByMaHDAsync(string maHD)
        {
            return await _context.YeuCauChinhSuas
                .AsNoTracking()
                .Include(x => x.BanGiaoSanPham)
                .Where(x =>
                    x.BanGiaoSanPham != null &&
                    x.BanGiaoSanPham.maHD == maHD)
                .OrderByDescending(x => x.ngayYeuCau)
                .ToListAsync();
        }

        // Thêm yêu cầu chỉnh sửa
        public async Task AddAsync(YeuCauChinhSua entity)
        {
            await _context.YeuCauChinhSuas.AddAsync(entity);
        }

        // Lưu thay đổi
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
