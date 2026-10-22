namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class DanhGia_RequestDTO
    {
        public string maHD { get; set; } = string.Empty;
        public int soSaoTongThe { get; set; }
        public byte? soSaoChatLuong { get; set; }
        public byte? soSaoDungHan { get; set; }
        public byte? soSaoGiaoTiep { get; set; }
        public string? nhanXet { get; set; }
    }
}
