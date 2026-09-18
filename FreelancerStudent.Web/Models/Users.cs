using System.ComponentModel.DataAnnotations;

namespace FreelancerStudent.Web.Models.Entities
{
    public class Users
    {
        [Key]
        public string userID { get; set; } = string.Empty;
        public string userName { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? phoneNumber { get; set; }
        public string loaiUser { get; set; } = string.Empty;
        public string status { get; set; } = "ACTIVE";
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        public virtual FreelancerSV? FreelancerSV { get; set; }
    }
}
