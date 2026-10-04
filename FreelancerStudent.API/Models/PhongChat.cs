using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerStudent.API.Models
{
    [Table("PhongChat")]
    public class PhongChat
    {
        [Key]
        public int maPhongChat { get; set; }
        public int maUserClient { get; set; }
        public int maFreelancerStudent { get; set; }
        public string? maJob { get; set; }
        public DateTime ngayTao { get; set; } = DateTime.Now;
        public string trangThai { get; set; } = "Active";

        [ForeignKey("maUserClient")]
        public virtual Users? UserClient { get; set; }

        [ForeignKey("maFreelancerStudent")]
        public virtual FreelamcerStudents? FreelancerStudent { get; set; }

        [ForeignKey("maJob")]
        public virtual JobPost? JobPost { get; set; }

        public virtual ICollection<TinNhanChat> TinNhanChats { get; set; } = new List<TinNhanChat>();
    }
}
