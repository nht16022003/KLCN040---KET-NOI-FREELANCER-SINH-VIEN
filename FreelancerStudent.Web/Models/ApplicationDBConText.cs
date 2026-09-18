using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.Web.Models.Entities
{
    public class ApplicationDBConText : DbContext
    {
        public ApplicationDBConText(DbContextOptions<ApplicationDBConText> options) : base(options) { }

        public DbSet<Users> User { get; set; }
        public DbSet<FreelancerSV> FreelancerSV { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bắt buộc Entity Users phải trỏ đúng vào tên bảng trong SQL Server
            modelBuilder.Entity<Users>().ToTable("Users");
        }
    }
}
