using FreelancerStudent.API.DTOs.RequestDTOs;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface INhaTuyenDungRepository
    {
        //Lấy tất cả nhà tuyển dụng
        Task<List<NhaTuyenDung>> layTatCaNhaTuyenDungAsync();

        //Tuấn
        //Thêm nhà tuyển dụng dựa vào maRole của Users
        Task<NhaTuyenDung> themNhaTuyenDungDuaVaoMaRoleCuaUsers(Users user, int maRole);

        //Tuan Anh
        Task<NhaTuyenDung?> layNhaTuyenDungTheoMaUserAsync(int maUser);

        Task<NhaTuyenDung?> layNhaTuyenDungTheoMaAsync(int maNhaTuyenDung);

        //Cập nhật thông tin cơ bản Nhà tuyển dụng
        Task<bool> capNhatThongTinNTDAsync(CapNhatThongTinNhaTuyenDung_RequestDTO dto);

        //Cập nhật giới thiệu đơn vị
        Task<bool> capNhatGioiThieuNTDAsync(int maUser, string? gioithieu);

        //Cập nhật logo doanh nghiệp
        Task<bool> capNhatLogoNTDAsync(int maUser, string logoUrl);
    }
}