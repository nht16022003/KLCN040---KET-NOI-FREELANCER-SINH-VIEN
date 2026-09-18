using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<FreelancerSV> FreelancerSVs { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Wallet> Wallets { get; set; } = null!;
        public DbSet<JobPost> JobPosts { get; set; } = null!;
        public DbSet<HopDong> HopDongs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(u => u.KhachHang)
                      .WithOne(kh => kh.User)
                      .HasForeignKey<KhachHang>(kh => kh.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.FreelancerSV)
                      .WithOne(fr => fr.User)
                      .HasForeignKey<FreelancerSV>(fr => fr.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.Admin)
                      .WithOne(ad => ad.User)
                      .HasForeignKey<Admin>(ad => ad.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.CusID);
            });

            modelBuilder.Entity<FreelancerSV>(entity =>
            {
                entity.HasKey(e => e.FreeID);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.UserID);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.WalletID);
            });

            modelBuilder.Entity<JobPost>(entity =>
            {
                entity.HasKey(e => e.JobID);
            });

            modelBuilder.Entity<HopDong>(entity =>
            {
                entity.HasKey(e => e.MaHD);
            });
        }
    }
}
