
using Microsoft.EntityFrameworkCore;
using FreelancerStudent.API.Models;
//Data là tầng kết nối giữa C# và Cơ sở dữ liệu

namespace FreelancerStudent.API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        //Khai báo các bảng trong Database, tương ứng với các bảng trong CSDL
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<FreelancerStudents> FreelancerStudents { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<ChuyenNganh> ChuyenNganhs { get; set; }

        public DbSet<KyNang> KyNangs { get; set; }

        public DbSet<NhaTuyenDung> NhaTuyenDungs { get; set; }

        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<LichSuGiaoDich> LichSuGiaoDichs { get; set; }

        public DbSet<UngTuyen> UngTuyens { get; set; }

        public DbSet<FreelancerStudent_KyNang> FreelancerStudent_KyNangs { get; set; }

        public DbSet<BaiDangTimViecFreelancerStudent> BaiDangTimViecFreelancerStudents { get; set; }

        public DbSet<PhongChat> PhongChats { get; set; }
        public DbSet<TinNhanChat> TinNhanChats { get; set; }

        public DbSet<GiaoDichNapTien> GiaoDichNapTiens { get; set; }

        public DbSet<Portfolio> Portfolios { get; set; }

        public DbSet<DuAnTrongPortfolio> DuAnTrongPortfolios { get; set; }

        public DbSet<GiaiDoan_HopDong> GiaiDoan_HopDongs { get; set; }

        public DbSet<Task_CongViec> Task_CongViecs { get; set; }

        public DbSet<ThongBao> ThongBaos { get; set; }

        public DbSet<DanhGia_NhanXet> DanhGia_NhanXets { get; set; }
        public DbSet<DanhGia_NhanXet_LichSu> DanhGia_NhanXet_LichSus { get; set; }
        public DbSet<CauHinhPhiHoaHong> CauHinhPhiHoaHongs { get; set; }
        public DbSet<TranhChap> TranhChaps { get; set; }



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

            modelBuilder.Entity<DanhGia_NhanXet>(entity =>
            {
                entity.ToTable("DanhGia_NhanXet");
                entity.HasKey(d => d.maDanhGia);
                entity.HasIndex(d => new { d.maHD, d.maNguoiDanhGia }).IsUnique();
                entity.HasIndex(d => d.maNguoiDuocDanhGia);
                entity.HasOne(d => d.HopDong)
                    .WithMany()
                    .HasForeignKey(d => d.maHD)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.NguoiDanhGia)
                    .WithMany()
                    .HasForeignKey(d => d.maNguoiDanhGia)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.NguoiDuocDanhGia)
                    .WithMany()
                    .HasForeignKey(d => d.maNguoiDuocDanhGia)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(d => d.nhanXet).HasMaxLength(1000);
            });

            modelBuilder.Entity<DanhGia_NhanXet_LichSu>(entity =>
            {
                entity.ToTable("DanhGia_NhanXet_LichSu");
                entity.HasKey(d => d.maLichSu);
                entity.HasOne(d => d.DanhGia)
                    .WithMany()
                    .HasForeignKey(d => d.maDanhGia)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CauHinhPhiHoaHong>(entity =>
            {
                entity.ToTable("CauHinhPhiHoaHong_PhiDangBai");
                entity.HasKey(c => c.maCauHinh);
            });

            modelBuilder.Entity<FreelancerStudents>(entity =>
            {
                entity.HasKey(f => f.maFreelancerStudents);
                entity.Property(f => f.chiPhiTu).HasPrecision(18, 2);

                entity.HasOne(f => f.User).WithOne(u => u.FreelancerStudents).HasForeignKey<FreelancerStudents>(f => f.maUser);
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
                entity.Property(j => j.thulao).HasPrecision(18, 2);
                entity.Property(j => j.phiDangBai).HasPrecision(18, 2);

                entity.HasOne(j => j.NhaTuyenDung).WithMany(n => n.JobPosts).HasForeignKey(f => f.maNhaTuyenDung)
                .OnDelete(DeleteBehavior.Cascade); //Nếu xóa nhà tuyển dụng thì xóa luôn bài đăng của ẻm
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(w => w.maWallet);
                entity.Property(w => w.soDuKhaDung).HasPrecision(18, 2);
                entity.Property(w => w.soDuDongBang).HasPrecision(18, 2);

                entity.HasOne(w => w.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(w => w.maUser)
                .OnDelete(DeleteBehavior.Cascade); //Xóa Users thì xóa luôn ví 
            });

            modelBuilder.Entity<UngTuyen>(entity =>
            {
                entity.HasKey(u => u.maUngTuyen);
                entity.Property(u => u.thulaoDeXuat).HasPrecision(18, 2);

                //1 JobPost có nhiều đơn ứng tuyển
                entity.HasOne(u => u.JobPosts).WithMany(j => j.UngTuyens).HasForeignKey(u => u.maJob).OnDelete(DeleteBehavior.Cascade);

                //1 sinh viên có thể nộp nhiều đơn ứng tuyển
                entity.HasOne(u => u.FreelamcerStudents).WithMany(f => f.UngTuyens).HasForeignKey(u => u.maFreelancerStudent).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BaiDangTimViecFreelancerStudent>(entity =>
            {
                entity.HasKey(b => b.maBaiDang);

                entity.HasOne(b => b.FreelamcerStudents)
                .WithMany(f => f.BaiDangTimViecs)
                .HasForeignKey(b => b.maFreelancerStudent)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<GiaoDichNapTien>(entity =>
           {
               entity.ToTable("GiaoDichNapTien");
               entity.HasKey(g => g.maNapTien);
               entity.Property(g => g.soTien).HasPrecision(18, 2);

               entity.HasIndex(g => g.maGiaoDich)
                .IsUnique();

               entity.HasOne(g => g.Wallet)
           .WithMany()
           .HasForeignKey(g => g.maWallet)
           .OnDelete(DeleteBehavior.Restrict);
           });


            modelBuilder.Entity<Portfolio>(entity =>
         {
             entity.HasKey(x => x.maPortfolio);

             entity.Property(x => x.maPortfolio)
                 .HasColumnName("maPortfolio")
                 .HasColumnType("varchar(20)");

             entity.Property(x => x.url_video)
                 .HasColumnName("url_video")
                 .HasColumnType("varchar(255)");

             entity.HasIndex(x => x.maFreelancerStudents)
                 .IsUnique();

             entity.HasOne(x => x.FreelancerStudent)
                 .WithMany()
                 .HasForeignKey(x => x.maFreelancerStudents)
                 .OnDelete(DeleteBehavior.Cascade);
         });

            modelBuilder.Entity<DuAnTrongPortfolio>(entity =>
            {
                entity.HasKey(x => x.maDA);

                entity.Property(x => x.maDA)
                    .HasColumnName("maDA")
                    .HasColumnType("varchar(20)");

                entity.Property(x => x.maPortfolio)
                    .HasColumnName("maPortfolio")
                    .HasColumnType("varchar(20)");

                entity.Property(x => x.tenDuAn).HasMaxLength(200);
                entity.Property(x => x.vaiTro).HasMaxLength(50);
                entity.Property(x => x.congnghe).HasMaxLength(50);
                entity.Property(x => x.linkGithub).HasMaxLength(255);
                entity.Property(x => x.linkDemo).HasMaxLength(255);
                entity.Property(x => x.link_file).HasMaxLength(255);
                entity.Property(x => x.laDuAnNoiBat).HasColumnName("laDuAnNoiBat");

                entity.HasOne(x => x.Portfolio)
                    .WithMany()
                    .HasForeignKey(x => x.maPortfolio)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<HopDong>(entity =>
             {
                 entity.ToTable("HopDong");

                 // Khóa chính
                 entity.HasKey(h => h.maHD);

                 entity.Property(h => h.maHD)
                     .HasMaxLength(20)
                     .IsUnicode(false);

                 entity.Property(h => h.maJob)
                     .HasMaxLength(20)
                     .IsUnicode(false);

                 // Một JobPost có thể liên kết hợp đồng
                 entity.HasOne(h => h.JobPost)
                     .WithMany()
                     .HasForeignKey(h => h.maJob)



                     .OnDelete(DeleteBehavior.Restrict);

                 // Một Freelancer có thể tham gia nhiều hợp đồng
                 entity.HasOne(h => h.FreelancerStudent)
                     .WithMany()
                     .HasForeignKey(h => h.maFreelancerStudent)
                     .OnDelete(DeleteBehavior.Restrict);

                 entity.Property(h => h.sotienkyquy)
                     .HasColumnType("decimal(18,2)");

                 entity.Property(h => h.trangThai)
                     .HasMaxLength(30);

                 entity.Property(h => h.dieuKhoan);
             });

             modelBuilder.Entity<GiaiDoan_HopDong>(entity =>
             {
                 entity.ToTable("GiaiDoan_HopDong");
                 entity.HasKey(g => g.maGiaiDoan);

                 entity.HasOne(g => g.HopDong)
                     .WithMany(h => h.GiaiDoan_HopDongs)
                     .HasForeignKey(g => g.maHD)
                     .OnDelete(DeleteBehavior.Cascade);
             });

             modelBuilder.Entity<ThongBao>(entity =>
             {
                 entity.ToTable("ThongBao");
                 entity.HasKey(t => t.maThongBao);

                 entity.HasOne(t => t.User)
                     .WithMany()
                     .HasForeignKey(t => t.maUser)
                     .OnDelete(DeleteBehavior.Cascade);
             });

             modelBuilder.Entity<Task_CongViec>(entity =>
             {
                 entity.ToTable("Task_CongViec");
                 entity.HasKey(t => t.maTask);

                 entity.HasOne(t => t.HopDong)
                     .WithMany(h => h.Tasks)
                     .HasForeignKey(t => t.maHD)
                     .OnDelete(DeleteBehavior.Cascade);

                 entity.HasOne(t => t.GiaiDoan_HopDong)
                     .WithMany()
                     .HasForeignKey(t => t.maGiaiDoan)
                     .OnDelete(DeleteBehavior.SetNull);
             });
        }
    }
}