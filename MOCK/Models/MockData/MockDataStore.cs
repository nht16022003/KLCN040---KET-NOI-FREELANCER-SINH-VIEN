using System;
using System.Collections.Generic;
using System.Linq;
using MOCK.Models.Entities;
using MOCK.Models.ViewModels;

namespace MOCK.Models.MockData
{
    public static class MockDataStore
    {
        public static List<Role> Roles { get; set; } = new()
        {
            new Role { MaRole = 1, TenRole = "FreelancerStudent" },
            new Role { MaRole = 2, TenRole = "NhaTuyenDung" },
            new Role { MaRole = 3, TenRole = "Admin" }
        };

        public static List<User> Users { get; set; } = new()
        {
            new User
            {
                MaUser = 1,
                HotenUser = "Nguyễn Văn An",
                TenTaiKhoanUser = "an_freelancer",
                PashwordHash = "hash_pass_123",
                EmailUser = "an.nguyen@student.edu.vn",
                SdtUser = "0901234567",
                Ngaysinh = new DateTime(2003, 5, 15),
                Status = "ACTIVE",
                NgayTao = DateTime.Now.AddMonths(-3),
                MaRole = 1
            },
            new User
            {
                MaUser = 2,
                HotenUser = "Trần Thị Bích",
                TenTaiKhoanUser = "bich_designer",
                PashwordHash = "hash_pass_456",
                EmailUser = "bich.tran@student.edu.vn",
                SdtUser = "0912345678",
                Ngaysinh = new DateTime(2002, 11, 20),
                Status = "ACTIVE",
                NgayTao = DateTime.Now.AddMonths(-2),
                MaRole = 1
            },
            new User
            {
                MaUser = 3,
                HotenUser = "Lê Hoàng Long",
                TenTaiKhoanUser = "long_coder",
                PashwordHash = "hash_pass_789",
                EmailUser = "long.le@student.edu.vn",
                SdtUser = "0923456789",
                Ngaysinh = new DateTime(2004, 2, 10),
                Status = "ACTIVE",
                NgayTao = DateTime.Now.AddMonths(-1),
                MaRole = 1
            },
            new User
            {
                MaUser = 4,
                HotenUser = "Công Ty Công Nghệ ABC",
                TenTaiKhoanUser = "employer_abc",
                PashwordHash = "hash_pass_abc",
                EmailUser = "hr@abc-tech.com",
                SdtUser = "0283838383",
                Ngaysinh = new DateTime(2015, 8, 12),
                Status = "ACTIVE",
                NgayTao = DateTime.Now.AddYears(-1),
                MaRole = 2
            },
            new User
            {
                MaUser = 5,
                HotenUser = "Quản Trị Viên Hệ Thống",
                TenTaiKhoanUser = "admin_sys",
                PashwordHash = "hash_pass_admin",
                EmailUser = "admin@freelancego.vn",
                SdtUser = "0988888888",
                Ngaysinh = new DateTime(1995, 1, 1),
                Status = "ACTIVE",
                NgayTao = DateTime.Now.AddYears(-2),
                MaRole = 3
            }
        };

        public static List<MinhChungFreelancerStudent> MinhChungs { get; set; } = new()
        {
            new MinhChungFreelancerStudent
            {
                MaMinhChung = 1,
                MaUser = 1,
                LoaiMinhChung = "Thẻ sinh viên",
                FileMinhChung = "uploads/minhchung/user1_the_sinh_vien.jpg",
                NgayNop = DateTime.Now.AddMonths(-3),
                NgayXacMinh = DateTime.Now.AddMonths(-3),
                TrangThaiGuiMinhChung = "Đã xác minh",
                LydoTuChoi = null,
                NguoiXacMinh = 5
            },
            new MinhChungFreelancerStudent
            {
                MaMinhChung = 2,
                MaUser = 2,
                LoaiMinhChung = "Giấy xác nhận sinh viên",
                FileMinhChung = "uploads/minhchung/user2_giay_xac_nhan.pdf",
                NgayNop = DateTime.Now.AddMonths(-2),
                NgayXacMinh = DateTime.Now.AddMonths(-2),
                TrangThaiGuiMinhChung = "Đã xác minh",
                LydoTuChoi = null,
                NguoiXacMinh = 5
            },
            new MinhChungFreelancerStudent
            {
                MaMinhChung = 3,
                MaUser = 3,
                LoaiMinhChung = "Thẻ sinh viên",
                FileMinhChung = "uploads/minhchung/user3_the_sinh_vien.jpg",
                NgayNop = DateTime.Now.AddDays(-2),
                NgayXacMinh = null,
                TrangThaiGuiMinhChung = "Đang gửi",
                LydoTuChoi = null,
                NguoiXacMinh = null
            }
        };

        public static List<ChuyenNganh> ChuyenNganhs { get; set; } = new()
        {
            new ChuyenNganh { MaChuyenNganh = "CNTT", TenChuyenNganh = "Công nghệ thông tin" },
            new ChuyenNganh { MaChuyenNganh = "KTPM", TenChuyenNganh = "Kỹ thuật phần mềm" },
            new ChuyenNganh { MaChuyenNganh = "HTTT", TenChuyenNganh = "Hệ thống thông tin" },
            new ChuyenNganh { MaChuyenNganh = "KHMT", TenChuyenNganh = "Khoa học máy tính" },
            new ChuyenNganh { MaChuyenNganh = "KHDL", TenChuyenNganh = "Khoa học dữ liệu" },
            new ChuyenNganh { MaChuyenNganh = "TTNT", TenChuyenNganh = "Trí tuệ nhân tạo" },
            new ChuyenNganh { MaChuyenNganh = "ATTT", TenChuyenNganh = "An toàn thông tin" },
            new ChuyenNganh { MaChuyenNganh = "KTMT", TenChuyenNganh = "Kỹ thuật máy tính" },
            new ChuyenNganh { MaChuyenNganh = "TMDT", TenChuyenNganh = "Thương mại điện tử" },
            new ChuyenNganh { MaChuyenNganh = "QTKD", TenChuyenNganh = "Quản trị kinh doanh" },
            new ChuyenNganh { MaChuyenNganh = "MKT", TenChuyenNganh = "Marketing" },
            new ChuyenNganh { MaChuyenNganh = "TNNH", TenChuyenNganh = "Tài chính - Ngân hàng" },
            new ChuyenNganh { MaChuyenNganh = "KT", TenChuyenNganh = "Kế toán" },
            new ChuyenNganh { MaChuyenNganh = "NNA", TenChuyenNganh = "Ngôn ngữ Anh" },
            new ChuyenNganh { MaChuyenNganh = "NNT", TenChuyenNganh = "Ngôn ngữ Trung Quốc" },
            new ChuyenNganh { MaChuyenNganh = "TKDH", TenChuyenNganh = "Thiết kế đồ họa" },
            new ChuyenNganh { MaChuyenNganh = "TKTT", TenChuyenNganh = "Thiết kế thời trang" },
            new ChuyenNganh { MaChuyenNganh = "KTR", TenChuyenNganh = "Kiến trúc" },
            new ChuyenNganh { MaChuyenNganh = "LAW", TenChuyenNganh = "Luật" },
            new ChuyenNganh { MaChuyenNganh = "DL", TenChuyenNganh = "Du lịch" },
            new ChuyenNganh { MaChuyenNganh = "QTNH", TenChuyenNganh = "Quản trị nhà hàng và dịch vụ ăn uống" }
        };

        public static List<FreelancerStudent> FreelancerStudents { get; set; } = new()
        {
            new FreelancerStudent
            {
                MaFreelancerStudents = 1,
                MaChuyenNganh = "KTPM",
                MaUser = 1,
                MaTruong = "UIT01",
                TenTruong = "Đại học Công nghệ Thông tin",
                DiaDiemTruong = "Thủ Đức, TP.HCM",
                DiaDiemFreelancerStudent = "Thủ Đức, TP.HCM",
                NgonNgu = "Tiếng Anh",
                KyNangCoBan = "Word, Excel, PowerPoint",
                NamThu = 3,
                GPA = 3.45,
                NienKhoa = "2023-2027",
                Gioithieu = "Sinh viên ngành Kỹ thuật phần mềm, có kinh nghiệm phát triển website và ứng dụng web.",
                Avatar = "uploads/avatar/user1.jpg",
                TrangthaiNhanViec = true,
                ChiPhiTu = 100000
            },
            new FreelancerStudent
            {
                MaFreelancerStudents = 2,
                MaChuyenNganh = "TKDH",
                MaUser = 2,
                MaTruong = "DHC01",
                TenTruong = "Đại học Mỹ thuật Công nghiệp",
                DiaDiemTruong = "Quận 3, TP.HCM",
                DiaDiemFreelancerStudent = "Quận 3, TP.HCM",
                NgonNgu = "Tiếng Anh",
                KyNangCoBan = "Word, PowerPoint",
                NamThu = 4,
                GPA = 3.20,
                NienKhoa = "2022-2026",
                Gioithieu = "Sinh viên thiết kế đồ họa, nhận thiết kế poster, banner và nội dung mạng xã hội.",
                Avatar = "uploads/avatar/user2.jpg",
                TrangthaiNhanViec = true,
                ChiPhiTu = 150000
            }
        };

        public static List<KynangChuyennganhFreelancerStudent> KyNangs { get; set; } = new()
        {
            new KynangChuyennganhFreelancerStudent { MaKyNang = 1, MaFreelancerStudents = 1, TenKyNang = "C# / ASP.NET Core" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 2, MaFreelancerStudents = 1, TenKyNang = "SQL Server" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 3, MaFreelancerStudents = 1, TenKyNang = "HTML/CSS/JavaScript" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 4, MaFreelancerStudents = 1, TenKyNang = "Flutter" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 5, MaFreelancerStudents = 2, TenKyNang = "Photoshop" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 6, MaFreelancerStudents = 2, TenKyNang = "Illustrator" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 7, MaFreelancerStudents = 2, TenKyNang = "Figma" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 8, MaFreelancerStudents = 2, TenKyNang = "Canva" },
            new KynangChuyennganhFreelancerStudent { MaKyNang = 9, MaFreelancerStudents = 2, TenKyNang = "Poster Design" }
        };

        public static List<NhaTuyenDung> NhaTuyenDungs { get; set; } = new()
        {
            new NhaTuyenDung
            {
                MaNhaTuyenDung = 1,
                MaUser = 4,
                Avatar = "uploads/avatar/employer1.jpg",
                Gioithieu = "Công ty hoạt động trong lĩnh vực công nghệ và phát triển phần mềm.",
                Tencongty = "Công ty TNHH Công Nghệ ABC",
                Linhvuc = "Công nghệ thông tin",
                Diachi = "Quận 1, TP.HCM",
                Link = "https://abc.com",
                Logo = "uploads/logo/abc.png",
                NgayDangKy = DateTime.Now.AddYears(-1),
                Trangthai = "Active",
                Sosaodanhgia = 4.8
            }
        };

        public static List<Wallet> Wallets { get; set; } = new()
        {
            new Wallet { MaWallet = 1, MaUser = 1, SoDuKhaDung = 2200000, SoDuDongBang = 500000 },
            new Wallet { MaWallet = 2, MaUser = 2, SoDuKhaDung = 800000, SoDuDongBang = 0 },
            new Wallet { MaWallet = 3, MaUser = 3, SoDuKhaDung = 2000000, SoDuDongBang = 300000 },
            new Wallet { MaWallet = 4, MaUser = 4, SoDuKhaDung = 5000000, SoDuDongBang = 1000000 }
        };

        public static List<LichSuGiaoDich> LichSuGiaoDichs { get; set; } = new()
        {
            new LichSuGiaoDich
            {
                MaGiaoDich = 1,
                MaWallet = 1,
                LoaiGiaoDich = "NapTien",
                SoTien = 2000000,
                SoDuTruoc = 0,
                SoDuSau = 2000000,
                NoiDung = "Nạp tiền vào ví qua VietQR MBBank",
                MaThamChieu = "TOPUP_20260901_001",
                PhuongThuc = "VietQR",
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now.AddDays(-15)
            },
            new LichSuGiaoDich
            {
                MaGiaoDich = 2,
                MaWallet = 1,
                LoaiGiaoDich = "KyQuy",
                SoTien = 500000,
                SoDuTruoc = 2000000,
                SoDuSau = 1500000,
                NoiDung = "Đóng băng ký quỹ hợp đồng thiết kế landing page HD_2026_001",
                MaThamChieu = "HD_2026_001",
                PhuongThuc = "HeThong",
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now.AddDays(-10)
            },
            new LichSuGiaoDich
            {
                MaGiaoDich = 3,
                MaWallet = 1,
                LoaiGiaoDich = "NapTien",
                SoTien = 500000,
                SoDuTruoc = 1500000,
                SoDuSau = 2000000,
                NoiDung = "Nạp tiền ví điện tử qua VNPAY",
                MaThamChieu = "TOPUP_20260912_002",
                PhuongThuc = "VNPAY",
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now.AddDays(-5)
            },
            new LichSuGiaoDich
            {
                MaGiaoDich = 4,
                MaWallet = 1,
                LoaiGiaoDich = "HoanTien",
                SoTien = 200000,
                SoDuTruoc = 2000000,
                SoDuSau = 2200000,
                NoiDung = "Hoàn tiền ký quỹ điều chỉnh phạm vi công việc",
                MaThamChieu = "HD_2026_001",
                PhuongThuc = "HeThong",
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now.AddDays(-2)
            },
            new LichSuGiaoDich
            {
                MaGiaoDich = 5,
                MaWallet = 4,
                LoaiGiaoDich = "NapTien",
                SoTien = 6000000,
                SoDuTruoc = 0,
                SoDuSau = 6000000,
                NoiDung = "Nạp tiền doanh nghiệp VietQR",
                MaThamChieu = "TOPUP_20260820_003",
                PhuongThuc = "VietQR",
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now.AddDays(-20)
            },
            new LichSuGiaoDich
            {
                MaGiaoDich = 6,
                MaWallet = 4,
                LoaiGiaoDich = "KyQuy",
                SoTien = 1000000,
                SoDuTruoc = 6000000,
                SoDuSau = 5000000,
                NoiDung = "Ký quỹ Escrow hợp đồng lập trình Web",
                MaThamChieu = "HD_2026_002",
                PhuongThuc = "HeThong",
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now.AddDays(-8)
            }
        };

        public static List<YeuCauNapTien> YeuCauNapTiens { get; set; } = new()
        {
            new YeuCauNapTien
            {
                MaYeuCau = 1,
                MaWallet = 1,
                SoTien = 1000000,
                PhuongThuc = "ChuyenKhoanVietQR",
                MaGiaoDichNganHang = "MBB20260918012398",
                AnhBienLai = "https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=600&auto=format&fit=crop&q=80",
                GhiChu = "Đã chuyển khoản thành công lúc 14:30 nhưng số dư chưa cập nhật, nhờ BQT kiểm tra giúp.",
                TrangThai = "DaDuyet",
                NgayTao = DateTime.Now.AddDays(-7),
                NgayDuyet = DateTime.Now.AddDays(-7).AddHours(1)
            },
            new YeuCauNapTien
            {
                MaYeuCau = 2,
                MaWallet = 1,
                SoTien = 500000,
                PhuongThuc = "ChuyenKhoanVietQR",
                MaGiaoDichNganHang = "VCB98234123490",
                AnhBienLai = "https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=600&auto=format&fit=crop&q=80",
                GhiChu = "Em quên ghi nội dung mã ví, gửi kèm biên lai nhờ Admin cộng tiền giúp ạ.",
                TrangThai = "ChoDuyet",
                NgayTao = DateTime.Now.AddHours(-3)
            }
        };

        public static List<BanGiao_SanPham> BanGiao_SanPhams { get; set; } = new()
        {
            new BanGiao_SanPham
            {
                MaBanGiao = 1,
                MaHD = "HD_2026_001",
                PhienBan = "v1.0 (Bản nháp)",
                FileSanPhamUrl = "https://example.com/downloads/landingpage_v1.zip",
                LinkDemo = "https://preview.vercel.app/student-landingpage-v1",
                MotaBanGiao = "Em đã hoàn thiện giao diện Figma và code HTML/CSS/JS cho trang chủ và trang chi tiết khóa học. Kính gửi quý công ty kiểm tra nghiệm thu ạ.",
                NgayNop = DateTime.Now.AddDays(-4),
                TrangThai = "YeuCauChinhSua",
                PhanHoi = "Giao diện trên mobile bị vỡ bố cục phần bảng giá khóa học. Vui lòng căn chỉnh lại responsive và tối ưu ảnh banner.",
                NgayPhanHoi = DateTime.Now.AddDays(-3)
            },
            new BanGiao_SanPham
            {
                MaBanGiao = 2,
                MaHD = "HD_2026_001",
                PhienBan = "v1.1 (Final Release)",
                FileSanPhamUrl = "https://example.com/downloads/landingpage_v1_1_final.zip",
                LinkDemo = "https://preview.vercel.app/student-landingpage-final",
                MotaBanGiao = "Em đã fix toàn bộ lỗi hiển thị responsive trên màn hình mobile, nén ảnh WebP tải nhanh dưới 1.2s. Em nộp bản bàn giao chính thức ạ!",
                NgayNop = DateTime.Now.AddHours(-18),
                TrangThai = "ChoNghiemThu"
            },
            new BanGiao_SanPham
            {
                MaBanGiao = 3,
                MaHD = "HD_2026_002",
                PhienBan = "v1.0 (Chính thức)",
                FileSanPhamUrl = "https://example.com/downloads/webapp_ecommerce_v1.zip",
                LinkDemo = "https://shopdemo.dev.vn",
                MotaBanGiao = "Đã hoàn thành module giỏ hàng, thanh toán VNPAY và xuất hóa đơn theo đúng yêu cầu hợp đồng.",
                NgayNop = DateTime.Now.AddDays(-6),
                TrangThai = "DaDuyet",
                PhanHoi = "Sản phẩm chạy rất mượt mà, code sạch sẽ và tài liệu hướng dẫn rất đầy đủ. Cảm ơn bạn rất nhiều!",
                NgayPhanHoi = DateTime.Now.AddDays(-5)
            }
        };

        public static List<TranhChap> TranhChaps { get; set; } = new()
        {
            new TranhChap
            {
                MaTranhChap = 1,
                MaHD = "HD_2026_001",
                NguoiTao = 1,
                LyDo = "Chậm tiến độ và phản hồi trễ",
                MoTaChiTiet = "Hợp đồng đã quá hạn 4 ngày nhưng phía Freelancer chưa phản hồi các lỗi giao diện quan trọng. Doanh nghiệp cần trọng tài hệ thống hỗ trợ can thiệp.",
                SoTienTranhChap = 500000,
                TrangThai = "DangXuLy",
                KetLuanAdmin = "Admin đang liên hệ với cả 2 bên qua email và phòng chat để đối soát mốc bàn giao.",
                NgayTao = DateTime.Now.AddDays(-2)
            }
        };

        public static List<BangChungTranhChap> BangChungTranhChaps { get; set; } = new()
        {
            new BangChungTranhChap
            {
                MaBangChung = 1,
                MaTranhChap = 1,
                NguoiTaiLen = 1,
                LoaiBangChung = "AnhChup",
                DuongDanFile = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&auto=format&fit=crop&q=80",
                MoTa = "Ảnh chụp màn hình thông báo nhắc nhở deadline và yêu cầu chỉnh sửa không được phản hồi.",
                NgayTaiLen = DateTime.Now.AddDays(-2)
            },
            new BangChungTranhChap
            {
                MaBangChung = 2,
                MaTranhChap = 1,
                NguoiTaiLen = 1,
                LoaiBangChung = "DoanChat",
                DuongDanFile = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=800&auto=format&fit=crop&q=80",
                MoTa = "Lịch sử đoạn chat thỏa thuận thời gian bàn giao bản vá lỗi.",
                NgayTaiLen = DateTime.Now.AddDays(-1)
            }
        };

        public static List<PhongChat> PhongChats { get; set; } = new()
        {
            new PhongChat
            {
                MaPhongChat = "CHAT_001",
                MaUserClient = 3,
                MaUserFreelancer = 1,
                MaJob = "JOB_01",
                MaHD = "HD_2026_001",
                TinNhanCuoi = "Em vừa nộp bản nghiệm thu v1.1 trên hệ thống, anh/chị xem qua giúp em nhé ạ!",
                ThoiGianTinNhanCuoi = DateTime.Now.AddHours(-2),
                SoTinNhanChuaDoc = 1,
                NgayTao = DateTime.Now.AddDays(-12),
                NgayCapNhat = DateTime.Now.AddHours(-2)
            },
            new PhongChat
            {
                MaPhongChat = "CHAT_002",
                MaUserClient = 4,
                MaUserFreelancer = 1,
                MaJob = "JOB_02",
                MaHD = "HD_2026_002",
                TinNhanCuoi = "Chào An, công ty rất ấn tượng với đồ án trong Portfolio của em. Em có thể bắt đầu từ tuần sau không?",
                ThoiGianTinNhanCuoi = DateTime.Now.AddDays(-1),
                SoTinNhanChuaDoc = 0,
                NgayTao = DateTime.Now.AddDays(-5),
                NgayCapNhat = DateTime.Now.AddDays(-1)
            },
            new PhongChat
            {
                MaPhongChat = "CHAT_003",
                MaUserClient = 3,
                MaUserFreelancer = 2,
                MaJob = "JOB_03",
                TinNhanCuoi = "Dạ em đã gửi demo 3 mẫu poster banner qua link Figma, anh xem qua giúp em nha.",
                ThoiGianTinNhanCuoi = DateTime.Now.AddDays(-3),
                SoTinNhanChuaDoc = 0,
                NgayTao = DateTime.Now.AddDays(-6),
                NgayCapNhat = DateTime.Now.AddDays(-3)
            }
        };

        public static List<TinNhan> TinNhans { get; set; } = new()
        {
            new TinNhan
            {
                MaTinNhan = 1,
                MaPhongChat = "CHAT_001",
                NguoiGui = 3, // Client
                NoiDung = "Chào em An, anh đã xem qua CV của em ứng tuyển vào bài đăng Thiết kế Landing Page.",
                LoaiTinNhan = "VanBan",
                DaDoc = true,
                NgayGui = DateTime.Now.AddDays(-10)
            },
            new TinNhan
            {
                MaTinNhan = 2,
                MaPhongChat = "CHAT_001",
                NguoiGui = 1, // Freelancer
                NoiDung = "Dạ em chào anh! Em rất vui được kết nối. Em xin gửi kèm thẻ dự án mẫu mà em đã từng làm tương tự trong Portfolio để anh tham khảo năng lực ạ:",
                LoaiTinNhan = "ThePortfolio",
                MaDuAnPortfolio = "PRJ_01",
                DaDoc = true,
                NgayGui = DateTime.Now.AddDays(-10).AddMinutes(5)
            },
            new TinNhan
            {
                MaTinNhan = 3,
                MaPhongChat = "CHAT_001",
                NguoiGui = 3, // Client
                NoiDung = "Dự án rất đẹp và chuyên nghiệp! Bên anh đồng ý mức ngân sách 2.000.000 VNĐ. Anh gửi kèm file đặc tả yêu cầu chi tiết ở đây nhé:",
                LoaiTinNhan = "TepDinhKem",
                FileDinhKemUrl = "https://example.com/docs/Requirement_Landingpage.pdf",
                TenFile = "Requirement_Landingpage.pdf (1.8 MB)",
                DaDoc = true,
                NgayGui = DateTime.Now.AddDays(-9)
            },
            new TinNhan
            {
                MaTinNhan = 4,
                MaPhongChat = "CHAT_001",
                NguoiGui = 1, // Freelancer
                NoiDung = "Dạ em đã nhận được file và đã xem kỹ các mốc giao diện. Em sẽ hoàn thành đúng hạn 25/09 ạ.",
                LoaiTinNhan = "VanBan",
                DaDoc = true,
                NgayGui = DateTime.Now.AddDays(-9).AddMinutes(15)
            },
            new TinNhan
            {
                MaTinNhan = 5,
                MaPhongChat = "CHAT_001",
                NguoiGui = 1, // Freelancer
                NoiDung = "Em vừa nộp bản nghiệm thu v1.1 trên hệ thống, anh/chị xem qua giúp em nhé ạ!",
                LoaiTinNhan = "VanBan",
                DaDoc = false,
                NgayGui = DateTime.Now.AddHours(-2)
            }
        };

        public static List<Portfolio> Portfolios { get; set; } = new()
        {
            new Portfolio
            {
                MaPortfolio = "PORT_01",
                MaFreelancerStudents = 1,
                MoTaBanThan = "Xin chào! Mình là An, sinh viên năm 3 KTPM đam mê phát triển web và ứng dụng di động. Từng hoàn thành nhiều đồ án thực tế.",
                Url_video = "https://youtube.com/watch?v=demo_an_port"
            },
            new Portfolio
            {
                MaPortfolio = "PORT_02",
                MaFreelancerStudents = 2,
                MoTaBanThan = "Xin chào, mình là Bích, sinh viên thiết kế đồ họa với tư duy sáng tạo và khả năng biến ý tưởng thương hiệu thành hình ảnh trực quan.",
                Url_video = "https://youtube.com/watch?v=demo_bich_port"
            }
        };

        public static List<FreelancerYeuThich> FreelancerYeuThichs { get; set; } = new()
        {
            new FreelancerYeuThich
            {
                MaBookMark = 1,
                MaNhaTuyenDung = 1,
                MaFreelancerStudent = 1,
                GhiChu = "Có kỹ năng ASP.NET Core và SQL Server rất tốt, phù hợp dự án Web sắp tới.",
                NgayLuu = DateTime.Now.AddDays(-5)
            },
            new FreelancerYeuThich
            {
                MaBookMark = 2,
                MaNhaTuyenDung = 1,
                MaFreelancerStudent = 2,
                GhiChu = "Thiết kế Poster, Banner và nhận diện thương hiệu ấn tượng, phong cách sáng tạo.",
                NgayLuu = DateTime.Now.AddDays(-2)
            }
        };

        public static List<NhaTuyenDungYeuThich> NhaTuyenDungYeuThichs { get; set; } = new()
        {
            new NhaTuyenDungYeuThich
            {
                MaBookMark = 1,
                MaFreelancerStudent = 1,
                MaNhaTuyenDung = 1,
                GhiChu = "Công ty công nghệ uy tín, chế độ đãi ngộ tốt và nhiều dự án phần mềm hay.",
                NgayLuu = DateTime.Now.AddDays(-3)
            }
        };

        // Bảng BaiDangTimViec_FreelancerStudent trong KLCN040_FREELANCERSTUDENT.sql
        public static List<BaiDangTimViecFreelancerStudent> BaiDangTimViecs { get; set; } = new()
        {
            new BaiDangTimViecFreelancerStudent
            {
                MaBaiDang = "BDTV_001",
                MaFreelancerStudent = 1,
                Tieude = "Nhận thiết kế & lập trình Fullstack Web ASP.NET Core MVC / ReactJS",
                Mota = "Em là sinh viên năm 3 ngành Kỹ thuật phần mềm HCMUTE, nhận phát triển các dự án Web thương mại điện tử, landing page, hệ thống quản lý chuẩn kiến trúc MVC, tích hợp cổng thanh toán trực tuyến.",
                Kynang = "ASP.NET Core, ReactJS, SQL Server, C#, Bootstrap 5",
                MucGiaTu = 1500000,
                Thoigiandang = DateTime.Now.AddDays(-10),
                Trangthai = "DangHienThi"
            },
            new BaiDangTimViecFreelancerStudent
            {
                MaBaiDang = "BDTV_002",
                MaFreelancerStudent = 1,
                Tieude = "Nhận xây dựng RESTful API và cơ sở dữ liệu SQL Server chuẩn hóa",
                Mota = "Chuyên thiết kế cơ sở dữ liệu tối ưu hóa, viết Stored Procedures, xây dựng Web API bảo mật JWT cho ứng dụng web và mobile.",
                Kynang = "RESTful API, SQL Server, C#, Entity Framework Core",
                MucGiaTu = 1000000,
                Thoigiandang = DateTime.Now.AddDays(-5),
                Trangthai = "DangHienThi"
            },
            new BaiDangTimViecFreelancerStudent
            {
                MaBaiDang = "BDTV_003",
                MaFreelancerStudent = 2,
                Tieude = "Thiết kế bộ nhận diện thương hiệu, Logo & Banner quảng cáo chuyên nghiệp",
                Mota = "Sinh viên chuyên ngành Thiết kế đồ họa, nhận vẽ minh họa, thiết kế ấn phẩm truyền thông, brochure, UI/UX mobile app trên Figma.",
                Kynang = "Figma, Adobe Photoshop, Illustrator, UI/UX Design",
                MucGiaTu = 800000,
                Thoigiandang = DateTime.Now.AddDays(-8),
                Trangthai = "DangHienThi"
            },
            new BaiDangTimViecFreelancerStudent
            {
                MaBaiDang = "BDTV_004",
                MaFreelancerStudent = 1,
                Tieude = "Dự án Lập trình Web cá nhân đã hoàn thành (Đã nhận việc)",
                Mota = "Dự án hợp đồng xây dựng website giới thiệu công ty đã bàn giao và nhận việc thành công.",
                Kynang = "HTML5, CSS3, JavaScript, jQuery",
                MucGiaTu = 2000000,
                Thoigiandang = DateTime.Now.AddDays(-20),
                Trangthai = "DaNhanViec"
            }
        };

        public static List<DuAnTrongPortfolio> DuAns { get; set; } = new()
        {
            new DuAnTrongPortfolio
            {
                MaDA = "DA_01",
                MaPortfolio = "PORT_01",
                TenDuAn = "Website Đặt Lịch Khám Bệnh Trực Tuyến",
                MoTa = "Hệ thống quản lý đặt khám bệnh viện, thông báo lịch hẹn qua email.",
                VaiTro = "Full-stack Developer",
                Congnghe = "ASP.NET Core MVC, SQL Server",
                LinkGithub = "https://github.com/demo/clinic-booking",
                LinkDemo = "https://clinic-demo.io",
                Link_file = null
            },
            new DuAnTrongPortfolio
            {
                MaDA = "DA_02",
                MaPortfolio = "PORT_01",
                TenDuAn = "Ứng Dụng Quản Lý Chi Tiêu Cá Nhân",
                MoTa = "App di động ghi chép thu chi hàng ngày và vẽ biểu đồ phân tích.",
                VaiTro = "Mobile Developer",
                Congnghe = "Flutter, SQLite",
                LinkGithub = "https://github.com/demo/expense-tracker",
                LinkDemo = null,
                Link_file = "uploads/projects/app_release.apk"
            },
            new DuAnTrongPortfolio
            {
                MaDA = "DA_03",
                MaPortfolio = "PORT_02",
                TenDuAn = "Bộ Nhận Diện Thương Hiệu Quán Cà Phê Mộc",
                MoTa = "Thiết kế trọn gói: Logo, Menu, Poster, Áo đồng phục và bao bì sản phẩm.",
                VaiTro = "Graphic Designer",
                Congnghe = "Photoshop, Illustrator",
                LinkGithub = null,
                LinkDemo = "https://behance.net/gallery/cafe-moc-branding",
                Link_file = "uploads/projects/branding_mockup.pdf"
            }
        };

        public static List<JobPost> JobPosts { get; set; } = new()
        {
            new JobPost
            {
                MaJob = "JOB_01",
                MaNhaTuyenDung = 1,
                Tieude = "Thiết kế Poster & Banner sự kiện khai trương",
                Mota = "Cần 1 bạn sinh viên thiết kế bộ ấn phẩm gồm 2 poster đứng, 3 banner Facebook và voucher giảm giá cho chuỗi cửa hàng.",
                Kynangyeucau = "Photoshop, Illustrator, Banner Design",
                Thulao = 1500000,
                Thoigiandangtuyen = DateTime.Now.AddDays(-5),
                Thoigiandukienhoanthanh = "2026-10-01",
                Status = "DangTuyen",
                Soluongtuyen = 1
            },
            new JobPost
            {
                MaJob = "JOB_02",
                MaNhaTuyenDung = 1,
                Tieude = "Xây dựng module Quản lý kho bằng ASP.NET MVC",
                Mota = "Viết module nhập xuất tồn kho hàng, hỗ trợ in phiếu xuất kho dạng PDF/Crystal Reports.",
                Kynangyeucau = "C#, ASP.NET Core, SQL Server",
                Thulao = 3500000,
                Thoigiandangtuyen = DateTime.Now.AddDays(-10),
                Thoigiandukienhoanthanh = "2026-10-15",
                Status = "DangThucHien",
                Soluongtuyen = 1
            },
            new JobPost
            {
                MaJob = "JOB_03",
                MaNhaTuyenDung = 1,
                Tieude = "Phát triển Landing Page giới thiệu sản phẩm",
                Mota = "Cần code giao diện Responsive chuẩn UI/UX từ file Figma có sẵn, ưu tiên HTML/CSS/Tailwind.",
                Kynangyeucau = "HTML, CSS, JavaScript, Figma",
                Thulao = 1000000,
                Thoigiandangtuyen = DateTime.Now.AddDays(-2),
                Thoigiandukienhoanthanh = "2026-10-05",
                Status = "DangTuyen",
                Soluongtuyen = 2
            }
        };

        public static List<HopDong> HopDongs { get; set; } = new()
        {
            new HopDong
            {
                MaHD = "HD_2026_001",
                MaJob = "JOB_02",
                MaFreelancerStudent = 1,
                NgayBatDau = DateTime.Now.AddDays(-7),
                NgayKetThuc = new DateTime(2026, 10, 15),
                Sotienkyquy = 3500000,
                Hinhthuclamviec = "Remote",
                TrangThai = "DangThucHien"
            }
        };

        public static List<UngTuyen> UngTuyens { get; set; } = new()
        {
            new UngTuyen
            {
                MaUngTuyen = 1,
                MaJob = "JOB_01",
                MaFreelancerStudent = 2,
                ThuGioiThieu = "Em chào anh/chị, em là Bích, sinh viên năm 4 ngành Thiết kế đồ họa. Em từng thực hiện nhiều bộ nhận diện thương hiệu và thiết kế banner khai trương chuyên nghiệp. Em cam kết bàn giao đúng hạn trong 3 ngày.",
                ThulaoDeXuat = 1500000,
                ThoiGianHoanThanhDeXuat = "3 ngày",
                FileCV = "uploads/cv/bich_portfolio_designer.pdf",
                NgayUngTuyen = DateTime.Now.AddDays(-2),
                TrangThaiUngTuyen = "ChoDuyet"
            },
            new UngTuyen
            {
                MaUngTuyen = 2,
                MaJob = "JOB_03",
                MaFreelancerStudent = 1,
                ThuGioiThieu = "Chào nhà tuyển dụng, em là An (KTPM - UIT). Em có kinh nghiệm code giao diện HTML/CSS/Tailwind chuẩn responsive từ Figma. Rất mong được hợp tác phát triển Landing Page.",
                ThulaoDeXuat = 1000000,
                ThoiGianHoanThanhDeXuat = "4 ngày",
                FileCV = "uploads/cv/an_frontend_cv.pdf",
                NgayUngTuyen = DateTime.Now.AddDays(-1),
                TrangThaiUngTuyen = "ChoDuyet"
            },
            new UngTuyen
            {
                MaUngTuyen = 3,
                MaJob = "JOB_02",
                MaFreelancerStudent = 1,
                ThuGioiThieu = "Em đã có kinh nghiệm làm đồ án quản lý kho bằng ASP.NET Core MVC và SQL Server.",
                ThulaoDeXuat = 3500000,
                ThoiGianHoanThanhDeXuat = "15 ngày",
                FileCV = "uploads/cv/an_aspnet_cv.pdf",
                NgayUngTuyen = DateTime.Now.AddDays(-8),
                TrangThaiUngTuyen = "DaLapHopDong"
            }
        };

        public static List<YeuCauThueFreelancer> YeuCauThues { get; set; } = new()
        {
            new YeuCauThueFreelancer
            {
                MaYeuCau = 1,
                MaNhaTuyenDung = 1,
                MaFreelancerStudent = 1,
                TieuDeCongViec = "Mời phát triển API tích hợp cổng thanh toán",
                MoTaCongViec = "Công ty cần viết thêm 3 endpoint kết nối VNPay cho hệ thống thương mại điện tử hiện có.",
                NganSachDeNghi = 2000000,
                ThoiHanDuKien = "5 ngày",
                NgayGui = DateTime.Now.AddDays(-3),
                TrangThai = "ChoPhanHoi",
                LyDoTuChoi = null
            }
        };

        public static List<DanhGiaNhanXet> DanhGias { get; set; } = new()
        {
            new DanhGiaNhanXet
            {
                MaDanhGia = 1,
                MaHD = "HD_01",
                MaNguoiDanhGia = 4, // Nhà tuyển dụng ABC (MaUser = 4)
                MaNguoiDuocDanhGia = 1, // Freelancer An (MaUser = 1)
                SoSao = 5,
                NhanXet = "Bạn An làm việc rất nghiêm túc, giao diện chuẩn UI/UX và hoàn thành trước thời hạn 2 ngày. Rất ấn tượng!",
                NgayDanhGia = DateTime.Now.AddDays(-10)
            },
            new DanhGiaNhanXet
            {
                MaDanhGia = 2,
                MaHD = "HD_02",
                MaNguoiDanhGia = 4,
                MaNguoiDuocDanhGia = 2, // Freelancer Bích (MaUser = 2)
                SoSao = 5,
                NhanXet = "Thiết kế bộ nhận diện thương hiệu rất sáng tạo, phối màu chuẩn cho sinh viên khởi nghiệp.",
                NgayDanhGia = DateTime.Now.AddDays(-5)
            },
            new DanhGiaNhanXet
            {
                MaDanhGia = 3,
                MaHD = "HD_01",
                MaNguoiDanhGia = 4,
                MaNguoiDuocDanhGia = 3, // Freelancer Long (MaUser = 3)
                SoSao = 4,
                NhanXet = "Code sạch sẽ, tài liệu bàn giao rõ ràng. Giao tiếp hợp tác rất tích cực.",
                NgayDanhGia = DateTime.Now.AddDays(-2)
            }
        };

        public static List<ThongBao> ThongBaos { get; set; } = new()
        {
            new ThongBao
            {
                MaThongBao = 1,
                MaUser = 4, // Nhà tuyển dụng (CUS)
                TieuDe = "Ứng viên mới nộp hồ sơ",
                NoiDung = "Freelancer Nguyễn Văn An vừa ứng tuyển vào vị trí 'Thiết kế landing page tuyển sinh' của bạn.",
                LoaiThongBao = "UngTuyen",
                LinkDieuHuong = "/Employer/Applicants?id=JOB_01",
                DaDoc = false,
                NgayTao = DateTime.Now.AddMinutes(-25)
            },
            new ThongBao
            {
                MaThongBao = 2,
                MaUser = 4,
                TieuDe = "Freelancer đã phản hồi lời mời",
                NoiDung = "Freelancer Trần Thị Bích đã đồng ý nhận lời mời làm việc trực tiếp cho dự án 'Thiết kế ấn phẩm truyền thông'.",
                LoaiThongBao = "UngThue",
                LinkDieuHuong = "/Employer/ManageJobs",
                DaDoc = false,
                NgayTao = DateTime.Now.AddHours(-2)
            },
            new ThongBao
            {
                MaThongBao = 3,
                MaUser = 4,
                TieuDe = "Hợp đồng đã hoàn thành nghiệm thu",
                NoiDung = "Hợp đồng HD_01 đã được hoàn tất bàn giao và đánh giá thành công. Số tiền ký quỹ đã được giải ngân.",
                LoaiThongBao = "HopDong",
                LinkDieuHuong = "/HopDong/Details?id=HD_01",
                DaDoc = true,
                NgayTao = DateTime.Now.AddDays(-1)
            },
            new ThongBao
            {
                MaThongBao = 4,
                MaUser = 4,
                TieuDe = "Nạp tiền ví thành công",
                NoiDung = "Giao dịch nạp 5,000,000 VNĐ qua cổng thanh toán VNPay đã được xử lý thành công vào ví chính.",
                LoaiThongBao = "ThanhToan",
                LinkDieuHuong = "/Wallet/Index",
                DaDoc = true,
                NgayTao = DateTime.Now.AddDays(-3)
            },
            new ThongBao
            {
                MaThongBao = 5,
                MaUser = 4,
                TieuDe = "Chào mừng bạn đến với hệ thống Freelancer Student!",
                NoiDung = "Hồ sơ doanh nghiệp của bạn đã được quản trị viên duyệt. Bạn có thể đăng tin tuyển dụng và tìm kiếm nhân tài sinh viên ngay bây giờ.",
                LoaiThongBao = "HeThong",
                LinkDieuHuong = "/JobPost/Create",
                DaDoc = true,
                NgayTao = DateTime.Now.AddMonths(-1)
            },
            new ThongBao
            {
                MaThongBao = 6,
                MaUser = 1, // Freelancer An
                TieuDe = "Bạn nhận được lời mời làm việc mới",
                NoiDung = "Công Ty Công Nghệ ABC đã gửi lời mời thuê bạn làm việc trực tiếp cho dự án 'Mời phát triển API tích hợp cổng thanh toán'.",
                LoaiThongBao = "UngThue",
                LinkDieuHuong = "/Freelancer/LoiMoiNhanViec",
                DaDoc = false,
                NgayTao = DateTime.Now.AddHours(-1)
            }
        };

        public static List<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; } = new()
        {
            new TaiKhoanNganHang
            {
                MaTKNH = 1,
                MaUser = 1, // Freelancer An
                TenNganHang = "Vietcombank",
                ChiNhanh = "Hồ Chí Minh",
                SoTaiKhoan = "1012345678",
                TenChuTaiKhoan = "NGUYEN VAN AN",
                LaMacDinh = true,
                NgayThem = DateTime.Now.AddMonths(-2)
            },
            new TaiKhoanNganHang
            {
                MaTKNH = 2,
                MaUser = 1,
                TenNganHang = "MB Bank",
                ChiNhanh = "Chi nhánh Tân Bình",
                SoTaiKhoan = "090123456789",
                TenChuTaiKhoan = "NGUYEN VAN AN",
                LaMacDinh = false,
                NgayThem = DateTime.Now.AddMonths(-1)
            },
            new TaiKhoanNganHang
            {
                MaTKNH = 3,
                MaUser = 2, // Freelancer Bích
                TenNganHang = "Techcombank",
                ChiNhanh = "Hà Nội",
                SoTaiKhoan = "19034567890123",
                TenChuTaiKhoan = "TRAN THI BICH",
                LaMacDinh = true,
                NgayThem = DateTime.Now.AddMonths(-1)
            }
        };

        public static List<YeuCauRutTien> YeuCauRutTiens { get; set; } = new()
        {
            new YeuCauRutTien
            {
                MaRutTien = 1,
                MaWallet = 1,
                SoTienRut = 500000,
                TenNganHang = "Vietcombank",
                SoTaiKhoan = "1012345678",
                TenChuTaiKhoan = "NGUYEN VAN AN",
                NgayYeuCau = DateTime.Now.AddDays(-10),
                NgayXuLy = DateTime.Now.AddDays(-9),
                TrangThai = "DaChuyenKhoan",
                LyDoTuChoi = null,
                MaAdminXuLy = 1
            },
            new YeuCauRutTien
            {
                MaRutTien = 2,
                MaWallet = 1,
                SoTienRut = 1200000,
                TenNganHang = "Vietcombank",
                SoTaiKhoan = "1012345678",
                TenChuTaiKhoan = "NGUYEN VAN AN",
                NgayYeuCau = DateTime.Now.AddDays(-2),
                NgayXuLy = null,
                TrangThai = "ChoDuyet",
                LyDoTuChoi = null,
                MaAdminXuLy = null
            }
        };

        public static List<Admin> Admins { get; set; } = new()
        {
            new Admin { MaAdmin = 1, HotenAdmin = "Quản Trị Viên Hệ Thống", MaUser = 5 }
        };

        public static List<LichSuXuLyTaiKhoan> LichSuXuLyTaiKhoans { get; set; } = new()
        {
            new LichSuXuLyTaiKhoan
            {
                MaLichSu = 1,
                MaUser = 3,
                MaAdmin = 1,
                HanhDong = "CANH_BAO",
                LyDo = "Nhận cảnh báo do phản hồi chậm trễ trong tiến độ giao hàng",
                TrangThaiTruocKhiXuLy = "ACTIVE",
                TrangThaiSauKhiXuLy = "ACTIVE",
                NgayXuLy = DateTime.Now.AddDays(-15)
            }
        };

        public static List<DieuChinhSoDu> DieuChinhSoDus { get; set; } = new()
        {
            new DieuChinhSoDu
            {
                MaDieuChinh = 1,
                MaWallet = 1,
                MaAdmin = 1,
                LoaiDieuChinh = "CongTien",
                SoTienDieuChinh = 200000,
                SoDuTruoc = 5000000,
                SoDuSau = 5200000,
                LyDo = "Cộng tiền thưởng sinh viên xuất sắc quý 1",
                NgayDieuChinh = DateTime.Now.AddDays(-20)
            }
        };

        public static List<CauHinhPhiHoaHongPhiDangBai> CauHinhPhis { get; set; } = new()
        {
            new CauHinhPhiHoaHongPhiDangBai
            {
                MaCauHinh = 1,
                TenCauHinh = "PhiDangBai",
                GiaTri = 2000,
                LoaiCauHinh = "TienMat",
                MoTa = "Phí đăng bài tuyển dụng cho mỗi tin mới",
                MaAdmin = 1,
                NgayCapNhat = DateTime.Now.AddMonths(-3),
                HieuLuc = true
            },
            new CauHinhPhiHoaHongPhiDangBai
            {
                MaCauHinh = 2,
                TenCauHinh = "HoaHongDuAn",
                GiaTri = 5.0m,
                LoaiCauHinh = "PhanTram",
                MoTa = "Phần trăm chiết khấu hoa hồng trên mỗi hợp đồng hoàn thành",
                MaAdmin = 1,
                NgayCapNhat = DateTime.Now.AddMonths(-3),
                HieuLuc = true
            }
        };

        public static List<YeuCauHoTro> YeuCauHoTros { get; set; } = new()
        {
            new YeuCauHoTro
            {
                MaYeuCau = 1,
                MaUser = 4, // NTD ABC
                LoaiYeuCau = "HoTroNapTien",
                TieuDe = "Cần kiểm tra giao dịch nạp tiền VietQR",
                NoiDung = "Tôi đã chuyển khoản 5,000,000 VNĐ qua VietQR nhưng số dư chưa cập nhật sau 10 phút.",
                TrangThai = "DaXuLy",
                PhanHoiAdmin = "Hệ thống đã đối soát ngân hàng và cộng tiền thành công vào ví của bạn.",
                MaAdmin = 1,
                NgayGui = DateTime.Now.AddDays(-3),
                NgayXuLy = DateTime.Now.AddDays(-3)
            },
            new YeuCauHoTro
            {
                MaYeuCau = 2,
                MaUser = 1, // Freelancer An
                LoaiYeuCau = "LoiHeThong",
                TieuDe = "Không tải được file zip sản phẩm nghiệm thu",
                NoiDung = "Em đính kèm file zip 25MB trong phần nghiệm thu nhưng bị báo timeout.",
                TrangThai = "DangXuLy",
                PhanHoiAdmin = null,
                MaAdmin = 1,
                NgayGui = DateTime.Now.AddHours(-4),
                NgayXuLy = null
            }
        };

        public static string MauHopDongHeThong { get; set; } = @"CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
Độc lập - Tự do - Hạnh phúc
---
HỢP ĐỒNG CUNG CẤP DỊCH VỤ FREELANCE SINH VIÊN

ĐIỀU 1: PHẠM VI CÔNG VIỆC VÀ SẢN PHẨM BÀN GIAO
- Bên B (Freelancer Sinh viên) cam kết thực hiện công việc theo đúng mô tả, tiêu chí kỹ thuật và thời hạn đã thỏa thuận.

ĐIỀU 2: GIÁ TRỊ HỢP ĐỒNG VÀ PHƯƠNG THỨC THANH TOÁN (ESCROW)
- Bên A (Nhà tuyển dụng) thực hiện ký quỹ 100% giá trị hợp đồng vào ví an toàn hệ thống.
- Tiền được giải ngân theo từng mốc giai đoạn sau khi Bên A nghiệm thu đạt yêu cầu.

ĐIỀU 3: BẢO HÀNH VÀ SỬA ĐỔI SẢN PHẨM
- Bên B hỗ trợ chỉnh sửa tối đa 3 lần theo đúng phạm vi ban đầu trong vòng 7 ngày kể từ ngày bàn giao.

ĐIỀU 4: GIẢI QUYẾT TRANH CHẤP
- Mọi khiếu nại được tiếp nhận và phân xử khách quan thông qua Hội đồng trọng tài Admin hệ thống Freelancer Student.";

        // ==================== HELPER METHODS FOR UI TESTING ====================

        public static EmployerManageJobsViewModel GetEmployerManageJobs(int employerId, string? statusFilter = null)
        {
            var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == employerId) ?? NhaTuyenDungs.First();
            var user = Users.FirstOrDefault(u => u.MaUser == employer.MaUser) ?? new User();
            var allJobs = JobPosts.Where(j => j.MaNhaTuyenDung == employerId).ToList();

            var jobItems = allJobs.Select(j =>
            {
                var applicants = UngTuyens.Where(u => u.MaJob == j.MaJob).ToList();
                var contract = HopDongs.FirstOrDefault(h => h.MaJob == j.MaJob);
                return new EmployerJobItemViewModel
                {
                    Job = j,
                    ApplicantCount = applicants.Count,
                    PendingApplicantCount = applicants.Count(a => a.TrangThaiUngTuyen == "ChoDuyet"),
                    AcceptedApplicantCount = applicants.Count(a => a.TrangThaiUngTuyen == "ChapNhan" || a.TrangThaiUngTuyen == "DaLapHopDong"),
                    Contract = contract
                };
            }).ToList();

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                jobItems = jobItems.Where(item => item.Job.Status == statusFilter).ToList();
            }

            return new EmployerManageJobsViewModel
            {
                Employer = employer,
                User = user,
                Jobs = jobItems,
                CurrentStatusFilter = statusFilter,
                ActiveJobsCount = allJobs.Count(j => j.Status == "DangTuyen"),
                InProgressJobsCount = allJobs.Count(j => j.Status == "DangThucHien"),
                ClosedJobsCount = allJobs.Count(j => j.Status == "DongTin" || j.Status == "HoanThanh")
            };
        }

        public static JobApplicantsViewModel? GetJobApplicants(string maJob)
        {
            var job = JobPosts.FirstOrDefault(j => j.MaJob == maJob);
            if (job == null) return null;

            var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == job.MaNhaTuyenDung) ?? new NhaTuyenDung();
            var rawApplicants = UngTuyens.Where(u => u.MaJob == maJob).ToList();

            var applicantItems = rawApplicants.Select(app =>
            {
                var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == app.MaFreelancerStudent) ?? new FreelancerStudent();
                var studentUser = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
                var skills = KyNangs.Where(k => k.MaFreelancerStudents == student.MaFreelancerStudents).ToList();
                var minhChung = MinhChungs.FirstOrDefault(m => m.MaUser == studentUser.MaUser);

                return new ApplicantItemViewModel
                {
                    Application = app,
                    Student = student,
                    StudentUser = studentUser,
                    Skills = skills,
                    MinhChung = minhChung
                };
            }).ToList();

            return new JobApplicantsViewModel
            {
                Job = job,
                Employer = employer,
                Applicants = applicantItems
            };
        }

        public static FreelancerProfileViewModel? GetFreelancerProfile(int freelancerId)
        {
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == freelancerId);
            if (student == null) return null;

            var user = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
            var skills = KyNangs.Where(k => k.MaFreelancerStudents == freelancerId).ToList();
            var portfolio = Portfolios.FirstOrDefault(p => p.MaFreelancerStudents == freelancerId);
            var projects = portfolio != null ? DuAns.Where(d => d.MaPortfolio == portfolio.MaPortfolio).ToList() : new();
            var contracts = HopDongs.Where(h => h.MaFreelancerStudent == freelancerId).ToList();
            var wallet = Wallets.FirstOrDefault(w => w.MaUser == user.MaUser);
            var minhChung = MinhChungs.FirstOrDefault(m => m.MaUser == user.MaUser);

            return new FreelancerProfileViewModel
            {
                User = user,
                FreelancerStudent = student,
                KyNangs = skills,
                Portfolio = portfolio,
                DuAns = projects,
                HopDongs = contracts,
                Wallet = wallet,
                MinhChung = minhChung
            };
        }

        public static JobDetailViewModel? GetJobDetail(string maJob)
        {
            var job = JobPosts.FirstOrDefault(j => j.MaJob == maJob);
            if (job == null) return null;

            var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == job.MaNhaTuyenDung);
            var employerUser = employer != null ? Users.FirstOrDefault(u => u.MaUser == employer.MaUser) : null;
            var relatedJobs = JobPosts.Where(j => j.MaJob != maJob && j.MaNhaTuyenDung == job.MaNhaTuyenDung).ToList();

            return new JobDetailViewModel
            {
                Job = job,
                Employer = employer,
                EmployerUser = employerUser,
                RelatedJobs = relatedJobs
            };
        }

        public static EmployerProfileViewModel? GetEmployerProfile(int employerId)
        {
            var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == employerId);
            if (employer == null) return null;

            var user = Users.FirstOrDefault(u => u.MaUser == employer.MaUser) ?? new User();
            var allJobs = JobPosts.Where(j => j.MaNhaTuyenDung == employerId).ToList();
            var activeJobs = allJobs.Where(j => j.Status == "DangTuyen").ToList();
            var wallet = Wallets.FirstOrDefault(w => w.MaUser == user.MaUser);

            return new EmployerProfileViewModel
            {
                User = user,
                Employer = employer,
                AllJobs = allJobs,
                ActiveJobs = activeJobs,
                Wallet = wallet
            };
        }

        public static ContractDetailViewModel? GetContractDetail(string maHD)
        {
            var contract = HopDongs.FirstOrDefault(h => h.MaHD == maHD);
            if (contract == null) return null;

            var job = JobPosts.FirstOrDefault(j => j.MaJob == contract.MaJob) ?? new JobPost();
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == contract.MaFreelancerStudent) ?? new FreelancerStudent();
            var studentUser = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
            var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == job.MaNhaTuyenDung) ?? new NhaTuyenDung();
            var employerUser = Users.FirstOrDefault(u => u.MaUser == employer.MaUser) ?? new User();

            return new ContractDetailViewModel
            {
                Contract = contract,
                Job = job,
                Freelancer = student,
                FreelancerUser = studentUser,
                Employer = employer,
                EmployerUser = employerUser
            };
        }

        public static AdminVerificationViewModel GetAdminVerificationList()
        {
            var items = MinhChungs.Select(mc =>
            {
                var user = Users.FirstOrDefault(u => u.MaUser == mc.MaUser) ?? new User();
                var student = FreelancerStudents.FirstOrDefault(s => s.MaUser == mc.MaUser);
                return new AdminVerificationItemViewModel
                {
                    MinhChung = mc,
                    StudentUser = user,
                    StudentProfile = student
                };
            }).ToList();

            return new AdminVerificationViewModel
            {
                PendingVerifications = items.Where(i => i.MinhChung.TrangThaiGuiMinhChung == "Đang gửi").ToList(),
                VerifiedList = items.Where(i => i.MinhChung.TrangThaiGuiMinhChung == "Đã xác minh").ToList(),
                RejectedList = items.Where(i => i.MinhChung.TrangThaiGuiMinhChung == "Đã từ chối").ToList()
            };
        }

        public static WalletViewModel? GetWalletView(int maUser)
        {
            var user = Users.FirstOrDefault(u => u.MaUser == maUser);
            if (user == null) return null;

            var wallet = Wallets.FirstOrDefault(w => w.MaUser == maUser) ?? new Wallet { MaWallet = 1, MaUser = maUser, SoDuKhaDung = 2200000, SoDuDongBang = 500000 };
            
            // Tìm hợp đồng liên quan
            var student = FreelancerStudents.FirstOrDefault(s => s.MaUser == maUser);
            var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaUser == maUser);
            List<HopDong> contracts = new();
            if (student != null)
            {
                contracts = HopDongs.Where(h => h.MaFreelancerStudent == student.MaFreelancerStudents).ToList();
            }
            else if (employer != null)
            {
                var employerJobIds = JobPosts.Where(j => j.MaNhaTuyenDung == employer.MaNhaTuyenDung).Select(j => j.MaJob).ToList();
                contracts = HopDongs.Where(h => employerJobIds.Contains(h.MaJob)).ToList();
            }

            var transactions = LichSuGiaoDichs.Where(t => t.MaWallet == wallet.MaWallet).OrderByDescending(t => t.NgayTao).ToList();

            decimal tongNap = transactions.Where(t => t.LoaiGiaoDich == "NapTien" && t.TrangThai == "ThanhCong").Sum(t => t.SoTien);
            decimal tongRut = transactions.Where(t => t.LoaiGiaoDich == "RutTien" && t.TrangThai == "ThanhCong").Sum(t => t.SoTien);
            decimal tongKyQuy = transactions.Where(t => t.LoaiGiaoDich == "KyQuy" && t.TrangThai == "ThanhCong").Sum(t => t.SoTien);
            decimal tongNhan = transactions.Where(t => (t.LoaiGiaoDich == "NhanTien" || t.LoaiGiaoDich == "HoanTien") && t.TrangThai == "ThanhCong").Sum(t => t.SoTien);

            return new WalletViewModel
            {
                User = user,
                Wallet = wallet,
                TongNapTien = tongNap,
                TongRutTien = tongRut,
                TongDaThanhToanEscrow = tongKyQuy,
                TongNhanTien = tongNhan,
                RecentTransactions = transactions.Take(5).ToList(),
                DangKyQuyContracts = contracts.Where(c => c.TrangThai == "DangThucHien" || c.TrangThai == "ChoNghiemThu").ToList()
            };
        }

        public static StudentAppliedJobsViewModel GetStudentAppliedJobs(int freelancerId, string? statusFilter = null)
        {
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == freelancerId) ?? FreelancerStudents.First();
            var user = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
            var allApplications = UngTuyens.Where(u => u.MaFreelancerStudent == freelancerId).ToList();

            var appliedItems = allApplications.Select(app =>
            {
                var job = JobPosts.FirstOrDefault(j => j.MaJob == app.MaJob) ?? new JobPost();
                var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == job.MaNhaTuyenDung) ?? new NhaTuyenDung();
                var empUser = Users.FirstOrDefault(u => u.MaUser == employer.MaUser) ?? new User();
                var contract = HopDongs.FirstOrDefault(h => h.MaJob == job.MaJob && h.MaFreelancerStudent == freelancerId);

                return new StudentAppliedJobItemViewModel
                {
                    Application = app,
                    Job = job,
                    Employer = employer,
                    EmployerUser = empUser,
                    Contract = contract
                };
            }).ToList();

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                appliedItems = appliedItems.Where(i => i.Application.TrangThaiUngTuyen == statusFilter).ToList();
            }

            return new StudentAppliedJobsViewModel
            {
                Student = student,
                User = user,
                AppliedJobs = appliedItems,
                CurrentStatusFilter = statusFilter,
                PendingCount = allApplications.Count(a => a.TrangThaiUngTuyen == "ChoDuyet"),
                AcceptedCount = allApplications.Count(a => a.TrangThaiUngTuyen == "ChapNhan" || a.TrangThaiUngTuyen == "DaLapHopDong"),
                RejectedCount = allApplications.Count(a => a.TrangThaiUngTuyen == "TuChoi")
            };
        }

        public static StudentInvitationsViewModel GetStudentInvitations(int freelancerId)
        {
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == freelancerId) ?? FreelancerStudents.First();
            var user = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
            var allInvites = YeuCauThues.Where(y => y.MaFreelancerStudent == freelancerId).ToList();

            var inviteItems = allInvites.Select(inv =>
            {
                var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == inv.MaNhaTuyenDung) ?? new NhaTuyenDung();
                var empUser = Users.FirstOrDefault(u => u.MaUser == employer.MaUser) ?? new User();

                return new StudentInvitationItemViewModel
                {
                    Invitation = inv,
                    Employer = employer,
                    EmployerUser = empUser
                };
            }).ToList();

            return new StudentInvitationsViewModel
            {
                Student = student,
                User = user,
                Invitations = inviteItems,
                PendingCount = allInvites.Count(i => i.TrangThai == "ChoPhanHoi"),
                AcceptedCount = allInvites.Count(i => i.TrangThai == "DongY" || i.TrangThai == "DaLapHopDong"),
                RejectedCount = allInvites.Count(i => i.TrangThai == "TuChoi")
            };
        }

        public static StudentPortfolioManageViewModel GetStudentPortfolioManage(int freelancerId)
        {
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == freelancerId) ?? FreelancerStudents.First();
            var user = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
            var portfolio = Portfolios.FirstOrDefault(p => p.MaFreelancerStudents == freelancerId);
            if (portfolio == null)
            {
                portfolio = new Portfolio
                {
                    MaPortfolio = $"PORT_{freelancerId:D2}",
                    MaFreelancerStudents = freelancerId,
                    MoTaBanThan = "Portfolio cá nhân của sinh viên.",
                    Url_video = "https://youtube.com/watch?v=demo"
                };
                Portfolios.Add(portfolio);
            }

            var projects = DuAns.Where(d => d.MaPortfolio == portfolio.MaPortfolio).ToList();

            return new StudentPortfolioManageViewModel
            {
                Student = student,
                User = user,
                Portfolio = portfolio,
                Projects = projects
            };
        }

        public static StudentProfileEditViewModel GetStudentProfileEdit(int freelancerId)
        {
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == freelancerId) ?? FreelancerStudents.First();
            var user = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
            var skills = KyNangs.Where(k => k.MaFreelancerStudents == freelancerId).ToList();

            return new StudentProfileEditViewModel
            {
                User = user,
                Student = student,
                KyNangs = skills,
                ChuyenNganhs = ChuyenNganhs,
                HotenUser = user.HotenUser,
                SdtUser = user.SdtUser,
                Gioithieu = student.Gioithieu,
                NgonNgu = student.NgonNgu,
                KyNangCoBan = student.KyNangCoBan,
                TrangthaiNhanViec = student.TrangthaiNhanViec,
                ChiPhiTu = student.ChiPhiTu ?? 100000,
                NewSkillsInput = string.Join(", ", skills.Select(s => s.TenKyNang))
            };
        }

        public static FreelancerYeuThichViewModel GetDanhSachFreelancerYeuThich(int maNhaTuyenDung, string? keyword = null, string? chuyenNganh = null, string? sortBy = null)
        {
            var bookmarks = FreelancerYeuThichs.Where(b => b.MaNhaTuyenDung == maNhaTuyenDung).ToList();

            var query = bookmarks.Select(b =>
            {
                var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == b.MaFreelancerStudent) ?? new FreelancerStudent();
                var user = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();
                var cn = ChuyenNganhs.FirstOrDefault(c => c.MaChuyenNganh == student.MaChuyenNganh);
                var skills = KyNangs.Where(k => k.MaFreelancerStudents == student.MaFreelancerStudents).Select(k => k.TenKyNang).ToList();
                var portfolio = Portfolios.FirstOrDefault(p => p.MaFreelancerStudents == student.MaFreelancerStudents);
                int projectCount = portfolio != null ? DuAns.Count(d => d.MaPortfolio == portfolio.MaPortfolio) : 0;
                int completedContracts = HopDongs.Count(h => h.MaFreelancerStudent == student.MaFreelancerStudents && h.TrangThai == "HoanThanh");

                return new FreelancerYeuThichItemViewModel
                {
                    MaBookMark = b.MaBookMark,
                    MaNhaTuyenDung = b.MaNhaTuyenDung,
                    MaFreelancerStudent = b.MaFreelancerStudent,
                    MaUser = student.MaUser,
                    HotenUser = user.HotenUser,
                    EmailUser = user.EmailUser,
                    SdtUser = user.SdtUser,
                    Avatar = !string.IsNullOrEmpty(student.Avatar) ? student.Avatar : "uploads/avatar/user1.jpg",
                    MaChuyenNganh = student.MaChuyenNganh,
                    TenChuyenNganh = cn?.TenChuyenNganh ?? "Công nghệ thông tin",
                    TenTruong = student.TenTruong,
                    NamThu = student.NamThu,
                    GPA = student.GPA,
                    NienKhoa = student.NienKhoa,
                    Gioithieu = student.Gioithieu,
                    ChiPhiTu = student.ChiPhiTu ?? 100000,
                    TrangthaiNhanViec = student.TrangthaiNhanViec,
                    KyNangChuyenNganh = skills,
                    GhiChu = b.GhiChu,
                    NgayLuu = b.NgayLuu,
                    DiemDanhGia = 4.9,
                    SoHopDongHoanThanh = completedContracts,
                    SoDuAnPortfolio = projectCount
                };
            }).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(i => i.HotenUser.ToLower().Contains(kw) ||
                                         i.TenTruong.ToLower().Contains(kw) ||
                                         i.KyNangChuyenNganh.Any(s => s.ToLower().Contains(kw)) ||
                                         (i.GhiChu != null && i.GhiChu.ToLower().Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(chuyenNganh))
            {
                query = query.Where(i => i.MaChuyenNganh.Equals(chuyenNganh, StringComparison.OrdinalIgnoreCase) ||
                                         i.TenChuyenNganh.Equals(chuyenNganh, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.ToList();
            if (sortBy == "gpa_desc")
            {
                list = list.OrderByDescending(i => i.GPA).ToList();
            }
            else if (sortBy == "price_asc")
            {
                list = list.OrderBy(i => i.ChiPhiTu ?? 0).ToList();
            }
            else if (sortBy == "name_asc")
            {
                list = list.OrderBy(i => i.HotenUser).ToList();
            }
            else
            {
                list = list.OrderByDescending(i => i.NgayLuu).ToList();
            }

            var allChuyenNganhs = ChuyenNganhs.Select(c => c.TenChuyenNganh).Distinct().ToList();

            return new FreelancerYeuThichViewModel
            {
                DanhSachYeuThich = list,
                AllChuyenNganhs = allChuyenNganhs,
                Keyword = keyword,
                SelectedChuyenNganh = chuyenNganh,
                SortBy = sortBy
            };
        }

        public static bool XoaFreelancerYeuThich(int maBookMark)
        {
            var item = FreelancerYeuThichs.FirstOrDefault(b => b.MaBookMark == maBookMark);
            if (item != null)
            {
                FreelancerYeuThichs.Remove(item);
                return true;
            }
            return false;
        }

        public static bool CapNhatGhiChuYeuThich(int maBookMark, string? ghiChuMoi)
        {
            var item = FreelancerYeuThichs.FirstOrDefault(b => b.MaBookMark == maBookMark);
            if (item != null)
            {
                item.GhiChu = ghiChuMoi;
                return true;
            }
            return false;
        }

        public static bool ToggleFreelancerYeuThich(int maNhaTuyenDung, int maFreelancerStudent, string? ghiChu = null)
        {
            var existing = FreelancerYeuThichs.FirstOrDefault(b => b.MaNhaTuyenDung == maNhaTuyenDung && b.MaFreelancerStudent == maFreelancerStudent);
            if (existing != null)
            {
                FreelancerYeuThichs.Remove(existing);
                return false; // Đã bỏ lưu
            }
            else
            {
                int nextId = FreelancerYeuThichs.Count > 0 ? FreelancerYeuThichs.Max(b => b.MaBookMark) + 1 : 1;
                FreelancerYeuThichs.Add(new FreelancerYeuThich
                {
                    MaBookMark = nextId,
                    MaNhaTuyenDung = maNhaTuyenDung,
                    MaFreelancerStudent = maFreelancerStudent,
                    GhiChu = ghiChu ?? "Ứng viên quan tâm tuyển dụng.",
                    NgayLuu = DateTime.Now
                });
                return true; // Đã thêm lưu
            }
        }

        public static NhaTuyenDungYeuThichViewModel GetDanhSachNhaTuyenDungYeuThich(int maFreelancerStudent, string? keyword = null, string? linhVuc = null, string? sortBy = null)
        {
            var bookmarks = NhaTuyenDungYeuThichs.Where(b => b.MaFreelancerStudent == maFreelancerStudent).ToList();

            var query = bookmarks.Select(b =>
            {
                var ntd = NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == b.MaNhaTuyenDung) ?? new NhaTuyenDung();
                var user = Users.FirstOrDefault(u => u.MaUser == ntd.MaUser) ?? new User();
                var allJobs = JobPosts.Where(j => j.MaNhaTuyenDung == ntd.MaNhaTuyenDung).ToList();
                var activeJobs = allJobs.Where(j => j.Status == "DangTuyen").ToList();

                return new NhaTuyenDungYeuThichItemViewModel
                {
                    MaBookMark = b.MaBookMark,
                    MaFreelancerStudent = b.MaFreelancerStudent,
                    MaNhaTuyenDung = b.MaNhaTuyenDung,
                    MaUser = ntd.MaUser,
                    Tencongty = ntd.Tencongty ?? user.HotenUser,
                    Linhvuc = ntd.Linhvuc ?? "Công nghệ thông tin",
                    Diachi = ntd.Diachi,
                    Logo = !string.IsNullOrEmpty(ntd.Logo) ? ntd.Logo : (ntd.Avatar ?? "uploads/logo/default_company.png"),
                    Link = ntd.Link,
                    Gioithieu = ntd.Gioithieu,
                    Sosaodanhgia = ntd.Sosaodanhgia ?? 5.0,
                    HotenNguoiDaiDien = user.HotenUser,
                    EmailUser = user.EmailUser,
                    SdtUser = user.SdtUser,
                    TotalJobs = allJobs.Count,
                    ActiveJobsCount = activeJobs.Count,
                    GhiChu = b.GhiChu,
                    NgayLuu = b.NgayLuu
                };
            }).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(i => i.Tencongty.ToLower().Contains(kw) ||
                                         (i.Linhvuc != null && i.Linhvuc.ToLower().Contains(kw)) ||
                                         (i.Diachi != null && i.Diachi.ToLower().Contains(kw)) ||
                                         (i.GhiChu != null && i.GhiChu.ToLower().Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(linhVuc))
            {
                query = query.Where(i => i.Linhvuc != null && i.Linhvuc.Equals(linhVuc, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.ToList();
            if (sortBy == "rating_desc")
            {
                list = list.OrderByDescending(i => i.Sosaodanhgia).ToList();
            }
            else if (sortBy == "jobs_desc")
            {
                list = list.OrderByDescending(i => i.ActiveJobsCount).ToList();
            }
            else if (sortBy == "name_asc")
            {
                list = list.OrderBy(i => i.Tencongty).ToList();
            }
            else
            {
                list = list.OrderByDescending(i => i.NgayLuu).ToList();
            }

            var allLinhVucs = NhaTuyenDungs.Where(n => !string.IsNullOrEmpty(n.Linhvuc)).Select(n => n.Linhvuc!).Distinct().ToList();

            return new NhaTuyenDungYeuThichViewModel
            {
                DanhSachYeuThich = list,
                AllLinhVucs = allLinhVucs,
                Keyword = keyword,
                SelectedLinhVuc = linhVuc,
                SortBy = sortBy
            };
        }

        public static bool XoaNhaTuyenDungYeuThich(int maBookMark)
        {
            var item = NhaTuyenDungYeuThichs.FirstOrDefault(b => b.MaBookMark == maBookMark);
            if (item != null)
            {
                NhaTuyenDungYeuThichs.Remove(item);
                return true;
            }
            return false;
        }

        public static bool CapNhatGhiChuNhaTuyenDungYeuThich(int maBookMark, string? ghiChuMoi)
        {
            var item = NhaTuyenDungYeuThichs.FirstOrDefault(b => b.MaBookMark == maBookMark);
            if (item != null)
            {
                item.GhiChu = ghiChuMoi;
                return true;
            }
            return false;
        }

        public static bool IsNhaTuyenDungYeuThich(int maFreelancerStudent, int maNhaTuyenDung)
        {
            return NhaTuyenDungYeuThichs.Any(b => b.MaFreelancerStudent == maFreelancerStudent && b.MaNhaTuyenDung == maNhaTuyenDung);
        }

        public static bool ToggleNhaTuyenDungYeuThich(int maFreelancerStudent, int maNhaTuyenDung, string? ghiChu = null)
        {
            var existing = NhaTuyenDungYeuThichs.FirstOrDefault(b => b.MaFreelancerStudent == maFreelancerStudent && b.MaNhaTuyenDung == maNhaTuyenDung);
            if (existing != null)
            {
                NhaTuyenDungYeuThichs.Remove(existing);
                return false; // Đã bỏ lưu
            }
            else
            {
                int nextId = NhaTuyenDungYeuThichs.Count > 0 ? NhaTuyenDungYeuThichs.Max(b => b.MaBookMark) + 1 : 1;
                NhaTuyenDungYeuThichs.Add(new NhaTuyenDungYeuThich
                {
                    MaBookMark = nextId,
                    MaFreelancerStudent = maFreelancerStudent,
                    MaNhaTuyenDung = maNhaTuyenDung,
                    GhiChu = ghiChu ?? "Nhà tuyển dụng quan tâm.",
                    NgayLuu = DateTime.Now
                });
                return true; // Đã thêm lưu
            }
        }

        // ==========================================
        // CUS-UC-07.02: XEM LỊCH SỬ GIAO DỊCH
        // ==========================================
        public static LichSuGiaoDichViewModel GetLichSuGiaoDich(int maUser, string? keyword = null, string? loaiGiaoDich = null, string? trangThai = null, DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var user = Users.FirstOrDefault(u => u.MaUser == maUser) ?? Users.First();
            var wallet = Wallets.FirstOrDefault(w => w.MaUser == maUser) ?? new Wallet { MaWallet = 1, MaUser = maUser, SoDuKhaDung = 2200000, SoDuDongBang = 500000 };

            var query = LichSuGiaoDichs.Where(t => t.MaWallet == wallet.MaWallet).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim().ToLower();
                query = query.Where(t => (t.NoiDung != null && t.NoiDung.ToLower().Contains(kw))
                                      || (t.MaThamChieu != null && t.MaThamChieu.ToLower().Contains(kw))
                                      || (t.PhuongThuc != null && t.PhuongThuc.ToLower().Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(loaiGiaoDich))
            {
                query = query.Where(t => t.LoaiGiaoDich == loaiGiaoDich);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(t => t.TrangThai == trangThai);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(t => t.NgayTao >= tuNgay.Value.Date);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(t => t.NgayTao <= denNgay.Value.Date.AddDays(1).AddTicks(-1));
            }

            var transactions = query.OrderByDescending(t => t.NgayTao).ToList();

            decimal tienVao = transactions.Where(t => (t.LoaiGiaoDich == "NapTien" || t.LoaiGiaoDich == "NhanTien" || t.LoaiGiaoDich == "HoanTien") && t.TrangThai == "ThanhCong").Sum(t => t.SoTien);
            decimal tienRa = transactions.Where(t => (t.LoaiGiaoDich == "RutTien" || t.LoaiGiaoDich == "KyQuy" || t.LoaiGiaoDich == "PhiDichVu") && t.TrangThai == "ThanhCong").Sum(t => t.SoTien);

            return new LichSuGiaoDichViewModel
            {
                User = user,
                Wallet = wallet,
                Transactions = transactions,
                Keyword = keyword,
                LoaiGiaoDich = loaiGiaoDich,
                TrangThai = trangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                TongTienVao = tienVao,
                TongTienRa = tienRa
            };
        }

        // ==========================================
        // CUS-UC-08.01: NẠP TIỀN VÀO VÍ
        // ==========================================
        public static NapTienViewModel GetNapTienView(int maUser, decimal? soTien = null, string? phuongThuc = null)
        {
            var user = Users.FirstOrDefault(u => u.MaUser == maUser) ?? Users.First();
            var wallet = Wallets.FirstOrDefault(w => w.MaUser == maUser) ?? new Wallet { MaWallet = 1, MaUser = maUser, SoDuKhaDung = 2200000, SoDuDongBang = 500000 };

            decimal selectedAmount = soTien.HasValue && soTien.Value > 0 ? soTien.Value : 500000;
            string selectedMethod = phuongThuc ?? "VietQR";
            string transferContent = $"NAPVI W{wallet.MaWallet} U{user.MaUser}";
            string refCode = $"TOPUP_{DateTime.Now:yyyyMMdd}_{new Random().Next(100, 999)}";

            // Sinh link VietQR chuẩn động của MBBank
            string qrUrl = $"https://img.vietqr.io/image/MB-999988886666-compact2.png?amount={(long)selectedAmount}&addInfo={Uri.EscapeDataString(transferContent)}&accountName=CTCP%20FREELANCERSTUDENT";

            var recentTopups = LichSuGiaoDichs.Where(t => t.MaWallet == wallet.MaWallet && t.LoaiGiaoDich == "NapTien")
                                              .OrderByDescending(t => t.NgayTao).Take(5).ToList();

            return new NapTienViewModel
            {
                User = user,
                Wallet = wallet,
                SoTien = selectedAmount,
                PhuongThuc = selectedMethod,
                NoiDungChuyenKhoan = transferContent,
                MaGiaoDichThamChieu = refCode,
                QrCodeImageUrl = qrUrl,
                RecentTopups = recentTopups
            };
        }

        public static bool NapTienVaoVi(int maUser, decimal soTien, string phuongThuc, string noiDung, string? maThamChieu = null)
        {
            if (soTien <= 0) return false;

            var wallet = Wallets.FirstOrDefault(w => w.MaUser == maUser);
            if (wallet == null)
            {
                int newWalletId = Wallets.Count > 0 ? Wallets.Max(w => w.MaWallet) + 1 : 1;
                wallet = new Wallet { MaWallet = newWalletId, MaUser = maUser, SoDuKhaDung = 0, SoDuDongBang = 0 };
                Wallets.Add(wallet);
            }

            decimal soDuTruoc = wallet.SoDuKhaDung;
            wallet.SoDuKhaDung += soTien;
            decimal soDuSau = wallet.SoDuKhaDung;

            int nextTxId = LichSuGiaoDichs.Count > 0 ? LichSuGiaoDichs.Max(t => t.MaGiaoDich) + 1 : 1;
            LichSuGiaoDichs.Add(new LichSuGiaoDich
            {
                MaGiaoDich = nextTxId,
                MaWallet = wallet.MaWallet,
                LoaiGiaoDich = "NapTien",
                SoTien = soTien,
                SoDuTruoc = soDuTruoc,
                SoDuSau = soDuSau,
                NoiDung = string.IsNullOrWhiteSpace(noiDung) ? $"Nạp {soTien:N0}đ vào ví qua {phuongThuc}" : noiDung,
                MaThamChieu = maThamChieu ?? $"TOPUP_{DateTime.Now:yyyyMMddHHmmss}",
                PhuongThuc = phuongThuc,
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now
            });

            return true;
        }

        // ==========================================
        // CUS-UC-08.02: GỬI YÊU CẦU HỖ TRỢ NẠP TIỀN
        // ==========================================
        public static HoTroNapTienViewModel GetHoTroNapTienView(int maUser)
        {
            var user = Users.FirstOrDefault(u => u.MaUser == maUser) ?? Users.First();
            var wallet = Wallets.FirstOrDefault(w => w.MaUser == maUser) ?? new Wallet { MaWallet = 1, MaUser = maUser, SoDuKhaDung = 2200000, SoDuDongBang = 500000 };

            var requests = YeuCauNapTiens.Where(y => y.MaWallet == wallet.MaWallet).OrderByDescending(y => y.NgayTao).ToList();

            return new HoTroNapTienViewModel
            {
                User = user,
                Wallet = wallet,
                DanhSachYeuCau = requests
            };
        }

        public static bool GuiYeuCauHoTroNapTien(int maUser, decimal soTien, string phuongThuc, string? maGDNganHang, string? ghiChu, string? anhBienLai)
        {
            if (soTien <= 0) return false;

            var wallet = Wallets.FirstOrDefault(w => w.MaUser == maUser) ?? Wallets.First();
            int nextId = YeuCauNapTiens.Count > 0 ? YeuCauNapTiens.Max(y => y.MaYeuCau) + 1 : 1;

            YeuCauNapTiens.Add(new YeuCauNapTien
            {
                MaYeuCau = nextId,
                MaWallet = wallet.MaWallet,
                SoTien = soTien,
                PhuongThuc = phuongThuc,
                MaGiaoDichNganHang = maGDNganHang,
                AnhBienLai = anhBienLai ?? "https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=600&auto=format&fit=crop&q=80",
                GhiChu = ghiChu,
                TrangThai = "ChoDuyet",
                NgayTao = DateTime.Now
            });

            return true;
        }

        // =========================================================================
        // CUS-UC-10.02: NGHIỆM THU VÀ PHẢN HỒI SẢN PHẨM
        // =========================================================================
        public static NghiemThuSanPhamViewModel? GetNghiemThuView(string maHD)
        {
            var contract = HopDongs.FirstOrDefault(h => h.MaHD == maHD);
            if (contract == null) return null;

            var job = JobPosts.FirstOrDefault(j => j.MaJob == contract.MaJob) ?? new JobPost();
            var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == job.MaNhaTuyenDung) ?? new NhaTuyenDung();
            var empUser = Users.FirstOrDefault(u => u.MaUser == employer.MaUser) ?? new User();
            var student = FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == contract.MaFreelancerStudent) ?? new FreelancerStudent();
            var flUser = Users.FirstOrDefault(u => u.MaUser == student.MaUser) ?? new User();

            var submissions = BanGiao_SanPhams.Where(b => b.MaHD == maHD).OrderByDescending(b => b.NgayNop).ToList();
            var latest = submissions.FirstOrDefault();

            return new NghiemThuSanPhamViewModel
            {
                Contract = contract,
                Job = job,
                Employer = employer,
                EmployerUser = empUser,
                Freelancer = student,
                FreelancerUser = flUser,
                LatestBanGiao = latest,
                LichSuBanGiao = submissions
            };
        }

        public static bool DuyetNghiemThu(int maBanGiao, string maHD, string? danhGia, int? soSao)
        {
            var banGiao = BanGiao_SanPhams.FirstOrDefault(b => b.MaBanGiao == maBanGiao);
            if (banGiao != null)
            {
                banGiao.TrangThai = "DaDuyet";
                banGiao.PhanHoi = string.IsNullOrWhiteSpace(danhGia) ? "Đã nghiệm thu đạt yêu cầu." : danhGia;
                banGiao.NgayPhanHoi = DateTime.Now;
            }

            var contract = HopDongs.FirstOrDefault(h => h.MaHD == maHD);
            if (contract != null)
            {
                contract.TrangThai = "DaHoanThanh";
                
                // Giải ngân số dư đóng băng ký quỹ sang ví Freelancer
                var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == contract.MaFreelancerStudent);
                if (student != null)
                {
                    var flWallet = Wallets.FirstOrDefault(w => w.MaUser == student.MaUser);
                    if (flWallet != null && contract.Sotienkyquy.HasValue)
                    {
                        flWallet.SoDuKhaDung += (decimal)contract.Sotienkyquy.Value;
                        
                        int nextTx = LichSuGiaoDichs.Count > 0 ? LichSuGiaoDichs.Max(t => t.MaGiaoDich) + 1 : 1;
                        LichSuGiaoDichs.Add(new LichSuGiaoDich
                        {
                            MaGiaoDich = nextTx,
                            MaWallet = flWallet.MaWallet,
                            LoaiGiaoDich = "NhanTien",
                            SoTien = (decimal)contract.Sotienkyquy.Value,
                            SoDuTruoc = flWallet.SoDuKhaDung - (decimal)contract.Sotienkyquy.Value,
                            SoDuSau = flWallet.SoDuKhaDung,
                            NoiDung = $"Nhận thù lao nghiệm thu hợp đồng {contract.MaHD}",
                            MaThamChieu = contract.MaHD,
                            PhuongThuc = "Escrow",
                            TrangThai = "ThanhCong",
                            NgayTao = DateTime.Now
                        });
                    }
                }
            }

            return true;
        }

        public static bool YeuCauChinhSua(int maBanGiao, string maHD, string lyDo)
        {
            var banGiao = BanGiao_SanPhams.FirstOrDefault(b => b.MaBanGiao == maBanGiao);
            if (banGiao != null)
            {
                banGiao.TrangThai = "YeuCauChinhSua";
                banGiao.PhanHoi = lyDo;
                banGiao.NgayPhanHoi = DateTime.Now;
            }

            var contract = HopDongs.FirstOrDefault(h => h.MaHD == maHD);
            if (contract != null)
            {
                contract.TrangThai = "DangThucHien";
            }

            return true;
        }

        // =========================================================================
        // CUS-UC-11.01 & 11.02: TRANH CHẤP & BẰNG CHỨNG
        // =========================================================================
        public static TranhChapListViewModel GetDanhSachTranhChap(int currentUserId, string? keyword = null, string? status = null)
        {
            var query = TranhChaps.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim().ToLower();
                query = query.Where(t => t.MaHD.ToLower().Contains(kw) || t.LyDo.ToLower().Contains(kw) || t.MoTaChiTiet.ToLower().Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(t => t.TrangThai.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.OrderByDescending(t => t.NgayTao).Select(tc =>
            {
                var hd = HopDongs.FirstOrDefault(h => h.MaHD == tc.MaHD) ?? new HopDong();
                var job = JobPosts.FirstOrDefault(j => j.MaJob == hd.MaJob) ?? new JobPost();
                var creator = Users.FirstOrDefault(u => u.MaUser == tc.NguoiTao) ?? new User();
                
                // Đối phương
                var emp = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == job.MaNhaTuyenDung);
                var fl = FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == hd.MaFreelancerStudent);
                User opponent = new User();
                if (tc.NguoiTao == emp?.MaUser && fl != null)
                {
                    opponent = Users.FirstOrDefault(u => u.MaUser == fl.MaUser) ?? new User();
                }
                else if (emp != null)
                {
                    opponent = Users.FirstOrDefault(u => u.MaUser == emp.MaUser) ?? new User();
                }

                int evidenceCount = BangChungTranhChaps.Count(b => b.MaTranhChap == tc.MaTranhChap);

                return new TranhChapItemViewModel
                {
                    TranhChap = tc,
                    HopDong = hd,
                    Job = job,
                    CreatorUser = creator,
                    OpponentUser = opponent,
                    SoLuongBangChung = evidenceCount
                };
            }).ToList();

            int allCount = TranhChaps.Count;
            int openCount = TranhChaps.Count(t => t.TrangThai == "DangMo");
            int inProgCount = TranhChaps.Count(t => t.TrangThai == "DangXuLy");
            int resCount = TranhChaps.Count(t => t.TrangThai == "DaGiaiQuyet");

            return new TranhChapListViewModel
            {
                TranhChaps = list,
                OpenCount = openCount,
                InProgressCount = inProgCount,
                ResolvedCount = resCount,
                Keyword = keyword,
                SelectedStatus = status ?? "All"
            };
        }

        public static TranhChapCreateViewModel GetTranhChapCreateView(int currentUserId, string? maHD = null)
        {
            var eligible = HopDongs.Where(h => h.TrangThai == "DangThucHien" || h.TrangThai == "ChoNghiemThu").ToList();
            
            HopDong? selectedContract = null;
            JobPost? selectedJob = null;
            decimal disputeAmount = 0;

            if (!string.IsNullOrEmpty(maHD))
            {
                selectedContract = HopDongs.FirstOrDefault(h => h.MaHD == maHD);
                if (selectedContract != null)
                {
                    selectedJob = JobPosts.FirstOrDefault(j => j.MaJob == selectedContract.MaJob);
                    disputeAmount = (decimal)(selectedContract.Sotienkyquy ?? 0);
                }
            }

            return new TranhChapCreateViewModel
            {
                MaHD = maHD,
                SelectedContract = selectedContract,
                SelectedJob = selectedJob,
                EligibleContracts = eligible,
                SoTienTranhChap = disputeAmount
            };
        }

        public static int TaoTranhChap(int currentUserId, string maHD, string lyDo, string moTa, decimal soTien, string loaiBangChung, string? fileUrl, string? moTaBC)
        {
            int nextId = TranhChaps.Count > 0 ? TranhChaps.Max(t => t.MaTranhChap) + 1 : 1;
            
            var tc = new TranhChap
            {
                MaTranhChap = nextId,
                MaHD = maHD,
                NguoiTao = currentUserId,
                LyDo = lyDo,
                MoTaChiTiet = moTa,
                SoTienTranhChap = soTien,
                TrangThai = "DangMo",
                NgayTao = DateTime.Now
            };
            TranhChaps.Add(tc);

            if (!string.IsNullOrEmpty(fileUrl))
            {
                int nextBcId = BangChungTranhChaps.Count > 0 ? BangChungTranhChaps.Max(b => b.MaBangChung) + 1 : 1;
                BangChungTranhChaps.Add(new BangChungTranhChap
                {
                    MaBangChung = nextBcId,
                    MaTranhChap = nextId,
                    NguoiTaiLen = currentUserId,
                    LoaiBangChung = loaiBangChung,
                    DuongDanFile = fileUrl,
                    MoTa = moTaBC ?? "Bằng chứng ban đầu đính kèm đơn tranh chấp",
                    NgayTaiLen = DateTime.Now
                });
            }

            return nextId;
        }

        public static TranhChapDetailViewModel? GetTranhChapDetail(int maTranhChap)
        {
            var tc = TranhChaps.FirstOrDefault(t => t.MaTranhChap == maTranhChap);
            if (tc == null) return null;

            var hd = HopDongs.FirstOrDefault(h => h.MaHD == tc.MaHD) ?? new HopDong();
            var job = JobPosts.FirstOrDefault(j => j.MaJob == hd.MaJob) ?? new JobPost();
            var creator = Users.FirstOrDefault(u => u.MaUser == tc.NguoiTao) ?? new User();
            
            var emp = NhaTuyenDungs.FirstOrDefault(e => e.MaNhaTuyenDung == job.MaNhaTuyenDung);
            var fl = FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == hd.MaFreelancerStudent);
            User opponent = new User();
            if (tc.NguoiTao == emp?.MaUser && fl != null)
            {
                opponent = Users.FirstOrDefault(u => u.MaUser == fl.MaUser) ?? new User();
            }
            else if (emp != null)
            {
                opponent = Users.FirstOrDefault(u => u.MaUser == emp.MaUser) ?? new User();
            }

            var evidences = BangChungTranhChaps.Where(b => b.MaTranhChap == maTranhChap).OrderByDescending(b => b.NgayTaiLen).ToList();
            foreach (var ev in evidences)
            {
                ev.Uploader = Users.FirstOrDefault(u => u.MaUser == ev.NguoiTaiLen);
            }

            return new TranhChapDetailViewModel
            {
                TranhChap = tc,
                HopDong = hd,
                Job = job,
                CreatorUser = creator,
                OpponentUser = opponent,
                BangChungs = evidences
            };
        }

        public static bool ThemBangChungTranhChap(int maTranhChap, int currentUserId, string loaiBangChung, string fileUrl, string? moTa)
        {
            if (string.IsNullOrEmpty(fileUrl)) return false;

            int nextBcId = BangChungTranhChaps.Count > 0 ? BangChungTranhChaps.Max(b => b.MaBangChung) + 1 : 1;
            BangChungTranhChaps.Add(new BangChungTranhChap
            {
                MaBangChung = nextBcId,
                MaTranhChap = maTranhChap,
                NguoiTaiLen = currentUserId,
                LoaiBangChung = loaiBangChung,
                DuongDanFile = fileUrl,
                MoTa = moTa,
                NgayTaiLen = DateTime.Now
            });

            return true;
        }

        // =========================================================================
        // FRL-UC-07 & CUS-UC-12.01: PHÒNG CHAT & TRAO ĐỔI TIN NHẮN
        // =========================================================================
        public static ChatViewModel GetChatView(int currentUserId, string? activeChatId = null, int? partnerUserId = null, string? jobId = null)
        {
            var currentUser = Users.FirstOrDefault(u => u.MaUser == currentUserId) ?? Users.First();
            string userRole = currentUser.MaRole == 2 ? "FreelancerStudent" : "NhaTuyenDung";

            // Nếu truyền partnerUserId hoặc jobId mà chưa có phòng chat -> Tự động khởi tạo hoặc lấy phòng đã có
            if (partnerUserId.HasValue && partnerUserId.Value != currentUserId)
            {
                var existingOrNew = TaoHoacLayPhongChat(currentUserId, partnerUserId.Value, jobId);
                activeChatId = existingOrNew.MaPhongChat;
            }

            // Lấy tất cả phòng chat liên quan tới user hiện tại
            var userRooms = PhongChats
                .Where(p => p.MaUserClient == currentUserId || p.MaUserFreelancer == currentUserId)
                .OrderByDescending(p => p.NgayCapNhat)
                .ToList();

            var conversationList = userRooms.Select(room =>
            {
                int partnerId = room.MaUserClient == currentUserId ? room.MaUserFreelancer : room.MaUserClient;
                var partner = Users.FirstOrDefault(u => u.MaUser == partnerId) ?? new User();
                
                string partnerRole = partner.MaRole == 2 ? "Freelancer" : "Doanh nghiệp";
                string partnerTitle = "";
                string partnerAvatar = "uploads/avatar/user1.jpg";

                if (partner.MaRole == 2)
                {
                    var student = FreelancerStudents.FirstOrDefault(s => s.MaUser == partnerId);
                    partnerTitle = student != null ? $"{student.TenTruong} (Năm {student.NamThu})" : "Sinh viên Freelancer";
                    partnerAvatar = student?.Avatar ?? "uploads/avatar/user1.jpg";
                }
                else
                {
                    var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaUser == partnerId);
                    partnerTitle = employer?.Tencongty ?? "Nhà tuyển dụng";
                    partnerAvatar = employer?.Logo ?? "uploads/avatar/user1.jpg";
                }

                var job = JobPosts.FirstOrDefault(j => j.MaJob == room.MaJob);
                var contract = HopDongs.FirstOrDefault(h => h.MaHD == room.MaHD);

                return new ChatConversationItemViewModel
                {
                    PhongChat = room,
                    PartnerUser = partner,
                    PartnerRole = partnerRole,
                    PartnerTitle = partnerTitle,
                    PartnerAvatar = partnerAvatar,
                    IsOnline = true,
                    RelatedJob = job,
                    RelatedContract = contract
                };
            }).ToList();

            // Xác định Active Conversation
            ChatConversationItemViewModel? activeConv = null;
            if (!string.IsNullOrEmpty(activeChatId))
            {
                activeConv = conversationList.FirstOrDefault(c => c.PhongChat.MaPhongChat == activeChatId);
            }
            if (activeConv == null && conversationList.Count > 0)
            {
                activeConv = conversationList.First();
            }

            // Lấy tin nhắn của active conversation
            List<ChatMessageItemViewModel> activeMessages = new();
            if (activeConv != null)
            {
                var msgs = TinNhans
                    .Where(m => m.MaPhongChat == activeConv.PhongChat.MaPhongChat)
                    .OrderBy(m => m.NgayGui)
                    .ToList();

                activeMessages = msgs.Select(m =>
                {
                    var sender = Users.FirstOrDefault(u => u.MaUser == m.NguoiGui) ?? new User();
                    DuAnTrongPortfolio? prj = null;
                    if (!string.IsNullOrEmpty(m.MaDuAnPortfolio))
                    {
                        prj = DuAns.FirstOrDefault(d => d.MaDA == m.MaDuAnPortfolio);
                    }

                    return new ChatMessageItemViewModel
                    {
                        TinNhan = m,
                        SenderUser = sender,
                        IsMine = m.NguoiGui == currentUserId,
                        PortfolioProject = prj
                    };
                }).ToList();
            }

            // Lấy danh sách portfolio projects của sinh viên nếu là Freelancer
            List<DuAnTrongPortfolio> studentProjects = new();
            var currentStudent = FreelancerStudents.FirstOrDefault(s => s.MaUser == currentUserId);
            if (currentStudent != null)
            {
                var port = Portfolios.FirstOrDefault(p => p.MaFreelancerStudents == currentStudent.MaFreelancerStudents);
                if (port != null)
                {
                    studentProjects = DuAns.Where(d => d.MaPortfolio == port.MaPortfolio).ToList();
                }
            }
            else
            {
                // Cho phép lấy demo projects
                studentProjects = DuAns.Take(4).ToList();
            }

            return new ChatViewModel
            {
                CurrentUser = currentUser,
                CurrentUserRole = userRole,
                Conversations = conversationList,
                ActiveConversation = activeConv,
                ActiveMessages = activeMessages,
                StudentPortfolioProjects = studentProjects
            };
        }

        public static PhongChat TaoHoacLayPhongChat(int currentUserId, int partnerUserId, string? jobId = null)
        {
            var currentUser = Users.FirstOrDefault(u => u.MaUser == currentUserId);
            int clientUserId = currentUser?.MaRole == 3 ? currentUserId : partnerUserId;
            int flUserId = currentUser?.MaRole == 2 ? currentUserId : partnerUserId;

            var existing = PhongChats.FirstOrDefault(p => p.MaUserClient == clientUserId && p.MaUserFreelancer == flUserId);
            if (existing != null)
            {
                if (!string.IsNullOrEmpty(jobId) && string.IsNullOrEmpty(existing.MaJob))
                {
                    existing.MaJob = jobId;
                }
                return existing;
            }

            string newId = $"CHAT_{PhongChats.Count + 1:D3}";
            var newRoom = new PhongChat
            {
                MaPhongChat = newId,
                MaUserClient = clientUserId,
                MaUserFreelancer = flUserId,
                MaJob = jobId ?? "JOB_01",
                TinNhanCuoi = "Đã khởi tạo cuộc trò chuyện.",
                ThoiGianTinNhanCuoi = DateTime.Now,
                SoTinNhanChuaDoc = 0,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };
            PhongChats.Insert(0, newRoom);
            return newRoom;
        }

        public static TinNhan GuiTinNhan(int senderUserId, string chatId, string noiDung, string? fileUrl = null, string? tenFile = null, string? loaiTinNhan = "VanBan", string? maDuAnPortfolio = null)
        {
            int nextId = TinNhans.Count > 0 ? TinNhans.Max(m => m.MaTinNhan) + 1 : 1;
            var newMsg = new TinNhan
            {
                MaTinNhan = nextId,
                MaPhongChat = chatId,
                NguoiGui = senderUserId,
                NoiDung = noiDung,
                LoaiTinNhan = loaiTinNhan ?? "VanBan",
                FileDinhKemUrl = fileUrl,
                TenFile = tenFile,
                MaDuAnPortfolio = maDuAnPortfolio,
                DaDoc = false,
                NgayGui = DateTime.Now
            };
            TinNhans.Add(newMsg);

            // Cập nhật room
            var room = PhongChats.FirstOrDefault(p => p.MaPhongChat == chatId);
            if (room != null)
            {
                room.TinNhanCuoi = string.IsNullOrWhiteSpace(noiDung) ? $"[Đã gửi {loaiTinNhan}]" : noiDung;
                room.ThoiGianTinNhanCuoi = DateTime.Now;
                room.NgayCapNhat = DateTime.Now;
            }

            return newMsg;
        }

        // ==================== CUS-UC-01: QUÊN MẬT KHẨU ====================
        public static bool ResetPasswordByEmailOrUsername(string emailOrUsername, string newPassword)
        {
            var user = Users.FirstOrDefault(u =>
                u.TenTaiKhoanUser.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                u.EmailUser.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase));

            if (user == null) return false;
            user.PashwordHash = newPassword;
            return true;
        }

        // ==================== CUS-UC-04: CHI TIẾT FREELANCER CHO EMPLOYER ====================
        public static EmployerFreelancerDetailViewModel? GetFreelancerDetailForEmployer(int maFreelancerStudent, int currentEmployerId = 1)
        {
            var freelancer = FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == maFreelancerStudent);
            if (freelancer == null) return null;

            var user = Users.FirstOrDefault(u => u.MaUser == freelancer.MaUser) ?? new User();
            var chuyenNganh = ChuyenNganhs.FirstOrDefault(c => c.MaChuyenNganh == freelancer.MaChuyenNganh)?.TenChuyenNganh ?? "Công nghệ thông tin";
            var skills = KyNangs.Where(k => k.MaFreelancerStudents == maFreelancerStudent).Select(k => k.TenKyNang).ToList();
            var portfolio = Portfolios.FirstOrDefault(p => p.MaFreelancerStudents == maFreelancerStudent);
            var duAns = portfolio != null ? DuAns.Where(d => d.MaPortfolio == portfolio.MaPortfolio).ToList() : new List<DuAnTrongPortfolio>();

            // Danh gia & Nhan xet (dựa vào MaUser của Freelancer)
            var danhGias = DanhGias.Where(d => d.MaNguoiDuocDanhGia == user.MaUser).Select(d =>
            {
                var reviewer = Users.FirstOrDefault(u => u.MaUser == d.MaNguoiDanhGia);
                var employer = NhaTuyenDungs.FirstOrDefault(e => e.MaUser == d.MaNguoiDanhGia);
                var jobTitle = HopDongs.FirstOrDefault(h => h.MaHD == d.MaHD)?.JobPost?.Tieude ?? "Dự án hoàn thành";

                return new FreelancerReviewItemViewModel
                {
                    MaDanhGia = d.MaDanhGia,
                    MaHD = d.MaHD,
                    TenNhaTuyenDung = employer?.Tencongty ?? reviewer?.HotenUser ?? "Nhà tuyển dụng",
                    AvatarNhaTuyenDung = employer?.Avatar ?? "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=100&auto=format&fit=crop&q=80",
                    TenCongViec = jobTitle,
                    SoSao = d.SoSao,
                    NhanXet = d.NhanXet,
                    NgayDanhGia = d.NgayDanhGia
                };
            }).ToList();

            double avgRating = danhGias.Count > 0 ? danhGias.Average(d => d.SoSao) : 5.0;
            int completedContracts = HopDongs.Count(h => h.MaFreelancerStudent == maFreelancerStudent && h.TrangThai == "DaNghiemThu");

            var bookmark = FreelancerYeuThichs.FirstOrDefault(b => b.MaNhaTuyenDung == currentEmployerId && b.MaFreelancerStudent == maFreelancerStudent);
            bool hasActiveOffer = YeuCauThues.Any(y => y.MaNhaTuyenDung == currentEmployerId && y.MaFreelancerStudent == maFreelancerStudent && y.TrangThai == "ChoPhanHoi");

            return new EmployerFreelancerDetailViewModel
            {
                Freelancer = freelancer,
                User = user,
                TenChuyenNganh = chuyenNganh,
                KyNangs = skills,
                Portfolio = portfolio,
                DuAns = duAns,
                DanhGias = danhGias,
                DiemDanhGiaTrungBinh = Math.Round(avgRating, 1),
                TongSoDanhGia = danhGias.Count,
                SoHopDongThanhCong = Math.Max(completedContracts, 1),
                TiLeHoanThanhDungHan = 100.0,
                IsBookmarked = bookmark != null,
                CurrentEmployerId = currentEmployerId,
                GhiChuQuanTam = bookmark?.GhiChu,
                HasActiveDirectOffer = hasActiveOffer,
                DaXacMinhSinhVien = true
            };
        }

        // ==================== CUS-UC-06: SỬA BÀI ĐĂNG ====================
        public static JobEditViewModel? GetJobEditViewModel(string maJob)
        {
            var job = JobPosts.FirstOrDefault(j => j.MaJob == maJob);
            if (job == null) return null;

            return new JobEditViewModel
            {
                MaJob = job.MaJob,
                MaNhaTuyenDung = job.MaNhaTuyenDung,
                Tieude = job.Tieude,
                Mota = job.Mota ?? string.Empty,
                Kynangyeucau = job.Kynangyeucau ?? string.Empty,
                Thulao = job.Thulao,
                Thoigiandukienhoanthanh = job.Thoigiandukienhoanthanh,
                Soluongtuyen = job.Soluongtuyen ?? 1,
                Status = job.Status,
                Thoigiandangtuyen = job.Thoigiandangtuyen,
                PhiDangBai = 0
            };
        }

        public static bool UpdateJobPost(JobEditViewModel model)
        {
            var job = JobPosts.FirstOrDefault(j => j.MaJob == model.MaJob);
            if (job == null) return false;

            job.Tieude = model.Tieude;
            job.Mota = model.Mota;
            job.Kynangyeucau = model.Kynangyeucau;
            job.Thulao = model.Thulao;
            job.Thoigiandukienhoanthanh = model.Thoigiandukienhoanthanh;
            job.Soluongtuyen = model.Soluongtuyen;
            job.Status = model.Status;

            return true;
        }

        // ==================== CUS-UC-06: GỬI LỜI MỜI THUÊ TRỰC TIẾP (UngThue) ====================
        public static SendDirectOfferViewModel? GetSendDirectOfferViewModel(int maFreelancerStudent, int employerId = 1)
        {
            var freelancer = FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == maFreelancerStudent);
            if (freelancer == null) return null;

            var user = Users.FirstOrDefault(u => u.MaUser == freelancer.MaUser) ?? new User();
            var chuyenNganh = ChuyenNganhs.FirstOrDefault(c => c.MaChuyenNganh == freelancer.MaChuyenNganh)?.TenChuyenNganh ?? "Công nghệ thông tin";

            return new SendDirectOfferViewModel
            {
                MaFreelancerStudent = maFreelancerStudent,
                MaNhaTuyenDung = employerId,
                TenFreelancer = user.HotenUser,
                AvatarFreelancer = freelancer.Avatar,
                TenTruong = freelancer.TenTruong,
                TenChuyenNganh = chuyenNganh,
                ChiPhiThamKhao = (double?)freelancer.ChiPhiTu,
                TieuDeCongViec = "",
                MoTaCongViec = "",
                NganSachDeNghi = (double?)freelancer.ChiPhiTu ?? 1000000,
                ThoiHanDuKien = "7 ngày kể từ khi bắt đầu"
            };
        }

        public static YeuCauThueFreelancer CreateUngThue(SendDirectOfferViewModel model)
        {
            int nextId = YeuCauThues.Count > 0 ? YeuCauThues.Max(y => y.MaYeuCau) + 1 : 1;
            var newOffer = new YeuCauThueFreelancer
            {
                MaYeuCau = nextId,
                MaNhaTuyenDung = model.MaNhaTuyenDung > 0 ? model.MaNhaTuyenDung : 1,
                MaFreelancerStudent = model.MaFreelancerStudent,
                TieuDeCongViec = model.TieuDeCongViec,
                MoTaCongViec = model.MoTaCongViec,
                NganSachDeNghi = model.NganSachDeNghi,
                ThoiHanDuKien = model.ThoiHanDuKien,
                NgayGui = DateTime.Now,
                TrangThai = "ChoPhanHoi",
                LyDoTuChoi = null
            };

            YeuCauThues.Insert(0, newOffer);

            // Tự động tạo thông báo gửi tới Freelancer
            var freelancer = FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == model.MaFreelancerStudent);
            if (freelancer != null)
            {
                int nextTbId = ThongBaos.Count > 0 ? ThongBaos.Max(t => t.MaThongBao) + 1 : 1;
                var ntd = NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == model.MaNhaTuyenDung);
                string ntdName = ntd?.Tencongty ?? "Một nhà tuyển dụng";

                ThongBaos.Insert(0, new ThongBao
                {
                    MaThongBao = nextTbId,
                    MaUser = freelancer.MaUser,
                    TieuDe = "Lời mời làm việc trực tiếp mới!",
                    NoiDung = $"{ntdName} vừa gửi cho bạn lời mời thuê trực tiếp: '{model.TieuDeCongViec}' với ngân sách {model.NganSachDeNghi:N0} VNĐ.",
                    LoaiThongBao = "UngThue",
                    LinkDieuHuong = "/Freelancer/LoiMoiNhanViec",
                    DaDoc = false,
                    NgayTao = DateTime.Now
                });
            }

            return newOffer;
        }

        // ==================== CUS-UC-12: QUẢN LÝ THÔNG BÁO ====================
        public static ThongBaoListViewModel GetThongBaos(int maUser, string? filterLoai = null, string? filterTrangThai = null)
        {
            var query = ThongBaos.Where(t => t.MaUser == maUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterLoai) && filterLoai != "All")
            {
                query = query.Where(t => t.LoaiThongBao.Equals(filterLoai, StringComparison.OrdinalIgnoreCase));
            }

            if (filterTrangThai == "Unread")
            {
                query = query.Where(t => !t.DaDoc);
            }
            else if (filterTrangThai == "Read")
            {
                query = query.Where(t => t.DaDoc);
            }

            var items = query.OrderByDescending(t => t.NgayTao).Select(t => new ThongBaoItemViewModel
            {
                MaThongBao = t.MaThongBao,
                MaUser = t.MaUser,
                TieuDe = t.TieuDe,
                NoiDung = t.NoiDung,
                LoaiThongBao = t.LoaiThongBao,
                LinkDieuHuong = t.LinkDieuHuong,
                DaDoc = t.DaDoc,
                NgayTao = t.NgayTao
            }).ToList();

            int unreadCount = ThongBaos.Count(t => t.MaUser == maUser && !t.DaDoc);

            return new ThongBaoListViewModel
            {
                Items = items,
                SoChuaDoc = unreadCount,
                FilterLoai = filterLoai,
                FilterTrangThai = filterTrangThai,
                CurrentUserId = maUser
            };
        }

        public static bool MarkThongBaoAsRead(int maThongBao)
        {
            var tb = ThongBaos.FirstOrDefault(t => t.MaThongBao == maThongBao);
            if (tb == null) return false;
            tb.DaDoc = true;
            return true;
        }

        public static void MarkAllThongBaoAsRead(int maUser)
        {
            var userTbs = ThongBaos.Where(t => t.MaUser == maUser).ToList();
            foreach (var tb in userTbs)
            {
                tb.DaDoc = true;
            }
        }

        // ==================== FRL-UC-06: ĐỀ XUẤT HỢP ĐỒNG ====================
        public static DeXuatHopDongViewModel GetDeXuatHopDongViewModel(string? maJob, int? maUngThue, int freelancerId = 1)
        {
            var model = new DeXuatHopDongViewModel
            {
                MaJob = maJob,
                MaYeuCauUngThue = maUngThue,
                MaFreelancerStudent = freelancerId,
                NgayBatDau = DateTime.Now,
                NgayKetThuc = DateTime.Now.AddDays(15),
                Hinhthuclamviec = "Remote"
            };

            if (!string.IsNullOrEmpty(maJob))
            {
                var job = JobPosts.FirstOrDefault(j => j.MaJob == maJob);
                if (job != null)
                {
                    var ntd = NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == job.MaNhaTuyenDung);
                    model.MaNhaTuyenDung = job.MaNhaTuyenDung;
                    model.TieuDeCongViec = job.Tieude;
                    model.TenNhaTuyenDung = ntd?.Tencongty ?? "Nhà tuyển dụng";
                    model.AvatarNhaTuyenDung = ntd?.Avatar ?? "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=100&auto=format&fit=crop&q=80";
                    model.TongGiaTriHopDong = job.Thulao ?? 2000000;
                }
            }
            else if (maUngThue.HasValue)
            {
                var ut = YeuCauThues.FirstOrDefault(u => u.MaYeuCau == maUngThue.Value);
                if (ut != null)
                {
                    var ntd = NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == ut.MaNhaTuyenDung);
                    model.MaNhaTuyenDung = ut.MaNhaTuyenDung;
                    model.TieuDeCongViec = ut.TieuDeCongViec;
                    model.TenNhaTuyenDung = ntd?.Tencongty ?? "Nhà tuyển dụng";
                    model.AvatarNhaTuyenDung = ntd?.Avatar ?? "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=100&auto=format&fit=crop&q=80";
                    model.TongGiaTriHopDong = ut.NganSachDeNghi ?? 2000000;
                }
            }
            else
            {
                var firstJob = JobPosts.FirstOrDefault();
                if (firstJob != null)
                {
                    var ntd = NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == firstJob.MaNhaTuyenDung);
                    model.MaJob = firstJob.MaJob;
                    model.MaNhaTuyenDung = firstJob.MaNhaTuyenDung;
                    model.TieuDeCongViec = firstJob.Tieude;
                    model.TenNhaTuyenDung = ntd?.Tencongty ?? "Công Ty Công Nghệ ABC";
                    model.AvatarNhaTuyenDung = ntd?.Avatar;
                    model.TongGiaTriHopDong = firstJob.Thulao ?? 2500000;
                }
            }

            return model;
        }

        public static HopDong CreateDeXuatHopDong(DeXuatHopDongViewModel model)
        {
            int nextHdIndex = HopDongs.Count + 1;
            string newMaHD = $"HD_2026_{nextHdIndex:D3}";
            string jobCode = !string.IsNullOrEmpty(model.MaJob) ? model.MaJob : $"JOB_DE_XUAT_{nextHdIndex:D2}";

            var newContract = new HopDong
            {
                MaHD = newMaHD,
                MaJob = jobCode,
                MaFreelancerStudent = model.MaFreelancerStudent,
                NgayBatDau = model.NgayBatDau,
                NgayKetThuc = model.NgayKetThuc,
                Sotienkyquy = model.TongGiaTriHopDong ?? 1500000,
                Hinhthuclamviec = model.Hinhthuclamviec,
                TrangThai = "ChoKyQuy"
            };

            HopDongs.Insert(0, newContract);

            // Gửi thông báo tới Nhà tuyển dụng
            var ntd = NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == model.MaNhaTuyenDung);
            if (ntd != null)
            {
                int nextTbId = ThongBaos.Count > 0 ? ThongBaos.Max(t => t.MaThongBao) + 1 : 1;
                ThongBaos.Insert(0, new ThongBao
                {
                    MaThongBao = nextTbId,
                    MaUser = ntd.MaUser,
                    TieuDe = "Freelancer gửi đề xuất hợp đồng mới",
                    NoiDung = $"Freelancer đã tạo dự thảo hợp đồng {newMaHD} cho công việc '{model.TieuDeCongViec}' với tổng giá trị {model.TongGiaTriHopDong:N0} VNĐ. Vui lòng xem xét và tiến hành ký quỹ.",
                    LoaiThongBao = "HopDong",
                    LinkDieuHuong = $"/HopDong/Details?id={newMaHD}",
                    DaDoc = false,
                    NgayTao = DateTime.Now
                });
            }

            return newContract;
        }

        // ==================== FRL-UC-11: RÚT TIỀN TỪ VÍ ====================
        public static RutTienViewModel GetRutTienViewModel(int userId = 1)
        {
            var wallet = Wallets.FirstOrDefault(w => w.MaUser == userId);
            decimal balance = wallet?.SoDuKhaDung ?? 5200000m;
            var accounts = TaiKhoanNganHangs.Where(t => t.MaUser == userId).ToList();
            var defaultAcc = accounts.FirstOrDefault(t => t.LaMacDinh) ?? accounts.FirstOrDefault();
            var withdrawalHistory = YeuCauRutTiens.Where(r => r.MaWallet == (wallet?.MaWallet ?? 1)).OrderByDescending(r => r.NgayYeuCau).ToList();

            return new RutTienViewModel
            {
                MaWallet = wallet?.MaWallet ?? 1,
                SoDuHienTai = balance,
                SoDuKhaDung = balance,
                SoTienRut = Math.Min(balance, 1000000m),
                TenNganHang = defaultAcc?.TenNganHang ?? "Vietcombank",
                SoTaiKhoan = defaultAcc?.SoTaiKhoan ?? "1012345678",
                TenChuTaiKhoan = defaultAcc?.TenChuTaiKhoan ?? "NGUYEN VAN AN",
                SelectedMaTKNH = defaultAcc?.MaTKNH,
                TaiKhoanDaLuus = accounts,
                LichSuRutTiens = withdrawalHistory
            };
        }

        public static bool CreateYeuCauRutTien(RutTienViewModel model, int userId = 1)
        {
            var wallet = Wallets.FirstOrDefault(w => w.MaUser == userId);
            if (wallet == null || wallet.SoDuKhaDung < model.SoTienRut) return false;

            decimal soDuTruoc = wallet.SoDuKhaDung;
            // Trừ số dư khả dụng
            wallet.SoDuKhaDung -= model.SoTienRut;
            decimal soDuSau = wallet.SoDuKhaDung;

            int nextId = YeuCauRutTiens.Count > 0 ? YeuCauRutTiens.Max(y => y.MaRutTien) + 1 : 1;
            var req = new YeuCauRutTien
            {
                MaRutTien = nextId,
                MaWallet = wallet.MaWallet,
                SoTienRut = model.SoTienRut,
                TenNganHang = model.TenNganHang,
                SoTaiKhoan = model.SoTaiKhoan,
                TenChuTaiKhoan = model.TenChuTaiKhoan.ToUpper(),
                NgayYeuCau = DateTime.Now,
                TrangThai = "ChoDuyet"
            };

            YeuCauRutTiens.Insert(0, req);

            // Ghi nhận lịch sử giao dịch
            int nextGd = LichSuGiaoDichs.Count > 0 ? LichSuGiaoDichs.Max(g => g.MaGiaoDich) + 1 : 1;
            LichSuGiaoDichs.Insert(0, new LichSuGiaoDich
            {
                MaGiaoDich = nextGd,
                MaWallet = wallet.MaWallet,
                LoaiGiaoDich = "RutTien",
                SoTien = model.SoTienRut,
                SoDuTruoc = soDuTruoc,
                SoDuSau = soDuSau,
                NoiDung = $"Yêu cầu rút tiền về {model.TenNganHang} ({model.SoTaiKhoan})",
                PhuongThuc = "ChuyenKhoan",
                TrangThai = "DangXuLy",
                NgayTao = DateTime.Now
            });

            return true;
        }

        // ==================== FRL-UC-11: QUẢN LÝ TÀI KHOẢN NGÂN HÀNG ====================
        public static TaiKhoanNganHangListViewModel GetTaiKhoanNganHangList(int userId = 1)
        {
            var accounts = TaiKhoanNganHangs.Where(t => t.MaUser == userId).OrderByDescending(t => t.LaMacDinh).ThenByDescending(t => t.NgayThem).ToList();
            var user = Users.FirstOrDefault(u => u.MaUser == userId);

            return new TaiKhoanNganHangListViewModel
            {
                TaiKhoans = accounts,
                NewAccount = new TaiKhoanNganHangCreateViewModel
                {
                    MaUser = userId,
                    TenChuTaiKhoan = user?.HotenUser?.ToUpper() ?? "NGUYEN VAN AN",
                    LaMacDinh = accounts.Count == 0
                }
            };
        }

        public static bool AddTaiKhoanNganHang(TaiKhoanNganHangCreateViewModel model, int userId = 1)
        {
            if (model.LaMacDinh)
            {
                var existing = TaiKhoanNganHangs.Where(t => t.MaUser == userId).ToList();
                foreach (var acc in existing)
                {
                    acc.LaMacDinh = false;
                }
            }

            int nextId = TaiKhoanNganHangs.Count > 0 ? TaiKhoanNganHangs.Max(t => t.MaTKNH) + 1 : 1;
            TaiKhoanNganHangs.Add(new TaiKhoanNganHang
            {
                MaTKNH = nextId,
                MaUser = userId,
                TenNganHang = model.TenNganHang,
                ChiNhanh = model.ChiNhanh,
                SoTaiKhoan = model.SoTaiKhoan,
                TenChuTaiKhoan = model.TenChuTaiKhoan.ToUpper(),
                LaMacDinh = model.LaMacDinh,
                NgayThem = DateTime.Now
            });

            return true;
        }

        public static bool SetDefaultTaiKhoanNganHang(int maTKNH, int userId = 1)
        {
            var accounts = TaiKhoanNganHangs.Where(t => t.MaUser == userId).ToList();
            var target = accounts.FirstOrDefault(t => t.MaTKNH == maTKNH);
            if (target == null) return false;

            foreach (var acc in accounts)
            {
                acc.LaMacDinh = (acc.MaTKNH == maTKNH);
            }
            return true;
        }

        public static bool DeleteTaiKhoanNganHang(int maTKNH, int userId = 1)
        {
            var target = TaiKhoanNganHangs.FirstOrDefault(t => t.MaTKNH == maTKNH && t.MaUser == userId);
            if (target == null) return false;

            TaiKhoanNganHangs.Remove(target);
            return true;
        }

        // =========================================================================
        // ADMIN HELPER METHODS (AD-UC-01 ĐẾN AD-UC-11)
        // =========================================================================

        // AD-UC-01.01: QUẢN LÝ TÀI KHOẢN NGƯỜI DÙNG
        public static AdminUserManagementViewModel GetAdminUserManagement(string? keyword = null, int? role = null, string? status = null)
        {
            var query = Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(u => u.HotenUser.ToLower().Contains(kw) ||
                                         u.TenTaiKhoanUser.ToLower().Contains(kw) ||
                                         u.EmailUser.ToLower().Contains(kw));
            }

            if (role.HasValue && role.Value > 0)
            {
                query = query.Where(u => u.MaRole == role.Value);
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(u => u.Status == status);
            }

            var items = query.AsEnumerable().Select(u =>
            {
                var wallet = Wallets.FirstOrDefault(w => w.MaUser == u.MaUser);
                string roleName = u.MaRole switch
                {
                    1 => "Freelancer Sinh viên",
                    2 => "Nhà tuyển dụng",
                    3 => "Quản trị viên",
                    _ => "Thành viên"
                };

                string? avatar = null;
                string? extraInfo = null;

                if (u.MaRole == 1)
                {
                    var student = FreelancerStudents.FirstOrDefault(s => s.MaUser == u.MaUser);
                    avatar = student != null ? student.Avatar : null;
                    extraInfo = student != null ? $"{student.TenTruong} - GPA: {student.GPA}" : null;
                }
                else if (u.MaRole == 2)
                {
                    var emp = NhaTuyenDungs.FirstOrDefault(e => e.MaUser == u.MaUser);
                    avatar = emp != null ? emp.Avatar : null;
                    extraInfo = emp != null ? emp.Tencongty : null;
                }

                bool verified = MinhChungs.Any(m => m.MaUser == u.MaUser && m.TrangThaiGuiMinhChung == "Đã xác minh");

                return new AdminUserItemViewModel
                {
                    MaUser = u.MaUser,
                    HotenUser = u.HotenUser,
                    TenTaiKhoanUser = u.TenTaiKhoanUser,
                    EmailUser = u.EmailUser,
                    SdtUser = u.SdtUser,
                    Status = u.Status,
                    NgayTao = u.NgayTao,
                    MaRole = u.MaRole,
                    RoleName = roleName,
                    Avatar = avatar,
                    ThongTinPhu = extraInfo,
                    SoDuVi = wallet != null ? wallet.SoDuKhaDung : 0,
                    DaXacMinhMinhChung = verified
                };
            }).ToList();

            return new AdminUserManagementViewModel
            {
                Users = items,
                Keyword = keyword,
                SelectedRole = role,
                SelectedStatus = status,
                SoSinhVien = Users.Count(u => u.MaRole == 1),
                SoNhaTuyenDung = Users.Count(u => u.MaRole == 2),
                SoBiKhoa = Users.Count(u => u.Status == "LOCKED")
            };
        }

        // AD-UC-02.01: XỬ LÝ TRẠNG THÁI TÀI KHOẢN
        public static AdminAccountStatusViewModel GetAdminAccountStatusView()
        {
            var users = GetAdminUserManagement().Users;
            return new AdminAccountStatusViewModel
            {
                DanhSachTaiKhoan = users,
                LichSuXuLys = LichSuXuLyTaiKhoans.OrderByDescending(l => l.NgayXuLy).ToList()
            };
        }

        public static bool XuLyTrangThaiTaiKhoan(int maUser, int adminId, string hanhDong, string lyDo)
        {
            var user = Users.FirstOrDefault(u => u.MaUser == maUser);
            if (user == null) return false;

            string oldStatus = user.Status;
            string newStatus = hanhDong == "KHOA_TAI_KHOAN" ? "LOCKED" : "ACTIVE";
            user.Status = newStatus;

            int nextId = LichSuXuLyTaiKhoans.Count > 0 ? LichSuXuLyTaiKhoans.Max(l => l.MaLichSu) + 1 : 1;
            LichSuXuLyTaiKhoans.Insert(0, new LichSuXuLyTaiKhoan
            {
                MaLichSu = nextId,
                MaUser = maUser,
                MaAdmin = adminId,
                HanhDong = hanhDong,
                LyDo = lyDo,
                TrangThaiTruocKhiXuLy = oldStatus,
                TrangThaiSauKhiXuLy = newStatus,
                NgayXuLy = DateTime.Now
            });

            return true;
        }

        // AD-UC-03.01: XỬ LÝ YÊU CẦU NẠP TIỀN
        public static AdminDepositManagementViewModel GetAdminDeposits(string? filterStatus = null)
        {
            var query = YeuCauNapTiens.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterStatus) && filterStatus != "All")
            {
                query = query.Where(y => y.TrangThai == filterStatus);
            }

            var items = query.AsEnumerable().OrderByDescending(y => y.NgayTao).Select(y =>
            {
                var wallet = Wallets.FirstOrDefault(w => w.MaWallet == y.MaWallet);
                var user = wallet != null ? Users.FirstOrDefault(u => u.MaUser == wallet.MaUser) : null;
                return new AdminDepositItemViewModel
                {
                    MaYeuCauNap = y.MaYeuCau,
                    MaUser = wallet != null ? wallet.MaUser : 0,
                    TenNguoiNap = user != null ? user.HotenUser : "Người dùng",
                    EmailNguoiNap = user != null ? user.EmailUser : "",
                    SoTienNap = y.SoTien,
                    PhuongThuc = y.PhuongThuc,
                    MaGDNganHang = y.MaGiaoDichNganHang,
                    AnhBienLai = y.AnhBienLai,
                    TrangThai = y.TrangThai,
                    NgayYeuCau = y.NgayTao,
                    GhiChu = y.GhiChu,
                    LyDoTuChoi = y.LyDoTuChoi
                };
            }).ToList();

            var pending = YeuCauNapTiens.Where(y => y.TrangThai == "ChoDuyet").ToList();

            return new AdminDepositManagementViewModel
            {
                YeuCauNaps = items,
                FilterTrangThai = filterStatus,
                SoChoDuyet = pending.Count,
                TongTienChoDuyet = pending.Sum(p => p.SoTien)
            };
        }

        public static bool DuyetYeuCauNap(int maYeuCauNap, int adminId = 1)
        {
            var req = YeuCauNapTiens.FirstOrDefault(y => y.MaYeuCau == maYeuCauNap);
            if (req == null || req.TrangThai != "ChoDuyet") return false;

            req.TrangThai = "DaDuyet";
            req.NgayDuyet = DateTime.Now;

            // Cộng tiền vào ví
            var wallet = Wallets.FirstOrDefault(w => w.MaWallet == req.MaWallet);
            if (wallet != null)
            {
                decimal oldBal = wallet.SoDuKhaDung;
                wallet.SoDuKhaDung += req.SoTien;

                int nextGd = LichSuGiaoDichs.Count > 0 ? LichSuGiaoDichs.Max(g => g.MaGiaoDich) + 1 : 1;
                LichSuGiaoDichs.Insert(0, new LichSuGiaoDich
                {
                    MaGiaoDich = nextGd,
                    MaWallet = wallet.MaWallet,
                    LoaiGiaoDich = "NapTien",
                    SoTien = req.SoTien,
                    SoDuTruoc = oldBal,
                    SoDuSau = wallet.SoDuKhaDung,
                    NoiDung = $"Phê duyệt nạp tiền qua {req.PhuongThuc} (#{req.MaYeuCau})",
                    PhuongThuc = req.PhuongThuc,
                    TrangThai = "ThanhCong",
                    NgayTao = DateTime.Now
                });
            }

            return true;
        }

        public static bool TuChoiYeuCauNap(int maYeuCauNap, int adminId, string lyDo)
        {
            var req = YeuCauNapTiens.FirstOrDefault(y => y.MaYeuCau == maYeuCauNap);
            if (req == null || req.TrangThai != "ChoDuyet") return false;

            req.TrangThai = "TuChoi";
            req.NgayDuyet = DateTime.Now;
            req.LyDoTuChoi = lyDo;

            return true;
        }

        // AD-UC-04.01: ĐIỀU CHỈNH SỐ DƯ TÀI KHOẢN
        public static AdminBalanceAdjustmentViewModel GetAdminBalanceAdjustment()
        {
            var wallets = Wallets.Select(w =>
            {
                var user = Users.FirstOrDefault(u => u.MaUser == w.MaUser);
                string roleName = user?.MaRole == 1 ? "Sinh viên" : (user?.MaRole == 2 ? "Nhà tuyển dụng" : "Admin");
                return new AdminWalletItemViewModel
                {
                    MaWallet = w.MaWallet,
                    MaUser = w.MaUser,
                    TenChuVi = user?.HotenUser ?? "Người dùng",
                    RoleName = roleName,
                    SoDuKhaDung = w.SoDuKhaDung,
                    SoDuDongBang = w.SoDuDongBang
                };
            }).ToList();

            return new AdminBalanceAdjustmentViewModel
            {
                Wallets = wallets,
                LichSuDieuChinhs = DieuChinhSoDus.OrderByDescending(d => d.NgayDieuChinh).ToList()
            };
        }

        public static bool ThucHienDieuChinhSoDu(int maWallet, int adminId, string loaiDieuChinh, decimal soTien, string lyDo)
        {
            var wallet = Wallets.FirstOrDefault(w => w.MaWallet == maWallet);
            if (wallet == null) return false;

            decimal oldBal = wallet.SoDuKhaDung;
            if (loaiDieuChinh == "TruTien" && wallet.SoDuKhaDung < soTien) return false;

            if (loaiDieuChinh == "CongTien")
            {
                wallet.SoDuKhaDung += soTien;
            }
            else
            {
                wallet.SoDuKhaDung -= soTien;
            }

            decimal newBal = wallet.SoDuKhaDung;

            int nextId = DieuChinhSoDus.Count > 0 ? DieuChinhSoDus.Max(d => d.MaDieuChinh) + 1 : 1;
            DieuChinhSoDus.Insert(0, new DieuChinhSoDu
            {
                MaDieuChinh = nextId,
                MaWallet = maWallet,
                MaAdmin = adminId,
                LoaiDieuChinh = loaiDieuChinh,
                SoTienDieuChinh = soTien,
                SoDuTruoc = oldBal,
                SoDuSau = newBal,
                LyDo = lyDo,
                NgayDieuChinh = DateTime.Now
            });

            // Ghi nhận vào lịch sử giao dịch
            int nextGd = LichSuGiaoDichs.Count > 0 ? LichSuGiaoDichs.Max(g => g.MaGiaoDich) + 1 : 1;
            LichSuGiaoDichs.Insert(0, new LichSuGiaoDich
            {
                MaGiaoDich = nextGd,
                MaWallet = maWallet,
                LoaiGiaoDich = "DieuChinhAdmin",
                SoTien = soTien,
                SoDuTruoc = oldBal,
                SoDuSau = newBal,
                NoiDung = $"Admin điều chỉnh ({loaiDieuChinh}): {lyDo}",
                PhuongThuc = "HeThong",
                TrangThai = "ThanhCong",
                NgayTao = DateTime.Now
            });

            return true;
        }

        // AD-UC-05.01: XỬ LÝ THANH TOÁN GIAO DỊCH (ESCROW)
        public static AdminTransactionManagementViewModel GetAdminTransactionManagement()
        {
            var contracts = HopDongs.Select(h =>
            {
                var job = JobPosts.FirstOrDefault(j => j.MaJob == h.MaJob);
                var ntd = job != null ? NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == job.MaNhaTuyenDung) : null;
                var freelancer = FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == h.MaFreelancerStudent);
                var fUser = freelancer != null ? Users.FirstOrDefault(u => u.MaUser == freelancer.MaUser) : null;

                return new AdminEscrowContractItemViewModel
                {
                    MaHD = h.MaHD,
                    TenCongViec = job?.Tieude ?? "Dự án Freelance",
                    TenNhaTuyenDung = ntd?.Tencongty ?? "Nhà tuyển dụng",
                    TenFreelancer = fUser?.HotenUser ?? "Freelancer",
                    SoTienKyQuy = (decimal)(h.Sotienkyquy ?? 0),
                    TrangThai = h.TrangThai,
                    NgayBatDau = h.NgayBatDau ?? DateTime.Now,
                    NgayKetThuc = h.NgayKetThuc
                };
            }).ToList();

            var activeEscrows = contracts.Where(c => c.TrangThai == "DangThucHien" || c.TrangThai == "ChoKyQuy").ToList();

            return new AdminTransactionManagementViewModel
            {
                EscrowContracts = contracts,
                RecentTransactions = LichSuGiaoDichs.OrderByDescending(l => l.NgayTao).Take(15).ToList(),
                TongTienDangKyQuy = activeEscrows.Sum(c => c.SoTienKyQuy),
                TongHopDongDangKyQuy = activeEscrows.Count
            };
        }

        // AD-UC-06.01: QUẢN LÝ PHÍ, HOA HỒNG & DOANH THU
        public static AdminFeeCommissionViewModel GetAdminFeeCommission()
        {
            var phiDangBai = CauHinhPhis.FirstOrDefault(c => c.TenCauHinh == "PhiDangBai")?.GiaTri ?? 2000m;
            var hoaHong = CauHinhPhis.FirstOrDefault(c => c.TenCauHinh == "HoaHongDuAn")?.GiaTri ?? 5.0m;

            return new AdminFeeCommissionViewModel
            {
                PhiDangBaiHienTai = phiDangBai,
                PhanTramHoaHongHienTai = hoaHong,
                LichSuCauHinhs = CauHinhPhis.OrderByDescending(c => c.NgayCapNhat).ToList(),
                TongDoanhThuPhiDangBai = 154000,
                TongDoanhThuHoaHong = 2850000,
                DoanhThuTheoThang = new List<MonthlyRevenueItemViewModel>
                {
                    new() { ThangNam = "Tháng 07/2026", PhiDangBai = 36000, HoaHong = 680000 },
                    new() { ThangNam = "Tháng 08/2026", PhiDangBai = 54000, HoaHong = 920000 },
                    new() { ThangNam = "Tháng 09/2026", PhiDangBai = 64000, HoaHong = 1250000 }
                }
            };
        }

        public static bool CapNhatPhiVaHoaHong(decimal phiDangBai, decimal phanTramHoaHong, int adminId, string moTa)
        {
            var p1 = CauHinhPhis.FirstOrDefault(c => c.TenCauHinh == "PhiDangBai");
            if (p1 != null)
            {
                p1.GiaTri = phiDangBai;
                p1.NgayCapNhat = DateTime.Now;
                p1.MaAdmin = adminId;
                if (!string.IsNullOrEmpty(moTa)) p1.MoTa = moTa;
            }

            var p2 = CauHinhPhis.FirstOrDefault(c => c.TenCauHinh == "HoaHongDuAn");
            if (p2 != null)
            {
                p2.GiaTri = phanTramHoaHong;
                p2.NgayCapNhat = DateTime.Now;
                p2.MaAdmin = adminId;
                if (!string.IsNullOrEmpty(moTa)) p2.MoTa = moTa;
            }

            return true;
        }

        // AD-UC-07.01: XỬ LÝ TRANH CHẤP
        public static AdminDisputeResolutionViewModel GetAdminDisputes()
        {
            var items = TranhChaps.Select(t =>
            {
                var hd = HopDongs.FirstOrDefault(h => h.MaHD == t.MaHD);
                var job = hd != null ? JobPosts.FirstOrDefault(j => j.MaJob == hd.MaJob) : null;
                var ntd = job != null ? NhaTuyenDungs.FirstOrDefault(n => n.MaNhaTuyenDung == job.MaNhaTuyenDung) : null;
                var ntdUser = ntd != null ? Users.FirstOrDefault(u => u.MaUser == ntd.MaUser) : null;

                var fl = hd != null ? FreelancerStudents.FirstOrDefault(f => f.MaFreelancerStudents == hd.MaFreelancerStudent) : null;
                var flUser = fl != null ? Users.FirstOrDefault(u => u.MaUser == fl.MaUser) : null;

                var sender = Users.FirstOrDefault(u => u.MaUser == t.NguoiTao);
                string senderName = sender?.HotenUser ?? "Người dùng";
                string senderRole = sender?.MaRole == 2 ? "Nhà tuyển dụng" : "Freelancer Sinh viên";

                var targetName = sender?.MaRole == 2 ? (flUser?.HotenUser ?? "Freelancer") : (ntdUser?.HotenUser ?? "Nhà tuyển dụng");
                var evidence = BangChungTranhChaps.Where(b => b.MaTranhChap == t.MaTranhChap).ToList();

                return new AdminDisputeItemViewModel
                {
                    MaTranhChap = t.MaTranhChap,
                    MaHD = t.MaHD,
                    TenCongViec = job?.Tieude ?? "Dự án",
                    NguoiKhieuNai = senderName,
                    RoleNguoiKhieuNai = senderRole,
                    NguoiBiKhieuNai = targetName,
                    LyDo = t.LyDo,
                    SoTienTranhChap = (decimal)(hd?.Sotienkyquy ?? 1500000),
                    TrangThai = t.TrangThai,
                    KetQuaPhanXu = t.KetLuanAdmin,
                    NgayTao = t.NgayTao,
                    BangChungs = evidence
                };
            }).ToList();

            return new AdminDisputeResolutionViewModel
            {
                TranhChaps = items,
                SoTranhChapDangCho = items.Count(i => i.TrangThai == "ChoXuLy" || i.TrangThai == "DangPhanXu")
            };
        }

        public static bool GiaiQuyetTranhChap(int maTranhChap, int adminId, string ketQua, decimal hoanTienCus, decimal traFRL)
        {
            var tc = TranhChaps.FirstOrDefault(t => t.MaTranhChap == maTranhChap);
            if (tc == null) return false;

            tc.TrangThai = "DaGiaiQuyet";
            tc.KetLuanAdmin = ketQua;
            tc.NgayGiaiQuyet = DateTime.Now;

            var hd = HopDongs.FirstOrDefault(h => h.MaHD == tc.MaHD);
            if (hd != null)
            {
                hd.TrangThai = "DaGiaiQuyetTranhChap";
            }

            return true;
        }

        // AD-UC-08.01: XỬ LÝ YÊU CẦU HỖ TRỢ
        public static AdminSupportTicketViewModel GetAdminSupportTickets(string? filterStatus = null)
        {
            var query = YeuCauHoTros.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterStatus) && filterStatus != "All")
            {
                query = query.Where(y => y.TrangThai == filterStatus);
            }

            return new AdminSupportTicketViewModel
            {
                YeuCaus = query.OrderByDescending(y => y.NgayGui).ToList(),
                SoTicketDangXuLy = YeuCauHoTros.Count(y => y.TrangThai == "DangXuLy"),
                FilterTrangThai = filterStatus
            };
        }

        public static bool PhanHoiYeuCauHoTro(int maYeuCau, int adminId, string phanHoi, string trangThai)
        {
            var ticket = YeuCauHoTros.FirstOrDefault(y => y.MaYeuCau == maYeuCau);
            if (ticket == null) return false;

            ticket.PhanHoiAdmin = phanHoi;
            ticket.TrangThai = trangThai;
            ticket.MaAdmin = adminId;
            ticket.NgayXuLy = DateTime.Now;

            return true;
        }

        // AD-UC-09.01: GIÁM SÁT HOẠT ĐỘNG HỆ THỐNG
        public static AdminSystemMonitoringViewModel GetAdminSystemMonitoring()
        {
            return new AdminSystemMonitoringViewModel
            {
                TongNguoiDung = Users.Count,
                TongBaiDangJob = JobPosts.Count,
                TongHopDong = HopDongs.Count,
                TongTranhChap = TranhChaps.Count,
                TongTienGiaoDichHeThong = LichSuGiaoDichs.Sum(l => l.SoTien),
                RecentSystemLogs = LichSuGiaoDichs.OrderByDescending(l => l.NgayTao).Take(20).ToList(),
                RecentAccountLogs = LichSuXuLyTaiKhoans.OrderByDescending(l => l.NgayXuLy).Take(10).ToList()
            };
        }

        // AD-UC-10: QUẢN LÝ HỢP ĐỒNG (MẪU HỢP ĐỒNG)
        public static AdminContractTemplateViewModel GetAdminContractTemplate()
        {
            var detailedList = HopDongs
                .Select(h => GetContractDetail(h.MaHD))
                .Where(d => d != null)
                .Select(d => d!)
                .OrderByDescending(d => d.Contract.NgayBatDau)
                .ToList();

            return new AdminContractTemplateViewModel
            {
                TenMauHopDong = "Hợp đồng Cung cấp Dịch vụ Freelance Sinh viên Chuẩn",
                DieuKhoanChung = MauHopDongHeThong,
                ThoiHanNghiemThuNgay = 3,
                TyLeGiaiNganMacDinh = 100,
                DanhSachHopDongHeThong = HopDongs.OrderByDescending(h => h.NgayBatDau).ToList(),
                DanhSachChiTietHopDongs = detailedList
            };
        }

        public static bool CapNhatMauHopDong(string tieuDe, string dieuKhoan, int thoiHan, decimal tyLe)
        {
            MauHopDongHeThong = dieuKhoan;
            return true;
        }

        // AD-UC-11: QUẢN LÝ PHÍ ĐĂNG BÀI
        public static AdminJobPostingFeeViewModel GetAdminJobPostingFee()
        {
            var config = CauHinhPhis.FirstOrDefault(c => c.TenCauHinh == "PhiDangBai");
            decimal fee = config?.GiaTri ?? 2000m;

            return new AdminJobPostingFeeViewModel
            {
                PhiHienTai = fee,
                PhiDangBaiMoi = fee,
                LichSuPhiDangBais = CauHinhPhis.Where(c => c.TenCauHinh == "PhiDangBai").OrderByDescending(c => c.NgayCapNhat).ToList()
            };
        }

        public static bool CapNhatPhiDangBai(decimal phiDangBaiMoi, int adminId, string ghiChu)
        {
            var config = CauHinhPhis.FirstOrDefault(c => c.TenCauHinh == "PhiDangBai");
            if (config != null)
            {
                config.GiaTri = phiDangBaiMoi;
                config.NgayCapNhat = DateTime.Now;
                config.MaAdmin = adminId;
                if (!string.IsNullOrEmpty(ghiChu)) config.MoTa = ghiChu;
            }
            return true;
        }

        // ADMIN DASHBOARD OVERVIEW
        public static AdminDashboardViewModel GetAdminDashboard()
        {
            return new AdminDashboardViewModel
            {
                TongSoUser = Users.Count,
                TongSoSinhVien = Users.Count(u => u.MaRole == 1),
                TongSoNhaTuyenDung = Users.Count(u => u.MaRole == 2),
                TongSoJob = JobPosts.Count,
                TongSoHopDong = HopDongs.Count,
                TongDoanhThuHeThong = 3004000m,
                SoYeuCauNapChoDuyet = YeuCauNapTiens.Count(y => y.TrangThai == "ChoDuyet"),
                SoTranhChapCanXuLy = TranhChaps.Count(t => t.TrangThai == "ChoXuLy" || t.TrangThai == "DangPhanXu"),
                SoTicketHoTro = YeuCauHoTros.Count(y => y.TrangThai == "DangXuLy"),
                GiaoDichMoiNhat = LichSuGiaoDichs.OrderByDescending(l => l.NgayTao).Take(8).ToList()
            };
        }

        // ========================================================
        // FRL: ĐĂNG BÀI TÌM VIỆC & QUẢN LÝ TÌM VIỆC (BaiDangTimViec_FreelancerStudent)
        // ========================================================
        public static QuanLyTimViecViewModel GetQuanLyTimViec(int maFreelancerStudent, string? status = null, string? keyword = null)
        {
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == maFreelancerStudent);
            var user = student != null ? Users.FirstOrDefault(u => u.MaUser == student.MaUser) : null;

            var query = BaiDangTimViecs.Where(b => b.MaFreelancerStudent == maFreelancerStudent).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(b => b.Trangthai == status);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(b => b.Tieude.ToLower().Contains(kw) || 
                                         (b.Mota != null && b.Mota.ToLower().Contains(kw)) ||
                                         (b.Kynang != null && b.Kynang.ToLower().Contains(kw)));
            }

            var list = query.OrderByDescending(b => b.Thoigiandang).ToList();
            var allOfStudent = BaiDangTimViecs.Where(b => b.MaFreelancerStudent == maFreelancerStudent).ToList();

            return new QuanLyTimViecViewModel
            {
                DanhSachBaiDang = list,
                StudentInfo = student,
                UserInfo = user,
                FilterStatus = status,
                Keyword = keyword,
                SoDangHienThi = allOfStudent.Count(b => b.Trangthai == "DangHienThi"),
                SoDaAn = allOfStudent.Count(b => b.Trangthai == "DaAn"),
                SoDaNhanViec = allOfStudent.Count(b => b.Trangthai == "DaNhanViec")
            };
        }

        public static DangTinTimViecViewModel? GetDangTinTimViecViewModel(string? maBaiDang, int maFreelancerStudent)
        {
            var student = FreelancerStudents.FirstOrDefault(s => s.MaFreelancerStudents == maFreelancerStudent);
            var user = student != null ? Users.FirstOrDefault(u => u.MaUser == student.MaUser) : null;

            if (string.IsNullOrEmpty(maBaiDang))
            {
                return new DangTinTimViecViewModel
                {
                    StudentInfo = student,
                    UserInfo = user,
                    MucGiaTu = (double)(student?.ChiPhiTu ?? 500000),
                    Kynang = string.Join(", ", KyNangs.Where(k => k.MaFreelancerStudents == maFreelancerStudent).Select(k => k.TenKyNang))
                };
            }

            var post = BaiDangTimViecs.FirstOrDefault(b => b.MaBaiDang == maBaiDang && b.MaFreelancerStudent == maFreelancerStudent);
            if (post == null) return null;

            return new DangTinTimViecViewModel
            {
                MaBaiDang = post.MaBaiDang,
                Tieude = post.Tieude,
                Mota = post.Mota ?? "",
                Kynang = post.Kynang ?? "",
                MucGiaTu = post.MucGiaTu ?? 500000,
                Trangthai = post.Trangthai,
                StudentInfo = student,
                UserInfo = user
            };
        }

        public static BaiDangTimViecFreelancerStudent CreateOrUpdateBaiDangTimViec(DangTinTimViecViewModel model, int maFreelancerStudent)
        {
            if (model.IsEditMode)
            {
                var post = BaiDangTimViecs.FirstOrDefault(b => b.MaBaiDang == model.MaBaiDang && b.MaFreelancerStudent == maFreelancerStudent);
                if (post != null)
                {
                    post.Tieude = model.Tieude;
                    post.Mota = model.Mota;
                    post.Kynang = model.Kynang;
                    post.MucGiaTu = model.MucGiaTu;
                    post.Trangthai = model.Trangthai;
                    return post;
                }
            }

            int nextNum = BaiDangTimViecs.Count + 1;
            var newPost = new BaiDangTimViecFreelancerStudent
            {
                MaBaiDang = $"BDTV_{nextNum:D3}",
                MaFreelancerStudent = maFreelancerStudent,
                Tieude = model.Tieude,
                Mota = model.Mota,
                Kynang = model.Kynang,
                MucGiaTu = model.MucGiaTu,
                Thoigiandang = DateTime.Now,
                Trangthai = model.Trangthai
            };
            BaiDangTimViecs.Insert(0, newPost);
            return newPost;
        }

        public static bool ToggleTrangThaiBaiDangTimViec(string maBaiDang, string newStatus, int maFreelancerStudent)
        {
            var post = BaiDangTimViecs.FirstOrDefault(b => b.MaBaiDang == maBaiDang && b.MaFreelancerStudent == maFreelancerStudent);
            if (post == null) return false;
            post.Trangthai = newStatus;
            return true;
        }

        public static bool DeleteBaiDangTimViec(string maBaiDang, int maFreelancerStudent)
        {
            var post = BaiDangTimViecs.FirstOrDefault(b => b.MaBaiDang == maBaiDang && b.MaFreelancerStudent == maFreelancerStudent);
            if (post == null) return false;
            BaiDangTimViecs.Remove(post);
            return true;
        }
    }
}
