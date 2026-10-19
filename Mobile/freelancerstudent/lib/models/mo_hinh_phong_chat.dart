import '../core/constants/diem_cuoi_api.dart';

/// DTO phòng chat ánh xạ trực tiếp từ Backend `PhongChat_ReponseDTO`
class PhongChatModel {
  final int maPhongChat;
  final int maUserClient;
  final String? tenClient;
  final String? avatarClient;
  final int maFreelancerStudent;
  final int maUserFreelancer;
  final String? tenFreelancer;
  final String? avatarFreelancer;
  final String? maJob;
  final String? tieuDeJob;
  final double? thulaoJob;
  final String? tinNhanCuoi;
  final DateTime? thoiGianTinNhanCuoi;
  final int soTinNhanChuaDoc;
  final String trangThai;
  final DateTime ngayTao;

  const PhongChatModel({
    required this.maPhongChat,
    required this.maUserClient,
    this.tenClient,
    this.avatarClient,
    required this.maFreelancerStudent,
    required this.maUserFreelancer,
    this.tenFreelancer,
    this.avatarFreelancer,
    this.maJob,
    this.tieuDeJob,
    this.thulaoJob,
    this.tinNhanCuoi,
    this.thoiGianTinNhanCuoi,
    this.soTinNhanChuaDoc = 0,
    this.trangThai = 'Active',
    required this.ngayTao,
  });

  String tenDoiPhuong(int currentUserId) {
    if (currentUserId == maUserFreelancer) {
      return tenClient?.isNotEmpty == true ? tenClient! : 'Nhà tuyển dụng';
    }
    return tenFreelancer?.isNotEmpty == true ? tenFreelancer! : 'Freelancer Sinh viên';
  }

  String avatarDoiPhuong(int currentUserId) {
    final rawUrl = currentUserId == maUserFreelancer ? avatarClient : avatarFreelancer;
    final defaultName = tenDoiPhuong(currentUserId);
    return ApiEndpoints.dinhDangUrlAnh(rawUrl, tenMacDinh: defaultName);
  }

  factory PhongChatModel.fromJson(Map<String, dynamic> json) {
    int asInt(dynamic v) => v is int ? v : int.tryParse(v?.toString() ?? '') ?? 0;
    double? asDouble(dynamic v) => v is num ? v.toDouble() : double.tryParse(v?.toString() ?? '');

    return PhongChatModel(
      maPhongChat: asInt(json['maPhongChat']),
      maUserClient: asInt(json['maUserClient']),
      tenClient: json['tenClient']?.toString(),
      avatarClient: json['avatarClient']?.toString(),
      maFreelancerStudent: asInt(json['maFreelancerStudent']),
      maUserFreelancer: asInt(json['maUserFreelancer']),
      tenFreelancer: json['tenFreelancer']?.toString(),
      avatarFreelancer: json['avatarFreelancer']?.toString(),
      maJob: json['maJob']?.toString(),
      tieuDeJob: json['tieuDeJob']?.toString(),
      thulaoJob: asDouble(json['thulaoJob']),
      tinNhanCuoi: json['tinNhanCuoi']?.toString(),
      thoiGianTinNhanCuoi: json['thoiGianTinNhanCuoi'] != null
          ? DateTime.tryParse(json['thoiGianTinNhanCuoi'].toString())
          : null,
      soTinNhanChuaDoc: asInt(json['soTinNhanChuaDoc']),
      trangThai: json['trangThai']?.toString() ?? 'Active',
      ngayTao: json['ngayTao'] != null
          ? (DateTime.tryParse(json['ngayTao'].toString()) ?? DateTime.now())
          : DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() => {
    'maPhongChat': maPhongChat,
    'maUserClient': maUserClient,
    'tenClient': tenClient,
    'avatarClient': avatarClient,
    'maFreelancerStudent': maFreelancerStudent,
    'maUserFreelancer': maUserFreelancer,
    'tenFreelancer': tenFreelancer,
    'avatarFreelancer': avatarFreelancer,
    'maJob': maJob,
    'tieuDeJob': tieuDeJob,
    'thulaoJob': thulaoJob,
    'tinNhanCuoi': tinNhanCuoi,
    'thoiGianTinNhanCuoi': thoiGianTinNhanCuoi?.toIso8601String(),
    'soTinNhanChuaDoc': soTinNhanChuaDoc,
    'trangThai': trangThai,
    'ngayTao': ngayTao.toIso8601String(),
  };
}
