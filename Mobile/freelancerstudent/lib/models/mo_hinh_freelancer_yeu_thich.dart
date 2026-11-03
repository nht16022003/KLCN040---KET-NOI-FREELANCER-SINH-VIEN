class FreelancerYeuThichModel {
  final int maBookMark;
  final int maNhaTuyenDung;
  final int maFreelancerStudent;
  final int maUser;
  final String hoTen;
  final String avatar;
  final String email;
  final String soDienThoai;
  final String chuyenNganh;
  final String tenTruong;
  final int namThu;
  final double gpa;
  final double? chiPhiTu;
  final String kyNangCoBan;
  final double diemDanhGiaTrungBinh;
  final int soLuotDanhGia;
  final int soDuAnHoanThanh;
  final String ghiChu;
  final DateTime? ngayLuu;

  FreelancerYeuThichModel({
    required this.maBookMark,
    required this.maNhaTuyenDung,
    required this.maFreelancerStudent,
    required this.maUser,
    required this.hoTen,
    required this.avatar,
    required this.email,
    required this.soDienThoai,
    required this.chuyenNganh,
    required this.tenTruong,
    required this.namThu,
    required this.gpa,
    this.chiPhiTu,
    required this.kyNangCoBan,
    required this.diemDanhGiaTrungBinh,
    required this.soLuotDanhGia,
    required this.soDuAnHoanThanh,
    required this.ghiChu,
    this.ngayLuu,
  });

  factory FreelancerYeuThichModel.fromJson(Map<String, dynamic> json) {
    return FreelancerYeuThichModel(
      maBookMark: json['maBookMark'] is int
          ? json['maBookMark']
          : int.tryParse(json['maBookMark']?.toString() ?? '0') ?? 0,
      maNhaTuyenDung: json['maNhaTuyenDung'] is int
          ? json['maNhaTuyenDung']
          : int.tryParse(json['maNhaTuyenDung']?.toString() ?? '0') ?? 0,
      maFreelancerStudent: json['maFreelancerStudent'] is int
          ? json['maFreelancerStudent']
          : int.tryParse(json['maFreelancerStudent']?.toString() ?? '0') ?? 0,
      maUser: json['maUser'] is int
          ? json['maUser']
          : int.tryParse(json['maUser']?.toString() ?? '0') ?? 0,
      hoTen: json['hoTen']?.toString() ?? 'Sinh viên',
      avatar: json['avatar']?.toString() ?? '',
      email: json['email']?.toString() ?? '',
      soDienThoai: json['soDienThoai']?.toString() ?? '',
      chuyenNganh: json['chuyenNganh']?.toString() ?? 'Công nghệ thông tin',
      tenTruong: json['tenTruong']?.toString() ?? 'Đại học',
      namThu: json['namThu'] is int
          ? json['namThu']
          : int.tryParse(json['namThu']?.toString() ?? '1') ?? 1,
      gpa: json['gpa'] != null
          ? double.tryParse(json['gpa'].toString()) ?? 0.0
          : 0.0,
      chiPhiTu: json['chiPhiTu'] != null
          ? double.tryParse(json['chiPhiTu'].toString())
          : null,
      kyNangCoBan: json['kyNangCoBan']?.toString() ?? '',
      diemDanhGiaTrungBinh: json['diemDanhGiaTrungBinh'] != null
          ? double.tryParse(json['diemDanhGiaTrungBinh'].toString()) ?? 0.0
          : 0.0,
      soLuotDanhGia: json['soLuotDanhGia'] is int
          ? json['soLuotDanhGia']
          : int.tryParse(json['soLuotDanhGia']?.toString() ?? '0') ?? 0,
      soDuAnHoanThanh: json['soDuAnHoanThanh'] is int
          ? json['soDuAnHoanThanh']
          : int.tryParse(json['soDuAnHoanThanh']?.toString() ?? '0') ?? 0,
      ghiChu: json['ghiChu']?.toString() ?? '',
      ngayLuu: json['ngayLuu'] != null
          ? DateTime.tryParse(json['ngayLuu'].toString())
          : null,
    );
  }
}

