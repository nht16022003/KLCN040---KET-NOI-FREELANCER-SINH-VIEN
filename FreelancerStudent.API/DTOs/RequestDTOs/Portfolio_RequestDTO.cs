using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.API.DTOs.ReponsesDTO
{
    public class DuAnTrongPortfolio_RequestDTO
    {
        [Required]
        public int maFreelancerStudents { get; set; }

        [Required]
        [MaxLength(200)]
        public string tenDuAn { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? vaiTro { get; set; }

        public string? moTa { get; set; }

        [MaxLength(50)]
        public string? congnghe { get; set; }

        [MaxLength(255)]
        public string? linkGithub { get; set; }

        [MaxLength(255)]
        public string? linkDemo { get; set; }

        [MaxLength(255)]
        public string? link_file { get; set; }
    }

    public class DuAnTrongPortfolio_UpdateRequestDTO : DuAnTrongPortfolio_RequestDTO
    {
        [Required]
        public string maDA { get; set; } = string.Empty;
    }

    public class DuAnNoiBat_RequestDTO
    {
        [Required]
        public int maFreelancerStudents { get; set; }

        public List<string> maDAs { get; set; } = new();
    }
}