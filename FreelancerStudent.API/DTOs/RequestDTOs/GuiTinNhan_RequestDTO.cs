using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class GuiTinNhan_RequestDTO
    {
        [Required(ErrorMessage = "Mã phòng chat không được để trống")]
        public int maPhongChat { get; set; }

        [Required(ErrorMessage = "Mã người gửi không được để trống")]
        public int maNguoiGui { get; set; } // maUser của người đang gửi tin

        [Required(ErrorMessage = "Nội dung tin nhắn không được để trống")]
        public string noiDung { get; set; } = string.Empty;

        public string? fileDinhKem { get; set; } // Đường dẫn file/ảnh đính kèm (nếu có)

        public string loaiTinNhan { get; set; } = "Text"; // "Text", "Image", "File"
    }
}
