class HopDongModel {
  final String maHD;
  final String maJob;
  final String tenCongViec;
  final String? moTaCongViec;
  final int maFreelancerStudent;
  final String tenFreelancer;
  final String? emailFreelancer;
  final String? sdtFreelancer;
  final int? maUserFreelancer;
  final int? maNhaTuyenDung;
  final String? tenNhaTuyenDung;
  final String? emailNhaTuyenDung;
  final String? sdtNhaTuyenDung;
  final int? maUserNhaTuyenDung;
  final DateTime? ngayBatDau;
  final DateTime? ngayKetThuc;
  final double sotienkyquy;
  final String? hinhthuclamviec;
  final String? dieuKhoan;
  final String trangThai;
  final List<GiaiDoanHopDongModel> giaiDoans;

  const HopDongModel({
    required this.maHD,
    required this.maJob,
    required this.tenCongViec,
    this.moTaCongViec,
    required this.maFreelancerStudent,
    required this.tenFreelancer,
    this.emailFreelancer,
    this.sdtFreelancer,
    this.maUserFreelancer,
    this.maNhaTuyenDung,
    this.tenNhaTuyenDung,
    this.emailNhaTuyenDung,
    this.sdtNhaTuyenDung,
    this.maUserNhaTuyenDung,
    this.ngayBatDau,
    this.ngayKetThuc,
    this.sotienkyquy = 0,
    this.hinhthuclamviec,
    this.dieuKhoan,
    this.trangThai = 'DuThao',
    this.giaiDoans = const [],
  });

  // Tên biến/getter thuần tiếng Việt
  String get maHopDong => maHD;
  String get tieuDeHopDong => tenCongViec;
  double get soTienKyQuy => sotienkyquy;
  String get trangThaiHopDong => trangThai;
  bool get coTheXacNhan => trangThai == 'DuThao' || trangThai == 'ChoXacNhan';
  bool get coTheKyQuy => trangThai == 'ChoKyQuy';
  bool get dangThucHien => trangThai == 'DangThucHien';

  factory HopDongModel.fromJson(Map<String, dynamic> json) {
    final rawGiaiDoans = json['giaiDoans'] is List ? json['giaiDoans'] as List : <dynamic>[];

    return HopDongModel(
      maHD: (json['maHD'] ?? json['id'] ?? '').toString(),
      maJob: (json['maJob'] ?? '').toString(),
      tenCongViec: (json['tenCongViec'] ?? json['tieuDe'] ?? '').toString(),
      moTaCongViec: json['moTaCongViec']?.toString(),
      maFreelancerStudent: json['maFreelancerStudent'] is int
          ? json['maFreelancerStudent']
          : int.tryParse(json['maFreelancerStudent']?.toString() ?? '') ?? 0,
      tenFreelancer: (json['tenFreelancer'] ?? '').toString(),
      emailFreelancer: json['emailFreelancer']?.toString(),
      sdtFreelancer: json['sdtFreelancer']?.toString(),
      maUserFreelancer: json['maUserFreelancer'] is int ? json['maUserFreelancer'] : null,
      maNhaTuyenDung: json['maNhaTuyenDung'] is int ? json['maNhaTuyenDung'] : null,
      tenNhaTuyenDung: json['tenNhaTuyenDung']?.toString(),
      emailNhaTuyenDung: json['emailNhaTuyenDung']?.toString(),
      sdtNhaTuyenDung: json['sdtNhaTuyenDung']?.toString(),
      maUserNhaTuyenDung: json['maUserNhaTuyenDung'] is int ? json['maUserNhaTuyenDung'] : null,
      ngayBatDau: json['ngayBatDau'] != null ? DateTime.tryParse(json['ngayBatDau'].toString()) : null,
      ngayKetThuc: json['ngayKetThuc'] != null ? DateTime.tryParse(json['ngayKetThuc'].toString()) : null,
      sotienkyquy: (json['sotienkyquy'] as num?)?.toDouble() ?? 0,
      hinhthuclamviec: json['hinhthuclamviec']?.toString(),
      dieuKhoan: json['dieuKhoan']?.toString(),
      trangThai: (json['trangThai'] ?? 'DuThao').toString(),
      giaiDoans: rawGiaiDoans
          .map((item) => GiaiDoanHopDongModel.fromJson(item as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() => {
    'maHD': maHD,
    'maJob': maJob,
    'tenCongViec': tenCongViec,
    'moTaCongViec': moTaCongViec,
    'maFreelancerStudent': maFreelancerStudent,
    'tenFreelancer': tenFreelancer,
    'emailFreelancer': emailFreelancer,
    'sdtFreelancer': sdtFreelancer,
    'maUserFreelancer': maUserFreelancer,
    'maNhaTuyenDung': maNhaTuyenDung,
    'tenNhaTuyenDung': tenNhaTuyenDung,
    'emailNhaTuyenDung': emailNhaTuyenDung,
    'sdtNhaTuyenDung': sdtNhaTuyenDung,
    'maUserNhaTuyenDung': maUserNhaTuyenDung,
    'ngayBatDau': ngayBatDau?.toIso8601String(),
    'ngayKetThuc': ngayKetThuc?.toIso8601String(),
    'sotienkyquy': sotienkyquy,
    'hinhthuclamviec': hinhthuclamviec,
    'dieuKhoan': dieuKhoan,
    'trangThai': trangThai,
    'giaiDoans': giaiDoans.map((e) => e.toJson()).toList(),
  };
}

class GiaiDoanHopDongModel {
  final int maGiaiDoan;
  final String maHD;
  final String tenGiaiDoan;
  final String? moTa;
  final DateTime? hanChot;
  final String trangThai;

  const GiaiDoanHopDongModel({
    required this.maGiaiDoan,
    required this.maHD,
    required this.tenGiaiDoan,
    this.moTa,
    this.hanChot,
    this.trangThai = 'ChuaHoanThanh',
  });

  factory GiaiDoanHopDongModel.fromJson(Map<String, dynamic> json) {
    return GiaiDoanHopDongModel(
      maGiaiDoan: json['maGiaiDoan'] is int ? json['maGiaiDoan'] : int.tryParse(json['maGiaiDoan']?.toString() ?? '') ?? 0,
      maHD: (json['maHD'] ?? '').toString(),
      tenGiaiDoan: (json['tenGiaiDoan'] ?? '').toString(),
      moTa: json['moTa']?.toString(),
      hanChot: json['hanChot'] != null ? DateTime.tryParse(json['hanChot'].toString()) : null,
      trangThai: (json['trangThai'] ?? 'ChuaHoanThanh').toString(),
    );
  }

  Map<String, dynamic> toJson() => {
    'maGiaiDoan': maGiaiDoan,
    'maHD': maHD,
    'tenGiaiDoan': tenGiaiDoan,
    'moTa': moTa,
    'hanChot': hanChot?.toIso8601String(),
    'trangThai': trangThai,
  };
}
