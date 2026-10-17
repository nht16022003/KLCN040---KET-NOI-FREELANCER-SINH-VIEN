class ApiEndpoints {
  // Cấu hình địa chỉ máy chủ
  // Dùng 10.0.2.2 cho Android Emulator, hoặc thay bằng IP mạng LAN máy tính (ví dụ: 192.168.1.x) khi chạy trên thiết bị thật
  static const String serverHost = '10.0.2.2';
  static const String apiBaseUrl = 'http://$serverHost:5152';
  static const String webBaseUrl = 'http://$serverHost:5290';
  static const String baseUrl = '$apiBaseUrl/api';

  /// Chuyển đổi đường dẫn ảnh tương đối lưu trong CSDL/Web thành URL tuyệt đối cho Mobile
  /// - Nếu đường dẫn rỗng: Tự sinh Avatar chuẩn theo tên qua UI-Avatars
  /// - Nếu là link HTTP/HTTPS: Giữ nguyên
  /// - Nếu là đường dẫn tương đối (ví dụ: /uploads/avatars/...): Ghép với webBaseUrl (hoặc apiBaseUrl)
  static String dinhDangUrlAnh(String? duongDan, {String tenMacDinh = 'User'}) {
    if (duongDan == null || duongDan.trim().isEmpty) {
      return 'https://ui-avatars.com/api/?name=${Uri.encodeComponent(tenMacDinh)}&background=E8F5E9&color=1B5E36';
    }

    final path = duongDan.trim();

    // 1. Link online tuyệt đối
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }

    // 2. Đường dẫn cục bộ từ web wwwroot/uploads
    final normalizedPath = path.startsWith('/') ? path : '/$path';
    return '$webBaseUrl$normalizedPath';
  }

  // ==========================================
  // XÁC THỰC & TÀI KHOẢN (AUTH & ACCOUNT)
  // ==========================================
  static const String dangNhap = '$baseUrl/Auth/DangNhap';
  static const String dangKy = '$baseUrl/Auth/dangky';
  static const String thongTinTaiKhoan = '$baseUrl/Auth/ThongTinTaiKhoan';
  static const String capNhatThongTinTaiKhoan = '$baseUrl/Auth/CapNhatThongTinTaiKhoan';
  static const String capNhatAvatar = '$baseUrl/Auth/CapNhatAvatar';
  static const String doiMatKhau = '$baseUrl/Auth/DoiMatKhau';
  static const String quenMatKhau = '$baseUrl/Auth/forgot-password';

  // ==========================================
  // VIỆC LÀM & TUYỂN DỤNG (JOB POST)
  // ==========================================
  static const String danhSachJobPost = '$baseUrl/JobPost/DanhSachJobPost';
  static const String chiTietJobPost = '$baseUrl/JobPost/ChiTietJobPost';
  static const String taoJob = '$baseUrl/JobPost/TaoJob';
  static const String jobCuaToi = '$baseUrl/JobPost/CuaToi';
  static const String capNhatJob = '$baseUrl/JobPost/CapNhat';
  static const String nopDonUngTuyen = '$baseUrl/JobPost/UngTuyen';
  static const String danhSachUngTuyen = '$baseUrl/JobPost/DanhSachUngTuyen';
  static const String duyetUngTuyen = '$baseUrl/JobPost/DuyetUngTuyen';

  // ==========================================
  // BÀI ĐĂNG TÌM VIỆC CỦA SINH VIÊN (STUDENT POSTS)
  // ==========================================
  static const String tatCaBaiDangTimViec = '$baseUrl/BaiDangTimViec/TatCa';
  static const String taoBaiDangTimViec = '$baseUrl/BaiDangTimViec/TaoBaiDang';
  static const String baiDangTimViecCuaToi = '$baseUrl/BaiDangTimViec/CuaToi';
  static const String capNhatBaiDangTimViec = '$baseUrl/BaiDangTimViec/CapNhat';
  static const String doiTrangThaiBaiDangTimViec = '$baseUrl/BaiDangTimViec/DoiTrangThai';
  static const String xoaBaiDangTimViec = '$baseUrl/BaiDangTimViec/Xoa';

  // ==========================================
  // VÍ ĐIỆN TỬ & GIAO DỊCH NẠP TIỀN (WALLET & SEPAY)
  // ==========================================
  static const String layViTheoUser = '$baseUrl/Wallet/LayViTheoUser';
  static const String taoGiaoDichNapTien = '$baseUrl/GiaoDichNapTien/TaoGiaoDich';
  static const String trangThaiGiaoDichNapTien = '$baseUrl/GiaoDichNapTien/TrangThai';

  // ==========================================
  // HỢP ĐỒNG & KÝ QUỸ (ESCROW CONTRACTS)
  // ==========================================
  static const String danhSachHopDongNTD = '$baseUrl/HopDong/danh-sach';
  static const String danhSachHopDongFreelancer = '$baseUrl/HopDong/danh-sach-freelancer';
  static const String chiTietHopDong = '$baseUrl/HopDong/chi-tiet';
  static const String hopDongTheoJob = '$baseUrl/HopDong/theo-job';
  static const String taoHopDong = '$baseUrl/HopDong/tao-hop-dong';
  static const String xacNhanHopDong = '$baseUrl/HopDong/xac-nhan';
  static const String kyQuyHopDong = '$baseUrl/HopDong/KyQuy';

  // ==========================================
  // CHAT THỜI GIAN THỰC (SIGNALR & REST CHAT)
  // ==========================================
  static const String chatHubUrl = '$baseUrl/chatHub';
  static const String taoHoacLayPhongChat = '$baseUrl/Chat/TaoHoacLayPhong';
  static const String danhSachPhongChat = '$baseUrl/Chat/DanhSachPhong';
  static const String lichSuTinNhan = '$baseUrl/Chat/TinNhan';
  static const String guiTinNhan = '$baseUrl/Chat/GuiTinNhan';

  // ==========================================
  // NHÀ TUYỂN DỤNG (EMPLOYERS)
  // ==========================================
  static const String danhSachNhaTuyenDung = '$baseUrl/NhaTuyenDung/DanhSachNhaTuyenDung';

  // ==========================================
  // HỒ SƠ & PORTFOLIO FREELANCER
  // ==========================================
  static const String danhSachFreelancerStudent = '$baseUrl/FreelancerStudent/DanhSachFreelancerStudent';
  static const String profileFreelancer = '$baseUrl/FreelancerStudent/Profile';
  static const String chiTietHoSoFreelancer = '$baseUrl/FreelancerStudent/ChiTiet';
  static const String capNhatHoSoFreelancer = '$baseUrl/FreelancerStudent/CapNhatHoSo';
  static const String portfolioFreelancer = '$baseUrl/Portfolio';
  static const String duAnPortfolio = '$baseUrl/Portfolio/projects';
  static const String duAnNoiBatPortfolio = '$baseUrl/Portfolio/featured';

  // Tương thích ngược (Backward Compatibility)
  static const String authLogin = dangNhap;
  static const String authRegister = dangKy;
  static const String authForgotPassword = quenMatKhau;
  static const String jobs = danhSachJobPost;
  static const String jobDetail = chiTietJobPost;
  static const String postJob = taoJob;
  static const String studentPosts = tatCaBaiDangTimViec;
  static const String applications = nopDonUngTuyen;
  static const String wallet = layViTheoUser;
  static const String depositTransactions = taoGiaoDichNapTien;
}
