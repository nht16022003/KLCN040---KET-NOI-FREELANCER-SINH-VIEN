import '../core/constants/diem_cuoi_api.dart';

class StudentPostModel {
  final String maBaiDang;
  final int maFreelancerStudent;
  final String tieude;
  final String mota;
  final String kynang;
  final double mucGiaTu;
  final String trangthai;

  // Thông tin sinh viên từ BaiDangTimViec_ReponseDTO
  final int maUser;
  final String? tenUser;
  final String? avatar;
  final String? tenTruong;
  final String? tenChuyenNganh;
  final int namThu;
  final double gpa;

  const StudentPostModel({
    required this.maBaiDang,
    this.maFreelancerStudent = 0,
    required this.tieude,
    required this.mota,
    required this.kynang,
    required this.mucGiaTu,
    this.trangthai = 'HienThi',
    this.maUser = 0,
    this.tenUser,
    this.avatar,
    this.tenTruong,
    this.tenChuyenNganh,
    this.namThu = 1,
    this.gpa = 0,
  });

  String get hinhAnhHienThi => ApiEndpoints.dinhDangUrlAnh(
        avatar,
        tenMacDinh: tenUser ?? 'Sinh viên',
      );

  // Tên biến/getter thuần tiếng Việt
  String get maTin => maBaiDang;
  String get tieuDe => tieude;
  String get moTa => mota;
  String get kyNang => kynang;
  double get giaMongMuon => mucGiaTu;
  String get trangThai => trangthai;
  String get tenSinhVien => tenUser ?? '';
  double get diemGpa => gpa;
  double get diemTrungBinh => gpa;

  // Tương thích ngược
  String get id => maBaiDang;
  String get title => tieude;
  String get summary => mota;
  double get expectedSalary => mucGiaTu;
  String get skill => kynang;

  factory StudentPostModel.fromJson(Map<String, dynamic> json) {
    final rawMaBaiDang = json['maBaiDang'] ?? json['id'] ?? '';
    final rawTieuDe = json['tieude'] ?? json['tieuDe'] ?? json['title'] ?? '';
    final rawMoTa = json['mota'] ?? json['moTa'] ?? json['summary'] ?? '';
    final rawKyNang = json['kynang'] ?? json['kyNang'] ?? json['skill'] ?? '';
    final rawMucGia = json['mucGiaTu'] ?? json['giaMongMuon'] ?? json['expectedSalary'] ?? 0;
    final rawTrangThai = json['trangthai'] ?? json['trangThai'] ?? 'HienThi';

    final rawMaFL = json['maFreelancerStudent'] ?? 0;
    final rawMaUser = json['maUser'] ?? 0;
    final rawNamThu = json['namThu'] ?? 1;
    final rawGPA = json['GPA'] ?? json['gpa'] ?? 0;

    return StudentPostModel(
      maBaiDang: rawMaBaiDang.toString(),
      maFreelancerStudent: rawMaFL is int ? rawMaFL : int.tryParse(rawMaFL.toString()) ?? 0,
      tieude: rawTieuDe.toString(),
      mota: rawMoTa.toString(),
      kynang: rawKyNang.toString(),
      mucGiaTu: rawMucGia is num ? rawMucGia.toDouble() : 0,
      trangthai: rawTrangThai.toString(),
      maUser: rawMaUser is int ? rawMaUser : int.tryParse(rawMaUser.toString()) ?? 0,
      tenUser: json['tenUser']?.toString() ?? json['hovaten']?.toString(),
      avatar: json['avatar']?.toString() ?? json['avatarUrl']?.toString(),
      tenTruong: json['tenTruong']?.toString(),
      tenChuyenNganh: json['tenChuyenNganh']?.toString(),
      namThu: rawNamThu is int ? rawNamThu : int.tryParse(rawNamThu.toString()) ?? 1,
      gpa: rawGPA is num ? rawGPA.toDouble() : 0,
    );
  }

  Map<String, dynamic> toJson() => {
    'maBaiDang': maBaiDang,
    'maFreelancerStudent': maFreelancerStudent,
    'tieude': tieude,
    'mota': mota,
    'kynang': kynang,
    'mucGiaTu': mucGiaTu,
    'trangthai': trangthai,
    'maUser': maUser,
    'tenUser': tenUser,
    'avatar': avatar,
    'tenTruong': tenTruong,
    'tenChuyenNganh': tenChuyenNganh,
    'namThu': namThu,
    'gpa': gpa,
    // Tương thích ngược
    'id': maBaiDang,
    'title': tieude,
    'summary': mota,
    'expectedSalary': mucGiaTu,
    'skill': kynang,
  };
}
