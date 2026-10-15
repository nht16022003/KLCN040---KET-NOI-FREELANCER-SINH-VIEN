class FreelancerProfileModel {
  final int maUser;
  final String hoTen;
  final String email;
  final String? sdt;
  final DateTime? ngaySinh;
  final String? avatarUrl;
  final int maFreelancerStudents;
  final String? maChuyenNganh;
  final String? tenChuyenNganh;
  final String? tenTruong;
  final String? diaDiemTruong;
  final String? diaDiemFreelancerStudent;
  final String? ngonNgu;
  final String? kyNangCoBan;
  final int namThu;
  final double gpa;
  final String nienKhoa;
  final String? gioiThieu;
  final bool trangThaiNhanViec;
  final double? chiPhiTu;
  final List<String> skills;
  final List<PortfolioProjectModel> projects;
  final List<ProofDocumentModel> proofs;
  final List<FreelancerJobPostModel> jobPosts;

  const FreelancerProfileModel({
    required this.maUser,
    required this.hoTen,
    required this.email,
    this.sdt,
    this.ngaySinh,
    this.avatarUrl,
    required this.maFreelancerStudents,
    this.maChuyenNganh,
    this.tenChuyenNganh,
    this.tenTruong,
    this.diaDiemTruong,
    this.diaDiemFreelancerStudent,
    this.ngonNgu,
    this.kyNangCoBan,
    required this.namThu,
    required this.gpa,
    required this.nienKhoa,
    this.gioiThieu,
    required this.trangThaiNhanViec,
    this.chiPhiTu,
    required this.skills,
    required this.projects,
    required this.proofs,
    required this.jobPosts,
  });

  factory FreelancerProfileModel.fromJson(Map<String, dynamic> json) {
    final root = json;
    final user = (root['user'] ?? root['User']) is Map
        ? Map<String, dynamic>.from(root['user'] ?? root['User'])
        : <String, dynamic>{};
    final freelancer =
        (root['freelancerStudent'] ??
                root['FreelancerStudent'] ??
                root['freelancer'] ??
                root['Freelancer'])
            is Map
        ? Map<String, dynamic>.from(
            root['freelancerStudent'] ??
                root['FreelancerStudent'] ??
                root['freelancer'] ??
                root['Freelancer'],
          )
        : <String, dynamic>{};
    final major = (root['chuyenNganh'] ?? root['ChuyenNganh']) is Map
        ? Map<String, dynamic>.from(root['chuyenNganh'] ?? root['ChuyenNganh'])
        : <String, dynamic>{};

    final skills =
        ((root['kyNangs'] ?? root['skills']) is List
                ? (root['kyNangs'] ?? root['skills']) as List
                : const <dynamic>[])
            .map(
              (e) =>
                  e is Map ? (e['tenKyNang'] ?? e['name'] ?? '') : e.toString(),
            )
            .where((e) => e.toString().trim().isNotEmpty)
            .map((e) => e.toString())
            .toList();

    final projects =
        ((root['duAns'] ?? root['portfolio']?['duAn'] ?? root['projects'])
                    is List
                ? (root['duAns'] ??
                          root['portfolio']?['duAn'] ??
                          root['projects'])
                      as List
                : const <dynamic>[])
            .map(
              (e) => PortfolioProjectModel.fromJson(
                Map<String, dynamic>.from(e as Map),
              ),
            )
            .toList();

    final proofs =
        ((root['minhChung'] ?? root['evidences']) is List
                ? (root['minhChung'] ?? root['evidences']) as List
                : const <dynamic>[])
            .map(
              (e) => ProofDocumentModel.fromJson(
                Map<String, dynamic>.from(e as Map),
              ),
            )
            .toList();

    final posts =
        ((root['baiDangTimViec'] ?? root['jobPosts']) is List
                ? (root['baiDangTimViec'] ?? root['jobPosts']) as List
                : const <dynamic>[])
            .map(
              (e) => FreelancerJobPostModel.fromJson(
                Map<String, dynamic>.from(e as Map),
              ),
            )
            .toList();

    return FreelancerProfileModel(
      maUser: (root['maUser'] ?? user['maUser'] ?? 0) is num
          ? (root['maUser'] ?? user['maUser'] ?? 0).toInt()
          : int.tryParse(
                  (root['maUser'] ?? user['maUser'] ?? '0').toString(),
                ) ??
                0,
      hoTen:
          (user['hotenUser'] ?? user['hoTen'] ?? root['hoTen'] ?? 'Sinh viên')
              .toString(),
      email: (user['email'] ?? root['email'] ?? '').toString(),
      sdt: (root['sdt'] ?? user['sdt'] ?? '') == ''
          ? null
          : (root['sdt'] ?? user['sdt'])?.toString(),
      ngaySinh: _parseDate(user['ngaysinh'] ?? root['ngaySinh']),
      avatarUrl: (root['avatarUrl'] ?? root['avatar'] ?? user['avatarUrl'])
          ?.toString(),
      maFreelancerStudents:
          (freelancer['maFreelancerStudents'] ??
                  root['maFreelancerStudents'] ??
                  0)
              is num
          ? (freelancer['maFreelancerStudents'] ??
                    root['maFreelancerStudents'] ??
                    0)
                .toInt()
          : int.tryParse(
                  (freelancer['maFreelancerStudents'] ??
                          root['maFreelancerStudents'] ??
                          '0')
                      .toString(),
                ) ??
                0,
      maChuyenNganh: (freelancer['maChuyenNganh'] ?? '').toString(),
      tenChuyenNganh:
          (major['tenChuyenNganh'] ?? freelancer['tenChuyenNganh'] ?? '')
                  .toString() ==
              ''
          ? null
          : (major['tenChuyenNganh'] ?? freelancer['tenChuyenNganh'] ?? '')
                .toString(),
      tenTruong: (freelancer['tenTruong'] ?? '').toString() == ''
          ? null
          : freelancer['tenTruong']?.toString(),
      diaDiemTruong: (freelancer['diaDiemTruong'] ?? '').toString() == ''
          ? null
          : freelancer['diaDiemTruong']?.toString(),
      diaDiemFreelancerStudent:
          (freelancer['diaDiemFreelancerStudent'] ?? '').toString() == ''
          ? null
          : freelancer['diaDiemFreelancerStudent']?.toString(),
      ngonNgu: (freelancer['ngonNgu'] ?? '').toString() == ''
          ? null
          : freelancer['ngonNgu']?.toString(),
      kyNangCoBan: (freelancer['kyNangCoBan'] ?? '').toString() == ''
          ? null
          : freelancer['kyNangCoBan']?.toString(),
      namThu: (freelancer['namThu'] ?? 1) is num
          ? (freelancer['namThu'] ?? 1).toInt()
          : int.tryParse((freelancer['namThu'] ?? '1').toString()) ?? 1,
      gpa: (freelancer['GPA'] ?? freelancer['gpa'] ?? 0.0) is num
          ? (freelancer['GPA'] ?? freelancer['gpa'] ?? 0.0).toDouble()
          : 0.0,
      nienKhoa: (freelancer['nienKhoa'] ?? '2022-2026').toString(),
      gioiThieu: (freelancer['gioithieu'] ?? '').toString() == ''
          ? null
          : freelancer['gioithieu']?.toString(),
      trangThaiNhanViec:
          freelancer['trangthaiNhanViec'] == true ||
          freelancer['trangthaiNhanViec'] == 1 ||
          freelancer['trangThaiNhanViec'] == true ||
          freelancer['trangThaiNhanViec'] == 1,
      chiPhiTu: _parseDouble(freelancer['chiPhiTu']),
      skills: skills,
      projects: projects,
      proofs: proofs,
      jobPosts: posts,
    );
  }

  static DateTime? _parseDate(dynamic value) {
    if (value == null || value == '') return null;
    if (value is DateTime) return value;
    return DateTime.tryParse(value.toString());
  }

  static double? _parseDouble(dynamic value) {
    if (value == null) return null;
    if (value is num) return value.toDouble();
    return double.tryParse(value.toString());
  }
}

class PortfolioProjectModel {
  final String tenDuAn;
  final String? moTa;
  final String? vaiTro;
  final String? congNghe;
  final String? linkGithub;
  final String? linkDemo;

  const PortfolioProjectModel({
    required this.tenDuAn,
    this.moTa,
    this.vaiTro,
    this.congNghe,
    this.linkGithub,
    this.linkDemo,
  });

  factory PortfolioProjectModel.fromJson(Map<String, dynamic> json) {
    return PortfolioProjectModel(
      tenDuAn: json['tenDuAn'] ?? 'Dự án cá nhân',
      moTa: json['moTa'] ?? json['description'],
      vaiTro: json['vaiTro'] ?? json['role'],
      congNghe: json['congnghe'] ?? json['technology'],
      linkGithub: json['linkGithub'] ?? json['github'],
      linkDemo: json['linkDemo'] ?? json['demo'],
    );
  }
}

class ProofDocumentModel {
  final String loaiMinhChung;
  final String? fileMinhChung;
  final String? trangThaiGuiMinhChung;
  final DateTime? ngayNop;

  const ProofDocumentModel({
    required this.loaiMinhChung,
    this.fileMinhChung,
    this.trangThaiGuiMinhChung,
    this.ngayNop,
  });

  factory ProofDocumentModel.fromJson(Map<String, dynamic> json) {
    return ProofDocumentModel(
      loaiMinhChung: json['loaiMinhChung'] ?? 'Minh chứng',
      fileMinhChung: json['fileMinhChung'],
      trangThaiGuiMinhChung: json['trangThaiGuiMinhChung'],
      ngayNop: FreelancerProfileModel._parseDate(json['ngayNop']),
    );
  }
}

class FreelancerJobPostModel {
  final String maBaiDang;
  final String tieude;
  final String? mota;
  final String? kynang;
  final double? mucGiaTu;
  final String? trangThai;
  final DateTime? thoiGianDang;

  const FreelancerJobPostModel({
    required this.maBaiDang,
    required this.tieude,
    this.mota,
    this.kynang,
    this.mucGiaTu,
    this.trangThai,
    this.thoiGianDang,
  });

  factory FreelancerJobPostModel.fromJson(Map<String, dynamic> json) {
    return FreelancerJobPostModel(
      maBaiDang: json['maBaiDang'] ?? 'BD001',
      tieude: json['tieude'] ?? 'Bài đăng tìm việc',
      mota: json['mota'],
      kynang: json['kynang'],
      mucGiaTu: FreelancerProfileModel._parseDouble(json['mucGiaTu']),
      trangThai: json['trangthai'] ?? json['trangThai'],
      thoiGianDang: FreelancerProfileModel._parseDate(json['thoigiandang']),
    );
  }
}
