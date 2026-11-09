class UngThueModel {
  final int maYeuCau;
  final int maNhaTuyenDung;
  final int maUserNTD;
  final String tenNhaTuyenDung;
  final String? avatarNhaTuyenDung;
  final int maFreelancerStudent;
  final int maUserFreelancer;
  final String tenFreelancer;
  final String? avatarFreelancer;
  final String? maJob;
  final String? tieuDeJob;
  final String? tieuDeCongViec;
  final String? moTaCongViec;
  final double? nganSachDeNghi;
  final DateTime? thoiHanDuKien;
  final DateTime? ngayGui;
  final String trangThai; // ChoPhanHoi, DaChapNhan, DaTuChoi
  final String? lyDoTuChoi;

  UngThueModel({
    required this.maYeuCau,
    required this.maNhaTuyenDung,
    required this.maUserNTD,
    required this.tenNhaTuyenDung,
    this.avatarNhaTuyenDung,
    required this.maFreelancerStudent,
    required this.maUserFreelancer,
    required this.tenFreelancer,
    this.avatarFreelancer,
    this.maJob,
    this.tieuDeJob,
    this.tieuDeCongViec,
    this.moTaCongViec,
    this.nganSachDeNghi,
    this.thoiHanDuKien,
    this.ngayGui,
    required this.trangThai,
    this.lyDoTuChoi,
  });

  factory UngThueModel.fromJson(Map<String, dynamic> json) {
    DateTime? parseDate(dynamic v) {
      if (v == null) return null;
      if (v is DateTime) return v;
      return DateTime.tryParse(v.toString());
    }

    double? parseDouble(dynamic v) {
      if (v == null) return null;
      if (v is num) return v.toDouble();
      return double.tryParse(v.toString());
    }

    int parseInt(dynamic v) {
      if (v == null) return 0;
      if (v is int) return v;
      if (v is num) return v.toInt();
      return int.tryParse(v.toString()) ?? 0;
    }

    return UngThueModel(
      maYeuCau: parseInt(json['maYeuCau']),
      maNhaTuyenDung: parseInt(json['maNhaTuyenDung']),
      maUserNTD: parseInt(json['maUserNTD']),
      tenNhaTuyenDung: json['tenNhaTuyenDung']?.toString() ?? 'Nhà tuyển dụng',
      avatarNhaTuyenDung: json['avatarNhaTuyenDung']?.toString(),
      maFreelancerStudent: parseInt(json['maFreelancerStudent']),
      maUserFreelancer: parseInt(json['maUserFreelancer']),
      tenFreelancer: json['tenFreelancer']?.toString() ?? 'Freelancer',
      avatarFreelancer: json['avatarFreelancer']?.toString(),
      maJob: json['maJob']?.toString(),
      tieuDeJob: json['tieuDeJob']?.toString(),
      tieuDeCongViec: json['tieuDeCongViec']?.toString(),
      moTaCongViec: json['moTaCongViec']?.toString(),
      nganSachDeNghi: parseDouble(json['nganSachDeNghi']),
      thoiHanDuKien: parseDate(json['thoiHanDuKien']),
      ngayGui: parseDate(json['ngayGui']),
      trangThai: json['trangThai']?.toString() ?? 'ChoPhanHoi',
      lyDoTuChoi: json['lyDoTuChoi']?.toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'maYeuCau': maYeuCau,
      'maNhaTuyenDung': maNhaTuyenDung,
      'maUserNTD': maUserNTD,
      'tenNhaTuyenDung': tenNhaTuyenDung,
      'avatarNhaTuyenDung': avatarNhaTuyenDung,
      'maFreelancerStudent': maFreelancerStudent,
      'maUserFreelancer': maUserFreelancer,
      'tenFreelancer': tenFreelancer,
      'avatarFreelancer': avatarFreelancer,
      'maJob': maJob,
      'tieuDeJob': tieuDeJob,
      'tieuDeCongViec': tieuDeCongViec,
      'moTaCongViec': moTaCongViec,
      'nganSachDeNghi': nganSachDeNghi,
      'thoiHanDuKien': thoiHanDuKien?.toIso8601String(),
      'ngayGui': ngayGui?.toIso8601String(),
      'trangThai': trangThai,
      'lyDoTuChoi': lyDoTuChoi,
    };
  }

  bool get laChoPhanHoi => trangThai == 'ChoPhanHoi';
  bool get laDaChapNhan => trangThai == 'DaChapNhan';
  bool get laDaTuChoi => trangThai == 'DaTuChoi';
  bool get laDaTaoHopDong => trangThai == 'DaTaoHopDong';

  String get tenHienThiCongViec {
    if (tieuDeCongViec != null && tieuDeCongViec!.trim().isNotEmpty) {
      return tieuDeCongViec!.trim();
    }
    if (tieuDeJob != null && tieuDeJob!.trim().isNotEmpty) {
      return tieuDeJob!.trim();
    }
    return 'Dự án thuê trực tiếp';
  }
}

class GuiYeuCauThueRequest {
  final int maUserNTD;
  final int maFreelancerStudent;
  final String? maJob;
  final String? tieuDeCongViec;
  final String? moTaCongViec;
  final double? nganSachDeNghi;
  final DateTime? thoiHanDuKien;

  GuiYeuCauThueRequest({
    required this.maUserNTD,
    required this.maFreelancerStudent,
    this.maJob,
    this.tieuDeCongViec,
    this.moTaCongViec,
    this.nganSachDeNghi,
    this.thoiHanDuKien,
  });

  Map<String, dynamic> toJson() {
    return {
      'maUserNTD': maUserNTD,
      'maFreelancerStudent': maFreelancerStudent,
      if (maJob != null && maJob!.isNotEmpty) 'maJob': maJob,
      if (tieuDeCongViec != null && tieuDeCongViec!.isNotEmpty)
        'tieuDeCongViec': tieuDeCongViec,
      if (moTaCongViec != null && moTaCongViec!.isNotEmpty)
        'moTaCongViec': moTaCongViec,
      if (nganSachDeNghi != null) 'nganSachDeNghi': nganSachDeNghi,
      if (thoiHanDuKien != null)
        'thoiHanDuKien': thoiHanDuKien!.toIso8601String(),
    };
  }
}

class PhanHoiYeuCauThueRequest {
  final int maYeuCau;
  final int maUserFreelancer;
  final bool chapNhan;
  final String? lyDoTuChoi;

  PhanHoiYeuCauThueRequest({
    required this.maYeuCau,
    required this.maUserFreelancer,
    required this.chapNhan,
    this.lyDoTuChoi,
  });

  Map<String, dynamic> toJson() {
    return {
      'maYeuCau': maYeuCau,
      'maUserFreelancer': maUserFreelancer,
      'chapNhan': chapNhan,
      if (lyDoTuChoi != null && lyDoTuChoi!.isNotEmpty)
        'lyDoTuChoi': lyDoTuChoi,
    };
  }
}

