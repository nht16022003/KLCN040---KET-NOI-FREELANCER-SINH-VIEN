
using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class HopDongRepository
    {
        private readonly ApplicationDBContext _context;

        public HopDongRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<HopDong>> GetDanhSachHopDongAsync(
            int maNhaTuyenDung,
            string? tuKhoa,
            string? trangThai)
        {
            // Lấy hợp đồng thuộc Nhà tuyển dụng
            var query = _context.HopDongs
                .AsNoTracking()
                .Include(h => h.JobPost)
                .Include(h => h.FreelancerStudent)
                .Where(h =>
                    h.JobPost != null &&
                    h.JobPost.maNhaTuyenDung == maNhaTuyenDung);

            // Tìm kiếm theo mã hợp đồng
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                string keyword = tuKhoa.Trim();

                query = query.Where(h =>
                    h.maHD.Contains(keyword));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                string status = trangThai.Trim();

                query = query.Where(h =>
                    h.trangThai == status);
            }

            // Trả về danh sách hợp đồng
            return await query
                .OrderByDescending(h => h.ngayBatDau)
                .ToListAsync();
        }
    }
}
