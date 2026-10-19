namespace FreelancerStudent.API.DTOs.RequestDTOs
{
    public class DanhGiaGiaiDoan_RequestDTO
    {
        public string maHD { get; set; } = string.Empty;
        public int? maGiaiDoan { get; set; } // null: đánh giá toàn bộ mốc giai đoạn trong hợp đồng
        public string trangThai { get; set; } = "Dat"; // "Dat" hoặc "ChuaDat"
    }
}
