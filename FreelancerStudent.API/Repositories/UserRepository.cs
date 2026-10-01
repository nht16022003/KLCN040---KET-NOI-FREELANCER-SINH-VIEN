using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;

        }

        //Kiểm tra email trùng
        public async Task<bool> kiemTraTonTaiEmailAsync(string email)
        {
            var ketqua = await _context.Users.FirstOrDefaultAsync(u => u.emailUser.ToLower() == email.ToLower());

            if (ketqua == null)
            {
                return false;
            }

            return true;

        }

        //Kiểm tra tentaikhoan trùng
        public async Task<bool> kiemTraTenTaiKhoanTonTaiChuaAsync(string tentaikhoa)
        {
            var ketqua = await _context.Users.FirstOrDefaultAsync(u => u.tenTaiKhoanUser.ToLower() == tentaikhoa.ToLower());

            if (ketqua == null)
            {
                return false;
            }

            return true;
        }

        public async Task<Users> themUserAsync(Users user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(); //Lưu xuống SQL
            return user;
        }

        //Lấy role theo mã
        public async Task<Roles> layThongTinRoleTheoMa(int maRole)
        {
            var ketqua = await _context.Roles.FirstOrDefaultAsync(u => u.maRole == maRole);

            if (ketqua == null)
            {
                return null!;
            }

            return ketqua;

        }

        public async Task<string> layTenRoleTheoUser(Users user, int maRole)
        {
            var truyvan = await _context.Users.Include(u => u.Roles).ToListAsync();
            var ketqua_laytenrole = truyvan.Where(u => u.maUser == user.maUser && u.maRole == maRole).Select(u => u.Roles!.tenRole).FirstOrDefault();
            return ketqua_laytenrole!;
        }


        //Lấy thông tin Users
        public async Task<Users> timUserTheoTenTaiKhoan(string tentaikhoan)
        {
            var ketqua = await _context.Users.FirstOrDefaultAsync(u => u.tenTaiKhoanUser.Trim().ToLower() == tentaikhoan.Trim().ToLower()
         || u.emailUser.Trim().ToLower() == tentaikhoan.Trim().ToLower());
            return ketqua!;
        }


        //Tuấn Anh

        public async Task<Users?> timUserTheoMaUserAsync(int maUser)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.maUser == maUser);
        }
        //Tuấn Anh

        public async Task<Users> capNhatThongTinUserAsync(Users user)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}