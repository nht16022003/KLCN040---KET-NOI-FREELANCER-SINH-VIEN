import '../core/constants/diem_cuoi_api.dart';

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
  final int? maUser;
  final double? nganSachMin;
  final double? nganSachMax;
  final String? tenNhaTuyenDung;
  final String? avatarNhaTuyenDung;
  final String? ngayHetHan;
  final DateTime? thoigiandukienhoanthanh;
  final DateTime? ngaydang;
  final double? phiDangBai;

  // Thuộc tính gợi ý công việc phù hợp
  final double? doPhuHop;
  final String? lyDoGoiY;
  final List<String> kyNangTrungKhop;

  const JobPostModel({
    required this.maJob,
    required this.maNhaTuyenDung,
    required this.tieude,
    required this.mota,
    required this.kynangyeucau,
    required this.thulao,
    required this.status,
    required this.soluongtuyen,
    this.maUser,
    this.nganSachMin,
    this.nganSachMax,
    this.tenNhaTuyenDung,
    this.avatarNhaTuyenDung,
    this.ngayHetHan,
    this.thoigiandukienhoanthanh,
    this.ngaydang,
    this.phiDangBai,
    this.doPhuHop,
    this.lyDoGoiY,
    this.kyNangTrungKhop = const [],
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

  // Mức độ phù hợp (%) chuẩn hóa (tối đa 100%)
  int? get phanTramPhuHop {
    if (doPhuHop == null || doPhuHop! <= 0) return null;
    final val = doPhuHop! <= 1.0 ? (doPhuHop! * 100).round() : doPhuHop!.round();
    return val > 100 ? 100 : val;
  }

  DateTime get hanHoanThanh {
    if (thoigiandukienhoanthanh != null) return thoigiandukienhoanthanh!;
    if (ngayHetHan != null && ngayHetHan!.isNotEmpty) {
      final parsed = DateTime.tryParse(ngayHetHan!);
      if (parsed != null) return parsed;
    }
    return DateTime.now().add(const Duration(days: 14));
  }

  factory JobPostModel.fromJson(Map<String, dynamic> json) {
    // Nếu API trả về dạng bọc { "job": {...}, "employer": {...} } từ ChiTietJobPost
    final jobMap = (json['job'] is Map<String, dynamic>)
        ? json['job'] as Map<String, dynamic>
        : (json['Job'] is Map<String, dynamic>)
            ? json['Job'] as Map<String, dynamic>
            : json;

    final employerMap = (json['employer'] is Map<String, dynamic>)
        ? json['employer'] as Map<String, dynamic>
        : (json['Employer'] is Map<String, dynamic>)
            ? json['Employer'] as Map<String, dynamic>
            : null;

    final idValue = jobMap['maJob'] ?? jobMap['id'] ?? '';
    final titleValue = jobMap['tieude'] ?? jobMap['tieuDe'] ?? jobMap['title'] ?? '';
    final descriptionValue = jobMap['mota'] ?? jobMap['moTa'] ?? jobMap['description'] ?? '';
    final skillsValue = jobMap['kynangyeucau'] ?? jobMap['kyNangYeuCau'] ?? jobMap['skills'] ?? '';
    final salaryValue = jobMap['thulao'] ?? jobMap['thuLao'] ?? jobMap['nganSachMin'] ?? jobMap['budget'] ?? 0;
    final statusValue = jobMap['status'] ?? jobMap['trangThai'] ?? 'DangTuyen';
    final quantityValue = jobMap['soluongtuyen'] ?? jobMap['soLuongTuyen'] ?? jobMap['quantity'] ?? 1;

    final minBudget = jobMap['nganSachMin'] is num ? (jobMap['nganSachMin'] as num).toDouble() : null;
    final maxBudget = jobMap['nganSachMax'] is num ? (jobMap['nganSachMax'] as num).toDouble() : null;

    final tenNtd = jobMap['tenNhaTuyenDung']?.toString() ??
        jobMap['tenCongTy']?.toString() ??
        employerMap?['tencongty']?.toString() ??
        employerMap?['hotenUser']?.toString();

    final avatarNtd = jobMap['avatarNhaTuyenDung']?.toString() ??
        jobMap['avatar']?.toString() ??
        employerMap?['avatar']?.toString() ??
        employerMap?['logo']?.toString();

    return JobPostModel(
      maJob: idValue.toString(),
      maNhaTuyenDung: (jobMap['maNhaTuyenDung'] ?? jobMap['employerId'] ?? '').toString(),
      tieude: titleValue.toString(),
      mota: descriptionValue.toString(),
      kynangyeucau: skillsValue.toString(),
      thulao: salaryValue is num ? salaryValue.toDouble() : 0,
      status: statusValue.toString(),
      soluongtuyen: quantityValue is num ? quantityValue.toInt() : 1,
      maUser: jobMap['maUser'] is int
          ? jobMap['maUser'] as int
          : int.tryParse(jobMap['maUser']?.toString() ?? ''),
      nganSachMin: minBudget,
      nganSachMax: maxBudget,
      tenNhaTuyenDung: tenNtd,
      avatarNhaTuyenDung: avatarNtd,
      ngayHetHan: jobMap['ngayHetHan']?.toString() ?? jobMap['thoigiandukienhoanthanh']?.toString(),
      thoigiandukienhoanthanh: jobMap['thoigiandukienhoanthanh'] != null
          ? DateTime.tryParse(jobMap['thoigiandukienhoanthanh'].toString())
          : (jobMap['ngayHetHan'] != null
              ? DateTime.tryParse(jobMap['ngayHetHan'].toString())
              : null),
      ngaydang: jobMap['ngaydang'] != null
          ? DateTime.tryParse(jobMap['ngaydang'].toString())
          : null,
      phiDangBai: jobMap['phiDangBai'] is num
          ? (jobMap['phiDangBai'] as num).toDouble()
          : null,
      doPhuHop: jobMap['doPhuHop'] is num ? (jobMap['doPhuHop'] as num).toDouble() : null,
      lyDoGoiY: jobMap['lyDoGoiY']?.toString(),
      kyNangTrungKhop: jobMap['kyNangTrungKhop'] is List
          ? (jobMap['kyNangTrungKhop'] as List).map((e) => e.toString()).toList()
          : const [],
    );
  }

  Map<String, dynamic> toJson() => {
    'maJob': maJob,
    'maNhaTuyenDung': maNhaTuyenDung,
    'maUser': maUser,
    'tieude': tieude,
    'mota': mota,
    'kynangyeucau': kynangyeucau,
    'thulao': thulao,
    'status': status,
    'soluongtuyen': soluongtuyen,
    'thoigiandukienhoanthanh': thoigiandukienhoanthanh?.toIso8601String(),
    'ngaydang': ngaydang?.toIso8601String(),
    'phiDangBai': phiDangBai,
    'nganSachMin': nganSachMin,
    'nganSachMax': nganSachMax,
    'tenNhaTuyenDung': tenNhaTuyenDung,
    'avatarNhaTuyenDung': avatarNhaTuyenDung,
    'ngayHetHan': ngayHetHan,
    'doPhuHop': doPhuHop,
    'lyDoGoiY': lyDoGoiY,
    'kyNangTrungKhop': kyNangTrungKhop,
  };

  JobPostModel copyWith({
    String? maJob,
    String? maNhaTuyenDung,
    int? maUser,
    String? tieude,
    String? mota,
    DateTime? thoigiandukienhoanthanh,
    DateTime? ngaydang,
    String? kynangyeucau,
    double? thulao,
    String? status,
    int? soluongtuyen,
    double? phiDangBai,
    double? nganSachMin,
    double? nganSachMax,
    String? tenNhaTuyenDung,
    String? avatarNhaTuyenDung,
    String? ngayHetHan,
    double? doPhuHop,
    String? lyDoGoiY,
    List<String>? kyNangTrungKhop,
  }) {
    return JobPostModel(
      maJob: maJob ?? this.maJob,
      maNhaTuyenDung: maNhaTuyenDung ?? this.maNhaTuyenDung,
      maUser: maUser ?? this.maUser,
      tieude: tieude ?? this.tieude,
      mota: mota ?? this.mota,
      thoigiandukienhoanthanh: thoigiandukienhoanthanh ?? this.thoigiandukienhoanthanh,
      ngaydang: ngaydang ?? this.ngaydang,
      kynangyeucau: kynangyeucau ?? this.kynangyeucau,
      thulao: thulao ?? this.thulao,
      status: status ?? this.status,
      soluongtuyen: soluongtuyen ?? this.soluongtuyen,
      phiDangBai: phiDangBai ?? this.phiDangBai,
      nganSachMin: nganSachMin ?? this.nganSachMin,
      nganSachMax: nganSachMax ?? this.nganSachMax,
      tenNhaTuyenDung: tenNhaTuyenDung ?? this.tenNhaTuyenDung,
      avatarNhaTuyenDung: avatarNhaTuyenDung ?? this.avatarNhaTuyenDung,
      ngayHetHan: ngayHetHan ?? this.ngayHetHan,
      doPhuHop: doPhuHop ?? this.doPhuHop,
      lyDoGoiY: lyDoGoiY ?? this.lyDoGoiY,
      kyNangTrungKhop: kyNangTrungKhop ?? this.kyNangTrungKhop,
    );
  }
}
