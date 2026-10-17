import 'package:flutter/material.dart';

class ChatDetailScreen extends StatelessWidget {
  final String? receiverId;
  final String? receiverName;
  final String? avatarUrl;

  const ChatDetailScreen({
    super.key,
    this.receiverId,
    this.receiverName,
    this.avatarUrl,
  });

  @override
  Widget build(BuildContext context) {
    final title = receiverName != null && receiverName!.isNotEmpty
        ? 'Chat với $receiverName'
        : 'Chat với Nguyễn Văn A';

    return Scaffold(
      appBar: AppBar(
        title: Text(title),
        backgroundColor: Colors.white,
        foregroundColor: const Color(0xFF1D5D3B),
        elevation: 0.5,
      ),
      body: Column(
        children: [
          Expanded(
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: const [
                Align(
                  alignment: Alignment.centerLeft,
                  child: Chip(label: Text('Xin chào!')),
                ),
                Align(
                  alignment: Alignment.centerRight,
                  child: Chip(label: Text('Chào bạn!')),
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(12),
            child: Row(
              children: [
                const Expanded(
                  child: TextField(
                    decoration: InputDecoration(
                      hintText: 'Nhập tin nhắn...',
                      border: OutlineInputBorder(),
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                IconButton(onPressed: () {}, icon: const Icon(Icons.send)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
