using FreelancerStudent.API.Data;
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
            var dsNhaTuyenDung = await _context.NhaTuyenDungs.ToListAsync();
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


    }
}