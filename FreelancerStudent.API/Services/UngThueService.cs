using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class UngThueService : IUngThueService
    {
        private readonly IUngThueRepository _ungThueRepo;
        private readonly ApplicationDBContext _context;

        public UngThueService(IUngThueRepository ungThueRepo, ApplicationDBContext context)
        {
            _ungThueRepo = ungThueRepo;
            _context = context;
        }

        public async Task<YeuCauThue_ReponseDTO> GuiYeuCauThueAsync(GuiYeuCauThue_RequestDTO request)
        {
            // 1. Xác thực Nhà tuyển dụng
            var ntd = await _context.NhaTuyenDungs.Include(n => n.User)
                .FirstOrDefaultAsync(n => n.maUser == request.maUserNTD);
            if (ntd == null)
            {
                throw new Exception("Tài khoản của bạn không phải là Nhà tuyển dụng hợp lệ.");
            }

            // 2. Xác thực Freelancer
            var freelancer = await _context.FreelancerStudents.Include(f => f.User)
                .FirstOrDefaultAsync(f => f.maFreelancerStudents == request.maFreelancerStudent || f.maUser == request.maFreelancerStudent);
            if (freelancer == null)
            {
                throw new Exception("Không tìm thấy hồ sơ Freelancer được chọn.");
            }

            // 3. Không cho phép tự thuê chính mình
            if (freelancer.maUser == request.maUserNTD)
            {
                throw new Exception("Bạn không thể tự gửi yêu cầu thuê chính tài khoản của mình.");
            }

            // 4. Nếu có maJob: Kiểm tra quyền sở hữu JobPost
            if (!string.IsNullOrEmpty(request.maJob))
            {
                var job = await _context.JobPosts.FirstOrDefaultAsync(j => j.maJob == request.maJob);
                if (job == null)
                {
                    throw new Exception("Không tìm thấy bài đăng tuyển dụng được chọn.");
                }
                if (job.maNhaTuyenDung != ntd.maNhaTuyenDung)
                {
                    throw new Exception("Bài đăng tuyển dụng được chọn không thuộc quyền sở hữu của bạn.");
                }
                if (string.Equals(job.status, "DaDong", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(job.status, "DaHuy", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Bài đăng tuyển dụng này đã đóng hoặc bị hủy, không thể thuê thêm.");
                }

                // Điền thông tin mặc định nếu người dùng không nhập
                if (string.IsNullOrWhiteSpace(request.tieuDeCongViec))
                {
                    request.tieuDeCongViec = job.tieude;
                }
                if (string.IsNullOrWhiteSpace(request.moTaCongViec))
                {
                    request.moTaCongViec = job.mota;
                }
                if (!request.nganSachDeNghi.HasValue && job.thulao.HasValue)
                {
                    request.nganSachDeNghi = job.thulao;
                }
            }

            // 5. Kiểm tra trùng lặp
            bool daTrung = await _ungThueRepo.KiemTraYeuCauTrungAsync(ntd.maNhaTuyenDung, freelancer.maFreelancerStudents, request.maJob);
            if (daTrung)
            {
                throw new Exception("Bạn đã gửi một yêu cầu thuê đang chờ phản hồi hoặc đã được chấp nhận cho công việc này.");
            }

            // 6. Tạo bản ghi UngThue
            var entity = new UngThue
            {
                maNhaTuyenDung = ntd.maNhaTuyenDung,
                maFreelancerStudent = freelancer.maFreelancerStudents,
                maJob = request.maJob,
                tieuDeCongViec = request.tieuDeCongViec,
                moTaCongViec = request.moTaCongViec,
                nganSachDeNghi = request.nganSachDeNghi,
                thoiHanDuKien = request.thoiHanDuKien,
                ngayGui = DateTime.Now,
                trangThai = "ChoPhanHoi"
            };

            var saved = await _ungThueRepo.TaoYeuCauThueAsync(entity);
            return ChuyenSangDTO(saved);
        }

        public async Task<List<YeuCauThue_ReponseDTO>> LayDanhSachTheoFreelancerAsync(int maUserFreelancer)
        {
            var freelancer = await _context.FreelancerStudents
                .FirstOrDefaultAsync(f => f.maUser == maUserFreelancer);
            if (freelancer == null)
            {
                return new List<YeuCauThue_ReponseDTO>();
            }

            var ds = await _ungThueRepo.LayDanhSachTheoFreelancerAsync(freelancer.maFreelancerStudents);
            return ds.Select(ChuyenSangDTO).ToList();
        }

        public async Task<List<YeuCauThue_ReponseDTO>> LayDanhSachTheoNTDAsync(int maUserNTD)
        {
            var ntd = await _context.NhaTuyenDungs
                .FirstOrDefaultAsync(n => n.maUser == maUserNTD);
            if (ntd == null)
            {
                return new List<YeuCauThue_ReponseDTO>();
            }

            var ds = await _ungThueRepo.LayDanhSachTheoNTDAsync(ntd.maNhaTuyenDung);
            return ds.Select(ChuyenSangDTO).ToList();
        }

        public async Task<YeuCauThue_ReponseDTO> PhanHoiYeuCauThueAsync(PhanHoiYeuCauThue_RequestDTO request)
        {
            var yeuCau = await _ungThueRepo.LayTheoMaAsync(request.maYeuCau);
            if (yeuCau == null)
            {
                throw new Exception("Không tìm thấy yêu cầu thuê.");
            }

            if (yeuCau.FreelancerStudent?.maUser != request.maUserFreelancer)
            {
                throw new Exception("Bạn không có quyền phản hồi yêu cầu thuê này.");
            }

            if (yeuCau.trangThai != "ChoPhanHoi")
            {
                throw new Exception($"Yêu cầu thuê này đã ở trạng thái '{yeuCau.trangThai}', không thể thay đổi.");
            }

            if (request.chapNhan)
            {
                yeuCau.trangThai = "DaChapNhan";
                yeuCau.lyDoTuChoi = null;
            }
            else
            {
                yeuCau.trangThai = "DaTuChoi";
                yeuCau.lyDoTuChoi = request.lyDoTuChoi;
            }

            await _ungThueRepo.CapNhatAsync(yeuCau);
            return ChuyenSangDTO(yeuCau);
        }

        private YeuCauThue_ReponseDTO ChuyenSangDTO(UngThue u)
        {
            return new YeuCauThue_ReponseDTO
            {
                maYeuCau = u.maYeuCau,
                maNhaTuyenDung = u.maNhaTuyenDung,
                maUserNTD = u.NhaTuyenDung?.maUser ?? 0,
                tenNhaTuyenDung = u.NhaTuyenDung?.tencongty ?? u.NhaTuyenDung?.User?.hotenUser ?? "Nhà tuyển dụng",
                avatarNhaTuyenDung = u.NhaTuyenDung?.User?.avatarUrl,
                maFreelancerStudent = u.maFreelancerStudent,
                maUserFreelancer = u.FreelancerStudent?.maUser ?? 0,
                tenFreelancer = u.FreelancerStudent?.User?.hotenUser ?? "Sinh viên Freelancer",
                avatarFreelancer = u.FreelancerStudent?.User?.avatarUrl,
                maJob = u.maJob,
                tieuDeJob = u.JobPost?.tieude,
                tieuDeCongViec = u.tieuDeCongViec,
                moTaCongViec = u.moTaCongViec,
                nganSachDeNghi = u.nganSachDeNghi,
                thoiHanDuKien = u.thoiHanDuKien,
                ngayGui = u.ngayGui,
                trangThai = u.trangThai,
                lyDoTuChoi = u.lyDoTuChoi
            };
        }
    }
}
