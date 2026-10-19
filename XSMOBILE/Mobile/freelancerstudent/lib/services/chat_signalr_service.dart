import '../models/chat_message_model.dart';

class ChatSignalrService {
  bool isConnected = false;

  Future<void> connect() async {
    isConnected = true;
  }

  Future<void> disconnect() async {
    isConnected = false;
  }

  Future<void> sendMessage(String receiverId, String content) async {
    if (receiverId.isEmpty || content.trim().isEmpty) {
      return;
    }
  }

  void onReceiveMessage(void Function(ChatMessageModel message) callback) {
    // Placeholder for SignalR callback registration.
  }
}
