using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IBaiDangTimViecRepository
    {
        //Tuấn
        Task<BaiDangTimViecFreelancerStudent> themBaiDangAsync(BaiDangTimViecFreelancerStudent baiDang);
        //Tuấn
        Task<List<BaiDangTimViecFreelancerStudent>> layDanhSachTheoFreelancerAsync(int maFreelancerStudent);
        //Tuấn
        Task<List<BaiDangTimViecFreelancerStudent>> layTatCaBaiDangHienThiAsync();
        //Tuấn
        Task<BaiDangTimViecFreelancerStudent?> layTheoMaAsync(string maBaiDang);
        //Tuấn
        Task<bool> doiTrangThaiAsync(string maBaiDang, string trangthaiMoi);
        //Tuấn
        Task<bool> xoaBaiDangAsync(string maBaiDang);

        //Tuấn
        Task<bool> capNhatBaiDangAsync(BaiDangTimViecFreelancerStudent baiDang);
    }
}
