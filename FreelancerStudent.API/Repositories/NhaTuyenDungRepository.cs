using FreelancerStudent.API.Data;
using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class NhaTuyenDungRepository : INhaTuyenDungRepository
    {
        private readonly ApplicationDBContext _context;

        public NhaTuyenDungRepository(ApplicationDBContext context)
        {
            _context = context;

        }


        public async Task<List<NhaTuyenDung>> layTatCaNhaTuyenDungAsync()
        {
            var dsNhaTuyenDung = await _context.NhaTuyenDungs
                .Include(x => x.User)
                .ToListAsync();
            return dsNhaTuyenDung;
        }

        public async Task<NhaTuyenDung> themNhaTuyenDungDuaVaoMaRoleCuaUsers(Users user, int maRole)
        {
            if (maRole != 2)
            {
                return null!; // Không phải Nhà tuyển dụng thì không tạo
            }

            var ntdMoi = new NhaTuyenDung
            {
                maUser = user.maUser,
                tencongty = user.hotenUser,
                ngayDangKy = DateTime.UtcNow,
                trangthai = "Active",
                sosaodanhgia = 0,
                gioithieu = "Chưa cập nhật giới thiệu của nhà tuyển dụng",
                linhvuc = "Chưa cập nhật lĩnh vực của nhà tuyển dụng",
                diachi = "Chưa cập nhật địa chỉ của nhà tuyển dụng"
            };
            await _context.NhaTuyenDungs.AddAsync(ntdMoi);
            await _context.SaveChangesAsync();

            return ntdMoi;
        }

        //Tuan Anh
        public async Task<NhaTuyenDung?> layNhaTuyenDungTheoMaUserAsync(int maUser)
        {
            return await _context.NhaTuyenDungs
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.maUser == maUser);
        }

        public async Task<NhaTuyenDung?> layNhaTuyenDungTheoMaAsync(int maNhaTuyenDung)
        {
            return await _context.NhaTuyenDungs
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.maNhaTuyenDung == maNhaTuyenDung);
        }

        public async Task<bool> capNhatThongTinNTDAsync(CapNhatThongTinNhaTuyenDung_RequestDTO dto)
        {
            var ntd = await _context.NhaTuyenDungs
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.maUser == dto.maUser);

            if (ntd == null)
            {
                return false;
            }

            ntd.tencongty = dto.tencongty;
            ntd.linhvuc = dto.linhvuc;
            ntd.diachi = dto.diachi;
            ntd.link = dto.link;

            if (ntd.User != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.hotenNguoiDaiDien))
                {
                    ntd.User.hotenUser = dto.hotenNguoiDaiDien;
                }
                ntd.User.sdtUser = dto.sdtLienHe;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> capNhatGioiThieuNTDAsync(int maUser, string? gioithieu)
        {
            var ntd = await _context.NhaTuyenDungs
                .FirstOrDefaultAsync(x => x.maUser == maUser);

            if (ntd == null)
            {
                return false;
            }

            ntd.gioithieu = gioithieu;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> capNhatLogoNTDAsync(int maUser, string logoUrl)
        {
            var ntd = await _context.NhaTuyenDungs
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.maUser == maUser);

            if (ntd == null)
            {
                return false;
            }

            ntd.logo = logoUrl;
            if (ntd.User != null)
            {
                ntd.User.avatarUrl = logoUrl;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}