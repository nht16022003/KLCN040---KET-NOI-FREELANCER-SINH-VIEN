using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        //1.Kiểm tra email đã tồn tại hay chưa
        Task<bool> kiemTraTonTaiEmailAsync(string email);
        Task<bool> kiemTraTenTaiKhoanTonTaiChuaAsync(string tentaikhoa);

        Task<Users> themUserAsync(Users user);

        Task<Roles> layThongTinRoleTheoMa(int maRole);

        Task<string> layTenRoleTheoUser(Users user, int maRole);

        Task<Users> timUserTheoTenTaiKhoan(string tentaikhoan);

        //Tuan Anh

        Task<Users?> timUserTheoMaUserAsync(int maUser);

        Task<Users> capNhatThongTinUserAsync(Users user);

    }
}