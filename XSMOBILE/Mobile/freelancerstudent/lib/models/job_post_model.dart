import '../core/constants/api_endpoints.dart';

class JobPostModel {
  final String maJob;
  final String maNhaTuyenDung;
  final String tieude;
  final String mota;
  final String kynangyeucau;
  final double thulao;
  final String status;
  final int soluongtuyen;

  // Thuộc tính bổ sung chuẩn từ API JobPost_ReponseDTO
  final double? nganSachMin;
  final double? nganSachMax;
  final String? tenNhaTuyenDung;
  final String? avatarNhaTuyenDung;
  final String? ngayHetHan;

  const JobPostModel({
    required this.maJob,
    required this.maNhaTuyenDung,
    required this.tieude,
    required this.mota,
    required this.kynangyeucau,
    required this.thulao,
    required this.status,
    required this.soluongtuyen,
    this.nganSachMin,
    this.nganSachMax,
    this.tenNhaTuyenDung,
    this.avatarNhaTuyenDung,
    this.ngayHetHan,
  });

  String get hinhAnhHienThi => ApiEndpoints.dinhDangUrlAnh(
        avatarNhaTuyenDung,
        tenMacDinh: tenNhaTuyenDung ?? 'NTD',
      );

  // Tên biến/getter thuần tiếng Việt
  String get maCongViec => maJob;
  String get tieuDe => tieude;
  String get moTa => mota;
  String get kyNangYeuCau => kynangyeucau;
  double get thuLao => thulao;
  String get trangThai => status;
  int get soLuongTuyen => soluongtuyen;

  // Getter tương thích ngược cho giao diện cũ
  String get id => maJob;
  String get title => tieude;
  String get description => mota;
  double get budget => thulao > 0 ? thulao : (nganSachMin ?? 0);

  factory JobPostModel.fromJson(Map<String, dynamic> json) {
    final idValue = json['maJob'] ?? json['id'] ?? '';
    final titleValue = json['tieude'] ?? json['tieuDe'] ?? json['title'] ?? '';
    final descriptionValue = json['mota'] ?? json['moTa'] ?? json['description'] ?? '';
    final skillsValue = json['kynangyeucau'] ?? json['kyNangYeuCau'] ?? json['skills'] ?? '';
    final salaryValue = json['thulao'] ?? json['thuLao'] ?? json['nganSachMin'] ?? json['budget'] ?? 0;
    final statusValue = json['status'] ?? json['trangThai'] ?? 'DangTuyen';
    final quantityValue = json['soluongtuyen'] ?? json['soLuongTuyen'] ?? json['quantity'] ?? 1;

    final minBudget = json['nganSachMin'] is num ? (json['nganSachMin'] as num).toDouble() : null;
    final maxBudget = json['nganSachMax'] is num ? (json['nganSachMax'] as num).toDouble() : null;

    return JobPostModel(
      maJob: idValue.toString(),
      maNhaTuyenDung: (json['maNhaTuyenDung'] ?? json['employerId'] ?? '').toString(),
      tieude: titleValue.toString(),
      mota: descriptionValue.toString(),
      kynangyeucau: skillsValue.toString(),
      thulao: salaryValue is num ? salaryValue.toDouble() : 0,
      status: statusValue.toString(),
      soluongtuyen: quantityValue is num ? quantityValue.toInt() : 1,
      nganSachMin: minBudget,
      nganSachMax: maxBudget,
      tenNhaTuyenDung: json['tenNhaTuyenDung']?.toString() ?? json['tenCongTy']?.toString(),
      avatarNhaTuyenDung: json['avatarNhaTuyenDung']?.toString() ?? json['avatar']?.toString(),
      ngayHetHan: json['ngayHetHan']?.toString() ?? json['thoigiandukienhoanthanh']?.toString(),
    );
  }

  Map<String, dynamic> toJson() => {
    'maJob': maJob,
    'maNhaTuyenDung': maNhaTuyenDung,
    'tieude': tieude,
    'mota': mota,
    'kynangyeucau': kynangyeucau,
    'thulao': thulao,
    'status': status,
    'soluongtuyen': soluongtuyen,
    'nganSachMin': nganSachMin,
    'nganSachMax': nganSachMax,
    'tenNhaTuyenDung': tenNhaTuyenDung,
    'avatarNhaTuyenDung': avatarNhaTuyenDung,
    'ngayHetHan': ngayHetHan,
  };
}
