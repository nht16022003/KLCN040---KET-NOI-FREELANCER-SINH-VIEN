namespace FreelancerStudent.API.DTOs.ReponseDTOs
{
    public class DanhGia_ReponseDTO
    {
        public int maDanhGia { get; set; }
        public string maHD { get; set; } = string.Empty;
        public string tenNguoiDanhGia { get; set; } = string.Empty;
        public string? avatar { get; set; }
        public string tenCongViec { get; set; } = string.Empty;
        public int soSaoTongThe { get; set; }
        public byte? soSaoChatLuong { get; set; }
        public byte? soSaoDungHan { get; set; }
        public byte? soSaoGiaoTiep { get; set; }
        public string? nhanXet { get; set; }
        public DateTime ngayDanhGia { get; set; }
        public DateTime? ngayCapNhat { get; set; }
        public bool daChinhSua { get; set; }
        public bool coTheChinhSua { get; set; }
        public DateTime? hanChinhSua { get; set; }
    }
}
