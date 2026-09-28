using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{

    public class FreelancerStudent_ReponseDTO
    {
        public int maFreelancerStudents { get; set; }


        public string maChuyenNganh { get; set; } = string.Empty;

        public string tenChuyenNganh { get; set; } = string.Empty;


        public int maUser { get; set; }

        public string? tenUser { get; set; }

        public string maTruong { get; set; } = string.Empty;

        public string tenTruong { get; set; } = string.Empty;

        public string diaDiemTruong { get; set; } = string.Empty;
        public string diaDiemFreelancerStudent { get; set; } = string.Empty;

        public string? ngonNgu { get; set; }

        public string? kyNangCoBan { get; set; }

        public int namThu { get; set; } = 1;

        public double GPA { get; set; } = 0;

        public string nienKhoa { get; set; } = string.Empty;

        public string? gioithieu { get; set; }


        public string? avatar { get; set; }


        public bool trangthaiNhanViec { get; set; } = true;

        public decimal? chiPhiTu { get; set; }


    }
}