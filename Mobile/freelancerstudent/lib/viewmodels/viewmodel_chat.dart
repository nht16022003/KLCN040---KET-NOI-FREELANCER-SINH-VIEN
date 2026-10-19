import 'package:flutter/foundation.dart';

import '../models/mo_hinh_tin_nhan_chat.dart';
import '../models/mo_hinh_phong_chat.dart';
import '../services/dich_vu_chat.dart';
import '../services/dich_vu_chat_signalr.dart';

class ChatViewModel extends ChangeNotifier {
  final ChatService _chatService = ChatService();
  final ChatSignalrService _signalrService = ChatSignalrService();

  bool _dangTaiPhong = false;
  bool _dangTaiTinNhan = false;
  bool _dangGuiTinNhan = false;
  String? _thongBaoLoi;

  List<PhongChatModel> _danhSachPhong = <PhongChatModel>[];
  List<ChatMessageModel> _danhSachTinNhan = <ChatMessageModel>[];
  PhongChatModel? _phongHienTai;

  bool get dangTaiPhong => _dangTaiPhong;
  bool get dangTaiTinNhan => _dangTaiTinNhan;
  bool get dangGuiTinNhan => _dangGuiTinNhan;
  String? get thongBaoLoi => _thongBaoLoi;

  List<PhongChatModel> get danhSachPhong => _danhSachPhong;
  List<ChatMessageModel> get danhSachTinNhan => _danhSachTinNhan;
  PhongChatModel? get phongHienTai => _phongHienTai;

  // Tương thích ngược
  bool get isLoading => _dangTaiTinNhan || _dangTaiPhong;
  List<ChatMessageModel> get messages => _danhSachTinNhan;

  ChatViewModel() {
    _signalrService.onMessageReceived = _onTinNhanMoiTuSignalR;
  }

  void _onTinNhanMoiTuSignalR(ChatMessageModel tinMoi) {
    if (_phongHienTai != null && tinMoi.maPhongChat == _phongHienTai!.maPhongChat) {
      final daTonTai = _danhSachTinNhan.any((m) => m.maTinNhan == tinMoi.maTinNhan);
      if (!daTonTai) {
        _danhSachTinNhan.add(tinMoi);
        notifyListeners();
      }
    }

    // Cập nhật tin nhắn cuối trong danh sách phòng
    final index = _danhSachPhong.indexWhere((p) => p.maPhongChat == tinMoi.maPhongChat);
    if (index >= 0) {
      final old = _danhSachPhong[index];
      _danhSachPhong[index] = PhongChatModel(
        maPhongChat: old.maPhongChat,
        maUserClient: old.maUserClient,
        tenClient: old.tenClient,
        avatarClient: old.avatarClient,
        maFreelancerStudent: old.maFreelancerStudent,
        maUserFreelancer: old.maUserFreelancer,
        tenFreelancer: old.tenFreelancer,
        avatarFreelancer: old.avatarFreelancer,
        maJob: old.maJob,
        tieuDeJob: old.tieuDeJob,
        thulaoJob: old.thulaoJob,
        tinNhanCuoi: tinMoi.noiDung,
        thoiGianTinNhanCuoi: tinMoi.ngayGui,
        soTinNhanChuaDoc: old.soTinNhanChuaDoc + 1,
        trangThai: old.trangThai,
        ngayTao: old.ngayTao,
      );
      notifyListeners();
    }
  }

  /// Tải danh sách phòng chat của User hiện tại
  Future<void> taiDanhSachPhong(int maUser) async {
    _dangTaiPhong = true;
    _thongBaoLoi = null;
    notifyListeners();

    final res = await _chatService.layDanhSachPhong(maUser);
    if (res.isSuccess && res.data != null) {
      _danhSachPhong = res.data!;
    } else {
      _thongBaoLoi = res.message;
    }

    _dangTaiPhong = false;
    notifyListeners();

    // Kết nối SignalR để sẵn sàng lắng nghe realtime
    _signalrService.connect();
  }

  /// Mở phòng chat: nếu đã có phòng thì load tin, nếu chưa có thì tạo/lấy theo User + Job
  Future<void> moPhongChat({
    required int currentUserId,
    int? maPhongChat,
    int? maUserClient,
    int? maFreelancerStudent,
    String? maJob,
  }) async {
    _dangTaiTinNhan = true;
    _thongBaoLoi = null;
    _danhSachTinNhan = [];
    notifyListeners();

    PhongChatModel? phong;

    // 1. Nếu có mã phòng chat trực tiếp
    if (maPhongChat != null && maPhongChat > 0) {
      final index = _danhSachPhong.indexWhere((p) => p.maPhongChat == maPhongChat);
      if (index >= 0) {
        phong = _danhSachPhong[index];
      }
    }

    // 2. Nếu chưa có phòng hoặc cần tạo/lấy theo đối phương
    if (phong == null && maUserClient != null && maFreelancerStudent != null) {
      final taoRes = await _chatService.taoHoacLayPhong(
        maUserClient: maUserClient,
        maFreelancerStudent: maFreelancerStudent,
        maJob: maJob,
      );
      if (taoRes.isSuccess && taoRes.data != null) {
        phong = taoRes.data!;
        maPhongChat = phong.maPhongChat;
      } else {
        _thongBaoLoi = taoRes.message;
        _dangTaiTinNhan = false;
        notifyListeners();
        return;
      }
    }

    _phongHienTai = phong;

    // 3. Tải lịch sử tin nhắn
    final idPhong = maPhongChat ?? phong?.maPhongChat;
    if (idPhong != null && idPhong > 0) {
      final res = await _chatService.layLichSuTinNhan(idPhong, currentUserId);
      if (res.isSuccess && res.data != null) {
        _danhSachTinNhan = res.data!;
      } else {
        _thongBaoLoi = res.message;
      }

      // Tham gia phòng trong SignalR
      await _signalrService.thamGiaPhongChat(idPhong);
    }

    _dangTaiTinNhan = false;
    notifyListeners();
  }

  /// Gửi tin nhắn mới
  Future<bool> guiTinNhan({
    required int maPhongChat,
    required int maNguoiGui,
    required String noiDung,
    String? fileDinhKem,
  }) async {
    if (noiDung.trim().isEmpty && (fileDinhKem == null || fileDinhKem.isEmpty)) {
      return false;
    }

    _dangGuiTinNhan = true;
    notifyListeners();

    final res = await _chatService.guiTinNhan(
      maPhongChat: maPhongChat,
      maNguoiGui: maNguoiGui,
      noiDung: noiDung.trim(),
      fileDinhKem: fileDinhKem,
    );

    _dangGuiTinNhan = false;

    if (res.isSuccess && res.data != null) {
      final tinDaLuu = res.data!;
      final daTonTai = _danhSachTinNhan.any((m) => m.maTinNhan == tinDaLuu.maTinNhan);
      if (!daTonTai) {
        _danhSachTinNhan.add(tinDaLuu);
      }
      notifyListeners();
      return true;
    } else {
      _thongBaoLoi = res.message;
      notifyListeners();
      return false;
    }
  }

  /// Rời phòng chat khi thoát màn hình chi tiết
  Future<void> roiPhongChat(int maPhongChat) async {
    await _signalrService.roiPhongChat(maPhongChat);
    if (_phongHienTai?.maPhongChat == maPhongChat) {
      _phongHienTai = null;
      _danhSachTinNhan = [];
    }
    notifyListeners();
  }

  // Tương thích ngược
  Future<void> connectChat() => _signalrService.connect();

  Future<void> sendMessage(String receiverId, String content) async {
    if (_phongHienTai != null) {
      await guiTinNhan(
        maPhongChat: _phongHienTai!.maPhongChat,
        maNguoiGui: _phongHienTai!.maUserFreelancer,
        noiDung: content,
      );
    }
  }

  @override
  void dispose() {
    _signalrService.disconnect();
    super.dispose();
  }
}
