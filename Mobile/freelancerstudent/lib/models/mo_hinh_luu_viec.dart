class CongViecDaLuuModel {
  final int maLuu;
  final String maJob;
  final String tieuDe;
  final String moTa;
  final double thuLao;
  final String kyNangYeuCau;
  final DateTime? thoiGianDangTuyen;
  final DateTime? thoiGianDuKienHoanThanh;
  final String status;
  final String tenNhaTuyenDung;
  final String avatarNhaTuyenDung;
  final int maNhaTuyenDung;
  final DateTime? ngayLuu;
  final bool daTuyenDu;
  final int soLuongUngTuyen;

  CongViecDaLuuModel({
    required this.maLuu,
    required this.maJob,
    required this.tieuDe,
    required this.moTa,
    required this.thuLao,
    required this.kyNangYeuCau,
    this.thoiGianDangTuyen,
    this.thoiGianDuKienHoanThanh,
    required this.status,
    required this.tenNhaTuyenDung,
    required this.avatarNhaTuyenDung,
    required this.maNhaTuyenDung,
    this.ngayLuu,
    this.daTuyenDu = false,
    this.soLuongUngTuyen = 0,
  });

  factory CongViecDaLuuModel.fromJson(Map<String, dynamic> json) {
    return CongViecDaLuuModel(
      maLuu: json['maLuu'] is int
          ? json['maLuu']
          : int.tryParse(json['maLuu']?.toString() ?? '0') ?? 0,
      maJob: json['maJob']?.toString() ?? '',
      tieuDe: json['tieuDe']?.toString() ?? 'Chưa có tiêu đề',
      moTa: json['moTa']?.toString() ?? '',
      thuLao: json['thuLao'] != null
          ? double.tryParse(json['thuLao'].toString()) ?? 0.0
          : 0.0,
      kyNangYeuCau: json['kyNangYeuCau']?.toString() ?? '',
      thoiGianDangTuyen: json['thoiGianDangTuyen'] != null
          ? DateTime.tryParse(json['thoiGianDangTuyen'].toString())
          : null,
      thoiGianDuKienHoanThanh: json['thoiGianDuKienHoanThanh'] != null
          ? DateTime.tryParse(json['thoiGianDuKienHoanThanh'].toString())
          : null,
      status: json['status']?.toString() ?? 'Opening',
      tenNhaTuyenDung: json['tenNhaTuyenDung']?.toString() ?? 'Nhà tuyển dụng',
      avatarNhaTuyenDung: json['avatarNhaTuyenDung']?.toString() ?? '',
      maNhaTuyenDung: json['maNhaTuyenDung'] is int
          ? json['maNhaTuyenDung']
          : int.tryParse(json['maNhaTuyenDung']?.toString() ?? '0') ?? 0,
      ngayLuu: json['ngayLuu'] != null
          ? DateTime.tryParse(json['ngayLuu'].toString())
          : null,
      daTuyenDu: json['daTuyenDu'] == true,
      soLuongUngTuyen: json['soLuongUngTuyen'] is int
          ? json['soLuongUngTuyen']
          : int.tryParse(json['soLuongUngTuyen']?.toString() ?? '0') ?? 0,
    );
  }
}

