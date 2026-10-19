import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_tin_nhan_chat.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_chat.dart';
import '../../viewmodels/viewmodel_ho_so_freelancer.dart';

class ChatDetailScreen extends StatefulWidget {
  final int? maPhongChat;
  final String? receiverId;
  final String? receiverName;
  final String? avatarUrl;
  final String? maJob;
  final int? maFreelancerStudent;

  const ChatDetailScreen({
    super.key,
    this.maPhongChat,
    this.receiverId,
    this.receiverName,
    this.avatarUrl,
    this.maJob,
    this.maFreelancerStudent,
  });

  @override
  State<ChatDetailScreen> createState() => _ChatDetailScreenState();
}

class _ChatDetailScreenState extends State<ChatDetailScreen> {
  final TextEditingController _noiDungController = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  int? _maPhongHienTai;

  @override
  void initState() {
    super.initState();
    _maPhongHienTai = widget.maPhongChat;
    WidgetsBinding.instance.addPostFrameCallback((_) => _khoiTaoPhong());
  }

  Future<void> _khoiTaoPhong() async {
    final authUser = context.read<AuthViewModel>().currentUser;
    if (authUser == null) return;

    final chatVM = context.read<ChatViewModel>();

    if (widget.maPhongChat != null && widget.maPhongChat! > 0) {
      _maPhongHienTai = widget.maPhongChat;
      await chatVM.moPhongChat(
        currentUserId: authUser.userId,
        maPhongChat: widget.maPhongChat,
      );
      _cuonXuongCuoi();
      return;
    }

    // Nếu chưa có phòng chat ID trực tiếp, mở/tạo phòng theo tài khoản
    int maUserClient = 0;
    int maFreelancerStudent = 0;

    final receiverIdInt = int.tryParse(widget.receiverId ?? '') ?? 0;

    if (authUser.role == 'NhaTuyenDung' || authUser.marole == 2) {
      maUserClient = authUser.userId;
      maFreelancerStudent = widget.maFreelancerStudent ?? receiverIdInt;
    } else {
      // Freelancer đang liên hệ Nhà tuyển dụng
      maUserClient = receiverIdInt;
      final hoSoVM = context.read<FreelancerProfileViewModel>();
      if (hoSoVM.profile == null) {
        await hoSoVM.loadProfile(authUser.userId);
      }
      maFreelancerStudent = hoSoVM.profile?.maFreelancerStudents ?? 0;
    }

    if (maUserClient > 0 && maFreelancerStudent > 0) {
      await chatVM.moPhongChat(
        currentUserId: authUser.userId,
        maUserClient: maUserClient,
        maFreelancerStudent: maFreelancerStudent,
        maJob: widget.maJob,
      );
      if (mounted) {
        setState(() {
          _maPhongHienTai = chatVM.phongHienTai?.maPhongChat;
        });
      }
      _cuonXuongCuoi();
    }
  }

  void _cuonXuongCuoi() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scrollController.hasClients) {
        _scrollController.animateTo(
          _scrollController.position.maxScrollExtent,
          duration: const Duration(milliseconds: 250),
          curve: Curves.easeOut,
        );
      }
    });
  }

  Future<void> _guiTinNhan() async {
    final noiDung = _noiDungController.text.trim();
    if (noiDung.isEmpty) return;

    final authUser = context.read<AuthViewModel>().currentUser;
    final chatVM = context.read<ChatViewModel>();
    final maPhong = _maPhongHienTai ?? chatVM.phongHienTai?.maPhongChat;

    if (authUser == null || maPhong == null || maPhong <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Chưa kết nối được phòng chat.')),
      );
      return;
    }

    _noiDungController.clear();
    final thanhCong = await chatVM.guiTinNhan(
      maPhongChat: maPhong,
      maNguoiGui: authUser.userId,
      noiDung: noiDung,
    );

    if (thanhCong) {
      _cuonXuongCuoi();
    } else {
      if (mounted && chatVM.thongBaoLoi != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(chatVM.thongBaoLoi!)),
        );
      }
    }
  }

  @override
  void dispose() {
    final maPhong = _maPhongHienTai;
    if (maPhong != null && maPhong > 0) {
      context.read<ChatViewModel>().roiPhongChat(maPhong);
    }
    _noiDungController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final authUser = context.watch<AuthViewModel>().currentUser;
    final chatVM = context.watch<ChatViewModel>();

    final title = widget.receiverName != null && widget.receiverName!.isNotEmpty
        ? widget.receiverName!
        : (chatVM.phongHienTai != null && authUser != null
            ? chatVM.phongHienTai!.tenDoiPhuong(authUser.userId)
            : 'Cuộc trò chuyện');

    final dsTinNhan = chatVM.danhSachTinNhan;
    final dangTai = chatVM.dangTaiTinNhan;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        titleSpacing: 0,
        title: Row(
          children: [
            CircleAvatar(
              radius: 18,
              backgroundColor: AppColors.lightGreenSurface,
              backgroundImage: widget.avatarUrl != null && widget.avatarUrl!.isNotEmpty
                  ? NetworkImage(widget.avatarUrl!)
                  : null,
              child: widget.avatarUrl == null || widget.avatarUrl!.isEmpty
                  ? Text(
                      title.isNotEmpty ? title[0].toUpperCase() : 'U',
                      style: const TextStyle(
                        color: AppColors.primary,
                        fontWeight: FontWeight.bold,
                      ),
                    )
                  : null,
            ),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                  if (widget.maJob != null && widget.maJob!.isNotEmpty)
                    Text(
                      'Công việc: ${widget.maJob}',
                      style: const TextStyle(fontSize: 11, color: AppColors.bodyText),
                    ),
                ],
              ),
            ),
          ],
        ),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: Column(
        children: [
          Expanded(
            child: dangTai && dsTinNhan.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : dsTinNhan.isEmpty
                    ? Center(
                        child: Text(
                          chatVM.thongBaoLoi ?? 'Chưa có tin nhắn nào. Hãy gửi lời chào!',
                          style: const TextStyle(color: Colors.black54),
                        ),
                      )
                    : ListView.builder(
                        controller: _scrollController,
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                        itemCount: dsTinNhan.length,
                        itemBuilder: (context, index) {
                          final tin = dsTinNhan[index];
                          final laCuaToi = authUser != null && tin.maNguoiGui == authUser.userId;
                          return _TinNhanBubble(
                            tinNhan: tin,
                            laCuaToi: laCuaToi,
                          );
                        },
                      ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            decoration: BoxDecoration(
              color: Colors.white,
              border: Border(top: BorderSide(color: Colors.grey.shade200)),
            ),
            child: SafeArea(
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _noiDungController,
                      textInputAction: TextInputAction.send,
                      onSubmitted: (_) => _guiTinNhan(),
                      decoration: const InputDecoration(
                        hintText: 'Nhập tin nhắn...',
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.all(Radius.circular(24)),
                        ),
                        contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  chatVM.dangGuiTinNhan
                      ? const SizedBox(
                          width: 40,
                          height: 40,
                          child: Padding(
                            padding: EdgeInsets.all(10),
                            child: CircularProgressIndicator(strokeWidth: 2),
                          ),
                        )
                      : IconButton(
                          onPressed: _guiTinNhan,
                          icon: const Icon(Icons.send),
                          color: AppColors.primary,
                        ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _TinNhanBubble extends StatelessWidget {
  final ChatMessageModel tinNhan;
  final bool laCuaToi;

  const _TinNhanBubble({
    required this.tinNhan,
    required this.laCuaToi,
  });

  @override
  Widget build(BuildContext context) {
    final gio = AppFormatters.formatDateTime(tinNhan.ngayGui);

    return Align(
      alignment: laCuaToi ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4),
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        constraints: BoxConstraints(
          maxWidth: MediaQuery.of(context).size.width * 0.75,
        ),
        decoration: BoxDecoration(
          color: laCuaToi ? AppColors.primary : AppColors.lightGreenSurface,
          borderRadius: BorderRadius.only(
            topLeft: const Radius.circular(16),
            topRight: const Radius.circular(16),
            bottomLeft: laCuaToi ? const Radius.circular(16) : Radius.zero,
            bottomRight: laCuaToi ? Radius.zero : const Radius.circular(16),
          ),
        ),
        child: Column(
          crossAxisAlignment:
              laCuaToi ? CrossAxisAlignment.end : CrossAxisAlignment.start,
          children: [
            Text(
              tinNhan.noiDung,
              style: TextStyle(
                color: laCuaToi ? Colors.white : AppColors.heading,
                fontSize: 15,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              gio,
              style: TextStyle(
                color: laCuaToi ? Colors.white70 : Colors.black45,
                fontSize: 10,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
