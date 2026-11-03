using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.DTOs.RequestDTO;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class YeuThichService : IYeuThichService
    {
        private readonly ApplicationDBContext _context;

        public YeuThichService(ApplicationDBContext context)
        {
            _context = context;
        }

        #region Nhóm Freelancer lưu việc

        public async Task<bool> LuuViecAsync(LuuViec_RequestDTO request)
        {
            var fs = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maUser == request.maUser);
            if (fs == null)
            {
                throw new Exception("Chỉ tài khoản Freelancer sinh viên mới có quyền lưu công việc!");
            }

            var job = await _context.JobPosts.FirstOrDefaultAsync(j => j.maJob == request.maJob);
            if (job == null)
            {
                throw new Exception("Công việc không tồn tại trong hệ thống!");
            }

            var daLuu = await _context.LuuViec_ChoFreelancers
                .AnyAsync(x => x.maFreelancerStudent == fs.maFreelancerStudents && x.maJob == request.maJob);
            if (daLuu)
            {
                return true; // Đã lưu trước đó, không lưu trùng
            }

            var entity = new LuuViec_ChoFreelancer
            {
                maFreelancerStudent = fs.maFreelancerStudents,
                maJob = request.maJob,
                ngayLuu = DateTime.Now,
                ghiChu = request.ghiChu
            };

            _context.LuuViec_ChoFreelancers.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BoLuuViecAsync(int maUser, string maJob)
        {
            var fs = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maUser == maUser);
            if (fs == null)
            {
                throw new Exception("Không tìm thấy thông tin Freelancer sinh viên!");
            }

            var entity = await _context.LuuViec_ChoFreelancers
                .FirstOrDefaultAsync(x => x.maFreelancerStudent == fs.maFreelancerStudents && x.maJob == maJob);
            if (entity != null)
            {
                _context.LuuViec_ChoFreelancers.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> KiemTraLuuViecAsync(int maUser, string maJob)
        {
            var fs = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maUser == maUser);
            if (fs == null) return false;

            return await _context.LuuViec_ChoFreelancers
                .AnyAsync(x => x.maFreelancerStudent == fs.maFreelancerStudents && x.maJob == maJob);
        }

        public async Task<List<CongViecDaLuu_ResponseDTO>> LayDanhSachViecDaLuuAsync(int maUser)
        {
            var fs = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maUser == maUser);
            if (fs == null)
            {
                return new List<CongViecDaLuu_ResponseDTO>();
            }

            var listSaved = await _context.LuuViec_ChoFreelancers
                .Where(x => x.maFreelancerStudent == fs.maFreelancerStudents)
                .Include(x => x.JobPost)
                    .ThenInclude(j => j!.NhaTuyenDung)
                        .ThenInclude(n => n!.User)
                .OrderByDescending(x => x.ngayLuu)
                .ToListAsync();

            var appliedJobIds = await _context.UngTuyens
                .Where(u => u.maFreelancerStudent == fs.maFreelancerStudents)
                .Select(u => u.maJob)
                .ToListAsync();

            var result = new List<CongViecDaLuu_ResponseDTO>();
            foreach (var item in listSaved)
            {
                if (item.JobPost == null) continue;
                var j = item.JobPost;
                var ntd = j.NhaTuyenDung;
                var ntdUser = ntd?.User;

                result.Add(new CongViecDaLuu_ResponseDTO
                {
                    maLuuViec = item.maLuuViec,
                    maJob = j.maJob,
                    tieuDe = j.tieude,
                    moTa = j.mota,
                    kyNangYeuCau = j.kynangyeucau,
                    thuLao = j.thulao,
                    thoiGianDangTuyen = j.thoigiandangtuyen,
                    thoiGianDuKienHoanThanh = j.thoigiandukienhoanthanh,
                    status = j.status ?? "DangTuyen",
                    soLuongTuyen = j.soluongtuyen,
                    maNhaTuyenDung = j.maNhaTuyenDung,
                    tenCongTy = !string.IsNullOrWhiteSpace(ntd?.tencongty) ? ntd.tencongty : (ntdUser?.hotenUser ?? "Nhà tuyển dụng"),
                    avatarNhaTuyenDung = ntdUser?.avatarUrl,
                    ngayLuu = item.ngayLuu,
                    ghiChu = item.ghiChu,
                    daUngTuyen = appliedJobIds.Contains(j.maJob)
                });
            }

            return result;
        }

        #endregion

        #region Nhóm Nhà tuyển dụng lưu Freelancer

        public async Task<bool> LuuFreelancerAsync(LuuFreelancer_RequestDTO request)
        {
            var ntd = await _context.NhaTuyenDungs.FirstOrDefaultAsync(n => n.maUser == request.maUser);
            if (ntd == null)
            {
                throw new Exception("Chỉ tài khoản Nhà tuyển dụng mới có quyền lưu Freelancer yêu thích!");
            }

            var fs = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maFreelancerStudents == request.maFreelancerStudent);
            if (fs == null)
            {
                throw new Exception("Freelancer sinh viên không tồn tại trong hệ thống!");
            }

            var daLuu = await _context.FreelancerYeuThichs
                .AnyAsync(x => x.maNhaTuyenDung == ntd.maNhaTuyenDung && x.maFreelancerStudent == request.maFreelancerStudent);
            if (daLuu)
            {
                return true; // Đã lưu trước đó
            }

            var entity = new FreelancerYeuThich
            {
                maNhaTuyenDung = ntd.maNhaTuyenDung,
                maFreelancerStudent = request.maFreelancerStudent,
                ngayLuu = DateTime.Now,
                ghiChu = request.ghiChu
            };

            _context.FreelancerYeuThichs.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BoLuuFreelancerAsync(int maUser, int maFreelancerStudent)
        {
            var ntd = await _context.NhaTuyenDungs.FirstOrDefaultAsync(n => n.maUser == maUser);
            if (ntd == null)
            {
                throw new Exception("Không tìm thấy thông tin Nhà tuyển dụng!");
            }

            var entity = await _context.FreelancerYeuThichs
                .FirstOrDefaultAsync(x => x.maNhaTuyenDung == ntd.maNhaTuyenDung && x.maFreelancerStudent == maFreelancerStudent);
            if (entity != null)
            {
                _context.FreelancerYeuThichs.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> KiemTraLuuFreelancerAsync(int maUser, int maFreelancerStudent)
        {
            var ntd = await _context.NhaTuyenDungs.FirstOrDefaultAsync(n => n.maUser == maUser);
            if (ntd == null) return false;

            return await _context.FreelancerYeuThichs
                .AnyAsync(x => x.maNhaTuyenDung == ntd.maNhaTuyenDung && x.maFreelancerStudent == maFreelancerStudent);
        }

        public async Task<List<FreelancerYeuThich_ResponseDTO>> LayDanhSachFreelancerYeuThichAsync(int maUser)
        {
            var ntd = await _context.NhaTuyenDungs.FirstOrDefaultAsync(n => n.maUser == maUser);
            if (ntd == null)
            {
                return new List<FreelancerYeuThich_ResponseDTO>();
            }

            var listSaved = await _context.FreelancerYeuThichs
                .Where(x => x.maNhaTuyenDung == ntd.maNhaTuyenDung)
                .Include(x => x.FreelancerStudent)
                    .ThenInclude(fs => fs!.User)
                .Include(x => x.FreelancerStudent)
                    .ThenInclude(fs => fs!.ChuyenNganh)
                .OrderByDescending(x => x.ngayLuu)
                .ToListAsync();

            var fsIds = listSaved.Select(x => x.maFreelancerStudent).ToList();

            var allSkills = await _context.FreelancerStudent_KyNangs
                .Where(k => fsIds.Contains(k.maFreelancerStudents))
                .Include(k => k.KyNang)
                .ToListAsync();

            var allReviews = await _context.DanhGia_NhanXets
                .Where(d => fsIds.Contains(d.maNguoiDuocDanhGia))
                .ToListAsync();

            var result = new List<FreelancerYeuThich_ResponseDTO>();
            foreach (var item in listSaved)
            {
                if (item.FreelancerStudent == null) continue;
                var fs = item.FreelancerStudent;
                var u = fs.User;

                var fsSkills = allSkills
                    .Where(k => k.maFreelancerStudents == fs.maFreelancerStudents && k.KyNang != null)
                    .Select(k => k.KyNang!.tenKyNang)
                    .ToList();

                var fsReviews = allReviews.Where(r => r.maNguoiDuocDanhGia == fs.maUser).ToList();
                double avgRating = fsReviews.Any() ? Math.Round(fsReviews.Average(r => (double)r.soSao), 1) : 0;

                result.Add(new FreelancerYeuThich_ResponseDTO
                {
                    maBookMark = item.maBookMark,
                    maFreelancerStudent = fs.maFreelancerStudents,
                    maUser = fs.maUser,
                    hoTen = u?.hotenUser ?? "Freelancer",
                    avatarUrl = u?.avatarUrl,
                    email = u?.emailUser,
                    tenChuyenNganh = fs.ChuyenNganh?.tenChuyenNganh ?? fs.maChuyenNganh,
                    tenTruong = fs.tenTruong,
                    namThu = fs.namThu,
                    gpa = fs.GPA,
                    diemDanhGiaTrungBinh = avgRating,
                    soLuotDanhGia = fsReviews.Count,
                    trangThaiNhanViec = fs.trangthaiNhanViec,
                    chiPhiTu = fs.chiPhiTu,
                    kyNangs = fsSkills,
                    ngayLuu = item.ngayLuu,
                    ghiChu = item.ghiChu
                });
            }

            return result;
        }

        #endregion
    }
}
