class BanGiaoModel {
  final int maBanGiao;
  final String maHD;
  final String phienBan;
  final String fileSanPhamUrl;
  final String? linkDemo;
  final String moTaBanGiao;
  final DateTime ngayNop;
  final String trangThai; // ChoNghiemThu, YeuCauChinhSua, DaNghiemThu
  final String? phanHoi;
  final DateTime? ngayPhanHoi;

  BanGiaoModel({
    required this.maBanGiao,
    required this.maHD,
    required this.phienBan,
    required this.fileSanPhamUrl,
    this.linkDemo,
    required this.moTaBanGiao,
    required this.ngayNop,
    required this.trangThai,
    this.phanHoi,
    this.ngayPhanHoi,
  });

  factory BanGiaoModel.fromJson(Map<String, dynamic> json) {
    return BanGiaoModel(
      maBanGiao: json['maBanGiao'] is int
          ? json['maBanGiao'] as int
          : int.tryParse(json['maBanGiao']?.toString() ?? '0') ?? 0,
      maHD: json['maHD']?.toString() ?? '',
      phienBan: json['phienBan']?.toString() ?? '1.0',
      fileSanPhamUrl: json['fileSanPhamUrl']?.toString() ?? '',
      linkDemo: json['linkDemo']?.toString(),
      moTaBanGiao: json['moTaBanGiao']?.toString() ?? '',
      ngayNop: json['ngayNop'] != null
          ? DateTime.tryParse(json['ngayNop'].toString()) ?? DateTime.now()
          : DateTime.now(),
      trangThai: json['trangThai']?.toString() ?? 'ChoNghiemThu',
      phanHoi: json['phanHoi']?.toString(),
      ngayPhanHoi: json['ngayPhanHoi'] != null
          ? DateTime.tryParse(json['ngayPhanHoi'].toString())
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'maBanGiao': maBanGiao,
        'maHD': maHD,
        'phienBan': phienBan,
        'fileSanPhamUrl': fileSanPhamUrl,
        'linkDemo': linkDemo,
        'moTaBanGiao': moTaBanGiao,
        'ngayNop': ngayNop.toIso8601String(),
        'trangThai': trangThai,
        'phanHoi': phanHoi,
        'ngayPhanHoi': ngayPhanHoi?.toIso8601String(),
      };
}

class YeuCauChinhSuaModel {
  final int maYeuCauChinhSua;
  final int maBanGiao;
  final String noiDungYeuCau;
  final String trangThai;
  final DateTime ngayYeuCau;
  final DateTime? ngayHoanThanh;

  YeuCauChinhSuaModel({
    required this.maYeuCauChinhSua,
    required this.maBanGiao,
    required this.noiDungYeuCau,
    required this.trangThai,
    required this.ngayYeuCau,
    this.ngayHoanThanh,
  });

  factory YeuCauChinhSuaModel.fromJson(Map<String, dynamic> json) {
    return YeuCauChinhSuaModel(
      maYeuCauChinhSua: json['maYeuCauChinhSua'] is int
          ? json['maYeuCauChinhSua'] as int
          : int.tryParse(json['maYeuCauChinhSua']?.toString() ?? '0') ?? 0,
      maBanGiao: json['maBanGiao'] is int
          ? json['maBanGiao'] as int
          : int.tryParse(json['maBanGiao']?.toString() ?? '0') ?? 0,
      noiDungYeuCau: json['noiDungYeuCau']?.toString() ?? '',
      trangThai: json['trangThai']?.toString() ?? '',
      ngayYeuCau: json['ngayYeuCau'] != null
          ? DateTime.tryParse(json['ngayYeuCau'].toString()) ?? DateTime.now()
          : DateTime.now(),
      ngayHoanThanh: json['ngayHoanThanh'] != null
          ? DateTime.tryParse(json['ngayHoanThanh'].toString())
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'maYeuCauChinhSua': maYeuCauChinhSua,
        'maBanGiao': maBanGiao,
        'noiDungYeuCau': noiDungYeuCau,
        'trangThai': trangThai,
        'ngayYeuCau': ngayYeuCau.toIso8601String(),
        'ngayHoanThanh': ngayHoanThanh?.toIso8601String(),
      };
}
