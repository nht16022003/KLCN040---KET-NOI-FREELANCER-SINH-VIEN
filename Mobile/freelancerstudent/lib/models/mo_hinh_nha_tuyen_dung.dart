import '../core/constants/diem_cuoi_api.dart';

class NhaTuyenDungModel {
  final int maNhaTuyenDung;
  final int maUser;
  final String? hotenUser;
  final String? emailUser;
  final String? sdtUser;
  final String? tencongty;
  final String? linhvuc;
  final String? diachi;
  final String? gioithieu;
  final String? avatar;
  final String? logo;
  final double? sosaodanhgia;
  final String trangthai;
  final DateTime? ngayDangKy;

  const NhaTuyenDungModel({
    required this.maNhaTuyenDung,
    required this.maUser,
    this.hotenUser,
    this.emailUser,
    this.sdtUser,
    this.tencongty,
    this.linhvuc,
    this.diachi,
    this.gioithieu,
    this.avatar,
    this.logo,
    this.sosaodanhgia,
    this.trangthai = 'HoatDong',
    this.ngayDangKy,
  });

  // Tên hiển thị ưu tiên tên công ty, nếu không có thì lấy họ tên đại diện
  String get tenHienThi {
    if (tencongty != null && tencongty!.trim().isNotEmpty) {
      return tencongty!.trim();
    }
    if (hotenUser != null && hotenUser!.trim().isNotEmpty) {
      return hotenUser!.trim();
    }
    return 'Nhà Tuyển Dụng';
  }

  // Hình ảnh đại diện (avatar hoặc logo hoặc ảnh placeholder chuẩn)
  String get hinhAnhHienThi {
    if (avatar != null && avatar!.trim().isNotEmpty) {
      return ApiEndpoints.dinhDangUrlAnh(avatar!.trim(), tenMacDinh: tenHienThi);
    }
    if (logo != null && logo!.trim().isNotEmpty) {
      return ApiEndpoints.dinhDangUrlAnh(logo!.trim(), tenMacDinh: tenHienThi);
    }
    return ApiEndpoints.dinhDangUrlAnh(null, tenMacDinh: tenHienThi);
  }

  String get linhVucHienThi {
    if (linhvuc != null && linhvuc!.trim().isNotEmpty) {
      return linhvuc!.trim();
    }
    return 'Đa ngành';
  }

  String get diaChiHienThi {
    if (diachi != null && diachi!.trim().isNotEmpty) {
      return diachi!.trim();
    }
    return 'Toàn quốc';
  }

  String get nguoiDaiDien => hotenUser ?? 'Chưa cập nhật';

  String get gioiThieuHienThi {
    if (gioithieu != null && gioithieu!.trim().isNotEmpty) {
      return gioithieu!.trim();
    }
    return 'Chưa cập nhật phần giới thiệu...';
  }

  double get diemDanhGia => sosaodanhgia ?? 5.0;

  factory NhaTuyenDungModel.fromJson(Map<String, dynamic> json) {
    final rawMaNTD = json['maNhaTuyenDung'] ?? json['id'] ?? 0;
    final rawMaUser = json['maUser'] ?? 0;
    final rawSao = json['sosaodanhgia'] ?? json['soSaoDanhGia'] ?? 5.0;

    return NhaTuyenDungModel(
      maNhaTuyenDung: rawMaNTD is int ? rawMaNTD : int.tryParse(rawMaNTD.toString()) ?? 0,
      maUser: rawMaUser is int ? rawMaUser : int.tryParse(rawMaUser.toString()) ?? 0,
      hotenUser: json['hotenUser']?.toString() ?? json['hovaten']?.toString(),
      emailUser: json['emailUser']?.toString() ?? json['email']?.toString(),
      sdtUser: json['sdtUser']?.toString() ?? json['sodienthoai']?.toString(),
      tencongty: json['tencongty']?.toString() ?? json['tenCongTy']?.toString(),
      linhvuc: json['linhvuc']?.toString() ?? json['linhVuc']?.toString(),
      diachi: json['diachi']?.toString() ?? json['diaChi']?.toString(),
      gioithieu: json['gioithieu']?.toString() ?? json['gioiThieu']?.toString(),
      avatar: json['avatar']?.toString() ?? json['avatarUrl']?.toString(),
      logo: json['logo']?.toString(),
      sosaodanhgia: rawSao is num ? rawSao.toDouble() : 5.0,
      trangthai: (json['trangthai'] ?? json['trangThai'] ?? 'HoatDong').toString(),
      ngayDangKy: json['ngayDangKy'] != null
          ? DateTime.tryParse(json['ngayDangKy'].toString())
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
    'maNhaTuyenDung': maNhaTuyenDung,
    'maUser': maUser,
    'hotenUser': hotenUser,
    'emailUser': emailUser,
    'sdtUser': sdtUser,
    'tencongty': tencongty,
    'linhvuc': linhvuc,
    'diachi': diachi,
    'gioithieu': gioithieu,
    'avatar': avatar,
    'logo': logo,
    'sosaodanhgia': sosaodanhgia,
    'trangthai': trangthai,
    'ngayDangKy': ngayDangKy?.toIso8601String(),
  };
}
