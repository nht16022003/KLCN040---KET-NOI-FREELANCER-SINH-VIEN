class ApplicationModel {
  final String maUngTuyen;
  final String maJob;
  final String maFreelancerStudents;
  final String thuGioiThieu;
  final double thulaoDeXuat;
  final String trangThaiUngTuyen;

  // Thuộc tính bổ sung từ UngTuyen_ReponseDTO
  final String tieude;
  final String? tenUser;
  final String? emailUser;
  final String? sdtUser;
  final String? avatar;
  final String? tenTruong;
  final String? tenChuyenNganh;
  final String? thoiGianHoanThanhDeXuat;
  final String? trangThaiHopDong;
  final String? maHopDong;

  const ApplicationModel({
    String maUngTuyen = '',
    String maJob = '',
    String maFreelancerStudents = '',
    String thuGioiThieu = '',
    double thulaoDeXuat = 0,
    String trangThaiUngTuyen = 'ChoDuyet',
    this.tieude = '',
    this.tenUser,
    this.emailUser,
    this.sdtUser,
    this.avatar,
    this.tenTruong,
    this.tenChuyenNganh,
    this.thoiGianHoanThanhDeXuat,
    this.trangThaiHopDong,
    this.maHopDong,
    // Tương thích ngược với constructor cũ
    String? id,
    String? jobId,
    String? applicantId,
    String? coverLetter,
    double? proposedBudget,
    String? status,
  })  : maUngTuyen = id ?? maUngTuyen,
        maJob = jobId ?? maJob,
        maFreelancerStudents = applicantId ?? maFreelancerStudents,
        thuGioiThieu = coverLetter ?? thuGioiThieu,
        thulaoDeXuat = proposedBudget ?? thulaoDeXuat,
        trangThaiUngTuyen = status ?? trangThaiUngTuyen;

  // Tên biến/getter thuần tiếng Việt
  String get maDonUngTuyen => maUngTuyen;
  String get maCongViec => maJob;
  String get maSinhVien => maFreelancerStudents;
  String get tieuDeCongViec => tieude;
  String get tenUngVien => tenUser ?? '';
  String get trangThai => trangThaiUngTuyen;
  double get thuLaoDeXuat => thulaoDeXuat;
  bool get daCoHopDong =>
      trangThaiHopDong != null &&
      trangThaiHopDong!.trim().isNotEmpty &&
      trangThaiHopDong!.trim() != 'DaHuy';

  // Tương thích ngược cho các màn hình cũ
  String get id => maUngTuyen;
  String get jobId => maJob;
  String get applicantId => maFreelancerStudents;
  String get coverLetter => thuGioiThieu;
  double get proposedBudget => thulaoDeXuat;
  String get status => trangThaiUngTuyen;

  ApplicationModel copyWith({
    String? status,
    String? trangThaiHopDong,
    String? maHopDong,
  }) => ApplicationModel(
    maUngTuyen: maUngTuyen,
    maJob: maJob,
    maFreelancerStudents: maFreelancerStudents,
    thuGioiThieu: thuGioiThieu,
    thulaoDeXuat: thulaoDeXuat,
    trangThaiUngTuyen: status ?? trangThaiUngTuyen,
    tieude: tieude,
    tenUser: tenUser,
    emailUser: emailUser,
    sdtUser: sdtUser,
    avatar: avatar,
    tenTruong: tenTruong,
    tenChuyenNganh: tenChuyenNganh,
    thoiGianHoanThanhDeXuat: thoiGianHoanThanhDeXuat,
    trangThaiHopDong: trangThaiHopDong ?? this.trangThaiHopDong,
    maHopDong: maHopDong ?? this.maHopDong,
  );

  factory ApplicationModel.fromJson(Map<String, dynamic> json) {
    final rawId = json['maUngTuyen'] ?? json['id'] ?? '';
    final rawJobId = json['maJob'] ?? json['jobId'] ?? '';
    final rawApplicantId = json['maFreelancerStudents'] ?? json['maFreelancerStudent'] ?? json['applicantId'] ?? '';
    final rawCover = json['thuGioiThieu'] ?? json['coverLetter'] ?? '';
    final rawBudget = json['thulaoDeXuat'] ?? json['thulao'] ?? json['proposedBudget'] ?? 0;
    final rawStatus = json['trangThaiUngTuyen'] ?? json['status'] ?? 'ChoDuyet';

    return ApplicationModel(
      maUngTuyen: rawId.toString(),
      maJob: rawJobId.toString(),
      maFreelancerStudents: rawApplicantId.toString(),
      thuGioiThieu: rawCover.toString(),
      thulaoDeXuat: rawBudget is num
          ? rawBudget.toDouble()
          : double.tryParse(rawBudget.toString()) ?? 0,
      trangThaiUngTuyen: rawStatus.toString(),
      tieude: (json['tieude'] ?? json['title'] ?? '').toString(),
      tenUser: json['tenUser']?.toString() ?? json['hovaten']?.toString(),
      emailUser: json['emailUser']?.toString() ?? json['email']?.toString(),
      sdtUser: json['sdtUser']?.toString() ?? json['sodienthoai']?.toString(),
      avatar: json['avatar']?.toString() ?? json['avatarUrl']?.toString(),
      tenTruong: json['tenTruong']?.toString(),
      tenChuyenNganh: json['tenChuyenNganh']?.toString(),
      thoiGianHoanThanhDeXuat: json['thoiGianHoanThanhDeXuat']?.toString(),
      trangThaiHopDong: (json['trangThaiHopDong'] ?? json['TrangThaiHopDong'])?.toString(),
      maHopDong: (json['maHopDong'] ?? json['MaHopDong'])?.toString(),
    );
  }

  Map<String, dynamic> toJson() => {
    'maUngTuyen': maUngTuyen,
    'maJob': maJob,
    'maFreelancerStudents': maFreelancerStudents,
    'thuGioiThieu': thuGioiThieu,
    'thulaoDeXuat': thulaoDeXuat,
    'trangThaiUngTuyen': trangThaiUngTuyen,
    'tieude': tieude,
    'tenUser': tenUser,
    'emailUser': emailUser,
    'sdtUser': sdtUser,
    'avatar': avatar,
    'tenTruong': tenTruong,
    'tenChuyenNganh': tenChuyenNganh,
    'thoiGianHoanThanhDeXuat': thoiGianHoanThanhDeXuat,
    'trangThaiHopDong': trangThaiHopDong,
    'maHopDong': maHopDong,
    // Tương thích ngược
    'id': maUngTuyen,
    'jobId': maJob,
    'applicantId': maFreelancerStudents,
    'coverLetter': thuGioiThieu,
    'proposedBudget': thulaoDeXuat,
    'status': trangThaiUngTuyen,
  };
}
