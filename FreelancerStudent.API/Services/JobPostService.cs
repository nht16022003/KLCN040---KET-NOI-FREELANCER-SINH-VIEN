using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponseDTO;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using FreelancerStudent.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class JobPostService : IJobPostService
    {
        private readonly IJobPostRepository _jopPostRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý
        private readonly INhaTuyenDungRepository _ntd;

        private readonly IUserRepository _user;

        private readonly IFreelancerStudentRepository _free;

        private readonly ApplicationDBContext _context;

        public JobPostService(
            IJobPostRepository nhaTuyenDungRepository,
            INhaTuyenDungRepository ntd,
            IUserRepository user,
            IFreelancerStudentRepository free,
            ApplicationDBContext context)
        {
            _jopPostRepository = nhaTuyenDungRepository;
            _ntd = ntd;
            _user = user;
            _free = free;
            _context = context;
        }


        //
        public async Task<List<JobPost_ReponseDTO>> layDanhSachJobPostAsync()
        {
            var layDS_JobSQL = await _jopPostRepository.layTatCaJobPostAsync(); //Đang trả về là Models
            var dsResult = layDS_JobSQL.Select(job => new JobPost_ReponseDTO
            {
                maJob = job.maJob,
                maNhaTuyenDung = job.maNhaTuyenDung,
                tieude = job.tieude,
                mota = job.mota,
                kynangyeucau = job.kynangyeucau,
                thulao = job.thulao,
                fileDinhKem = job.fileDinhKem,
                phiDangBai = job.phiDangBai,
                thoigiandangtuyen = job.thoigiandangtuyen,
                thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                status = job.status,
                soluongtuyen = job.soluongtuyen,
                tenCongTy = job.NhaTuyenDung?.tencongty ?? job.NhaTuyenDung?.User?.hotenUser ?? string.Empty,
                avatarNhaTuyenDung = job.NhaTuyenDung?.User?.avatarUrl ?? job.NhaTuyenDung?.logo ?? string.Empty
            }).ToList();

            return dsResult;
        }

        //XS
        public async Task<JobPostDetail_ReponsesDTO?> layChiTietJobPostAsync(string maJob)
        {
            if (string.IsNullOrWhiteSpace(maJob))
            {
                throw new ArgumentException("Mã công việc không được để trống!", nameof(maJob));
            }

            var job = await _jopPostRepository.layJobPostTheoMaAsync(maJob.Trim());
            if (job == null)
            {
                return null;
            }

            var jobs = await _jopPostRepository.layTatCaJobPostAsync();
            var employers = await _ntd.layTatCaNhaTuyenDungAsync();
            var employer = employers.FirstOrDefault(x => x.maNhaTuyenDung == job.maNhaTuyenDung);

            var jobDto = new JobPost_ReponseDTO
            {
                maJob = job.maJob,
                maNhaTuyenDung = job.maNhaTuyenDung,
                tieude = job.tieude,
                mota = job.mota,
                kynangyeucau = job.kynangyeucau,
                thulao = job.thulao,
                fileDinhKem = job.fileDinhKem,
                phiDangBai = job.phiDangBai,
                thoigiandangtuyen = job.thoigiandangtuyen,
                thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                status = job.status,
                soluongtuyen = job.soluongtuyen,
                tenCongTy = employer?.tencongty ?? employer?.User?.hotenUser ?? string.Empty,
                avatarNhaTuyenDung = employer?.User?.avatarUrl ?? employer?.logo ?? string.Empty
            };

            return new JobPostDetail_ReponsesDTO
            {
                Job = jobDto,
                Employer = employer == null ? null : new NhaTuyenDung_ResponseDTO
                {
                    maNhaTuyenDung = employer.maNhaTuyenDung,
                    maUser = employer.maUser,
                    hotenUser = employer.User?.hotenUser,
                    emailUser = employer.User?.emailUser,
                    sdtUser = employer.User?.sdtUser,
                    tencongty = employer.tencongty,
                    linhvuc = employer.linhvuc,
                    diachi = employer.diachi,
                    gioithieu = employer.gioithieu,
                    avatar = employer.User?.avatarUrl,
                    logo = employer.logo,
                    sosaodanhgia = employer.sosaodanhgia,
                    trangthai = employer.trangthai,
                    ngayDangKy = employer.ngayDangKy
                },
                RelatedJobs = jobs
                    .Where(x => x.maNhaTuyenDung == job.maNhaTuyenDung && x.maJob != job.maJob)
                    .Select(x => new JobPost_ReponseDTO
                    {
                        maJob = x.maJob,
                        maNhaTuyenDung = x.maNhaTuyenDung,
                        tieude = x.tieude,
                        mota = x.mota,
                        kynangyeucau = x.kynangyeucau,
                        thulao = x.thulao,
                        fileDinhKem = x.fileDinhKem,
                        phiDangBai = x.phiDangBai,
                        thoigiandangtuyen = x.thoigiandangtuyen,
                        thoigiandukienhoanthanh = x.thoigiandukienhoanthanh,
                        status = x.status,
                        soluongtuyen = x.soluongtuyen
                    }).ToList()
            };
        }


        public async Task<JobPost_ReponseDTO> taoJobPostAsync(JobPost_RequestDTO request)
        {
            //Kiểm tra ngày hoàn thành phải sau ngày hiện tại
            if (request.thoigiandukienhoanthanh <= DateTime.Now)
            {
                throw new Exception("Thời gian hoàn thành dự kiến phải lớn hơn thời gian hiện tại!");
            }


            //Tuấn bổ sung:
            //Lấy thông tin mã nhà tuyển dụng dựa trên request.user

            var NTD_Theo_maUser = await _ntd.layNhaTuyenDungTheoMaUserAsync(request.maUser);

            if (NTD_Theo_maUser == null)
            {
                throw new Exception($"Không tìm thấy hồ sơ Nhà tuyển dụng của User ID {request.maUser}!");
            }


            //Gán mã ntd theo mã user
            int maNTD_TheoUser = NTD_Theo_maUser!.maNhaTuyenDung;


            //Tự động sinh mã VARCHARR cho maJob
            string maJobTuSinh = "JOB" + DateTime.Now.ToString("yyMMddHHmmsss");

            //Tạo model JobPost để chuẩn bị lưu xuống Database
            var jobMoi = new JobPost
            {
                maJob = maJobTuSinh,
                maNhaTuyenDung = maNTD_TheoUser, //sửa
                tieude = request.tieude.Trim(),
                mota = request.mota!.Trim(),
                kynangyeucau = request.kynangyeucau?.Trim(),
                thulao = request.thulao,
                soluongtuyen = request.soluongtuyen,
                thoigiandukienhoanthanh = request.thoigiandukienhoanthanh,
                fileDinhKem = request.fileDinhKem ?? string.Empty,
                phiDangBai = request.phiDangBai,
                thoigiandangtuyen = DateTime.UtcNow,
                status = "DangTuyen"
            };

            //Gọi repo để lưu
            var luuJob = await _jopPostRepository.themJobPostAsync(jobMoi);
            // 5. Chuyển Model thành DTO trả về cho Controller
            return new JobPost_ReponseDTO
            {
                maJob = luuJob.maJob,
                maNhaTuyenDung = luuJob.maNhaTuyenDung,
                tieude = luuJob.tieude,
                mota = luuJob.mota,
                kynangyeucau = luuJob.kynangyeucau,
                thulao = luuJob.thulao,
                fileDinhKem = luuJob.fileDinhKem,
                phiDangBai = luuJob.phiDangBai,
                thoigiandangtuyen = luuJob.thoigiandangtuyen,
                thoigiandukienhoanthanh = luuJob.thoigiandukienhoanthanh,
                status = luuJob.status,
                soluongtuyen = luuJob.soluongtuyen
            };
        }

        public async Task<List<UngTuyen_ReponseDTO>> layDanhSachUngTuyen_JobPost_TheoMaUser(int maUser)
        {
            if (maUser <= 0)
            {
                throw new Exception("Mã người dùng không hợp lệ!");
            }

            var user = await _user.timUserTheoMaUserAsync(maUser);
            if (user == null)
            {
                throw new Exception("Không tìm thấy người dùng!");
            }

            List<UngTuyen> dsUngTuyen = new List<UngTuyen>();

            if (user.maRole == 2)
            {
                var ntd = await _ntd.layNhaTuyenDungTheoMaUserAsync(user.maUser);
                if (ntd == null)
                {
                    throw new Exception("Chưa tìm thấy hồ sơ Nhà tuyển dụng của tài khoản này!");
                }

                dsUngTuyen = await _jopPostRepository.layTatCaDanhSachUngTuyenVaoJobPost_TheoNTD(ntd.maNhaTuyenDung);
            }
            else if (user.maRole == 1)
            {
                var freelacerstudent = await _free.layFreelancerStudent_TheoMaUser(maUser);
                if (freelacerstudent == null)
                {
                    return new List<UngTuyen_ReponseDTO>();
                }

                dsUngTuyen = await _jopPostRepository.layTatCaDanhSachUngTuyenVaoJobPost_TheoFreelancerStudents(freelacerstudent.maFreelancerStudents);
            }

            if (dsUngTuyen == null || !dsUngTuyen.Any())
            {
                return new List<UngTuyen_ReponseDTO>();
            }

            var maJobs = dsUngTuyen
                .Select(u => u.maJob)
                .Distinct()
                .ToList();

            var maFreelancers = dsUngTuyen
                .Select(u => u.maFreelancerStudent)
                .Distinct()
                .ToList();

            var dsHopDong = await _context.HopDongs
                .AsNoTracking()
                .Where(h =>
                    maJobs.Contains(h.maJob) &&
                    maFreelancers.Contains(h.maFreelancerStudent))
                .ToListAsync();
            return dsUngTuyen.Select(u => new UngTuyen_ReponseDTO
            {
                maUngTuyen = u.maUngTuyen,
                maJob = u.maJob,
                tieude = u.JobPosts?.tieude ?? string.Empty,
                thulao = u.JobPosts?.thulao,
                mota = u.JobPosts?.mota,
                kynangyeucau = u.JobPosts?.kynangyeucau,
                soluongtuyen = u.JobPosts?.soluongtuyen,
                status = u.JobPosts?.status,
                phiDangBai = u.JobPosts?.phiDangBai ?? 0,
                thoigiandangtuyen = u.JobPosts?.thoigiandangtuyen ?? DateTime.UtcNow,
                thoigiandukienhoanthanh = u.JobPosts?.thoigiandukienhoanthanh ?? DateTime.UtcNow,

                //THÔNG TIN NHÀ TUYỂN DỤNG CỦA BÀI ĐĂNG
                maNhaTuyenDung = u.JobPosts?.maNhaTuyenDung ?? 0,
                tenCongTy = u.JobPosts?.NhaTuyenDung?.tencongty ?? u.JobPosts?.NhaTuyenDung?.User?.hotenUser ?? string.Empty,
                avatarNhaTuyenDung = u.JobPosts?.NhaTuyenDung?.User?.avatarUrl ?? "",

                // Thông tin ứng tuyển
                maFreelancerStudents = u.maFreelancerStudent,
                thuGioiThieu = u.thuGioiThieu,
                thulaoDeXuat = u.thulaoDeXuat,
                thoiGianHoanThanhDeXuat = u.thoiGianHoanThanhDeXuat,
                fileCV = u.fileCV,
                ngayUngTuyen = u.ngayUngTuyen,
                trangThaiUngTuyen = u.trangThaiUngTuyen,

                // Thông tin sinh viên nộp đơn
                maUser = u.FreelamcerStudents?.User?.maUser ?? 0,
                tenUser = u.FreelamcerStudents?.User?.hotenUser,
                emailUser = u.FreelamcerStudents?.User?.emailUser,
                sdtUser = u.FreelamcerStudents?.User?.sdtUser,
                avatar = u.FreelamcerStudents?.User?.avatarUrl,
                tenTruong = u.FreelamcerStudents?.tenTruong ?? string.Empty,
                GPA = u.FreelamcerStudents?.GPA ?? 0,
                namThu = u.FreelamcerStudents?.namThu ?? 1,
                tenChuyenNganh = u.FreelamcerStudents?.ChuyenNganh?.tenChuyenNganh ?? string.Empty,
                TrangThaiHopDong =
                    dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent &&
                            h.trangThai != "DaHuy")
                        .Select(h => h.trangThai)
                        .FirstOrDefault()
                    ?? dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent &&
                            h.trangThai == "DaHuy")
                        .Select(h => h.trangThai)
                        .FirstOrDefault(),

                MaHopDong =
                    dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent &&
                            h.trangThai != "DaHuy")
                        .Select(h => h.maHD)
                        .FirstOrDefault()
                    ?? dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent &&
                            h.trangThai == "DaHuy")
                        .Select(h => h.maHD)
                        .FirstOrDefault(),
            }).ToList();

        }

        //Tuấn
        //Tuấn
        public async Task<bool> duyetUngTuyenAsync(DuyetUngTuyen_RequestDTO request)
        {
            if (request.maUngTuyen <= 0)
            {
                throw new Exception("Mã đơn ứng tuyển không hợp lệ!");
            }
            if (request.trangThai != "ChapNhan" && request.trangThai != "TuChoi")
            {
                throw new Exception("Trạng thái duyệt không hợp lệ! (Chỉ chấp nhận 'ChapNhan' hoặc 'TuChoi')");
            }
            var donUngTuyen = await _jopPostRepository.layUngTuyenTheoMaAsync(request.maUngTuyen);

            if (donUngTuyen == null)
            {
                throw new ArgumentException(
                    "Không tìm thấy đơn ứng tuyển.");
            }

            if (donUngTuyen.trangThaiUngTuyen != "ChoDuyet")
            {
                throw new InvalidOperationException(
                    "Chỉ được duyệt hoặc từ chối đơn đang chờ duyệt.");
            }
            var ketQua = await _jopPostRepository.capNhatTrangThaiUngTuyenAsync(request.maUngTuyen, request.trangThai);

            if (!ketQua)
            {
                throw new Exception("Không tìm thấy đơn ứng tuyển để cập nhật!");
            }
            return true;

        }

        public async Task<bool> nopDonUngTuyenAsync(UngTuyen_RequestDTO request)
        {
            if (request == null ||
                request.maUser <= 0 ||
                string.IsNullOrWhiteSpace(request.maJob))
            {
                throw new ArgumentException("Thông tin ứng tuyển không hợp lệ.");
            }

            var sinhvien = await _free.layFreelancerStudent_TheoMaUser(
                request.maUser);

            if (sinhvien == null)
            {
                var user = await _user.timUserTheoMaUserAsync(request.maUser);

                if (user != null && user.maRole == 1)
                {
                    sinhvien = await _free.themFreelancerStudent_DuaVaoMaRoleCuaUsers(
                        user, user.maRole);
                }

                if (sinhvien == null)
                {
                    throw new InvalidOperationException(
                        "Chưa tìm thấy hồ sơ sinh viên để ứng tuyển.");
                }
            }

            var donCu = await _jopPostRepository
                .layDonUngTuyenTheoJobVaFreelancerAsync(
                    request.maJob,
                    sinhvien.maFreelancerStudents);

            if (donCu != null)
            {
                if (donCu.trangThaiUngTuyen == "ChoDuyet")
                {
                    throw new InvalidOperationException(
                        "Bạn đã nộp đơn cho công việc này. Vui lòng chờ duyệt.");
                }

                if (donCu.trangThaiUngTuyen == "ChapNhan")
                {
                    throw new InvalidOperationException(
                        "Đơn ứng tuyển của bạn đã được chấp nhận.");
                }

                if (donCu.trangThaiUngTuyen != "TuChoi")
                {
                    throw new InvalidOperationException(
                        "Đơn ứng tuyển hiện tại không thể nộp lại.");
                }

                // Đơn bị từ chối: cho phép ứng tuyển lại
                donCu.thuGioiThieu =
                    (request.thuGioiThieu ?? string.Empty).Trim();

                donCu.thulaoDeXuat = request.thulaoDeXuat;

                donCu.thoiGianHoanThanhDeXuat =
                    request.thoiGianHoanThanhDeXuat;

                donCu.fileCV = request.fileCV;

                donCu.ngayUngTuyen = DateTime.UtcNow;

                donCu.trangThaiUngTuyen = "ChoDuyet";

                await _jopPostRepository.capNhatDonUngTuyenAsync(donCu);

                return true;
            }

            // Chưa từng ứng tuyển: tạo đơn mới
            var ungTuyenMoi = new UngTuyen
            {
                maJob = request.maJob,
                maFreelancerStudent = sinhvien.maFreelancerStudents,
                thuGioiThieu =
                    (request.thuGioiThieu ?? string.Empty).Trim(),
                thulaoDeXuat = request.thulaoDeXuat,
                thoiGianHoanThanhDeXuat =
                    request.thoiGianHoanThanhDeXuat,
                fileCV = request.fileCV,
                ngayUngTuyen = DateTime.UtcNow,
                trangThaiUngTuyen = "ChoDuyet"
            };

            await _jopPostRepository.themUngTuyenAsync(ungTuyenMoi);

            return true;
        }


        //Tuấn
        public async Task<List<JobPost_ReponseDTO>> layJobPostTheoMaUserAsync(int maUser)
        {
            var ntd = await _ntd.layNhaTuyenDungTheoMaUserAsync(maUser);
            if (ntd == null) return new List<JobPost_ReponseDTO>();
            var ds = await _jopPostRepository.layJobPostTheoMaNTDAsync(ntd.maNhaTuyenDung);
            return ds.Select(job => new JobPost_ReponseDTO
            {
                maJob = job.maJob,
                maNhaTuyenDung = job.maNhaTuyenDung,
                tieude = job.tieude,
                mota = job.mota,
                kynangyeucau = job.kynangyeucau,
                thulao = job.thulao,
                soluongtuyen = job.soluongtuyen,
                thoigiandangtuyen = job.thoigiandangtuyen,
                thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                status = job.status,
                soLuongUngTuyen = job.UngTuyens?.Count ?? 0
            }).ToList();
        }


        //Tuấn
        public async Task<bool> capNhatJobPostAsync(JobPost_RequestDTO reqeuest)
        {
            var job = await _jopPostRepository.layJobPostTheoMaAsync(reqeuest.maJob);
            if (job == null) throw new Exception("Không tìm thấy bài tuyển dụng!");
            job.tieude = reqeuest.tieude.Trim();
            job.mota = reqeuest.mota?.Trim();
            job.kynangyeucau = reqeuest.kynangyeucau?.Trim();
            job.thulao = reqeuest.thulao;
            job.soluongtuyen = reqeuest.soluongtuyen;
            job.thoigiandukienhoanthanh = reqeuest.thoigiandukienhoanthanh;
            if (!string.IsNullOrWhiteSpace(reqeuest.status))
            {
                job.status = reqeuest.status;
            }
            return await _jopPostRepository.capNhatJobPostAsync(job);
        }

        // Gợi ý việc làm phù hợp theo kỹ năng của Freelancer
        public async Task<List<JobPost_ReponseDTO>> layDanhSachJobGoiYChoUserAsync(int maUser)
        {
            // 1. Tìm thông tin FreelancerStudent kèm kỹ năng & chuyên ngành
            var freelancer = await _context.FreelancerStudents
                .Include(f => f.ChuyenNganh)
                .Include(f => f.FreelancerStudent_KyNangs)
                    .ThenInclude(k => k.KyNang)
                .FirstOrDefaultAsync(f => f.maUser == maUser || f.maFreelancerStudents == maUser);

            var danhSachKyNangUser = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string? tenChuyenNganh = freelancer?.ChuyenNganh?.tenChuyenNganh;

            if (freelancer != null)
            {
                if (freelancer.FreelancerStudent_KyNangs != null)
                {
                    foreach (var k in freelancer.FreelancerStudent_KyNangs)
                    {
                        if (k.KyNang != null && !string.IsNullOrWhiteSpace(k.KyNang.tenKyNang))
                        {
                            danhSachKyNangUser.Add(k.KyNang.tenKyNang.Trim());
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(freelancer.kyNangCoBan))
                {
                    var splitCoBan = freelancer.kyNangCoBan.Split(new[] { ',', ';', '|', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in splitCoBan)
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            danhSachKyNangUser.Add(s.Trim());
                        }
                    }
                }
            }

            // 2. Lấy danh sách các công việc đang tuyển
            var dsJobSQL = await _context.JobPosts
                .Include(j => j.NhaTuyenDung)
                    .ThenInclude(ntd => ntd.User)
                .Include(j => j.UngTuyens)
                .Where(j => j.status == "DangTuyen")
                .ToListAsync();

            // 3. Tính toán độ phù hợp CHỈ DỰA VÀO KỸ NĂNG CỦA FREELANCER (Tối đa 100%)
            var ketQua = new List<JobPost_ReponseDTO>();

            foreach (var job in dsJobSQL)
            {
                // Tách danh sách kỹ năng yêu cầu của công việc
                var kyNangJob = new List<string>();
                if (!string.IsNullOrWhiteSpace(job.kynangyeucau))
                {
                    var splitKyNang = job.kynangyeucau.Split(
                        new[] { ',', ';', '|', '\n', '/' },
                        StringSplitOptions.RemoveEmptyEntries
                    );
                    foreach (var kn in splitKyNang)
                    {
                        var trimmed = kn.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmed) &&
                            !kyNangJob.Any(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase)))
                        {
                            kyNangJob.Add(trimmed);
                        }
                    }
                }

                var textJob = $"{job.tieude} {job.mota} {job.kynangyeucau}".ToLowerInvariant();
                var kyNangKhop = new List<string>();

                if (kyNangJob.Count > 0)
                {
                    // Trường hợp 1: Công việc có khai báo cụ thể danh sách kỹ năng yêu cầu
                    foreach (var knJob in kyNangJob)
                    {
                        var knJobLower = knJob.ToLowerInvariant();
                        // Kiểm tra sinh viên có kỹ năng này không
                        var khop = danhSachKyNangUser.Any(knUser =>
                        {
                            var knUserLower = knUser.ToLowerInvariant();
                            return knUserLower == knJobLower ||
                                   knJobLower.Contains(knUserLower) ||
                                   knUserLower.Contains(knJobLower);
                        });

                        if (khop && !kyNangKhop.Contains(knJob))
                        {
                            kyNangKhop.Add(knJob);
                        }
                    }
                }
                else
                {
                    // Trường hợp 2: Công việc không phân tách kỹ năng cụ thể, tìm từ khóa kỹ năng của user trong mô tả/tiêu đề
                    foreach (var knUser in danhSachKyNangUser)
                    {
                        if (textJob.Contains(knUser.ToLowerInvariant()) && !kyNangKhop.Contains(knUser))
                        {
                            kyNangKhop.Add(knUser);
                        }
                    }
                }

                // Tính toán tỷ lệ % phù hợp (Maximum 100%) dựa trên số kỹ năng đáp ứng được
                double phanTramPhuHop = 0;
                string lyDo = string.Empty;

                if (danhSachKyNangUser.Count == 0)
                {
                    phanTramPhuHop = 0;
                    lyDo = "Bạn chưa cập nhật kỹ năng vào hồ sơ nên chưa thể tính toán độ phù hợp.";
                }
                else if (kyNangJob.Count > 0)
                {
                    // Công thức: (Số kỹ năng đáp ứng / Tổng số kỹ năng yêu cầu) * 100
                    int tongKyNangYeuCau = kyNangJob.Count;
                    int soKyNangDapUng = kyNangKhop.Count;

                    phanTramPhuHop = Math.Min(100.0, Math.Round(((double)soKyNangDapUng / tongKyNangYeuCau) * 100.0, 0));

                    if (soKyNangDapUng == tongKyNangYeuCau && tongKyNangYeuCau > 0)
                    {
                        lyDo = $"Phù hợp 100%: Bạn đáp ứng đầy đủ tất cả {tongKyNangYeuCau}/{tongKyNangYeuCau} kỹ năng yêu cầu ({string.Join(", ", kyNangKhop)}).";
                    }
                    else if (soKyNangDapUng > 0)
                    {
                        lyDo = $"Phù hợp {phanTramPhuHop:0}%: Bạn đáp ứng {soKyNangDapUng}/{tongKyNangYeuCau} kỹ năng yêu cầu ({string.Join(", ", kyNangKhop)}).";
                    }
                    else
                    {
                        lyDo = $"0% phù hợp: Bạn chưa có kỹ năng nào trong số {tongKyNangYeuCau} kỹ năng công việc yêu cầu ({string.Join(", ", kyNangJob)}).";
                    }
                }
                else
                {
                    // Công việc không ghi trường kỹ năng rõ ràng
                    if (kyNangKhop.Count > 0)
                    {
                        int tongKyNangUser = Math.Max(1, danhSachKyNangUser.Count);
                        phanTramPhuHop = Math.Min(100.0, Math.Round(((double)kyNangKhop.Count / tongKyNangUser) * 100.0, 0));
                        lyDo = $"Phù hợp {phanTramPhuHop:0}%: Công việc có yêu cầu {kyNangKhop.Count} kỹ năng bạn sở hữu ({string.Join(", ", kyNangKhop)}).";
                    }
                    else
                    {
                        phanTramPhuHop = 0;
                        lyDo = "Không tìm thấy kỹ năng phù hợp với công việc này.";
                    }
                }

                // Chỉ gợi ý công việc có ít nhất 1 kỹ năng trùng khớp (> 0%)
                if (phanTramPhuHop > 0)
                {
                    ketQua.Add(new JobPost_ReponseDTO
                    {
                        maJob = job.maJob,
                        maNhaTuyenDung = job.maNhaTuyenDung,
                        tieude = job.tieude,
                        mota = job.mota,
                        kynangyeucau = job.kynangyeucau,
                        thulao = job.thulao,
                        fileDinhKem = job.fileDinhKem,
                        phiDangBai = job.phiDangBai,
                        thoigiandangtuyen = job.thoigiandangtuyen,
                        thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                        status = job.status,
                        soluongtuyen = job.soluongtuyen,
                        soLuongUngTuyen = job.UngTuyens?.Count ?? 0,
                        tenCongTy = job.NhaTuyenDung?.tencongty ?? job.NhaTuyenDung?.User?.hotenUser ?? string.Empty,
                        avatarNhaTuyenDung = job.NhaTuyenDung?.User?.avatarUrl ?? job.NhaTuyenDung?.logo ?? string.Empty,
                        doPhuHop = phanTramPhuHop,
                        lyDoGoiY = lyDo,
                        kyNangTrungKhop = kyNangKhop
                    });
                }
            }

            // Sắp xếp: Tỷ lệ phù hợp cao nhất (tối đa 100%) lên đầu, sau đó đến thời gian đăng mới nhất
            return ketQua
                .OrderByDescending(j => j.doPhuHop)
                .ThenByDescending(j => j.thoigiandangtuyen)
                .ToList();
        }
    }
}