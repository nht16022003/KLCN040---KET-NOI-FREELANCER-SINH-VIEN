class DanhGiaModel {
  final int maDanhGia;
  final String maHD;
  final String tenNguoiDanhGia;
  final String? avatar;
  final String tenCongViec;
  final int soSaoTongThe;
  final int? soSaoChatLuong;
  final int? soSaoDungHan;
  final int? soSaoGiaoTiep;
  final String? nhanXet;
  final DateTime? ngayDanhGia;
  final DateTime? ngayCapNhat;
  final bool daChinhSua;
  final bool coTheChinhSua;
  final DateTime? hanChinhSua;

  DanhGiaModel({
    required this.maDanhGia,
    required this.maHD,
    required this.tenNguoiDanhGia,
    this.avatar,
    required this.tenCongViec,
    required this.soSaoTongThe,
    this.soSaoChatLuong,
    this.soSaoDungHan,
    this.soSaoGiaoTiep,
    this.nhanXet,
    this.ngayDanhGia,
    this.ngayCapNhat,
    this.daChinhSua = false,
    this.coTheChinhSua = false,
    this.hanChinhSua,
  });

  factory DanhGiaModel.fromJson(Map<String, dynamic> json) {
    return DanhGiaModel(
      maDanhGia: json['maDanhGia'] is int
          ? json['maDanhGia']
          : int.tryParse(json['maDanhGia']?.toString() ?? '0') ?? 0,
      maHD: json['maHD']?.toString() ?? '',
      tenNguoiDanhGia: json['tenNguoiDanhGia']?.toString() ?? '',
      avatar: json['avatar']?.toString(),
      tenCongViec: json['tenCongViec']?.toString() ?? '',
      soSaoTongThe: json['soSaoTongThe'] is int
          ? json['soSaoTongThe']
          : int.tryParse(json['soSaoTongThe']?.toString() ?? '5') ?? 5,
      soSaoChatLuong: json['soSaoChatLuong'] != null
          ? int.tryParse(json['soSaoChatLuong'].toString())
          : null,
      soSaoDungHan: json['soSaoDungHan'] != null
          ? int.tryParse(json['soSaoDungHan'].toString())
          : null,
      soSaoGiaoTiep: json['soSaoGiaoTiep'] != null
          ? int.tryParse(json['soSaoGiaoTiep'].toString())
          : null,
      nhanXet: json['nhanXet']?.toString(),
      ngayDanhGia: json['ngayDanhGia'] != null
          ? DateTime.tryParse(json['ngayDanhGia'].toString())
          : null,
      ngayCapNhat: json['ngayCapNhat'] != null
          ? DateTime.tryParse(json['ngayCapNhat'].toString())
          : null,
      daChinhSua: json['daChinhSua'] == true,
      coTheChinhSua: json['coTheChinhSua'] == true,
      hanChinhSua: json['hanChinhSua'] != null
          ? DateTime.tryParse(json['hanChinhSua'].toString())
          : null,
    );
  }
}

class HopDongDanhGiaModel {
  final String maHD;
  final String tenCongViec;
  final String tenDoiTac;
  final String vaiTroDoiTac;
  final int? maFreelancerStudent;
  final int? maNhaTuyenDung;
  final double? tongTien;
  final DateTime? ngayHoanThanh;
  final String trangThaiHopDong;
  final String trangThaiDanhGia;
  final int? maDanhGia;
  final int? soSao;
  final int? soSaoChatLuong;
  final int? soSaoDungHan;
  final int? soSaoGiaoTiep;
  final String? nhanXet;
  final DateTime? ngayDanhGia;
  final bool coTheDanhGia;
  final bool coTheChinhSuaDanhGia;
  final DateTime? hanDanhGia;
  final bool doiTacDaDanhGia;
  final int? soSaoDoiTac;
  final String? nhanXetDoiTac;
  final DateTime? ngayDanhGiaDoiTac;

  HopDongDanhGiaModel({
    required this.maHD,
    required this.tenCongViec,
    required this.tenDoiTac,
    required this.vaiTroDoiTac,
    this.maFreelancerStudent,
    this.maNhaTuyenDung,
    this.tongTien,
    this.ngayHoanThanh,
    required this.trangThaiHopDong,
    this.trangThaiDanhGia = 'ChoDanhGia',
    this.maDanhGia,
    this.soSao,
    this.soSaoChatLuong,
    this.soSaoDungHan,
    this.soSaoGiaoTiep,
    this.nhanXet,
    this.ngayDanhGia,
    this.coTheDanhGia = false,
    this.coTheChinhSuaDanhGia = false,
    this.hanDanhGia,
    this.doiTacDaDanhGia = false,
    this.soSaoDoiTac,
    this.nhanXetDoiTac,
    this.ngayDanhGiaDoiTac,
  });

  factory HopDongDanhGiaModel.fromJson(Map<String, dynamic> json) {
    return HopDongDanhGiaModel(
      maHD: json['maHD']?.toString() ?? '',
      tenCongViec: json['tenCongViec']?.toString() ?? '',
      tenDoiTac: json['tenDoiTac']?.toString() ?? '',
      vaiTroDoiTac: json['vaiTroDoiTac']?.toString() ?? '',
      maFreelancerStudent: json['maFreelancerStudent'] != null
          ? int.tryParse(json['maFreelancerStudent'].toString())
          : null,
      maNhaTuyenDung: json['maNhaTuyenDung'] != null
          ? int.tryParse(json['maNhaTuyenDung'].toString())
          : null,
      tongTien: json['tongTien'] != null
          ? double.tryParse(json['tongTien'].toString())
          : null,
      ngayHoanThanh: json['ngayHoanThanh'] != null
          ? DateTime.tryParse(json['ngayHoanThanh'].toString())
          : null,
      trangThaiHopDong: json['trangThaiHopDong']?.toString() ?? '',
      trangThaiDanhGia: json['trangThaiDanhGia']?.toString() ?? 'ChoDanhGia',
      maDanhGia: json['maDanhGia'] != null
          ? int.tryParse(json['maDanhGia'].toString())
          : null,
      soSao: json['soSao'] != null
          ? int.tryParse(json['soSao'].toString())
          : null,
      soSaoChatLuong: json['soSaoChatLuong'] != null
          ? int.tryParse(json['soSaoChatLuong'].toString())
          : null,
      soSaoDungHan: json['soSaoDungHan'] != null
          ? int.tryParse(json['soSaoDungHan'].toString())
          : null,
      soSaoGiaoTiep: json['soSaoGiaoTiep'] != null
          ? int.tryParse(json['soSaoGiaoTiep'].toString())
          : null,
      nhanXet: json['nhanXet']?.toString(),
      ngayDanhGia: json['ngayDanhGia'] != null
          ? DateTime.tryParse(json['ngayDanhGia'].toString())
          : null,
      coTheDanhGia: json['coTheDanhGia'] == true,
      coTheChinhSuaDanhGia: json['coTheChinhSuaDanhGia'] == true,
      hanDanhGia: json['hanDanhGia'] != null
          ? DateTime.tryParse(json['hanDanhGia'].toString())
          : null,
      doiTacDaDanhGia: json['doiTacDaDanhGia'] == true,
      soSaoDoiTac: json['soSaoDoiTac'] != null
          ? int.tryParse(json['soSaoDoiTac'].toString())
          : null,
      nhanXetDoiTac: json['nhanXetDoiTac']?.toString(),
      ngayDanhGiaDoiTac: json['ngayDanhGiaDoiTac'] != null
          ? DateTime.tryParse(json['ngayDanhGiaDoiTac'].toString())
          : null,
    );
  }
}

class FormDanhGiaModel {
  final String maHD;
  final String tenDoiTac;
  final String tenCongViec;
  final String vaiTroNguoiDanhGia;
  final String trangThaiDanhGia;
  final bool coTheDanhGia;
  final DateTime? hanDanhGia;
  final DanhGiaModel? danhGiaHienTai;
  final String? thongBaoLoi;

  FormDanhGiaModel({
    required this.maHD,
    required this.tenDoiTac,
    required this.tenCongViec,
    required this.vaiTroNguoiDanhGia,
    this.trangThaiDanhGia = 'ChoDanhGia',
    this.coTheDanhGia = false,
    this.hanDanhGia,
    this.danhGiaHienTai,
    this.thongBaoLoi,
  });

  factory FormDanhGiaModel.fromJson(Map<String, dynamic> json) {
    return FormDanhGiaModel(
      maHD: json['maHD']?.toString() ?? '',
      tenDoiTac: json['tenDoiTac']?.toString() ?? '',
      tenCongViec: json['tenCongViec']?.toString() ?? '',
      vaiTroNguoiDanhGia: json['vaiTroNguoiDanhGia']?.toString() ?? '',
      trangThaiDanhGia: json['trangThaiDanhGia']?.toString() ?? 'ChoDanhGia',
      coTheDanhGia: json['coTheDanhGia'] == true,
      hanDanhGia: json['hanDanhGia'] != null
          ? DateTime.tryParse(json['hanDanhGia'].toString())
          : null,
      danhGiaHienTai: json['danhGiaHienTai'] is Map<String, dynamic>
          ? DanhGiaModel.fromJson(json['danhGiaHienTai'] as Map<String, dynamic>)
          : null,
      thongBaoLoi: json['thongBaoLoi']?.toString(),
    );
  }
}

class ThongKeDanhGiaModel {
  final double diemTrungBinh;
  final int tongLuotDanhGia;
  final double trungBinhChatLuong;
  final double trungBinhDungHan;
  final double trungBinhGiaoTiep;
  final List<int> phanBoSao;
  final double tyLeHaiLong;
  final int trang;
  final int kichThuocTrang;
  final int tongSoTrang;
  final List<DanhGiaModel> danhSach;

  ThongKeDanhGiaModel({
    this.diemTrungBinh = 0,
    this.tongLuotDanhGia = 0,
    this.trungBinhChatLuong = 0,
    this.trungBinhDungHan = 0,
    this.trungBinhGiaoTiep = 0,
    this.phanBoSao = const [],
    this.tyLeHaiLong = 0,
    this.trang = 1,
    this.kichThuocTrang = 10,
    this.tongSoTrang = 1,
    this.danhSach = const [],
  });

  factory ThongKeDanhGiaModel.fromJson(Map<String, dynamic> json) {
    final listRaw = json['danhSach'] is List ? json['danhSach'] as List : [];
    final phanBoRaw = json['phanBoSao'] is List ? json['phanBoSao'] as List : [];

    return ThongKeDanhGiaModel(
      diemTrungBinh: double.tryParse(json['diemTrungBinh']?.toString() ?? '0') ?? 0,
      tongLuotDanhGia: int.tryParse(json['tongLuotDanhGia']?.toString() ?? '0') ?? 0,
      trungBinhChatLuong: double.tryParse(json['trungBinhChatLuong']?.toString() ?? '0') ?? 0,
      trungBinhDungHan: double.tryParse(json['trungBinhDungHan']?.toString() ?? '0') ?? 0,
      trungBinhGiaoTiep: double.tryParse(json['trungBinhGiaoTiep']?.toString() ?? '0') ?? 0,
      phanBoSao: phanBoRaw.map((e) => int.tryParse(e.toString()) ?? 0).toList(),
      tyLeHaiLong: double.tryParse(json['tyLeHaiLong']?.toString() ?? '0') ?? 0,
      trang: int.tryParse(json['trang']?.toString() ?? '1') ?? 1,
      kichThuocTrang: int.tryParse(json['kichThuocTrang']?.toString() ?? '10') ?? 10,
      tongSoTrang: int.tryParse(json['tongSoTrang']?.toString() ?? '1') ?? 1,
      danhSach: listRaw
          .whereType<Map<String, dynamic>>()
          .map((e) => DanhGiaModel.fromJson(e))
          .toList(),
    );
  }
}
