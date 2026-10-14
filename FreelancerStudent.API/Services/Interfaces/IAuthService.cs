using FreelancerStudent.API.DTOs.ReponsesDTO;


namespace FreelancerStudent.API.Services.Interfaces
{
    public interface IAuthService
    {
        //Nhận rì quét và trả về ReponsesDTO sau khi xử lý đăng ký
        Task<DangKy_ReponseDTO> DangKyTaiKhoanAsync(DangKy_RequestDTO request);

        Task<DangNhap_ReponseDTO> DangNhapTaiKhoanAsync(DangNhap_RequestDTO request);


        //Tuấn Anh
        Task<ThongTinTaiKhoan_ReponseDTO> layThongTinTaiKhoanAsync(int maUser);

        //Tuấn Anh
        Task<ThongTinTaiKhoan_ReponseDTO> capNhatThongTinTaiKhoanAsync(CapNhatThongTinTaiKhoan_RequestDTO request);

        //Tuấn Anh
        Task<bool> capNhatAvatarAsync(CapNhatAvatar_RequestDTO request);

        //Tuấn Anh
        Task doiMatKhauAsync(DoiMatKhau_RequestDTO request);
    }
}