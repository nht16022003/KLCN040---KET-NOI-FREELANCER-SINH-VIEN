import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../core/constants/diem_cuoi_api.dart';
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
  final ImagePicker _picker = ImagePicker();

  int? _maPhongHienTai;
  File? _tepDinhKemChon;
  String? _tenTepChon;
  bool _dangTaiLenTep = false;

  @override
  void initState() {
    super.initState();
    _maPhongHienTai = widget.maPhongChat;
    WidgetsBinding.instance.addPostFrameCallback((_) => _khoiTaoPhong());
  }

  @override
  void dispose() {
    _noiDungController.dispose();
    _scrollController.dispose();
    super.dispose();
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

    int maUserClient = 0;
    int maFreelancerStudent = 0;
    final receiverIdInt = int.tryParse(widget.receiverId ?? '') ?? 0;

    if (authUser.role == 'NhaTuyenDung' || authUser.marole == 2) {
      maUserClient = authUser.userId;
      maFreelancerStudent = widget.maFreelancerStudent ?? receiverIdInt;
    } else {
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

  Future<void> _chonAnh(ImageSource source) async {
    try {
      final picked = await _picker.pickImage(
        source: source,
        imageQuality: 85,
        maxWidth: 1600,
      );
      if (picked != null) {
        setState(() {
          _tepDinhKemChon = File(picked.path);
          _tenTepChon = picked.name;
        });
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Không thể chọn ảnh: $e')),
        );
      }
    }
  }

  void _moLuaChonDinhKem() {
    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) => SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 8),
          child: Wrap(
            children: [
              ListTile(
                leading: const CircleAvatar(
                  backgroundColor: Color(0xFFE8F5E9),
                  child: Icon(Icons.photo_library, color: AppColors.primary),
                ),
                title: const Text('Chọn hình ảnh từ thư viện'),
                onTap: () {
                  Navigator.pop(ctx);
                  _chonAnh(ImageSource.gallery);
                },
              ),
              ListTile(
                leading: const CircleAvatar(
                  backgroundColor: Color(0xFFE8F5E9),
                  child: Icon(Icons.camera_alt, color: AppColors.primary),
                ),
                title: const Text('Chụp ảnh từ máy ảnh'),
                onTap: () {
                  Navigator.pop(ctx);
                  _chonAnh(ImageSource.camera);
                },
              ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _guiTinNhan() async {
    final noiDung = _noiDungController.text.trim();
    if (noiDung.isEmpty && _tepDinhKemChon == null) return;

    final authUser = context.read<AuthViewModel>().currentUser;
    final chatVM = context.read<ChatViewModel>();
    final maPhong = _maPhongHienTai ?? chatVM.phongHienTai?.maPhongChat;

    if (authUser == null || maPhong == null || maPhong <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Chưa kết nối được phòng chat.')),
      );
      return;
    }

    String? duongDanFile;
    if (_tepDinhKemChon != null) {
      setState(() => _dangTaiLenTep = true);
      final uploadRes = await chatVM.uploadFileChat(_tepDinhKemChon!);
      setState(() => _dangTaiLenTep = false);

      if (uploadRes != null && uploadRes['fileUrl'] != null) {
        duongDanFile = uploadRes['fileUrl'].toString();
      } else {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(
                chatVM.thongBaoLoi ?? 'Tải tệp đính kèm thất bại.',
              ),
              backgroundColor: Colors.red[700],
            ),
          );
        }
        return;
      }
    }

    _noiDungController.clear();
    setState(() {
      _tepDinhKemChon = null;
      _tenTepChon = null;
    });

    final thanhCong = await chatVM.guiTinNhan(
      maPhongChat: maPhong,
      maNguoiGui: authUser.userId,
      noiDung: noiDung.isEmpty ? 'Đã gửi một hình ảnh/tệp' : noiDung,
      fileDinhKem: duongDanFile,
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
  Widget build(BuildContext context) {
    final chatVM = context.watch<ChatViewModel>();
    final authUser = context.watch<AuthViewModel>().currentUser;
    final dsTinNhan = chatVM.danhSachTinNhan;
    final dangTai = chatVM.dangTaiTinNhan;

    final avatarUrl = ApiEndpoints.dinhDangUrlAnh(
      widget.avatarUrl,
      tenMacDinh: widget.receiverName ?? 'User',
    );

    return Scaffold(
      backgroundColor: const Color(0xFFF1F5F9),
      appBar: AppBar(
        titleSpacing: 0,
        title: Row(
          children: [
            CircleAvatar(
              radius: 19,
              backgroundColor: const Color(0xFFE2E8F0),
              backgroundImage: NetworkImage(avatarUrl),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    widget.receiverName ?? 'Cuộc trò chuyện',
                    style: const TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                  if (widget.maJob != null && widget.maJob!.isNotEmpty)
                    Text(
                      'Công việc: ${widget.maJob}',
                      style: const TextStyle(
                        fontSize: 11,
                        color: AppColors.mutedText,
                      ),
                      overflow: TextOverflow.ellipsis,
                    )
                  else
                    const Text(
                      'Đang hoạt động',
                      style: TextStyle(
                        fontSize: 11,
                        color: AppColors.primary,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                ],
              ),
            ),
          ],
        ),
        backgroundColor: Colors.white,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
      ),
      body: Column(
        children: [
          // Khu vực tin nhắn
          Expanded(
            child: dangTai && dsTinNhan.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : dsTinNhan.isEmpty
                    ? Center(
                        child: Text(
                          chatVM.thongBaoLoi ??
                              'Chưa có tin nhắn nào. Hãy gửi lời chào!',
                          style: const TextStyle(color: Colors.black54),
                        ),
                      )
                    : ListView.builder(
                        controller: _scrollController,
                        padding: const EdgeInsets.symmetric(
                            horizontal: 16, vertical: 12),
                        itemCount: dsTinNhan.length,
                        itemBuilder: (context, index) {
                          final tin = dsTinNhan[index];
                          final laCuaToi = authUser != null &&
                              tin.maNguoiGui == authUser.userId;
                          return _TinNhanBubble(
                            tinNhan: tin,
                            laCuaToi: laCuaToi,
                          );
                        },
                      ),
          ),

          // Xem trước tệp đính kèm đã chọn
          if (_tepDinhKemChon != null)
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              color: Colors.white,
              child: Row(
                children: [
                  ClipRRect(
                    borderRadius: BorderRadius.circular(6),
                    child: Image.file(
                      _tepDinhKemChon!,
                      width: 44,
                      height: 44,
                      fit: BoxFit.cover,
                      errorBuilder: (ctx, err, stack) => Container(
                        width: 44,
                        height: 44,
                        color: Colors.grey[200],
                        child: const Icon(Icons.attach_file,
                            color: AppColors.primary),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          _tenTepChon ?? 'Ảnh đã chọn',
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: const TextStyle(
                            fontSize: 13,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const Text(
                          'Sẵn sàng gửi',
                          style: TextStyle(
                            fontSize: 11,
                            color: AppColors.primary,
                          ),
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close, size: 20, color: Colors.grey),
                    onPressed: () {
                      setState(() {
                        _tepDinhKemChon = null;
                        _tenTepChon = null;
                      });
                    },
                  ),
                ],
              ),
            ),

          // Khu vực nhập tin nhắn
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
            decoration: BoxDecoration(
              color: Colors.white,
              border: Border(top: BorderSide(color: Colors.grey.shade200)),
            ),
            child: SafeArea(
              child: Row(
                children: [
                  // Nút đính kèm ảnh / file
                  IconButton(
                    icon: const Icon(Icons.attach_file),
                    color: AppColors.primary,
                    onPressed: _moLuaChonDinhKem,
                  ),

                  // Ô nhập text
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
                        contentPadding:
                            EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                      ),
                    ),
                  ),

                  const SizedBox(width: 6),

                  // Nút gửi
                  (_dangTaiLenTep || chatVM.dangGuiTinNhan)
                      ? const SizedBox(
                          width: 40,
                          height: 40,
                          child: Padding(
                            padding: EdgeInsets.all(10),
                            child: CircularProgressIndicator(strokeWidth: 2.5),
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

  bool _laFileAnh(String path) {
    final lower = path.toLowerCase();
    return lower.endsWith('.jpg') ||
        lower.endsWith('.jpeg') ||
        lower.endsWith('.png') ||
        lower.endsWith('.gif') ||
        lower.endsWith('.webp');
  }

  Future<void> _moTepDinhKem(String url) async {
    try {
      final uri = Uri.parse(url);
      if (await canLaunchUrl(uri)) {
        await launchUrl(uri, mode: LaunchMode.externalApplication);
      }
    } catch (_) {}
  }

  @override
  Widget build(BuildContext context) {
    final gio = AppFormatters.formatDateTime(tinNhan.ngayGui);
    final coFile =
        tinNhan.fileDinhKem != null && tinNhan.fileDinhKem!.trim().isNotEmpty;
    final fileUrl =
        coFile ? ApiEndpoints.dinhDangUrlAnh(tinNhan.fileDinhKem) : '';
    final laAnh = coFile &&
        (tinNhan.loaiTinNhan == 'Image' || _laFileAnh(tinNhan.fileDinhKem!));

    return Align(
      alignment: laCuaToi ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4),
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        constraints: BoxConstraints(
          maxWidth: MediaQuery.of(context).size.width * 0.78,
        ),
        decoration: BoxDecoration(
          color: laCuaToi ? AppColors.primary : Colors.white,
          borderRadius: BorderRadius.only(
            topLeft: const Radius.circular(16),
            topRight: const Radius.circular(16),
            bottomLeft: laCuaToi ? const Radius.circular(16) : Radius.zero,
            bottomRight: laCuaToi ? Radius.zero : const Radius.circular(16),
          ),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 4,
              offset: const Offset(0, 1),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment:
              laCuaToi ? CrossAxisAlignment.end : CrossAxisAlignment.start,
          children: [
            // Nội dung tin nhắn
            if (tinNhan.noiDung.isNotEmpty &&
                !(coFile &&
                    tinNhan.noiDung == 'Đã gửi một hình ảnh/tệp' &&
                    laAnh))
              Text(
                tinNhan.noiDung,
                style: TextStyle(
                  color: laCuaToi ? Colors.white : AppColors.heading,
                  fontSize: 14.5,
                  height: 1.35,
                ),
              ),

            // Nếu có tệp đính kèm
            if (coFile) ...[
              const SizedBox(height: 6),
              if (laAnh)
                GestureDetector(
                  onTap: () => _moTepDinhKem(fileUrl),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(8),
                    child: Image.network(
                      fileUrl,
                      width: 220,
                      height: 160,
                      fit: BoxFit.cover,
                      errorBuilder: (ctx, err, stack) => Container(
                        width: 220,
                        height: 100,
                        color: Colors.grey[200],
                        child: const Center(
                          child: Icon(Icons.broken_image, color: Colors.grey),
                        ),
                      ),
                    ),
                  ),
                )
              else
                InkWell(
                  onTap: () => _moTepDinhKem(fileUrl),
                  child: Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                    decoration: BoxDecoration(
                      color: laCuaToi
                          ? Colors.white.withOpacity(0.2)
                          : const Color(0xFFF1F5F9),
                      borderRadius: BorderRadius.circular(8),
                      border: Border.all(
                        color: laCuaToi ? Colors.white30 : Colors.black12,
                      ),
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Icon(
                          Icons.insert_drive_file,
                          size: 20,
                          color: laCuaToi ? Colors.white : AppColors.primary,
                        ),
                        const SizedBox(width: 8),
                        Flexible(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                tinNhan.fileDinhKem!.split('/').last,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: TextStyle(
                                  fontSize: 12.5,
                                  fontWeight: FontWeight.bold,
                                  color: laCuaToi
                                      ? Colors.white
                                      : AppColors.heading,
                                ),
                              ),
                              Text(
                                'Nhấn để mở / tải về',
                                style: TextStyle(
                                  fontSize: 10.5,
                                  color: laCuaToi
                                      ? Colors.white70
                                      : Colors.grey[600],
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
            ],

            const SizedBox(height: 4),

            // Thời gian
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
