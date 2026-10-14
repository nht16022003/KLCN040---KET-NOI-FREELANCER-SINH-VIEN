using System.ComponentModel.DataAnnotations;


public class DoiMatKhau_RequestDTO
{
    public int maUser { get; set; }

    [Required]
    public string matKhauHienTai { get; set; } = string.Empty;

    [Required]
    public string matKhauMoi { get; set; } = string.Empty;

    [Required]
    public string xacNhanMatKhauMoi { get; set; } = string.Empty;
}
