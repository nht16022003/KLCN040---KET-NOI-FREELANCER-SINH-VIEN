using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.ReponseDTOs;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Services
{
    public class NhaTuyenDungService : INhaTuyenDungService
    {
        private readonly INhaTuyenDungRepository _nhaTuyenDungRepository;
        private readonly ApplicationDBContext _context;

        public NhaTuyenDungService(INhaTuyenDungRepository nhaTuyenDungRepository, ApplicationDBContext context)
        {
            _nhaTuyenDungRepository = nhaTuyenDungRepository;
            _context = context;
        }


        //
        public async Task<List<NhaTuyenDung_ResponseDTO>> layDanhSachNhaTuyenDungAsync()
        {
            var ds_layTuSQL = await _nhaTuyenDungRepository.layTatCaNhaTuyenDungAsync();

            var dsResult = ds_layTuSQL.Select(ntd => new NhaTuyenDung_ResponseDTO
            {
                maNhaTuyenDung = ntd.maNhaTuyenDung,
                maUser = ntd.maUser,
                hotenUser = ntd.User?.hotenUser,
                emailUser = ntd.User?.emailUser,
                sdtUser = ntd.User?.sdtUser,
                tencongty = ntd.tencongty,
                linhvuc = ntd.linhvuc,
                diachi = ntd.diachi,
                gioithieu = ntd.gioithieu,
                avatar = ntd.User?.avatarUrl,
                logo = ntd.logo,
                sosaodanhgia = ntd.sosaodanhgia,
                soLuotDanhGia = ntd.soLuotDanhGia,
                trangthai = ntd.trangthai,
                ngayDangKy = ntd.ngayDangKy
            }).ToList();

            return dsResult;
        }

        public async Task<NhaTuyenDungProfile_ReponseDTO?> layProfileNhaTuyenDung(int maNhaTuyenDung)
        {
            if (maNhaTuyenDung <= 0)
            {
                return null;
            }

            var ntd = await _nhaTuyenDungRepository.layNhaTuyenDungTheoMaAsync(maNhaTuyenDung);
            return await BuildProfileResponse(ntd);
        }

        public async Task<NhaTuyenDungProfile_ReponseDTO?> layProfileNhaTuyenDungTheoMaUser(int maUser)
        {
            if (maUser <= 0)
            {
                return null;
            }

            var ntd = await _nhaTuyenDungRepository.layNhaTuyenDungTheoMaUserAsync(maUser);
            return await BuildProfileResponse(ntd);
        }

        private async Task<NhaTuyenDungProfile_ReponseDTO?> BuildProfileResponse(NhaTuyenDung? ntd)
        {
            if (ntd == null)
            {
                return null;
            }

            // Danh sách bài đăng tuyển dụng của NTD
            var jobPosts = await _context.JobPosts.AsNoTracking()
                .Where(j => j.maNhaTuyenDung == ntd.maNhaTuyenDung)
                .OrderByDescending(j => j.thoigiandangtuyen)
                .ToListAsync();

            // Thống kê đánh giá
            var reviewsQuery = _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => d.maNguoiDuocDanhGia == ntd.maUser);
            var tongDanhGia = await reviewsQuery.CountAsync();
            var danhGiaTot = await reviewsQuery.CountAsync(d => d.soSao >= 4);
            var tyLeHaiLong = tongDanhGia == 0 ? 0 : Math.Round(danhGiaTot * 100d / tongDanhGia, 1);

            // Thống kê hợp đồng và freelancer
            var hopDongsQuery = _context.HopDongs.AsNoTracking()
                .Where(h => h.JobPost != null && h.JobPost.maNhaTuyenDung == ntd.maNhaTuyenDung);

            var soHopDongHoanThanh = await hopDongsQuery
                .CountAsync(h => h.trangThai == "DaThanhToan" || h.trangThai == "DaHoanThanh");

            var soFreelancerHopTac = await hopDongsQuery
                .Where(h => h.trangThai == "DaThanhToan" || h.trangThai == "DaHoanThanh" || h.trangThai == "DangThucHien")
                .Select(h => h.maFreelancerStudent)
                .Distinct()
                .CountAsync();

            // Top đánh giá gần đây
            var danhGiaGanDay = await _context.DanhGia_NhanXets.AsNoTracking()
                .Where(d => d.maNguoiDuocDanhGia == ntd.maUser)
                .Include(d => d.NguoiDanhGia)
                .Include(d => d.HopDong).ThenInclude(h => h!.JobPost)
                .OrderByDescending(d => d.ngayDanhGia)
                .Take(5)
                .ToListAsync();

            return new NhaTuyenDungProfile_ReponseDTO
            {
                User = new ProfileUserResponse
                {
                    TenTaiKhoanUser = ntd.User?.tenTaiKhoanUser ?? string.Empty,
                    HotenUser = ntd.User?.hotenUser ?? string.Empty,
                    Email = ntd.User?.emailUser,
                    Sodienthoai = ntd.User?.sdtUser,
                    AvatarUrl = ntd.User?.avatarUrl,
                    Ngaysinh = ntd.User?.ngaysinh,
                    NgayTao = ntd.User?.ngayTao ?? ntd.ngayDangKy
                },
                NhaTuyenDung = new ProfileNhaTuyenDungResponse
                {
                    MaNhaTuyenDung = ntd.maNhaTuyenDung,
                    MaUser = ntd.maUser,
                    Tencongty = ntd.tencongty,
                    Linhvuc = ntd.linhvuc,
                    Diachi = ntd.diachi,
                    Gioithieu = ntd.gioithieu,
                    Link = ntd.link,
                    Logo = ntd.logo,
                    NgayDangKy = ntd.ngayDangKy,
                    Trangthai = ntd.trangthai,
                    Sosaodanhgia = ntd.sosaodanhgia ?? 0,
                    SoLuotDanhGia = ntd.soLuotDanhGia,
                    TyLeHaiLong = tyLeHaiLong,
                    TongTinTuyenDung = jobPosts.Count,
                    TinDangTuyen = jobPosts.Count(j => j.status == "DangTuyen"),
                    SoHopDongHoanThanh = soHopDongHoanThanh,
                    SoFreelancerHopTac = soFreelancerHopTac,
                    DanhGiaGanDay = danhGiaGanDay.Select(d => new DanhGia_ReponseDTO
                    {
                        maDanhGia = d.maDanhGia,
                        maHD = d.maHD,
                        tenNguoiDanhGia = d.NguoiDanhGia?.hotenUser ?? "Freelancer",
                        avatar = d.NguoiDanhGia?.avatarUrl,
                        tenCongViec = d.HopDong?.JobPost?.tieude ?? string.Empty,
                        soSaoTongThe = d.soSao,
                        soSaoChatLuong = d.soSaoChatLuong,
                        soSaoDungHan = d.soSaoDungHan,
                        soSaoGiaoTiep = d.soSaoGiaoTiep,
                        nhanXet = d.nhanXet,
                        ngayDanhGia = d.ngayDanhGia,
                        hanChinhSua = d.ngayDanhGia.AddDays(7),
                        daChinhSua = d.soLanChinhSua > 0,
                        ngayCapNhat = d.ngayCapNhat
                    }).ToList()
                },
                DanhSachTinTuyenDung = jobPosts.Select(j => new ProfileJobPostResponse
                {
                    MaJob = j.maJob,
                    Tieude = j.tieude,
                    Mota = j.mota,
                    Thulao = j.thulao,
                    Kynangyeucau = j.kynangyeucau,
                    Status = j.status,
                    Thoigiandangtuyen = j.thoigiandangtuyen,
                    Thoigiandukienhoanthanh = j.thoigiandukienhoanthanh,
                    Soluongtuyen = j.soluongtuyen
                }).ToList()
            };
        }
    }
}