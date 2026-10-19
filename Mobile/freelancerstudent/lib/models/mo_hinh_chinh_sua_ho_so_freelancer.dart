import 'mo_hinh_ho_so_freelancer.dart';

class FreelancerProfileEditModel {
  final int maFreelancerStudents;
  final int maUser;
  final String tenUser;
  final String? sdt;
  final String? email;
  final String? avatar;
  final String maChuyenNganh;
  final String tenChuyenNganh;
  final String tenTruong;
  final String diaDiemFreelancerStudent;
  final int namThu;
  final double gpa;
  final String nienKhoa;
  final String? gioithieu;
  final String? kyNangCoBan;
  final String? ngonNgu;
  final bool trangthaiNhanViec;
  final double? chiPhiTu;
  final List<String> danhSachKyNang;

  const FreelancerProfileEditModel({
    this.maFreelancerStudents = 0,
    this.maUser = 0,
    this.tenUser = '',
    this.sdt,
    this.email,
    this.avatar,
    this.maChuyenNganh = '',
    this.tenChuyenNganh = '',
    this.tenTruong = '',
    this.diaDiemFreelancerStudent = '',
    this.namThu = 0,
    this.gpa = 0,
    this.nienKhoa = '',
    this.gioithieu,
    this.kyNangCoBan,
    this.ngonNgu,
    this.trangthaiNhanViec = true,
    this.chiPhiTu,
    this.danhSachKyNang = const [],
  });

  factory FreelancerProfileEditModel.fromJson(Map<String, dynamic> json) {
    final rawSkills = json['danhSachKyNang'];
    final rawAvailable = json['trangthaiNhanViec'];
    final rawPrice = json['chiPhiTu'];

    return FreelancerProfileEditModel(
      maFreelancerStudents: _readInt(json['maFreelancerStudents']),
      maUser: _readInt(json['maUser']),
      tenUser: _readString(json['tenUser']),
      sdt: _readNullableString(json['sdt']),
      email: _readNullableString(json['email']),
      avatar: _readNullableString(json['avatar']),
      maChuyenNganh: _readString(json['maChuyenNganh']),
      tenChuyenNganh: _readString(json['tenChuyenNganh']),
      tenTruong: _readString(json['tenTruong']),
      diaDiemFreelancerStudent: _readString(json['diaDiemFreelancerStudent']),
      namThu: _readInt(json['namThu']),
      gpa: _readDouble(json['GPA']),
      nienKhoa: _readString(json['nienKhoa']),
      gioithieu: _readNullableString(json['gioithieu']),
      kyNangCoBan: _readNullableString(json['kyNangCoBan']),
      ngonNgu: _readNullableString(json['ngonNgu']),
      trangthaiNhanViec:
          rawAvailable == null ||
          rawAvailable == true ||
          rawAvailable == 1 ||
          rawAvailable?.toString().toLowerCase() == 'true',
      chiPhiTu: _readNullableDouble(rawPrice),
      danhSachKyNang: rawSkills is List
          ? rawSkills.map((skill) => skill.toString()).toList()
          : const [],
    );
  }

  factory FreelancerProfileEditModel.fromProfile(
    FreelancerProfileModel profile,
  ) {
    return FreelancerProfileEditModel(
      maFreelancerStudents: profile.maFreelancerStudents,
      maUser: profile.maUser,
      tenUser: profile.hoTen,
      sdt: profile.sdt,
      email: profile.email,
      avatar: profile.avatarUrl,
      maChuyenNganh: profile.maChuyenNganh ?? '',
      tenChuyenNganh: profile.tenChuyenNganh ?? '',
      tenTruong: profile.tenTruong ?? '',
      diaDiemFreelancerStudent: profile.diaDiemFreelancerStudent ?? '',
      namThu: profile.namThu,
      gpa: profile.gpa,
      nienKhoa: profile.nienKhoa,
      gioithieu: profile.gioiThieu,
      kyNangCoBan: profile.kyNangCoBan,
      ngonNgu: profile.ngonNgu,
      trangthaiNhanViec: profile.trangThaiNhanViec,
      chiPhiTu: profile.chiPhiTu,
      danhSachKyNang: profile.skills,
    );
  }

  Map<String, dynamic> toJson() => {
    'maFreelancerStudents': maFreelancerStudents,
    'maUser': maUser,
    'tenUser': tenUser,
    'sdt': sdt,
    'email': email,
    'avatar': avatar,
    'maChuyenNganh': maChuyenNganh,
    'tenChuyenNganh': tenChuyenNganh,
    'tenTruong': tenTruong,
    'diaDiemFreelancerStudent': diaDiemFreelancerStudent,
    'namThu': namThu,
    'GPA': gpa,
    'nienKhoa': nienKhoa,
    'gioithieu': gioithieu,
    'kyNangCoBan': kyNangCoBan,
    'ngonNgu': ngonNgu,
    'trangthaiNhanViec': trangthaiNhanViec,
    'chiPhiTu': chiPhiTu,
    'danhSachKyNang': danhSachKyNang,
  };

  static int _readInt(dynamic value) {
    if (value is num) return value.toInt();
    return int.tryParse(value?.toString() ?? '') ?? 0;
  }

  static double _readDouble(dynamic value) {
    if (value is num) return value.toDouble();
    return double.tryParse(value?.toString() ?? '') ?? 0;
  }

  static double? _readNullableDouble(dynamic value) {
    if (value == null) return null;
    if (value is num) return value.toDouble();
    return double.tryParse(value.toString());
  }

  static String _readString(dynamic value) => value?.toString() ?? '';

  static String? _readNullableString(dynamic value) => value?.toString();
}
