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
        private readonly IKyNangGoiYService _kyNangGoiYService;
        private readonly IMinhChungService _minhChungService;

        public JobPostService(
            IJobPostRepository nhaTuyenDungRepository,
            INhaTuyenDungRepository ntd,
            IUserRepository user,
            IFreelancerStudentRepository free,
            ApplicationDBContext context,
            IKyNangGoiYService kyNangGoiYService,
            IMinhChungService minhChungService)
        {
            _jopPostRepository = nhaTuyenDungRepository;
            _ntd = ntd;
            _user = user;
            _free = free;
            _context = context;
            _kyNangGoiYService = kyNangGoiYService;
            _minhChungService = minhChungService;
        }


        //
        public async Task<List<JobPost_ReponseDTO>> layDanhSachJobPostAsync()
        {
            var layDS_JobSQL = await _jopPostRepository.layTatCaJobPostAsync(); // Đã sắp xếp mới nhất trước
            
            // Lấy danh sách các mã Job đã có ứng viên được chấp nhận hoặc hợp đồng đã hoàn thành
            var dsJobDaDuyet = await _context.UngTuyens.AsNoTracking()
                .Where(u => u.trangThaiUngTuyen == "ChapNhan")
                .Select(u => u.maJob)
                .Distinct()
                .ToListAsync();

            var dsJobHoanThanh = await _context.HopDongs.AsNoTracking()
                .Where(h => h.trangThai == "DaHoanThanh")
                .Select(h => h.maJob)
                .Distinct()
                .ToListAsync();

            // Tính toán đánh giá thực tế từ bảng DanhGia_NhanXet
            var ntdUserIds = layDS_JobSQL
                .Where(j => j.NhaTuyenDung != null && j.NhaTuyenDung.maUser > 0)
                .Select(j => j.NhaTuyenDung!.maUser)
                .Distinct()
                .ToList();

            var reviewStats = await _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => ntdUserIds.Contains(d.maNguoiDuocDanhGia))
                .GroupBy(d => d.maNguoiDuocDanhGia)
                .Select(g => new
                {
                    MaUser = g.Key,
                    SoLuot = g.Count(),
                    DiemTB = g.Average(d => (double)d.soSao)
                })
                .ToListAsync();

            var dsResult = layDS_JobSQL.Select(job =>
            {
                var ntd = job.NhaTuyenDung;
                var stat = reviewStats.FirstOrDefault(s => ntd != null && s.MaUser == ntd.maUser);
                
                double? sao = (stat != null && stat.SoLuot > 0) 
                    ? stat.DiemTB 
                    : (ntd?.sosaodanhgia > 0 ? ntd.sosaodanhgia : null);
                int soLuot = (stat != null && stat.SoLuot > 0) 
                    ? stat.SoLuot 
                    : (ntd?.soLuotDanhGia ?? 0);

                bool isJobHoanThanh = job.status == "DaHoanThanh" || dsJobHoanThanh.Contains(job.maJob);
                bool isDaTuyenDu = isJobHoanThanh || job.status == "DaTuyenDu" || job.status == "Closed" || job.status == "DaDong" || dsJobDaDuyet.Contains(job.maJob);
                string effectiveStatus = isJobHoanThanh ? "DaHoanThanh" : (isDaTuyenDu && job.status == "DangTuyen" ? "DaTuyenDu" : (job.status ?? "DangTuyen"));

                return new JobPost_ReponseDTO
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
                    status = effectiveStatus,
                    soluongtuyen = job.soluongtuyen,
                    DaTuyenDu = isDaTuyenDu,
                    tenCongTy = ntd?.tencongty ?? ntd?.User?.hotenUser ?? string.Empty,
                    avatarNhaTuyenDung = ntd?.User?.avatarUrl ?? ntd?.logo ?? string.Empty,
                    soSaoDanhGiaNhaTuyenDung = sao,
                    soLuotDanhGiaNhaTuyenDung = soLuot,
                    diaChiNhaTuyenDung = ntd?.diachi,
                    linhVucNhaTuyenDung = ntd?.linhvuc
                };
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

            bool isJobHoanThanhChiTiet = job.status == "DaHoanThanh" 
                || await _context.HopDongs.AsNoTracking().AnyAsync(h => h.maJob == job.maJob && h.trangThai == "DaHoanThanh");
            bool isDaCoUngVienDuyetChiTiet = await _context.UngTuyens.AsNoTracking().AnyAsync(u => u.maJob == job.maJob && u.trangThaiUngTuyen == "ChapNhan");
            bool isDaTuyenDuChiTiet = isJobHoanThanhChiTiet || isDaCoUngVienDuyetChiTiet || job.status == "DaTuyenDu" || job.status == "Closed" || job.status == "DaDong";
            string effectiveStatusChiTiet = isJobHoanThanhChiTiet ? "DaHoanThanh" : (isDaTuyenDuChiTiet && job.status == "DangTuyen" ? "DaTuyenDu" : (job.status ?? "DangTuyen"));

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
                status = effectiveStatusChiTiet,
                soluongtuyen = job.soluongtuyen,
                DaTuyenDu = isDaTuyenDuChiTiet,
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


        public async Task<decimal> layPhiDangBaiHienTaiAsync()
        {
            var cauHinh = await _context.CauHinhPhiHoaHongs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.tenCauHinh == "PhiDangBai" && c.hieuLuc);

            if (cauHinh == null || cauHinh.giaTri < 0)
            {
                return 0m;
            }

            return cauHinh.giaTri;
        }

        public async Task<JobPost_ReponseDTO> taoJobPostAsync(JobPost_RequestDTO request)
        {
            // Kiểm tra ngày hoàn thành phải sau ngày hiện tại
            if (request.thoigiandukienhoanthanh <= DateTime.Now)
            {
                throw new Exception("Thời gian hoàn thành dự kiến phải lớn hơn thời gian hiện tại!");
            }

            // Lấy thông tin mã nhà tuyển dụng dựa trên request.maUser
            var NTD_Theo_maUser = await _ntd.layNhaTuyenDungTheoMaUserAsync(request.maUser);

            if (NTD_Theo_maUser == null)
            {
                throw new Exception($"Không tìm thấy hồ sơ Nhà tuyển dụng của User ID {request.maUser}!");
            }

            int maNTD_TheoUser = NTD_Theo_maUser!.maNhaTuyenDung;
            string maJobTuSinh = "JOB" + DateTime.Now.ToString("yyMMddHHmmsss");

            // Lấy mức phí đăng bài hiện hành do Admin thiết lập
            decimal phiDangBaiApDung = await layPhiDangBaiHienTaiAsync();

            // Nếu hệ thống áp dụng phí đăng bài (> 0 VNĐ)
            if (phiDangBaiApDung > 0)
            {
                var walletNTD = await _context.Wallets.FirstOrDefaultAsync(w => w.maUser == request.maUser);
                if (walletNTD == null || (walletNTD.soDuKhaDung ?? 0) < phiDangBaiApDung)
                {
                    decimal soDuHienCo = walletNTD?.soDuKhaDung ?? 0m;
                    throw new InvalidOperationException($"Số dư khả dụng trong ví ({soDuHienCo:N0} VNĐ) không đủ để thanh toán phí đăng tin ({phiDangBaiApDung:N0} VNĐ). Vui lòng nạp thêm tiền vào ví để tiếp tục.");
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Trừ thẳng số dư khả dụng của Nhà tuyển dụng (kiểm tra điều kiện số dư đồng thời)
                    decimal soDuNTDTruoc = walletNTD.soDuKhaDung ?? 0m;
                    var rowUpdatedNTD = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE Wallet 
                        SET soDuKhaDung = soDuKhaDung - {phiDangBaiApDung}
                        WHERE maUser = {request.maUser} AND soDuKhaDung >= {phiDangBaiApDung}");

                    if (rowUpdatedNTD != 1)
                    {
                        throw new InvalidOperationException("Giao dịch thất bại: Số dư khả dụng trong ví không đủ hoặc vừa thay đổi. Vui lòng kiểm tra lại ví.");
                    }

                    // 2. Tìm hoặc khởi tạo Ví của Quản trị viên hệ thống (maRole = 3)
                    var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.maRole == 3);
                    if (adminUser == null)
                    {
                        throw new InvalidOperationException("Không tìm thấy tài khoản Quản trị viên hệ thống để tiếp nhận phí đăng bài.");
                    }

                    var walletAdmin = await _context.Wallets.FirstOrDefaultAsync(w => w.maUser == adminUser.maUser);
                    if (walletAdmin == null)
                    {
                        walletAdmin = new Wallet
                        {
                            maUser = adminUser.maUser,
                            soDuKhaDung = 0,
                            soDuDongBang = 0
                        };
                        await _context.Wallets.AddAsync(walletAdmin);
                        await _context.SaveChangesAsync();
                    }

                    decimal soDuAdminTruoc = walletAdmin.soDuKhaDung ?? 0m;
                    var rowUpdatedAdmin = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE Wallet 
                        SET soDuKhaDung = soDuKhaDung + {phiDangBaiApDung}
                        WHERE maUser = {adminUser.maUser}");

                    if (rowUpdatedAdmin != 1)
                    {
                        throw new InvalidOperationException("Không thể cộng phí đăng bài vào ví quản trị viên hệ thống.");
                    }

                    // 3. Tạo bản ghi JobPost
                    var jobMoi = new JobPost
                    {
                        maJob = maJobTuSinh,
                        maNhaTuyenDung = maNTD_TheoUser,
                        tieude = request.tieude.Trim(),
                        mota = request.mota!.Trim(),
                        kynangyeucau = request.kynangyeucau?.Trim(),
                        thulao = request.thulao,
                        soluongtuyen = request.soluongtuyen,
                        thoigiandukienhoanthanh = request.thoigiandukienhoanthanh,
                        fileDinhKem = request.fileDinhKem ?? string.Empty,
                        phiDangBai = phiDangBaiApDung,
                        thoigiandangtuyen = DateTime.UtcNow,
                        status = "DangTuyen"
                    };

                    await _jopPostRepository.themJobPostAsync(jobMoi);

                    // 4. Ghi nhận Sổ cái LichSuGiaoDich cho 2 bên
                    var walletNTDSau = await _context.Wallets.AsNoTracking().FirstAsync(w => w.maUser == request.maUser);
                    var walletAdminSau = await _context.Wallets.AsNoTracking().FirstAsync(w => w.maUser == adminUser.maUser);

                    _context.LichSuGiaoDichs.AddRange(
                        new LichSuGiaoDich
                        {
                            maWallet = walletNTD.maWallet,
                            loaiGiaoDich = "PhiDangBai",
                            soTien = phiDangBaiApDung,
                            soDuTruocGD = soDuNTDTruoc,
                            soDuSauGD = walletNTDSau.soDuKhaDung ?? 0m,
                            noiDung = $"Thanh toán phí đăng tin tuyển dụng {maJobTuSinh} - \"{jobMoi.tieude}\"",
                            phuongThucThanhToan = "ViNoiBo",
                            trangThai = "ThanhCong",
                            ngayTao = DateTime.Now
                        },
                        new LichSuGiaoDich
                        {
                            maWallet = walletAdmin.maWallet,
                            loaiGiaoDich = "ThuPhiDangBai",
                            soTien = phiDangBaiApDung,
                            soDuTruocGD = soDuAdminTruoc,
                            soDuSauGD = walletAdminSau.soDuKhaDung ?? 0m,
                            noiDung = $"Thu phí đăng tin {maJobTuSinh} từ NTD {NTD_Theo_maUser.tencongty ?? NTD_Theo_maUser.User?.hotenUser ?? $"#{request.maUser}"}",
                            phuongThucThanhToan = "ViNoiBo",
                            trangThai = "ThanhCong",
                            ngayTao = DateTime.Now
                        }
                    );

                    // 5. Gửi thông báo cho Nhà tuyển dụng
                    _context.ThongBaos.Add(new ThongBao
                    {
                        maUser = request.maUser,
                        tieuDe = "Đăng tin tuyển dụng thành công",
                        noiDung = $"Tin tuyển dụng \"{jobMoi.tieude}\" đã được xuất bản thành công. Phí dịch vụ {phiDangBaiApDung:N0} VNĐ đã được khấu trừ từ số dư khả dụng.",
                        loaiThongBao = "ThanhToan",
                        linkDieuHuong = $"/JobPost/Detail?maJob={maJobTuSinh}",
                        ngayTao = DateTime.Now
                    });

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new JobPost_ReponseDTO
                    {
                        maJob = jobMoi.maJob,
                        maNhaTuyenDung = jobMoi.maNhaTuyenDung,
                        tieude = jobMoi.tieude,
                        mota = jobMoi.mota,
                        kynangyeucau = jobMoi.kynangyeucau,
                        thulao = jobMoi.thulao,
                        fileDinhKem = jobMoi.fileDinhKem,
                        phiDangBai = jobMoi.phiDangBai,
                        thoigiandangtuyen = jobMoi.thoigiandangtuyen,
                        thoigiandukienhoanthanh = jobMoi.thoigiandukienhoanthanh,
                        status = jobMoi.status,
                        soluongtuyen = jobMoi.soluongtuyen
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                // Trường hợp phí đăng bài = 0 (miễn phí)
                var jobMoi = new JobPost
                {
                    maJob = maJobTuSinh,
                    maNhaTuyenDung = maNTD_TheoUser,
                    tieude = request.tieude.Trim(),
                    mota = request.mota!.Trim(),
                    kynangyeucau = request.kynangyeucau?.Trim(),
                    thulao = request.thulao,
                    soluongtuyen = request.soluongtuyen,
                    thoigiandukienhoanthanh = request.thoigiandukienhoanthanh,
                    fileDinhKem = request.fileDinhKem ?? string.Empty,
                    phiDangBai = 0,
                    thoigiandangtuyen = DateTime.UtcNow,
                    status = "DangTuyen"
                };

                var luuJob = await _jopPostRepository.themJobPostAsync(jobMoi);

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
                .Where(h => maJobs.Contains(h.maJob))
                .ToListAsync();

            var maUserFreelancers = dsUngTuyen
                .Where(u => u.FreelamcerStudents != null && u.FreelamcerStudents.maUser > 0)
                .Select(u => u.FreelamcerStudents!.maUser)
                .Distinct()
                .ToList();

            var dsMinhChung = await _context.MinhChung_FreelancerStudents
                .AsNoTracking()
                .Where(m => maUserFreelancers.Contains(m.maUser))
                .OrderByDescending(m => m.ngayNop)
                .ToListAsync();

            return dsUngTuyen.Select(u => {
                var userStudentId = u.FreelamcerStudents?.maUser ?? 0;
                var mc = dsMinhChung.FirstOrDefault(m => m.maUser == userStudentId);
                bool isDaXacMinh = mc != null && mc.trangThaiGuiMinhChung == "Đã xác minh";
                string ttXacMinh = mc?.trangThaiGuiMinhChung ?? "Chưa gửi";

                return new UngTuyen_ReponseDTO
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
                diemDanhGiaTrungBinh = (double)(u.FreelamcerStudents?.diemDanhGiaTrungBinh ?? 0),
                soLuotDanhGia = u.FreelamcerStudents?.soLuotDanhGia ?? 0,
                namThu = u.FreelamcerStudents?.namThu ?? 1,
                tenChuyenNganh = u.FreelamcerStudents?.ChuyenNganh?.tenChuyenNganh ?? string.Empty,
                TrangThaiHopDong =
                    dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent &&
                            h.trangThai != "DaHuy" &&
                            h.trangThai != "TuChoi")
                        .Select(h => h.trangThai)
                        .FirstOrDefault()
                    ?? dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.trangThai != "DaHuy" &&
                            h.trangThai != "TuChoi")
                        .Select(h => h.trangThai)
                        .FirstOrDefault()
                    ?? dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent)
                        .Select(h => h.trangThai)
                        .FirstOrDefault(),

                MaHopDong =
                    dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent &&
                            h.trangThai != "DaHuy" &&
                            h.trangThai != "TuChoi")
                        .Select(h => h.maHD)
                        .FirstOrDefault()
                    ?? dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.trangThai != "DaHuy" &&
                            h.trangThai != "TuChoi")
                        .Select(h => h.maHD)
                        .FirstOrDefault()
                    ?? dsHopDong
                        .Where(h =>
                            h.maJob == u.maJob &&
                            h.maFreelancerStudent == u.maFreelancerStudent)
                        .Select(h => h.maHD)
                        .FirstOrDefault(),

                daXacMinhSinhVien = isDaXacMinh,
                trangThaiXacMinh = ttXacMinh,
            }; }).ToList();

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

            // QUY TẮC NGHIỆP VỤ: Nếu Nhà tuyển dụng chấp nhận ứng viên
            if (request.trangThai == "ChapNhan")
            {
                var jobPost = await _context.JobPosts.FirstOrDefaultAsync(j => j.maJob == donUngTuyen.maJob);
                if (jobPost == null)
                {
                    throw new InvalidOperationException("Không tìm thấy bài tuyển dụng tương ứng!");
                }
                if (jobPost.status == "DaHoanThanh")
                {
                    throw new InvalidOperationException("Công việc này đã hoàn thành nghiệm thu dự án, không thể duyệt thêm ứng viên!");
                }
                if (jobPost.status == "Closed" || jobPost.status == "DaDong")
                {
                    throw new InvalidOperationException("Công việc này đã đóng tuyển dụng, không thể duyệt thêm ứng viên!");
                }

                // Kiểm tra xem công việc này đã có ứng viên nào khác được chấp nhận chưa
                bool daCoUngVienDuyet = await _context.UngTuyens
                    .AnyAsync(u => u.maJob == donUngTuyen.maJob && u.trangThaiUngTuyen == "ChapNhan" && u.maUngTuyen != request.maUngTuyen);

                if (daCoUngVienDuyet)
                {
                    throw new InvalidOperationException("Công việc này đã có một ứng viên được chấp nhận (đã tuyển đủ). Mỗi công việc chỉ được chấp nhận tối đa 1 Freelancer!");
                }

                // Cập nhật đơn này thành ChapNhan
                var ketQua = await _jopPostRepository.capNhatTrangThaiUngTuyenAsync(request.maUngTuyen, "ChapNhan");
                if (!ketQua)
                {
                    throw new Exception("Không tìm thấy đơn ứng tuyển để cập nhật!");
                }

                // Cập nhật trạng thái JobPost thành DaTuyenDu nếu đang là DangTuyen
                if (jobPost.status == "DangTuyen")
                {
                    jobPost.status = "DaTuyenDu";
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            else
            {
                var ketQua = await _jopPostRepository.capNhatTrangThaiUngTuyenAsync(request.maUngTuyen, request.trangThai);
                if (!ketQua)
                {
                    throw new Exception("Không tìm thấy đơn ứng tuyển để cập nhật!");
                }
                return true;
            }
        }

        public async Task<bool> nopDonUngTuyenAsync(UngTuyen_RequestDTO request)
        {
            if (request == null ||
                request.maUser <= 0 ||
                string.IsNullOrWhiteSpace(request.maJob))
            {
                throw new ArgumentException("Thông tin ứng tuyển không hợp lệ.");
            }

            // KIỂM TRA ĐIỀU KIỆN TRẠNG THÁI JOBPOST
            var jobPost = await _context.JobPosts.FirstOrDefaultAsync(j => j.maJob == request.maJob);
            if (jobPost == null)
            {
                throw new InvalidOperationException("Không tìm thấy bài tuyển dụng!");
            }
            if (jobPost.status == "DaHoanThanh")
            {
                throw new InvalidOperationException("Công việc này đã hoàn thành nghiệm thu dự án và đóng tuyển dụng.");
            }
            if (jobPost.status == "Closed" || jobPost.status == "DaDong")
            {
                throw new InvalidOperationException("Công việc này đã đóng tuyển dụng, không tiếp nhận thêm hồ sơ.");
            }

            bool daCoUngVienDuyet = await _context.UngTuyens
                .AnyAsync(u => u.maJob == request.maJob && u.trangThaiUngTuyen == "ChapNhan")
                || jobPost.status == "DaTuyenDu";

            if (daCoUngVienDuyet)
            {
                throw new InvalidOperationException("Công việc này đã tuyển đủ ứng viên (đã có ứng viên được chọn). Không thể tiếp nhận thêm hồ sơ mới!");
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

            // Kiểm tra xác thực minh chứng và niên khóa sinh viên
            var checkXacThuc = await _minhChungService.KiemTraTrangThaiXacThucAsync(request.maUser);
            if (!checkXacThuc.DaXacMinh)
            {
                throw new InvalidOperationException(checkXacThuc.ThongBao);
            }

            var donCu = await _jopPostRepository
                .layDonUngTuyenTheoJobVaFreelancerAsync(
                    request.maJob,
                    sinhvien.maFreelancerStudents);

            // Lấy thời gian dự kiến hoàn thành chuẩn từ JobPost của Nhà tuyển dụng
            string thoiGianHoanThanhTuJob = jobPost.thoigiandukienhoanthanh.ToString("dd/MM/yyyy");

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

                // Ràng buộc cố định theo thời gian của JobPost, không tin tưởng giá trị client gửi lên
                donCu.thoiGianHoanThanhDeXuat = thoiGianHoanThanhTuJob;

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
                // Ràng buộc cố định theo thời gian của JobPost, không tin tưởng giá trị client gửi lên
                thoiGianHoanThanhDeXuat = thoiGianHoanThanhTuJob,
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

            var dsMaJob = ds.Select(j => j.maJob).ToList();
            var dsJobDaDuyet = await _context.UngTuyens.AsNoTracking()
                .Where(u => dsMaJob.Contains(u.maJob) && u.trangThaiUngTuyen == "ChapNhan")
                .Select(u => u.maJob)
                .Distinct()
                .ToListAsync();

            var dsJobHoanThanh = await _context.HopDongs.AsNoTracking()
                .Where(h => dsMaJob.Contains(h.maJob) && h.trangThai == "DaHoanThanh")
                .Select(h => h.maJob)
                .Distinct()
                .ToListAsync();

            return ds.Select(job =>
            {
                bool isJobHoanThanh = job.status == "DaHoanThanh" || dsJobHoanThanh.Contains(job.maJob);
                bool isDaTuyenDu = isJobHoanThanh || job.status == "DaTuyenDu" || job.status == "Closed" || job.status == "DaDong" || dsJobDaDuyet.Contains(job.maJob);
                string effectiveStatus = isJobHoanThanh ? "DaHoanThanh" : (isDaTuyenDu && job.status == "DangTuyen" ? "DaTuyenDu" : (job.status ?? "DangTuyen"));

                return new JobPost_ReponseDTO
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
                    status = effectiveStatus,
                    DaTuyenDu = isDaTuyenDu,
                    soLuongUngTuyen = job.UngTuyens?.Count ?? 0
                };
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

                var kyNangKhop = new List<string>();

                if (kyNangJob.Count > 0)
                {
                    // So khớp trực tiếp giữa kỹ năng sinh viên có và kỹ năng yêu cầu trong bài Job (kynangyeucau)
                    foreach (var knJob in kyNangJob)
                    {
                        var knJobLower = knJob.Trim().ToLowerInvariant();
                        // Kiểm tra sinh viên có kỹ năng này không
                        var khop = danhSachKyNangUser.Any(knUser =>
                        {
                            var knUserLower = knUser.Trim().ToLowerInvariant();
                            if (knUserLower == knJobLower) return true;
                            if (knUserLower.Length > 2 && knJobLower.Length > 2)
                            {
                                return knJobLower.Contains(knUserLower) || knUserLower.Contains(knJobLower);
                            }
                            return false;
                        });

                        if (khop && !kyNangKhop.Contains(knJob))
                        {
                            kyNangKhop.Add(knJob);
                        }
                    }
                }

                // Tính toán tỷ lệ % phù hợp dựa trên số kỹ năng đáp ứng / tổng số kỹ năng yêu cầu của bài Job
                double phanTramPhuHop = 0;
                string lyDo = string.Empty;

                if (danhSachKyNangUser.Count == 0)
                {
                    phanTramPhuHop = 0;
                    lyDo = "Bạn chưa cập nhật kỹ năng vào hồ sơ nên chưa thể tính toán độ phù hợp.";
                }
                else if (kyNangJob.Count > 0)
                {
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
                    // Công việc không yêu cầu kỹ năng cụ thể
                    phanTramPhuHop = 0;
                    lyDo = "Công việc không yêu cầu kỹ năng cụ thể.";
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