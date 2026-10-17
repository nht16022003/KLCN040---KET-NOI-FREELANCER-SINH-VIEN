class JobPostModel {
  final String maJob;
  final String maNhaTuyenDung;
  final String tieude;
  final String mota;
  final String kynangyeucau;
  final double thulao;
  final String status;
  final int soluongtuyen;

  const JobPostModel({
    required this.maJob,
    required this.maNhaTuyenDung,
    required this.tieude,
    required this.mota,
    required this.kynangyeucau,
    required this.thulao,
    required this.status,
    required this.soluongtuyen,
  });

  String get id => maJob;
  String get title => tieude;
  String get description => mota;
  double get budget => thulao;

  factory JobPostModel.fromJson(Map<String, dynamic> json) {
    final idValue = json['maJob'] ?? json['id'] ?? '';
    final titleValue = json['tieude'] ?? json['title'] ?? '';
    final descriptionValue = json['mota'] ?? json['description'] ?? '';
    final skillsValue = json['kynangyeucau'] ?? json['skills'] ?? '';
    final salaryValue = json['thulao'] ?? json['budget'] ?? json['thue'] ?? 0;
    final statusValue = json['status'] ?? 'DangTuyen';
    final quantityValue = json['soluongtuyen'] ?? json['quantity'] ?? 1;

    return JobPostModel(
      maJob: idValue.toString(),
      maNhaTuyenDung: (json['maNhaTuyenDung'] ?? json['employerId'] ?? '')
          .toString(),
      tieude: titleValue.toString(),
      mota: descriptionValue.toString(),
      kynangyeucau: skillsValue.toString(),
      thulao: salaryValue is num ? salaryValue.toDouble() : 0,
      status: statusValue.toString(),
      soluongtuyen: quantityValue is num ? quantityValue.toInt() : 1,
    );
  }

  Map<String, dynamic> toJson() => {
    'maJob': maJob,
    'maNhaTuyenDung': maNhaTuyenDung,
    'tieude': tieude,
    'mota': mota,
    'kynangyeucau': kynangyeucau,
    'thulao': thulao,
    'status': status,
    'soluongtuyen': soluongtuyen,
  };
}
