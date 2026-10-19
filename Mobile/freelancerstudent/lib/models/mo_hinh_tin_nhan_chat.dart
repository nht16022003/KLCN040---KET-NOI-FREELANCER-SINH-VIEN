/// Model tin nhắn chat đồng bộ từ `TinNhan_ReponseDTO` của Backend
class ChatMessageModel {
  final int maTinNhan;
  final int maPhongChat;
  final int maNguoiGui;
  final String? tenNguoiGui;
  final String? avatarNguoiGui;
  final String noiDung;
  final String? fileDinhKem;
  final String loaiTinNhan;
  final bool daDoc;
  final DateTime ngayGui;

  const ChatMessageModel({
    required this.maTinNhan,
    required this.maPhongChat,
    required this.maNguoiGui,
    this.tenNguoiGui,
    this.avatarNguoiGui,
    required this.noiDung,
    this.fileDinhKem,
    this.loaiTinNhan = 'Text',
    this.daDoc = false,
    required this.ngayGui,
  });

  // Getter tương thích ngược cho giao diện cũ
  String get id => maTinNhan.toString();
  String get senderId => maNguoiGui.toString();
  String get receiverId => '';
  String get content => noiDung;
  DateTime get createdAt => ngayGui;

  factory ChatMessageModel.fromJson(Map<String, dynamic> json) {
    int asInt(dynamic v) => v is int ? v : int.tryParse(v?.toString() ?? '') ?? 0;

    return ChatMessageModel(
      maTinNhan: asInt(json['maTinNhan'] ?? json['id']),
      maPhongChat: asInt(json['maPhongChat']),
      maNguoiGui: asInt(json['maNguoiGui'] ?? json['senderId']),
      tenNguoiGui: json['tenNguoiGui']?.toString(),
      avatarNguoiGui: json['avatarNguoiGui']?.toString(),
      noiDung: (json['noiDung'] ?? json['content'] ?? '').toString(),
      fileDinhKem: json['fileDinhKem']?.toString(),
      loaiTinNhan: json['loaiTinNhan']?.toString() ?? 'Text',
      daDoc: json['daDoc'] == true,
      ngayGui: json['ngayGui'] != null
          ? (DateTime.tryParse(json['ngayGui'].toString()) ?? DateTime.now())
          : (json['createdAt'] != null
              ? (DateTime.tryParse(json['createdAt'].toString()) ?? DateTime.now())
              : DateTime.now()),
    );
  }

  Map<String, dynamic> toJson() => {
    'maTinNhan': maTinNhan,
    'maPhongChat': maPhongChat,
    'maNguoiGui': maNguoiGui,
    'tenNguoiGui': tenNguoiGui,
    'avatarNguoiGui': avatarNguoiGui,
    'noiDung': noiDung,
    'fileDinhKem': fileDinhKem,
    'loaiTinNhan': loaiTinNhan,
    'daDoc': daDoc,
    'ngayGui': ngayGui.toIso8601String(),
    // Backward compatibility
    'id': maTinNhan.toString(),
    'senderId': maNguoiGui.toString(),
    'content': noiDung,
    'createdAt': ngayGui.toIso8601String(),
  };
}
