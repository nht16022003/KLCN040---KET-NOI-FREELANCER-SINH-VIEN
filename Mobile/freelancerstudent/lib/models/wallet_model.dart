class WalletModel {
  final int maWallet;
  final String tenUsers;
  final double soDuKhaDung;
  final double soDuDongBang;
  final String loaiTienTe;

  const WalletModel({
    required this.maWallet,
    required this.tenUsers,
    required this.soDuKhaDung,
    required this.soDuDongBang,
    this.loaiTienTe = 'VND',
  });

  // Tên biến/getter thuần tiếng Việt
  int get maVi => maWallet;
  String get tenChuVi => tenUsers;
  double get tongSoDu => soDuKhaDung + soDuDongBang;

  // Tương thích ngược
  String get id => maWallet.toString();
  double get balance => soDuKhaDung;
  double get frozenBalance => soDuDongBang;
  String get currency => loaiTienTe;

  factory WalletModel.fromJson(Map<String, dynamic> json) {
    final rawMaWallet = json['maWallet'] ?? json['id'] ?? 0;
    final parsedMaWallet = rawMaWallet is int ? rawMaWallet : int.tryParse(rawMaWallet.toString()) ?? 0;

    final rawSoDuKhaDung = json['soDuKhaDung'] ?? json['balance'] ?? 0;
    final rawSoDuDongBang = json['soDuDongBang'] ?? json['frozenBalance'] ?? 0;

    return WalletModel(
      maWallet: parsedMaWallet,
      tenUsers: (json['tenUsers'] ?? json['userName'] ?? '').toString(),
      soDuKhaDung: rawSoDuKhaDung is num ? rawSoDuKhaDung.toDouble() : 0,
      soDuDongBang: rawSoDuDongBang is num ? rawSoDuDongBang.toDouble() : 0,
      loaiTienTe: json['currency']?.toString() ?? 'VND',
    );
  }

  Map<String, dynamic> toJson() => {
    'maWallet': maWallet,
    'tenUsers': tenUsers,
    'soDuKhaDung': soDuKhaDung,
    'soDuDongBang': soDuDongBang,
    'loaiTienTe': loaiTienTe,
    // Tương thích ngược
    'id': maWallet.toString(),
    'balance': soDuKhaDung,
    'currency': loaiTienTe,
  };
}
