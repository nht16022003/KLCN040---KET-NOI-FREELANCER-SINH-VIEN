
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
                    .ThenInclude(j => j.NhaTuyenDung)
                        .ThenInclude(n => n.User)
                .Include(h => h.FreelancerStudent)
                    .ThenInclude(f => f.User)
                .Where(h =>
                    h.JobPost != null &&
                    h.JobPost.maNhaTuyenDung == maNhaTuyenDung);

            // Tìm kiếm theo mã hợp đồng hoặc tên công việc hoặc tên freelancer
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                string keyword = tuKhoa.Trim();

                query = query.Where(h =>
                    h.maHD.Contains(keyword) ||
                    (h.JobPost != null && h.JobPost.tieude.Contains(keyword)) ||
                    (h.FreelancerStudent != null && h.FreelancerStudent.User != null && h.FreelancerStudent.User.hotenUser.Contains(keyword)));
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

        public async Task<List<HopDong>> GetDanhSachHopDongTheoFreelancerAsync(
            int maFreelancerStudent,
            string? tuKhoa,
            string? trangThai)
        {
            // Lấy hợp đồng thuộc FreelancerStudent
            var query = _context.HopDongs
                .AsNoTracking()
                .Include(h => h.JobPost)
                    .ThenInclude(j => j.NhaTuyenDung)
                        .ThenInclude(n => n.User)
                .Include(h => h.FreelancerStudent)
                    .ThenInclude(f => f.User)
                .Where(h => h.maFreelancerStudent == maFreelancerStudent);

            // Tìm kiếm theo mã hợp đồng, tên công việc, hoặc tên nhà tuyển dụng
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                string keyword = tuKhoa.Trim();

                query = query.Where(h =>
                    h.maHD.Contains(keyword) ||
                    (h.JobPost != null && h.JobPost.tieude.Contains(keyword)) ||
                    (h.JobPost != null && h.JobPost.NhaTuyenDung != null && (
                        (!string.IsNullOrEmpty(h.JobPost.NhaTuyenDung.tencongty) && h.JobPost.NhaTuyenDung.tencongty.Contains(keyword)) ||
                        (h.JobPost.NhaTuyenDung.User != null && h.JobPost.NhaTuyenDung.User.hotenUser.Contains(keyword))
                    )));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                string status = trangThai.Trim();

                query = query.Where(h =>
                    h.trangThai == status);
            }

            return await query
                .OrderByDescending(h => h.ngayBatDau)
                .ToListAsync();
        }

        public async Task<HopDong> TaoHopDongAsync(
            HopDong hopDong,
            List<GiaiDoan_HopDong>? giaiDoans,
            int? maUngTuyen,
            ThongBao? thongBao)
        {
            await _context.HopDongs.AddAsync(hopDong);

            if (giaiDoans != null && giaiDoans.Any())
            {
                foreach (var gd in giaiDoans)
                {
                    gd.maHD = hopDong.maHD;
                    await _context.GiaiDoan_HopDongs.AddAsync(gd);
                }
            }

            if (maUngTuyen.HasValue && maUngTuyen.Value > 0)
            {
                var ungTuyen = await _context.UngTuyens.FirstOrDefaultAsync(u => u.maUngTuyen == maUngTuyen.Value);
                if (ungTuyen != null)
                {
                    ungTuyen.trangThaiUngTuyen = "ChapNhan";
                }
            }

            if (thongBao != null)
            {
                await _context.ThongBaos.AddAsync(thongBao);
            }

            await _context.SaveChangesAsync();

            return await LayChiTietHopDongTheoMaAsync(hopDong.maHD) ?? hopDong;
        }

        public async Task<HopDong?> LayChiTietHopDongTheoMaAsync(string maHD)
        {
            return await _context.HopDongs
                .AsNoTracking()
                .Include(h => h.JobPost)
                    .ThenInclude(j => j.NhaTuyenDung)
                        .ThenInclude(n => n.User)
                .Include(h => h.FreelancerStudent)
                    .ThenInclude(f => f.User)
                .Include(h => h.GiaiDoan_HopDongs)
                .FirstOrDefaultAsync(h => h.maHD == maHD);
        }

        public async Task<HopDong?> LayHopDongTheoMaJobAsync(string maJob)
        {
            return await _context.HopDongs
                .AsNoTracking()
                .Include(h => h.JobPost)
                    .ThenInclude(j => j.NhaTuyenDung)
                        .ThenInclude(n => n.User)
                .Include(h => h.FreelancerStudent)
                    .ThenInclude(f => f.User)
                .Include(h => h.GiaiDoan_HopDongs)
                .FirstOrDefaultAsync(h => h.maJob == maJob);
        }

        public async Task<JobPost?> LayJobPostTheoMaAsync(string maJob)
        {
            return await _context.JobPosts
                .Include(j => j.NhaTuyenDung)
                    .ThenInclude(n => n.User)
                .FirstOrDefaultAsync(j => j.maJob == maJob);
        }

        public async Task<FreelancerStudents?> LayFreelancerTheoMaAsync(int maFreelancerStudent)
        {
            return await _context.FreelancerStudents
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.maFreelancerStudents == maFreelancerStudent);
        }

        public async Task<bool> CapNhatTrangThaiHopDongAsync(string maHD, string trangThaiMoi, ThongBao? thongBao)
        {
            var hopDong = await _context.HopDongs.FirstOrDefaultAsync(h => h.maHD == maHD);
            if (hopDong == null) return false;

            hopDong.trangThai = trangThaiMoi;

            if (thongBao != null)
            {
                await _context.ThongBaos.AddAsync(thongBao);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}