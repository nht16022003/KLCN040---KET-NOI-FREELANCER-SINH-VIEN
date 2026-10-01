using System.ComponentModel.DataAnnotations;


public class CapNhatThongTinTaiKhoan_RequestDTO
{
    [Required]
    public int maUser { get; set; }

    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(100)]
    public string hovaten { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? sodienthoai { get; set; }
}
