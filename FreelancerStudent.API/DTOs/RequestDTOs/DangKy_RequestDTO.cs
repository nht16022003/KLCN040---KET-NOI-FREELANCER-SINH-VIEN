using System.ComponentModel.DataAnnotations;

public class DangKy_RequestDTO
{
    [Required(ErrorMessage = "Họ và tên không được để trống")]

    public string hovaten { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên tài khoản không được để trống")]
    public string tentaikhoan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không được để trống")]
    public string? sodienthoai { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải tối thiểu 6 ký tự")]
    public string password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải tối thiểu 6 ký tự")]
    public string xacnhanmatkhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn loại tài khoản")]
    public int marole { get; set; }
}