import 'package:flutter/foundation.dart';

import '../models/chat_message_model.dart';
import '../services/chat_signalr_service.dart';

class ChatViewModel extends ChangeNotifier {
  final ChatSignalrService _chatService = ChatSignalrService();

  bool _isLoading = false;
  final List<ChatMessageModel> _messages = <ChatMessageModel>[];

  bool get isLoading => _isLoading;
  List<ChatMessageModel> get messages => _messages;

  Future<void> connectChat() async {
    _isLoading = true;
    notifyListeners();
    await _chatService.connect();
    _isLoading = false;
    notifyListeners();
  }

  Future<void> sendMessage(String receiverId, String content) async {
    await _chatService.sendMessage(receiverId, content);
  }
}
