using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.ViewModels
{
    public class JobPostViewModel
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
    }
}