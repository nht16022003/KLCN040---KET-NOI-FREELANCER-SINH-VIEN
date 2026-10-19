import 'package:flutter/foundation.dart';
import 'package:signalr_netcore/signalr_client.dart';

import '../core/constants/diem_cuoi_api.dart';
import '../models/mo_hinh_tin_nhan_chat.dart';

class ChatSignalrService {
  HubConnection? _hubConnection;
  bool _isConnected = false;
  int? _currentRoomId;

  bool get isConnected => _isConnected;
  int? get currentRoomId => _currentRoomId;

  void Function(ChatMessageModel message)? onMessageReceived;

  Future<void> connect() async {
    if (_isConnected && _hubConnection != null) return;

    try {
      _hubConnection = HubConnectionBuilder()
          .withUrl(ApiEndpoints.chatHubUrl)
          .withAutomaticReconnect()
          .build();

      _hubConnection?.onclose(({error}) {
        _isConnected = false;
        debugPrint('SignalR disconnected: $error');
      });

      _hubConnection?.on('NhanTinNhanMoi', (arguments) {
        if (arguments != null && arguments.isNotEmpty) {
          final data = arguments[0];
          if (data is Map<String, dynamic>) {
            final msg = ChatMessageModel.fromJson(data);
            onMessageReceived?.call(msg);
          }
        }
      });

      await _hubConnection?.start();
      _isConnected = true;
      debugPrint('SignalR ChatHub connected successfully.');
    } catch (e) {
      _isConnected = false;
      debugPrint('SignalR ChatHub connection failed: $e');
    }
  }

  Future<void> thamGiaPhongChat(int maPhongChat) async {
    _currentRoomId = maPhongChat;
    if (!_isConnected || _hubConnection == null) {
      await connect();
    }
    try {
      await _hubConnection?.invoke('ThamGiaPhongChat', args: [maPhongChat]);
    } catch (e) {
      debugPrint('SignalR ThamGiaPhongChat error: $e');
    }
  }

  Future<void> roiPhongChat(int maPhongChat) async {
    if (_currentRoomId == maPhongChat) {
      _currentRoomId = null;
    }
    if (!_isConnected || _hubConnection == null) return;
    try {
      await _hubConnection?.invoke('RoiPhongChat', args: [maPhongChat]);
    } catch (e) {
      debugPrint('SignalR RoiPhongChat error: $e');
    }
  }

  Future<void> disconnect() async {
    try {
      if (_currentRoomId != null) {
        await roiPhongChat(_currentRoomId!);
      }
      await _hubConnection?.stop();
    } catch (e) {
      debugPrint('SignalR disconnect error: $e');
    } finally {
      _isConnected = false;
      _hubConnection = null;
    }
  }
}
