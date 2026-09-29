using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface INhaTuyenDungRepository
    {
        //Lấy tất cả nhà tuyển dụng
        Task<List<NhaTuyenDung>> layTatCaNhaTuyenDungAsync();

        //Thêm nhà tuyển dụng dựa vào maRole của Users
        Task<NhaTuyenDung> themNhaTuyenDungDuaVaoMaRoleCuaUsers(Users user, int maRole);
        Task<NhaTuyenDung?> layNhaTuyenDungTheoMaUserAsync(int maUser);
    }
}