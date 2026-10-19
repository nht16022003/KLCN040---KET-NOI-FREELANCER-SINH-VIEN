import 'mo_hinh_hop_dong.dart';

/// Đầu việc (Task_CongViec) — khớp `TaskCongViec_ResponseDTO` của Backend.
class TaskCongViecModel {
  final int maTask;
  final String maHD;
  final int? maGiaiDoan;
  final String? tenGiaiDoan;
  final String tenTask;
  final String moTa;
  final String? linkSanPham;
  final DateTime? hanHoanThanh;
  final String trangThai;
  final DateTime? ngayCapNhat;

  const TaskCongViecModel({
    required this.maTask,
    required this.maHD,
    this.maGiaiDoan,
    this.tenGiaiDoan,
    required this.tenTask,
    required this.moTa,
    this.linkSanPham,
    this.hanHoanThanh,
    required this.trangThai,
    this.ngayCapNhat,
  });

  factory TaskCongViecModel.fromJson(Map<String, dynamic> json) {
    int? asInt(dynamic v) => v is int ? v : int.tryParse(v?.toString() ?? '');
    return TaskCongViecModel(
      maTask: asInt(json['maTask']) ?? 0,
      maHD: (json['maHD'] ?? '').toString(),
      maGiaiDoan: asInt(json['maGiaiDoan']),
      tenGiaiDoan: json['tenGiaiDoan']?.toString(),
      tenTask: (json['tenTask'] ?? '').toString(),
      moTa: (json['moTa'] ?? '').toString(),
      linkSanPham: json['linkSanPham']?.toString(),
      hanHoanThanh: json['hanHoanThanh'] != null
          ? DateTime.tryParse(json['hanHoanThanh'].toString())
          : null,
      trangThai: (json['trangThai'] ?? 'ChuaThucHien').toString(),
      ngayCapNhat: json['ngayCapNhat'] != null
          ? DateTime.tryParse(json['ngayCapNhat'].toString())
          : null,
    );
  }
}

/// Tiến độ dự án — khớp `TienDoDuAn_ResponseDTO`.
class TienDoDuAnModel {
  final String maHD;
  final String trangThaiHopDong;
  final DateTime? ngayBatDauHopDong;
  final DateTime? ngayKetThucHopDong;
  final int tongSoTask;
  final int soTaskChuaThucHien;
  final int soTaskDangThucHien;
  final int soTaskHoanThanh;
  final double tyLeHoanThanh;
  final bool coQuyenCapNhat;
  final bool coQuyenDanhGia;
  final bool duAnDaDat;
  final List<TaskCongViecModel> danhSachTask;
  final List<GiaiDoanHopDongModel> danhSachGiaiDoan;

  const TienDoDuAnModel({
    required this.maHD,
    required this.trangThaiHopDong,
    this.ngayBatDauHopDong,
    this.ngayKetThucHopDong,
    required this.tongSoTask,
    required this.soTaskChuaThucHien,
    required this.soTaskDangThucHien,
    required this.soTaskHoanThanh,
    required this.tyLeHoanThanh,
    required this.coQuyenCapNhat,
    this.coQuyenDanhGia = false,
    this.duAnDaDat = false,
    required this.danhSachTask,
    this.danhSachGiaiDoan = const [],
  });

  factory TienDoDuAnModel.fromJson(Map<String, dynamic> json) {
    int asInt(dynamic v) => v is int ? v : int.tryParse(v?.toString() ?? '') ?? 0;
    final rawTasks = json['danhSachTask'] is List
        ? json['danhSachTask'] as List
        : <dynamic>[];
    final rawGiaiDoans = json['danhSachGiaiDoan'] is List
        ? json['danhSachGiaiDoan'] as List
        : <dynamic>[];

    final dsGiaiDoanParsed = rawGiaiDoans
        .whereType<Map<String, dynamic>>()
        .map(GiaiDoanHopDongModel.fromJson)
        .toList();

    final bool allDat = dsGiaiDoanParsed.isNotEmpty &&
        dsGiaiDoanParsed.every((g) => g.trangThai.toLowerCase() == 'dat');

    return TienDoDuAnModel(
      maHD: (json['maHD'] ?? '').toString(),
      trangThaiHopDong: (json['trangThaiHopDong'] ?? '').toString(),
      ngayBatDauHopDong: json['ngayBatDauHopDong'] != null
          ? DateTime.tryParse(json['ngayBatDauHopDong'].toString())
          : null,
      ngayKetThucHopDong: json['ngayKetThucHopDong'] != null
          ? DateTime.tryParse(json['ngayKetThucHopDong'].toString())
          : null,
      tongSoTask: asInt(json['tongSoTask']),
      soTaskChuaThucHien: asInt(json['soTaskChuaThucHien']),
      soTaskDangThucHien: asInt(json['soTaskDangThucHien']),
      soTaskHoanThanh: asInt(json['soTaskHoanThanh']),
      tyLeHoanThanh: (json['tyLeHoanThanh'] as num?)?.toDouble() ?? 0,
      coQuyenCapNhat: json['coQuyenCapNhat'] == true,
      coQuyenDanhGia: json['coQuyenDanhGia'] == true,
      duAnDaDat: json['duAnDaDat'] == true || allDat,
      danhSachTask: rawTasks
          .whereType<Map<String, dynamic>>()
          .map(TaskCongViecModel.fromJson)
          .toList(),
      danhSachGiaiDoan: dsGiaiDoanParsed,
    );
  }
}
