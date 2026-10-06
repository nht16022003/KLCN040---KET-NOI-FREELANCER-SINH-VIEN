using FreelancerStudent.API.Models;
using Microsoft.EntityFrameworkCore;

//Data là tầng kết nối giữa C# và Cơ sở dữ liệu

namespace FreelancerStudent.API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        //Khai báo các bảng trong Database, tương ứng với các bảng trong CSDL
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<FreelamcerStudents> FreelamcerStudents { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<ChuyenNganh> ChuyenNganhs { get; set; }

        public DbSet<KyNang> KyNangs { get; set; }

        public DbSet<NhaTuyenDung> NhaTuyenDungs { get; set; }

        public DbSet<JobPost> JobPosts { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<GiaoDichNapTien> GiaoDichNapTiens { get; set; }

        //Sử dụng phương thức OnModelCreating và dối tượng modelBuilder để cấu hình toàn bộ quy tắc CSDL, quan hệ giữa các bảng, khóa chính/ ngoại,
        //index và dữ liệu mẫu
        //Cơ chế hoạt động
        //EF CORE sẽ tự động gọi phương thức này đúng 1 lần duy nhất khi ứng dụng khởi động và lần đầu tiên kết nối vào CSDL để
        //xây dựng sơ đồ dữ liệu trong bộ nhớ
        //Lệnh base.OnModelCreating(modelBuilder) ở dòng đầu tiên là bắt buộc để áp dụng tất cả cấu hình này mặc định từ class cha

        /*
        Cấu hình giữa các bảng

        modelBuilder.Entity<Users>()
        .HasOne(u => u.Role)                 // 1 User chỉ thuộc về 1 Role
        .WithMany(r => r.User)               // 1 Role có danh sách nhiều Users
        .HasForeignKey(u => u.maRole)        // Cột khóa ngoại nằm ở bảng Users là maRole
        .OnDelete(DeleteBehavior.Restrict);  // Quy tắc khi xóa (Xem giải thích bên dưới)

        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //Cấu hình Roles và Users
            modelBuilder.Entity<Roles>
            (entity =>
            {
                entity.HasKey(r => r.maRole); //r là đối tượng trong Roles

                entity.HasMany(r => r.Users).WithOne(u => u.Roles).HasForeignKey(u => u.maRole); //r là đối tượng trong Roles và u.maRole trong Users chính là khóa ngoại đến Roles

            });

            modelBuilder.Entity<FreelamcerStudents>(entity =>
            {
                entity.HasKey(f => f.maFreelancerStudents);

                entity.HasOne(f => f.User).WithOne(u => u.FreelamcerStudents).HasForeignKey<FreelamcerStudents>(f => f.maUser);
                //Một freelancer có một user, ở phía user cũng chỉ có 1 freelancerstudent ở freelancer thì có khóa ngoại là f.mauuser

                entity.HasOne(f => f.ChuyenNganh).WithMany(c => c.FreelancerStudents).HasForeignKey(f => f.maChuyenNganh);
                //Một freelancer có một chuyên ngành

                entity.HasMany(f => f.FreelancerStudent_KyNangs).WithOne(x => x.FreelancerStudent).HasForeignKey(x => x.maFreelancerStudents);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(a => a.maAdmin);

                entity.HasOne(a => a.User).WithOne(u => u.Admin).HasForeignKey<Admin>(u => u.maAdmin);
            });

            modelBuilder.Entity<KyNang>(entity =>
            {
                entity.HasKey(n => n.maKyNang);
            });


            modelBuilder.Entity<FreelancerStudent_KyNang>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.maFreelancerStudents,
                    x.maKyNang
                });
                entity.HasOne(x => x.KyNang).WithMany(k => k.FreelancerStudent_KyNangs).HasForeignKey(k => k.maKyNang);
            });


            modelBuilder.Entity<NhaTuyenDung>(entity =>
            {
                entity.HasKey(n => n.maNhaTuyenDung);

                entity.HasIndex(n => n.maUser);

                entity.HasOne(n => n.User).WithOne(u => u.NhaTuyenDung).HasForeignKey<NhaTuyenDung>(n => n.maUser);
            });

            modelBuilder.Entity<JobPost>(entity =>
            {
                entity.HasKey(j => j.maJob);

                entity.HasOne(j => j.NhaTuyenDung).WithMany(n => n.JobPosts).HasForeignKey(f => f.maNhaTuyenDung)
                .OnDelete(DeleteBehavior.Cascade); //Nếu xóa nhà tuyển dụng thì xóa luôn bài đăng của ẻm
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(w => w.maWallet);

                entity.HasOne(w => w.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(w => w.maUser)
                .OnDelete(DeleteBehavior.Cascade); //Xóa Users thì xóa luôn ví 
            });
            modelBuilder.Entity<GiaoDichNapTien>(entity =>
{
                entity.ToTable("GiaoDichNapTien");
                entity.HasKey(g => g.maNapTien);

                entity.HasIndex(g => g.maGiaoDich)
                    .IsUnique();

                entity.HasOne(g => g.Wallet)
                    .WithMany()
                    .HasForeignKey(g => g.maWallet)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}