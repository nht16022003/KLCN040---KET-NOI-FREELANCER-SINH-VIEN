using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        // 1. Bảng Users chính
        public DbSet<User> Users { get; set; } = null!;

        // 2. 3 Bảng hồ sơ tương ứng 3 loại người dùng (loaiUser)
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<FreelancerSV> FreelancerSVs { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Unique Constraints theo đúng Schema CSDL
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<KhachHang>()
                .HasIndex(k => k.UserID)
                .IsUnique();

            modelBuilder.Entity<FreelancerSV>()
                .HasIndex(f => f.UserID)
                .IsUnique();
        }
    }
}
