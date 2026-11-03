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
  final String? link;
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
    this.link,
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
    if (logo != null && logo!.trim().isNotEmpty) {
      return ApiEndpoints.dinhDangUrlAnh(logo!.trim(), tenMacDinh: tenHienThi);
    }
    if (avatar != null && avatar!.trim().isNotEmpty) {
      return ApiEndpoints.dinhDangUrlAnh(avatar!.trim(), tenMacDinh: tenHienThi);
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
      return gioiithieuClean(gioithieu!);
    }
    return 'Chưa cập nhật phần giới thiệu...';
  }

  String gioiithieuClean(String raw) => raw.trim();

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
      link: json['link']?.toString(),
      avatar: json['avatar']?.toString() ?? json['avatarUrl']?.toString(),
      logo: json['logo']?.toString(),
      sosaodanhgia: rawSao is num ? rawSao.toDouble() : (double.tryParse(rawSao.toString()) ?? 5.0),
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
    'link': link,
    'avatar': avatar,
    'logo': logo,
    'sosaodanhgia': sosaodanhgia,
    'trangthai': trangthai,
    'ngayDangKy': ngayDangKy?.toIso8601String(),
  };
}

class NhaTuyenDungJobItemModel {
  final String maJob;
  final String tieude;
  final String? mota;
  final double thulao;
  final String? kynangyeucau;
  final String status;
  final DateTime? thoigiandangtuyen;
  final DateTime? thoigiandukienhoanthanh;
  final int soluongtuyen;

  const NhaTuyenDungJobItemModel({
    required this.maJob,
    required this.tieude,
    this.mota,
    this.thulao = 0,
    this.kynangyeucau,
    this.status = 'Đang tuyển',
    this.thoigiandangtuyen,
    this.thoigiandukienhoanthanh,
    this.soluongtuyen = 1,
  });

  List<String> get danhSachKyNang => (kynangyeucau ?? '')
      .split(RegExp(r'[,;]'))
      .map((s) => s.trim())
      .where((s) => s.isNotEmpty)
      .toList();

  factory NhaTuyenDungJobItemModel.fromJson(Map<String, dynamic> json) {
    final rawThuLao = json['thulao'] ?? json['thuLao'] ?? 0;
    final rawSoLuong = json['soluongtuyen'] ?? json['soLuongTuyen'] ?? 1;
    return NhaTuyenDungJobItemModel(
      maJob: (json['maJob'] ?? '').toString(),
      tieude: (json['tieude'] ?? json['tieuDe'] ?? '').toString(),
      mota: json['mota']?.toString() ?? json['moTa']?.toString(),
      thulao: rawThuLao is num
          ? rawThuLao.toDouble()
          : (double.tryParse(rawThuLao.toString()) ?? 0),
      kynangyeucau:
          json['kynangyeucau']?.toString() ?? json['kyNangYeuCau']?.toString(),
      status: (json['status'] ?? json['trangThai'] ?? 'Đang tuyển').toString(),
      thoigiandangtuyen: json['thoigiandangtuyen'] != null
          ? DateTime.tryParse(json['thoigiandangtuyen'].toString())
          : null,
      thoigiandukienhoanthanh: json['thoigiandukienhoanthanh'] != null
          ? DateTime.tryParse(json['thoigiandukienhoanthanh'].toString())
          : null,
      soluongtuyen: rawSoLuong is int
          ? rawSoLuong
          : (int.tryParse(rawSoLuong.toString()) ?? 1),
    );
  }
}

class NhaTuyenDungProfileModel {
  final NhaTuyenDungModel nhaTuyenDung;
  final int soLuotDanhGia;
  final double tyLeHaiLong;
  final int tongTinTuyenDung;
  final int tinDangTuyen;
  final int soHopDongHoanThanh;
  final int soFreelancerHopTac;
  final List<NhaTuyenDungJobItemModel> danhSachTinTuyenDung;

  const NhaTuyenDungProfileModel({
    required this.nhaTuyenDung,
    this.soLuotDanhGia = 0,
    this.tyLeHaiLong = 100,
    this.tongTinTuyenDung = 0,
    this.tinDangTuyen = 0,
    this.soHopDongHoanThanh = 0,
    this.soFreelancerHopTac = 0,
    this.danhSachTinTuyenDung = const [],
  });

  factory NhaTuyenDungProfileModel.fromJson(Map<String, dynamic> json) {
    final userJson = json['user'] is Map<String, dynamic>
        ? json['user'] as Map<String, dynamic>
        : <String, dynamic>{};
    final ntdJson = json['nhaTuyenDung'] is Map<String, dynamic>
        ? json['nhaTuyenDung'] as Map<String, dynamic>
        : json;

    final merged = <String, dynamic>{
      ...userJson,
      ...ntdJson,
      'hotenUser': ntdJson['hotenUser'] ?? userJson['hotenUser'] ?? userJson['hovaten'],
      'emailUser': ntdJson['emailUser'] ?? userJson['emailUser'] ?? userJson['email'],
      'sdtUser': ntdJson['sdtUser'] ?? userJson['sdtUser'] ?? userJson['sodienthoai'],
      'avatar': ntdJson['avatar'] ?? userJson['avatarUrl'] ?? userJson['avatar'],
    };

    final ntd = NhaTuyenDungModel.fromJson(merged);

    int toInt(dynamic v) => v is int ? v : (int.tryParse(v?.toString() ?? '') ?? 0);
    double toDouble(dynamic v) =>
        v is num ? v.toDouble() : (double.tryParse(v?.toString() ?? '') ?? 0);

    final rawJobs = json['danhSachTinTuyenDung'] is List
        ? json['danhSachTinTuyenDung'] as List
        : <dynamic>[];

    final jobs = rawJobs
        .whereType<Map<String, dynamic>>()
        .map(NhaTuyenDungJobItemModel.fromJson)
        .toList();

    return NhaTuyenDungProfileModel(
      nhaTuyenDung: ntd,
      soLuotDanhGia: toInt(ntdJson['soLuotDanhGia'] ?? json['soLuotDanhGia']),
      tyLeHaiLong: toDouble(ntdJson['tyLeHaiLong'] ?? json['tyLeHaiLong'] ?? 100),
      tongTinTuyenDung: toInt(
        ntdJson['tongTinTuyenDung'] ??
            ntdJson['tongBaiDang'] ??
            json['tongTinTuyenDung'] ??
            jobs.length,
      ),
      tinDangTuyen: toInt(
        ntdJson['tinDangTuyen'] ??
            ntdJson['baiDangDangTuyen'] ??
            json['tinDangTuyen'] ??
            jobs.where((j) => j.status.toLowerCase().contains('tuyển')).length,
      ),
      soHopDongHoanThanh: toInt(
        ntdJson['soHopDongHoanThanh'] ??
            ntdJson['hopDongHoanThanh'] ??
            json['soHopDongHoanThanh'],
      ),
      soFreelancerHopTac: toInt(
        ntdJson['soFreelancerHopTac'] ??
            ntdJson['tongHopDong'] ??
            json['soFreelancerHopTac'],
      ),
      danhSachTinTuyenDung: jobs,
    );
  }
}
