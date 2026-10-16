class WalletModel {
  final String id;
  final double balance;
  final String currency;

  const WalletModel({
    required this.id,
    required this.balance,
    required this.currency,
  });

  factory WalletModel.fromJson(Map<String, dynamic> json) {
    return WalletModel(
      id: json['id']?.toString() ?? '',
      balance: (json['balance'] as num?)?.toDouble() ?? 0,
      currency: json['currency'] ?? 'VND',
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'balance': balance,
    'currency': currency,
  };
}
