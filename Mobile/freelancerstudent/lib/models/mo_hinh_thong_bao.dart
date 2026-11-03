class ThongBaoModel {
  final int maThongBao;
  final String tieuDe;
  final String noiDung;
  final String loaiThongBao;
  final String? linkDieuHuong;
  final bool daDoc;
  final DateTime ngayTao;

  ThongBaoModel({
    required this.maThongBao,
    required this.tieuDe,
    required this.noiDung,
    required this.loaiThongBao,
    this.linkDieuHuong,
    required this.daDoc,
    required this.ngayTao,
  });

  factory ThongBaoModel.fromJson(Map<String, dynamic> json) {
    return ThongBaoModel(
      maThongBao: json['maThongBao'] is int
          ? json['maThongBao']
          : int.tryParse(json['maThongBao']?.toString() ?? '0') ?? 0,
      tieuDe: json['tieuDe']?.toString() ?? '',
      noiDung: json['noiDung']?.toString() ?? '',
      loaiThongBao: json['loaiThongBao']?.toString() ?? '',
      linkDieuHuong: json['linkDieuHuong']?.toString(),
      daDoc: json['daDoc'] == true,
      ngayTao: json['ngayTao'] != null
          ? DateTime.tryParse(json['ngayTao'].toString()) ?? DateTime.now()
          : DateTime.now(),
    );
  }
}

class ThongBaoDanhSachModel {
  final List<ThongBaoModel> danhSach;
  final int trang;
  final int kichThuocTrang;
  final int tongSoThongBao;
  final int soThongBaoChuaDoc;

  ThongBaoDanhSachModel({
    this.danhSach = const [],
    this.trang = 1,
    this.kichThuocTrang = 10,
    this.tongSoThongBao = 0,
    this.soThongBaoChuaDoc = 0,
  });

  factory ThongBaoDanhSachModel.fromJson(Map<String, dynamic> json) {
    final rawList = json['danhSach'] is List ? json['danhSach'] as List : [];
    return ThongBaoDanhSachModel(
      danhSach: rawList
          .whereType<Map<String, dynamic>>()
          .map((e) => ThongBaoModel.fromJson(e))
          .toList(),
      trang: int.tryParse(json['trang']?.toString() ?? '1') ?? 1,
      kichThuocTrang: int.tryParse(json['kichThuocTrang']?.toString() ?? '10') ?? 10,
      tongSoThongBao: int.tryParse(json['tongSoThongBao']?.toString() ?? '0') ?? 0,
      soThongBaoChuaDoc: int.tryParse(json['soThongBaoChuaDoc']?.toString() ?? '0') ?? 0,
    );
  }
}
