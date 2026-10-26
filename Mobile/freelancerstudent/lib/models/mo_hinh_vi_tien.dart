class WalletModel {
  final int maWallet;
  final String tenUsers;
  final double soDuKhaDung;
  final double soDuDongBang;
  final String loaiTienTe;

  const WalletModel({
    required this.maWallet,
    required this.tenUsers,
    required this.soDuKhaDung,
    required this.soDuDongBang,
    this.loaiTienTe = 'VND',
  });

  // Tên biến/getter thuần tiếng Việt
  int get maVi => maWallet;
  String get tenChuVi => tenUsers;
  double get tongSoDu => soDuKhaDung + soDuDongBang;

  // Tương thích ngược
  String get id => maWallet.toString();
  double get balance => soDuKhaDung;
  double get frozenBalance => soDuDongBang;
  String get currency => loaiTienTe;

  factory WalletModel.fromJson(Map<String, dynamic> json) {
    final rawMaWallet = json['maWallet'] ?? json['id'] ?? 0;
    final parsedMaWallet = rawMaWallet is int ? rawMaWallet : int.tryParse(rawMaWallet.toString()) ?? 0;

    final rawSoDuKhaDung = json['soDuKhaDung'] ?? json['balance'] ?? 0;
    final rawSoDuDongBang = json['soDuDongBang'] ?? json['frozenBalance'] ?? 0;

    return WalletModel(
      maWallet: parsedMaWallet,
      tenUsers: (json['tenUsers'] ?? json['userName'] ?? '').toString(),
      soDuKhaDung: rawSoDuKhaDung is num ? rawSoDuKhaDung.toDouble() : 0,
      soDuDongBang: rawSoDuDongBang is num ? rawSoDuDongBang.toDouble() : 0,
      loaiTienTe: json['currency']?.toString() ?? 'VND',
    );
  }

  Map<String, dynamic> toJson() => {
    'maWallet': maWallet,
    'tenUsers': tenUsers,
    'soDuKhaDung': soDuKhaDung,
    'soDuDongBang': soDuDongBang,
    'loaiTienTe': loaiTienTe,
    // Tương thích ngược
    'id': maWallet.toString(),
    'balance': soDuKhaDung,
    'currency': loaiTienTe,
  };
}

/// Mô hình nhật ký biến động số dư ví (LichSuGiaoDich_ResponseDTO)
class LichSuGiaoDichModel {
  final int maGiaoDich;
  final int maWallet;
  final String loaiGiaoDich;
  final double soTien;
  final double soDuTruocGD;
  final double soDuSauGD;
  final String? noiDung;
  final String? phuongThucThanhToan;
  final String? maGiaoDichNgoai;
  final String? trangThai;
  final DateTime? ngayTao;

  const LichSuGiaoDichModel({
    required this.maGiaoDich,
    required this.maWallet,
    required this.loaiGiaoDich,
    required this.soTien,
    required this.soDuTruocGD,
    required this.soDuSauGD,
    this.noiDung,
    this.phuongThucThanhToan,
    this.maGiaoDichNgoai,
    this.trangThai,
    this.ngayTao,
  });

  bool get laGiaoDichCong {
    final loai = loaiGiaoDich.toLowerCase();
    return loai.contains('nap') || loai.contains('nhantien') || loai.contains('hoan') || loai.contains('cong');
  }

  factory LichSuGiaoDichModel.fromJson(Map<String, dynamic> json) {
    final rawMaGD = json['maGiaoDich'] ?? 0;
    final rawMaWallet = json['maWallet'] ?? 0;
    final rawSoTien = json['soTien'] ?? 0;
    final rawTruoc = json['soDuTruocGD'] ?? 0;
    final rawSau = json['soDuSauGD'] ?? 0;

    DateTime? parsedNgay;
    if (json['ngayTao'] != null) {
      parsedNgay = DateTime.tryParse(json['ngayTao'].toString());
    }

    return LichSuGiaoDichModel(
      maGiaoDich: rawMaGD is int ? rawMaGD : int.tryParse(rawMaGD.toString()) ?? 0,
      maWallet: rawMaWallet is int ? rawMaWallet : int.tryParse(rawMaWallet.toString()) ?? 0,
      loaiGiaoDich: (json['loaiGiaoDich'] ?? '').toString(),
      soTien: rawSoTien is num ? rawSoTien.toDouble() : 0,
      soDuTruocGD: rawTruoc is num ? rawTruoc.toDouble() : 0,
      soDuSauGD: rawSau is num ? rawSau.toDouble() : 0,
      noiDung: json['noiDung']?.toString(),
      phuongThucThanhToan: json['phuongThucThanhToan']?.toString(),
      maGiaoDichNgoai: json['maGiaoDichNgoai']?.toString(),
      trangThai: json['trangThai']?.toString(),
      ngayTao: parsedNgay,
    );
  }
}

/// Mô hình yêu cầu rút tiền (YeuCauRutTien_ResponseDTO)
class YeuCauRutTienModel {
  final int maRutTien;
  final int maWallet;
  final double soTienRut;
  final String tenNganHang;
  final String soTaiKhoan;
  final String tenChuTaiKhoan;
  final DateTime? ngayYeuCau;
  final DateTime? ngayXuLy;
  final String trangThai;
  final String? lyDoTuChoi;

  const YeuCauRutTienModel({
    required this.maRutTien,
    required this.maWallet,
    required this.soTienRut,
    required this.tenNganHang,
    required this.soTaiKhoan,
    required this.tenChuTaiKhoan,
    this.ngayYeuCau,
    this.ngayXuLy,
    required this.trangThai,
    this.lyDoTuChoi,
  });

  factory YeuCauRutTienModel.fromJson(Map<String, dynamic> json) {
    final rawMa = json['maRutTien'] ?? 0;
    final rawMaWallet = json['maWallet'] ?? 0;
    final rawSoTien = json['soTienRut'] ?? 0;

    DateTime? ngayYC;
    if (json['ngayYeuCau'] != null) {
      ngayYC = DateTime.tryParse(json['ngayYeuCau'].toString());
    }

    DateTime? ngayXL;
    if (json['ngayXuLy'] != null) {
      ngayXL = DateTime.tryParse(json['ngayXuLy'].toString());
    }

    return YeuCauRutTienModel(
      maRutTien: rawMa is int ? rawMa : int.tryParse(rawMa.toString()) ?? 0,
      maWallet: rawMaWallet is int ? rawMaWallet : int.tryParse(rawMaWallet.toString()) ?? 0,
      soTienRut: rawSoTien is num ? rawSoTien.toDouble() : 0,
      tenNganHang: (json['tenNganHang'] ?? '').toString(),
      soTaiKhoan: (json['soTaiKhoan'] ?? '').toString(),
      tenChuTaiKhoan: (json['tenChuTaiKhoan'] ?? '').toString(),
      ngayYeuCau: ngayYC,
      ngayXuLy: ngayXL,
      trangThai: (json['trangThai'] ?? 'ChoXuLy').toString(),
      lyDoTuChoi: json['lyDoTuChoi']?.toString(),
    );
  }
}
