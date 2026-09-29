using System.ComponentModel.DataAnnotations;

public class DangNhap_RequestDTO
{

    [Required(ErrorMessage = "Tên tài khoản hoặc email không được để trống")]
    public string tentaikhoan_email { get; set; } = string.Empty;


    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải tối thiểu 6 ký tự")]
    public string password { get; set; } = string.Empty;

}