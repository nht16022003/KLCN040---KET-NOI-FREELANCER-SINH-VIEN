class ChatMessageModel {
  final String id;
  final String senderId;
  final String receiverId;
  final String content;
  final DateTime createdAt;

  const ChatMessageModel({
    required this.id,
    required this.senderId,
    required this.receiverId,
    required this.content,
    required this.createdAt,
  });

  factory ChatMessageModel.fromJson(Map<String, dynamic> json) {
    return ChatMessageModel(
      id: json['id']?.toString() ?? '',
      senderId: json['senderId']?.toString() ?? '',
      receiverId: json['receiverId']?.toString() ?? '',
      content: json['content'] ?? '',
      createdAt: DateTime.tryParse(json['createdAt'] ?? '') ?? DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'senderId': senderId,
    'receiverId': receiverId,
    'content': content,
    'createdAt': createdAt.toIso8601String(),
  };
}
