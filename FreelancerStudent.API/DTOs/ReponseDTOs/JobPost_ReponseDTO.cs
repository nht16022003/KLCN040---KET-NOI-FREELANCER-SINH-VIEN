namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class JobPost_ReponseDTO
    {

        public string maJob { get; set; } = string.Empty;

        public int maNhaTuyenDung { get; set; }


        public string tieude { get; set; } = string.Empty;

        public string? mota { get; set; }


        public string? kynangyeucau { get; set; }

        public decimal? thulao { get; set; }


        public string? fileDinhKem { get; set; } = string.Empty;


        public decimal phiDangBai { get; set; }


        public DateTime thoigiandangtuyen { get; set; } = DateTime.UtcNow;


        public DateTime thoigiandukienhoanthanh { get; set; }


        public string? status { get; set; } = "DangTuyen";

        public int? soluongtuyen { get; set; }
        public int soLuongUngTuyen { get; set; }

        public string? tenCongTy { get; set; }
        public string? avatarNhaTuyenDung { get; set; }


    }
}