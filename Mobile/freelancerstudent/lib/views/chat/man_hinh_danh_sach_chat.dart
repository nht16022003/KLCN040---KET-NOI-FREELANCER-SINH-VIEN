import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../../viewmodels/viewmodel_chat.dart';
import 'man_hinh_chi_tiet_chat.dart';

class ChatListScreen extends StatefulWidget {
  const ChatListScreen({super.key});

  @override
  State<ChatListScreen> createState() => _ChatListScreenState();
}

class _ChatListScreenState extends State<ChatListScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _taiDanhSach());
  }

  Future<void> _taiDanhSach() async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      await context.read<ChatViewModel>().taiDanhSachPhong(user.userId);
    }
  }

  @override
  Widget build(BuildContext context) {
    final user = context.watch<AuthViewModel>().currentUser;
    final chatVM = context.watch<ChatViewModel>();

    if (user == null) {
      return Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          title: const Text('Tin nhắn'),
          backgroundColor: AppColors.surface,
          foregroundColor: AppColors.heading,
          elevation: 0.5,
        ),
        body: const Center(
          child: Text('Vui lòng đăng nhập để xem tin nhắn.'),
        ),
      );
    }

    final dsPhong = chatVM.danhSachPhong;
    final dangTai = chatVM.dangTaiPhong;
    final loi = chatVM.thongBaoLoi;

    Widget body;
    if (dangTai && dsPhong.isEmpty) {
      body = const Center(child: CircularProgressIndicator());
    } else if (loi != null && dsPhong.isEmpty) {
      body = Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(loi, textAlign: TextAlign.center),
              const SizedBox(height: 12),
              FilledButton(
                onPressed: _taiDanhSach,
                child: const Text('Thử lại'),
              ),
            ],
          ),
        ),
      );
    } else if (dsPhong.isEmpty) {
      body = ListView(
        children: const [
          SizedBox(height: 120),
          Center(
            child: Text(
              'Chưa có cuộc trò chuyện nào.',
              style: TextStyle(color: Colors.black54),
            ),
          ),
        ],
      );
    } else {
      body = ListView.separated(
        padding: const EdgeInsets.symmetric(vertical: 8),
        itemCount: dsPhong.length,
        separatorBuilder: (_, _) => const Divider(height: 1, indent: 76),
        itemBuilder: (context, index) {
          final phong = dsPhong[index];
          final tenDoiPhuong = phong.tenDoiPhuong(user.userId);
          final avatarUrl = phong.avatarDoiPhuong(user.userId);
          final thoiGian = phong.thoiGianTinNhanCuoi != null
              ? AppFormatters.timeAgo(phong.thoiGianTinNhanCuoi!)
              : '';

          return ListTile(
            leading: CircleAvatar(
              radius: 24,
              backgroundColor: AppColors.lightGreenSurface,
              backgroundImage: NetworkImage(avatarUrl),
              onBackgroundImageError: (_, _) {},
              child: avatarUrl.isEmpty
                  ? Text(
                      tenDoiPhuong.isNotEmpty ? tenDoiPhuong[0].toUpperCase() : 'U',
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        color: AppColors.primary,
                      ),
                    )
                  : null,
            ),
            title: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    tenDoiPhuong,
                    style: TextStyle(
                      fontWeight: phong.soTinNhanChuaDoc > 0
                          ? FontWeight.bold
                          : FontWeight.w600,
                      fontSize: 16,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
                if (thoiGian.isNotEmpty)
                  Text(
                    thoiGian,
                    style: const TextStyle(fontSize: 12, color: AppColors.bodyText),
                  ),
              ],
            ),
            subtitle: Row(
              children: [
                Expanded(
                  child: Text(
                    phong.tinNhanCuoi?.isNotEmpty == true
                        ? phong.tinNhanCuoi!
                        : (phong.tieuDeJob != null
                            ? 'Trao đổi về: ${phong.tieuDeJob}'
                            : 'Bắt đầu cuộc trò chuyện'),
                    style: TextStyle(
                      color: phong.soTinNhanChuaDoc > 0
                          ? AppColors.heading
                          : AppColors.bodyText,
                      fontWeight: phong.soTinNhanChuaDoc > 0
                          ? FontWeight.w500
                          : FontWeight.normal,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
                if (phong.soTinNhanChuaDoc > 0)
                  Container(
                    margin: const EdgeInsets.only(left: 8),
                    padding: const EdgeInsets.symmetric(horizontal: 7, vertical: 2),
                    decoration: BoxDecoration(
                      color: AppColors.primary,
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: Text(
                      '${phong.soTinNhanChuaDoc}',
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 11,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
              ],
            ),
            onTap: () async {
              await Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => ChatDetailScreen(
                    maPhongChat: phong.maPhongChat,
                    receiverName: tenDoiPhuong,
                    avatarUrl: avatarUrl,
                    maJob: phong.maJob,
                  ),
                ),
              );
              if (mounted) _taiDanhSach();
            },
          );
        },
      );
    }

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Tin nhắn'),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: RefreshIndicator(
        onRefresh: _taiDanhSach,
        child: body,
      ),
    );
  }
}
