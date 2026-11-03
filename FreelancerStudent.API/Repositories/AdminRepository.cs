using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.AdminDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDBContext _context;

        // Static in-memory cache for Contract Template (since SQL Server has no MauHopDong table)
        private static AdminMauHopDongDTO _mauHopDong = new AdminMauHopDongDTO
        {
            TenMauHopDong = "Hợp đồng Cung cấp Dịch vụ Freelance Sinh viên Chuẩn",
            DieuKhoanChung = "1. BÊN A (Nhà tuyển dụng) có trách nhiệm thanh toán ký quỹ 100% thù lao thỏa thuận vào hệ thống trước khi hợp đồng bắt đầu có hiệu lực.\n" +
                             "2. BÊN B (Freelancer Sinh viên) cam kết thực hiện đúng tiến độ, chất lượng các đầu việc được giao theo thỏa thuận.\n" +
                             "3. Khi sản phẩm được bàn giao, BÊN A có thời hạn tối đa 3 ngày làm việc để kiểm thử, nghiệm thu hoặc yêu cầu chỉnh sửa.\n" +
                             "4. Tiền ký quỹ được giải ngân tự động sau khi BÊN A bấm 'Nghiệm thu đạt' hoặc tự động giải ngân sau thời hạn quy định nếu không có khiếu nại.\n" +
                             "5. Mọi tranh chấp phát sinh sẽ do Ban Quản trị sàn Freelancer Students làm trọng tài phân xử độc lập và quyết định cuối cùng.",
            ThoiHanNghiemThuNgay = 3,
            TyLeGiaiNganMacDinh = 100
        };

        public AdminRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. DASHBOARD & GIÁM SÁT
        // ==========================================
        public async Task<AdminDashboardResponseDTO> LayDashboardAsync()
        {
            var tongUser = await _context.Users.CountAsync();
            var tongSV = await _context.Users.CountAsync(u => u.maRole == 1);
            var tongNTD = await _context.Users.CountAsync(u => u.maRole == 2);
            var tongJob = await _context.JobPosts.CountAsync();
            var tongHopDong = await _context.HopDongs.CountAsync();
            var tongKyQuy = await _context.Wallets.SumAsync(w => w.soDuDongBang ?? 0);

            // Doanh thu ước tính: hoa hồng từ các giao dịch giải ngân hợp đồng
            var tongGiaiNgan = await _context.LichSuGiaoDichs
                .Where(x => x.loaiGiaoDich == "NhanTienHopDong" && x.trangThai == "ThanhCong")
                .SumAsync(x => (decimal?)x.soTien) ?? 0;
            var tongDoanhThu = tongGiaiNgan * 0.05m; // 5% hoa hồng

            // Doanh thu từ phí đăng tin tuyển dụng
            var tongPhiDangBai = await _context.LichSuGiaoDichs
                .Where(x => x.loaiGiaoDich == "ThuPhiDangBai" && x.trangThai == "ThanhCong")
                .SumAsync(x => (decimal?)x.soTien) ?? 0;

            var phiDangBaiHienTai = await _context.CauHinhPhiHoaHongs
                .Where(c => c.tenCauHinh == "PhiDangBai" && c.hieuLuc)
                .Select(c => c.giaTri)
                .FirstOrDefaultAsync();

            var recentTxs = await _context.LichSuGiaoDichs
                .OrderByDescending(x => x.ngayTao)
                .Take(10)
                .Select(x => new AdminRecentTransactionDTO
                {
                    MaGiaoDich = x.maGiaoDich,
                    MaWallet = x.maWallet,
                    LoaiGiaoDich = x.loaiGiaoDich,
                    SoTien = x.soTien,
                    NoiDung = x.noiDung,
                    TrangThai = x.trangThai,
                    NgayTao = x.ngayTao
                })
                .ToListAsync();

            return new AdminDashboardResponseDTO
            {
                TongSoUser = tongUser,
                TongSoSinhVien = tongSV,
                TongSoNhaTuyenDung = tongNTD,
                TongSoJob = tongJob,
                TongSoHopDong = tongHopDong,
                TongTienDangKyQuy = tongKyQuy,
                TongDoanhThuHeThong = tongDoanhThu + tongPhiDangBai,
                TongDoanhThuPhiDangBai = tongPhiDangBai,
                PhiDangBaiHienTai = phiDangBaiHienTai,
                GiaoDichMoiNhat = recentTxs
            };
        }

        // ==========================================
        // 2. QUẢN LÝ NGƯỜI DÙNG (AD-UC-01)
        // ==========================================
        public async Task<List<AdminUserItemDTO>> LayDanhSachUsersAsync(string? keyword, int? role, string? status)
        {
            var query = _context.Users
                .Include(u => u.Roles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(u => u.hotenUser.ToLower().Contains(kw) ||
                                         u.emailUser.ToLower().Contains(kw) ||
                                         (u.sdtUser != null && u.sdtUser.Contains(kw)) ||
                                         u.tenTaiKhoanUser.ToLower().Contains(kw));
            }

            if (role.HasValue && role.Value > 0)
            {
                query = query.Where(u => u.maRole == role.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(u => u.status == status);
            }

            var users = await query
                .OrderByDescending(u => u.ngayTao)
                .ToListAsync();

            var userIds = users.Select(u => u.maUser).ToList();

            // Lấy thông tin bổ sung: Wallet, Sinh viên, Nhà tuyển dụng
            var wallets = await _context.Wallets
                .Where(w => userIds.Contains(w.maUser))
                .ToDictionaryAsync(w => w.maUser, w => w.soDuKhaDung ?? 0);

            var sinhViens = await _context.FreelancerStudents
                .Where(s => userIds.Contains(s.maUser))
                .ToDictionaryAsync(s => s.maUser, s => s.tenTruong);

            var ntds = await _context.NhaTuyenDungs
                .Where(n => userIds.Contains(n.maUser))
                .ToDictionaryAsync(n => n.maUser, n => n.tencongty);

            return users.Select(u => new AdminUserItemDTO
            {
                MaUser = u.maUser,
                HotenUser = u.hotenUser,
                TenTaiKhoanUser = u.tenTaiKhoanUser,
                EmailUser = u.emailUser,
                SdtUser = u.sdtUser,
                Status = u.status ?? "HoatDong",
                NgayTao = u.ngayTao,
                MaRole = u.maRole,
                RoleName = u.Roles?.tenRole ?? (u.maRole == 1 ? "FreelancerStudent" : u.maRole == 2 ? "NhaTuyenDung" : "Admin"),
                Avatar = u.avatarUrl,
                ThongTinPhu = u.maRole == 1 
                    ? sinhViens.GetValueOrDefault(u.maUser) 
                    : u.maRole == 2 
                        ? ntds.GetValueOrDefault(u.maUser) 
                        : "Ban Quản trị",
                SoDuVi = wallets.GetValueOrDefault(u.maUser, 0)
            }).ToList();
        }

        // ==========================================
        // 3. KHÓA / MỞ KHÓA TÀI KHOẢN (AD-UC-02)
        // ==========================================
        public async Task<bool> XuLyTrangThaiUserAsync(AdminKhoaMoUserRequestDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.maUser == request.MaUser);
            if (user == null) return false;

            string trangThaiCu = user.status ?? "HoatDong";
            string trangThaiMoi = request.HanhDong == "KHOA_TAI_KHOAN" ? "BiKhoa" : "HoatDong";

            user.status = trangThaiMoi;

            // Tìm Admin xử lý
            int adminId = request.MaAdmin ?? 1;
            var adminObj = await _context.Admins.FirstOrDefaultAsync();
            if (adminObj != null) adminId = adminObj.maAdmin;

            var log = new LichSu_XuLyTaiKhoan
            {
                maUser = user.maUser,
                maAdmin = adminId,
                hanhDong = request.HanhDong,
                lyDo = request.LyDo,
                trangThaiTruocKhiXuLy = trangThaiCu,
                trangThaiSauKhiXuLy = trangThaiMoi,
                ngayXuLy = DateTime.Now
            };

            await _context.LichSu_XuLyTaiKhoans.AddAsync(log);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AdminLichSuXuLyUserDTO>> LayLichSuXuLyUserAsync(int? maUser)
        {
            var query = _context.LichSu_XuLyTaiKhoans
                .Include(l => l.User)
                .Include(l => l.Admin)
                .AsQueryable();

            if (maUser.HasValue && maUser.Value > 0)
            {
                query = query.Where(l => l.maUser == maUser.Value);
            }

            return await query
                .OrderByDescending(l => l.ngayXuLy)
                .Select(l => new AdminLichSuXuLyUserDTO
                {
                    MaLichSu = l.maLichSu,
                    MaUser = l.maUser,
                    TenUser = l.User != null ? l.User.hotenUser : $"User #{l.maUser}",
                    MaAdmin = l.maAdmin,
                    TenAdmin = l.Admin != null ? l.Admin.hotenAdmin : "Quản trị viên",
                    HanhDong = l.hanhDong,
                    LyDo = l.lyDo,
                    TrangThaiTruoc = l.trangThaiTruocKhiXuLy,
                    TrangThaiSau = l.trangThaiSauKhiXuLy,
                    NgayXuLy = l.ngayXuLy
                })
                .ToListAsync();
        }

        // ==========================================
        // 4. ĐIỀU CHỈNH SỐ DƯ VÍ (AD-UC-04)
        // ==========================================
        public async Task<List<AdminWalletItemDTO>> LayDanhSachWalletsAsync()
        {
            var wallets = await _context.Wallets
                .Include(w => w.User)
                    .ThenInclude(u => u!.Roles)
                .OrderByDescending(w => w.soDuKhaDung)
                .ToListAsync();

            return wallets.Select(w => new AdminWalletItemDTO
            {
                MaWallet = w.maWallet,
                MaUser = w.maUser,
                TenChuVi = w.User?.hotenUser ?? $"User #{w.maUser}",
                RoleName = w.User?.Roles?.tenRole ?? "Người dùng",
                SoDuKhaDung = w.soDuKhaDung ?? 0,
                SoDuDongBang = w.soDuDongBang ?? 0
            }).ToList();
        }

        public async Task<bool> DieuChinhSoDuViAsync(AdminDieuChinhViRequestDTO request)
        {
            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.maWallet == request.MaWallet);

            if (wallet == null) return false;

            decimal soDuTruoc = wallet.soDuKhaDung ?? 0;
            decimal soDuSau = soDuTruoc;

            if (request.LoaiDieuChinh == "CongTien")
            {
                soDuSau = soDuTruoc + request.SoTien;
            }
            else if (request.LoaiDieuChinh == "TruTien")
            {
                if (soDuTruoc < request.SoTien)
                {
                    throw new InvalidOperationException("Số dư khả dụng của ví không đủ để thực hiện thao tác trừ tiền!");
                }
                soDuSau = soDuTruoc - request.SoTien;
            }
            else
            {
                return false;
            }

            wallet.soDuKhaDung = soDuSau;

            // Tìm Admin
            int adminId = request.MaAdmin ?? 1;
            var adminObj = await _context.Admins.FirstOrDefaultAsync();
            if (adminObj != null) adminId = adminObj.maAdmin;

            var record = new DieuChinhSoDu
            {
                maWallet = wallet.maWallet,
                maAdmin = adminId,
                loaiDieuChinh = request.LoaiDieuChinh,
                soTienDieuChinh = request.SoTien,
                soDuTruoc = soDuTruoc,
                soDuSau = soDuSau,
                lyDo = request.LyDo,
                ngayDieuChinh = DateTime.Now
            };

            await _context.DieuChinhSoDus.AddAsync(record);

            // Ghi nhận LichSuGiaoDich
            var gd = new LichSuGiaoDich
            {
                maWallet = wallet.maWallet,
                loaiGiaoDich = request.LoaiDieuChinh == "CongTien" ? "AdminCongTien" : "AdminTruTien",
                soTien = request.SoTien,
                soDuTruocGD = soDuTruoc,
                soDuSauGD = soDuSau,
                noiDung = $"Admin điều chỉnh số dư: {request.LyDo}",
                phuongThucThanhToan = "ViNoiBo",
                trangThai = "ThanhCong",
                ngayTao = DateTime.Now
            };

            await _context.LichSuGiaoDichs.AddAsync(gd);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AdminLichSuDieuChinhViDTO>> LayLichSuDieuChinhViAsync(int? maWallet)
        {
            var query = _context.DieuChinhSoDus
                .Include(d => d.Wallet)
                    .ThenInclude(w => w!.User)
                .Include(d => d.Admin)
                .AsQueryable();

            if (maWallet.HasValue && maWallet.Value > 0)
            {
                query = query.Where(d => d.maWallet == maWallet.Value);
            }

            return await query
                .OrderByDescending(d => d.ngayDieuChinh)
                .Select(d => new AdminLichSuDieuChinhViDTO
                {
                    MaDieuChinh = d.maDieuChinh,
                    MaWallet = d.maWallet,
                    TenChuVi = d.Wallet != null && d.Wallet.User != null ? d.Wallet.User.hotenUser : $"Ví #{d.maWallet}",
                    MaAdmin = d.maAdmin,
                    TenAdmin = d.Admin != null ? d.Admin.hotenAdmin : "Quản trị viên",
                    LoaiDieuChinh = d.loaiDieuChinh,
                    SoTienDieuChinh = d.soTienDieuChinh,
                    SoDuTruoc = d.soDuTruoc,
                    SoDuSau = d.soDuSau,
                    LyDo = d.lyDo,
                    NgayDieuChinh = d.ngayDieuChinh
                })
                .ToListAsync();
        }

        // ==========================================
        // 5. QUẢN LÝ HỢP ĐỒNG (AD-UC-10)
        // ==========================================
        public async Task<List<AdminHopDongItemDTO>> LayDanhSachHopDongAsync()
        {
            var list = await _context.HopDongs
                .Include(h => h.JobPost)
                    .ThenInclude(j => j!.NhaTuyenDung)
                        .ThenInclude(n => n!.User)
                .Include(h => h.FreelancerStudent)
                    .ThenInclude(f => f!.User)
                .OrderByDescending(h => h.ngayBatDau)
                .ToListAsync();

            return list.Select(h => new AdminHopDongItemDTO
            {
                MaHD = h.maHD,
                MaJob = h.maJob,
                TenCongViec = h.JobPost?.tieude ?? "Dự án Freelance",
                MaFreelancer = h.maFreelancerStudent,
                TenFreelancer = h.FreelancerStudent?.User?.hotenUser ?? $"Freelancer #{h.maFreelancerStudent}",
                MaNhaTuyenDung = h.JobPost?.maNhaTuyenDung ?? 0,
                TenNhaTuyenDung = h.JobPost?.NhaTuyenDung?.tencongty ?? h.JobPost?.NhaTuyenDung?.User?.hotenUser ?? "Nhà tuyển dụng",
                SoTienKyQuy = h.sotienkyquy ?? 0,
                TrangThai = h.trangThai ?? "DangThucHien",
                HinhThucLamViec = h.hinhthuclamviec ?? "Online",
                NgayBatDau = h.ngayBatDau,
                NgayKetThuc = h.ngayKetThuc,
                DieuKhoan = h.dieuKhoan
            }).ToList();
        }

        public async Task<AdminHopDongItemDTO?> LayChiTietHopDongAsync(string maHD)
        {
            var h = await _context.HopDongs
                .Include(h => h.JobPost)
                    .ThenInclude(j => j!.NhaTuyenDung)
                        .ThenInclude(n => n!.User)
                .Include(h => h.FreelancerStudent)
                    .ThenInclude(f => f!.User)
                .FirstOrDefaultAsync(x => x.maHD == maHD);

            if (h == null) return null;

            return new AdminHopDongItemDTO
            {
                MaHD = h.maHD,
                MaJob = h.maJob,
                TenCongViec = h.JobPost?.tieude ?? "Dự án Freelance",
                MaFreelancer = h.maFreelancerStudent,
                TenFreelancer = h.FreelancerStudent?.User?.hotenUser ?? $"Freelancer #{h.maFreelancerStudent}",
                MaNhaTuyenDung = h.JobPost?.maNhaTuyenDung ?? 0,
                TenNhaTuyenDung = h.JobPost?.NhaTuyenDung?.tencongty ?? h.JobPost?.NhaTuyenDung?.User?.hotenUser ?? "Nhà tuyển dụng",
                SoTienKyQuy = h.sotienkyquy ?? 0,
                TrangThai = h.trangThai ?? "DangThucHien",
                HinhThucLamViec = h.hinhthuclamviec ?? "Online",
                NgayBatDau = h.ngayBatDau,
                NgayKetThuc = h.ngayKetThuc,
                DieuKhoan = h.dieuKhoan
            };
        }

        public Task<AdminMauHopDongDTO> LayMauHopDongAsync()
        {
            return Task.FromResult(_mauHopDong);
        }

        public Task<bool> CapNhatMauHopDongAsync(AdminMauHopDongDTO model)
        {
            if (model == null) return Task.FromResult(false);
            _mauHopDong.TenMauHopDong = model.TenMauHopDong;
            _mauHopDong.DieuKhoanChung = model.DieuKhoanChung;
            _mauHopDong.ThoiHanNghiemThuNgay = model.ThoiHanNghiemThuNgay;
            _mauHopDong.TyLeGiaiNganMacDinh = model.TyLeGiaiNganMacDinh;
            return Task.FromResult(true);
        }

        // ==========================================
        // 6. QUẢN LÝ & DUYỆT RÚT TIỀN (AD-UC-05)
        // ==========================================
        public async Task<List<AdminYeuCauRutTienItemDTO>> LayDanhSachYeuCauRutTienAsync(string? trangThai)
        {
            var query = _context.YeuCauRutTiens
                .Include(r => r.Wallet)
                    .ThenInclude(w => w!.User)
                        .ThenInclude(u => u!.Roles)
                .Include(r => r.Admin)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "TatCa")
            {
                query = query.Where(r => r.trangThai == trangThai);
            }

            var list = await query
                .OrderByDescending(r => r.ngayYeuCau)
                .ThenByDescending(r => r.maRutTien)
                .ToListAsync();

            return list.Select(r => new AdminYeuCauRutTienItemDTO
            {
                MaRutTien = r.maRutTien,
                MaWallet = r.maWallet,
                MaUser = r.Wallet?.maUser ?? 0,
                HoTenUser = r.Wallet?.User?.hotenUser ?? $"User #{r.Wallet?.maUser}",
                EmailUser = r.Wallet?.User?.emailUser ?? string.Empty,
                SdtUser = r.Wallet?.User?.sdtUser,
                RoleName = r.Wallet?.User?.Roles?.tenRole ?? (r.Wallet?.User?.maRole == 1 ? "Freelancer" : r.Wallet?.User?.maRole == 2 ? "Nhà tuyển dụng" : "Người dùng"),
                SoTienRut = r.soTienRut,
                TenNganHang = r.tenNganHang,
                SoTaiKhoan = r.soTaiKhoan,
                TenChuTaiKhoan = r.tenChuTaiKhoan,
                NgayYeuCau = r.ngayYeuCau,
                NgayXuLy = r.ngayXuLy,
                TrangThai = r.trangThai ?? "ChoXuLy",
                LyDoTuChoi = r.lyDoTuChoi,
                MaAdminXuLy = r.maAdminXuLy,
                TenAdminXuLy = r.Admin?.hotenAdmin,
                SoDuKhaDungVi = r.Wallet?.soDuKhaDung ?? 0,
                SoDuDongBangVi = r.Wallet?.soDuDongBang ?? 0
            }).ToList();
        }

        public async Task<bool> XuLyYeuCauRutTienAsync(AdminXuLyRutTienRequestDTO request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var yeuCau = await _context.YeuCauRutTiens
                    .Include(r => r.Wallet)
                    .FirstOrDefaultAsync(r => r.maRutTien == request.MaRutTien);

                if (yeuCau == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy yêu cầu rút tiền.");
                }

                if (yeuCau.Wallet == null)
                {
                    throw new InvalidOperationException("Không tìm thấy thông tin ví tương ứng.");
                }

                if (yeuCau.trangThai != "ChoXuLy")
                {
                    throw new InvalidOperationException($"Yêu cầu này đang ở trạng thái '{yeuCau.trangThai}', không thể xử lý lại.");
                }

                // Lấy thông tin Admin
                int adminId = request.MaAdmin ?? 1;
                var adminObj = await _context.Admins.FirstOrDefaultAsync();
                if (adminObj != null) adminId = adminObj.maAdmin;

                var wallet = yeuCau.Wallet;
                decimal soDuKhaDungHienTai = wallet.soDuKhaDung ?? 0;

                if (request.ChapNhan)
                {
                    // DUYỆT GIẢI NGÂN: Trừ số dư đóng băng
                    wallet.soDuDongBang = Math.Max(0, (wallet.soDuDongBang ?? 0) - yeuCau.soTienRut);
                    yeuCau.trangThai = "HoanThanh";
                    yeuCau.ngayXuLy = DateTime.Now;
                    yeuCau.maAdminXuLy = adminId;

                    _context.Wallets.Update(wallet);
                    _context.YeuCauRutTiens.Update(yeuCau);

                    var lichSu = new LichSuGiaoDich
                    {
                        maWallet = wallet.maWallet,
                        loaiGiaoDich = "RutTien",
                        soTien = yeuCau.soTienRut,
                        soDuTruocGD = soDuKhaDungHienTai,
                        soDuSauGD = soDuKhaDungHienTai,
                        noiDung = $"Admin duyệt giải ngân rút tiền về {yeuCau.tenNganHang} ({yeuCau.soTaiKhoan}) [Mô phỏng Demo]",
                        phuongThucThanhToan = "ChuyenKhoan",
                        maGiaoDichNgoai = $"RUT_{yeuCau.maRutTien}",
                        trangThai = "ThanhCong",
                        ngayTao = DateTime.Now
                    };
                    await _context.LichSuGiaoDichs.AddAsync(lichSu);
                }
                else
                {
                    // TỪ CHỐI: Hoàn tiền từ đóng băng lại số dư khả dụng
                    wallet.soDuDongBang = Math.Max(0, (wallet.soDuDongBang ?? 0) - yeuCau.soTienRut);
                    wallet.soDuKhaDung = soDuKhaDungHienTai + yeuCau.soTienRut;

                    yeuCau.trangThai = "TuChoi";
                    yeuCau.ngayXuLy = DateTime.Now;
                    yeuCau.lyDoTuChoi = string.IsNullOrWhiteSpace(request.LyDoTuChoi) ? "Yêu cầu rút tiền bị Quản trị viên từ chối." : request.LyDoTuChoi.Trim();
                    yeuCau.maAdminXuLy = adminId;

                    _context.Wallets.Update(wallet);
                    _context.YeuCauRutTiens.Update(yeuCau);

                    var lichSu = new LichSuGiaoDich
                    {
                        maWallet = wallet.maWallet,
                        loaiGiaoDich = "HoanTienRut",
                        soTien = yeuCau.soTienRut,
                        soDuTruocGD = soDuKhaDungHienTai,
                        soDuSauGD = wallet.soDuKhaDung.Value,
                        noiDung = $"Hoàn tiền yêu cầu rút #{yeuCau.maRutTien} về số dư khả dụng do Admin từ chối: {yeuCau.lyDoTuChoi}",
                        phuongThucThanhToan = "ViNoiBo",
                        maGiaoDichNgoai = $"RUT_{yeuCau.maRutTien}",
                        trangThai = "ThanhCong",
                        ngayTao = DateTime.Now
                    };
                    await _context.LichSuGiaoDichs.AddAsync(lichSu);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ==========================================
        // 7. CẤU HÌNH PHÍ SÀN & PHÍ ĐĂNG TIN
        // ==========================================
        public async Task<List<AdminCauHinhPhiDTO>> LayDanhSachCauHinhPhiAsync()
        {
            var ds = await _context.CauHinhPhiHoaHongs.AsNoTracking().ToListAsync();
            var admins = await _context.Admins.AsNoTracking().ToDictionaryAsync(a => a.maAdmin, a => a.hotenAdmin);

            return ds.Select(c => new AdminCauHinhPhiDTO
            {
                MaCauHinh = c.maCauHinh,
                TenCauHinh = c.tenCauHinh ?? string.Empty,
                GiaTri = c.giaTri,
                LoaiCauHinh = c.loaiCauHinh,
                MoTa = c.moTa,
                MaAdmin = c.maAdmin,
                TenAdmin = c.maAdmin.HasValue && admins.ContainsKey(c.maAdmin.Value) ? admins[c.maAdmin.Value] : null,
                NgayCapNhat = c.ngayCapNhat,
                HieuLuc = c.hieuLuc
            }).ToList();
        }

        public async Task<bool> CapNhatPhiDangBaiAsync(AdminCapNhatPhiDangBaiRequestDTO request)
        {
            if (request.PhiDangBai < 0)
            {
                throw new ArgumentException("Mức phí đăng tin không được nhỏ hơn 0.");
            }

            var cauHinh = await _context.CauHinhPhiHoaHongs
                .FirstOrDefaultAsync(c => c.tenCauHinh == "PhiDangBai");

            if (cauHinh == null)
            {
                cauHinh = new CauHinhPhiHoaHong
                {
                    tenCauHinh = "PhiDangBai",
                    giaTri = request.PhiDangBai,
                    loaiCauHinh = "TienMat",
                    moTa = "Phí xuất bản bài đăng tuyển dụng tiêu chuẩn trên hệ thống",
                    maAdmin = request.MaAdmin,
                    ngayCapNhat = DateTime.Now,
                    hieuLuc = true
                };
                await _context.CauHinhPhiHoaHongs.AddAsync(cauHinh);
            }
            else
            {
                cauHinh.giaTri = request.PhiDangBai;
                cauHinh.maAdmin = request.MaAdmin;
                cauHinh.ngayCapNhat = DateTime.Now;
                cauHinh.hieuLuc = true;
                _context.CauHinhPhiHoaHongs.Update(cauHinh);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
