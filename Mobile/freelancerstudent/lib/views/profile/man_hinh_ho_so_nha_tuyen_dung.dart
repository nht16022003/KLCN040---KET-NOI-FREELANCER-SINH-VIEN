import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_danh_gia.dart';
import '../../models/mo_hinh_nha_tuyen_dung.dart';
import '../../services/dich_vu_danh_gia.dart';
import '../../services/dich_vu_nha_tuyen_dung.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../chat/man_hinh_chi_tiet_chat.dart';
import '../danh_gia/man_hinh_danh_gia.dart';
import '../danh_gia/man_hinh_danh_sach_tat_ca_danh_gia.dart';
import '../job_detail/man_hinh_chi_tiet_cong_viec.dart';

class NhaTuyenDungProfileScreen extends StatefulWidget {
  final int? maNhaTuyenDung;
  final int? maUser;
  final NhaTuyenDungModel? initialData;

  const NhaTuyenDungProfileScreen({
    super.key,
    this.maNhaTuyenDung,
    this.maUser,
    this.initialData,
  });

  @override
  State<NhaTuyenDungProfileScreen> createState() =>
      _NhaTuyenDungProfileScreenState();
}

class _NhaTuyenDungProfileScreenState extends State<NhaTuyenDungProfileScreen> {
  final NhaTuyenDungService _service = NhaTuyenDungService();
  final DanhGiaService _danhGiaService = DanhGiaService();

  bool _dangTai = true;
  String? _loi;
  NhaTuyenDungProfileModel? _profile;
  ThongKeDanhGiaModel? _thongKeDanhGia;

  @override
  void initState() {
    super.initState();
    _taiDuLieu();
  }

  Future<void> _taiDuLieu() async {
    setState(() {
      _dangTai = true;
      _loi = null;
    });

    final targetMaNTD =
        widget.maNhaTuyenDung ?? widget.initialData?.maNhaTuyenDung ?? 0;
    final targetMaUser = widget.maUser ?? widget.initialData?.maUser ?? 0;

    if (targetMaNTD > 0) {
      final res = await _service.layProfileNhaTuyenDung(targetMaNTD);
      if (res.isSuccess && res.data != null) {
        _profile = res.data;
      } else {
        _loi = res.message;
      }
    } else if (targetMaUser > 0) {
      final res = await _service.layProfileNhaTuyenDungTheoUser(targetMaUser);
      if (res.isSuccess && res.data != null) {
        _profile = res.data;
      } else {
        _loi = res.message;
      }
    } else if (widget.initialData != null) {
      _profile = NhaTuyenDungProfileModel(nhaTuyenDung: widget.initialData!);
    }

    final resolvedMaNTD = _profile?.nhaTuyenDung.maNhaTuyenDung ?? targetMaNTD;
    if (resolvedMaNTD > 0) {
      final dgRes = await _danhGiaService.layDanhGiaNhaTuyenDung(
        maNhaTuyenDung: resolvedMaNTD,
        trang: 1,
        kichThuocTrang: 10,
      );
      if (dgRes.isSuccess) {
        _thongKeDanhGia = dgRes.data;
      }
    }

    if (mounted) {
      setState(() {
        _dangTai = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final currentUser = context.watch<AuthViewModel>().currentUser;
    final ntd = _profile?.nhaTuyenDung ?? widget.initialData;

    if (_dangTai && ntd == null) {
      return Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(title: const Text('Hồ sơ Nhà tuyển dụng')),
        body: const Center(child: CircularProgressIndicator()),
      );
    }

    if (ntd == null) {
      return Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(title: const Text('Hồ sơ Nhà tuyển dụng')),
        body: Center(
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  _loi ?? 'Không tìm thấy thông tin Nhà tuyển dụng.',
                  textAlign: TextAlign.center,
                  style: const TextStyle(color: AppColors.error),
                ),
                const SizedBox(height: 12),
                FilledButton(
                  onPressed: _taiDuLieu,
                  child: const Text('Thử lại'),
                ),
              ],
            ),
          ),
        ),
      );
    }

    final isOwner = currentUser != null &&
        ntd.maUser > 0 &&
        currentUser.userId == ntd.maUser;

    final diemTB = _thongKeDanhGia?.diemTrungBinh ?? ntd.diemDanhGia;
    final soLuotDG =
        _thongKeDanhGia?.tongLuotDanhGia ?? (_profile?.soLuotDanhGia ?? 0);
    final jobs = _profile?.danhSachTinTuyenDung ?? const <NhaTuyenDungJobItemModel>[];

    return DefaultTabController(
      length: 3,
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          title: Text(
            isOwner ? 'Hồ sơ Doanh nghiệp của tôi' : 'Hồ sơ Nhà tuyển dụng',
          ),
          backgroundColor: AppColors.surface,
          foregroundColor: AppColors.heading,
          elevation: 0.5,
          actions: [
            if (!isOwner && ntd.maUser > 0)
              IconButton(
                tooltip: 'Nhắn tin liên hệ',
                icon: const Icon(Icons.chat_bubble_outline, color: AppColors.primary),
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => ChatDetailScreen(
                        receiverId: ntd.maUser.toString(),
                        receiverName: ntd.tenHienThi,
                        avatarUrl: ntd.hinhAnhHienThi,
                      ),
                    ),
                  );
                },
              ),
          ],
          bottom: TabBar(
            indicatorColor: AppColors.primary,
            labelColor: AppColors.primary,
            unselectedLabelColor: AppColors.bodyText,
            tabs: [
              const Tab(text: 'Tổng quan'),
              Tab(text: 'Tin tuyển dụng (${jobs.length})'),
              Tab(text: 'Đánh giá ($soLuotDG)'),
            ],
          ),
        ),
        body: RefreshIndicator(
          onRefresh: _taiDuLieu,
          child: TabBarView(
            children: [
              _buildOverviewTab(context, ntd, isOwner, diemTB, soLuotDG),
              _buildJobPostsTab(context, jobs),
              _buildReviewsTab(context, ntd, isOwner),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildOverviewTab(
    BuildContext context,
    NhaTuyenDungModel ntd,
    bool isOwner,
    double diemTB,
    int soLuotDG,
  ) {
    final p = _profile;
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // Banner & Thông tin định danh công ty
        Container(
          decoration: BoxDecoration(
            color: AppColors.surface,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: AppColors.border),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                height: 52,
                decoration: const BoxDecoration(
                  color: AppColors.lightGreenSurface,
                  borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Transform.translate(
                      offset: const Offset(0, -24),
                      child: Row(
                        crossAxisAlignment: CrossAxisAlignment.end,
                        children: [
                          Container(
                            width: 72,
                            height: 72,
                            decoration: BoxDecoration(
                              color: Colors.white,
                              borderRadius: BorderRadius.circular(14),
                              border: Border.all(color: Colors.white, width: 3),
                              boxShadow: const [
                                BoxShadow(
                                  color: Colors.black12,
                                  blurRadius: 6,
                                  offset: Offset(0, 2),
                                ),
                              ],
                            ),
                            child: ClipRRect(
                              borderRadius: BorderRadius.circular(11),
                              child: Image.network(
                                ntd.hinhAnhHienThi,
                                fit: BoxFit.cover,
                                errorBuilder: (_, _, _) => Center(
                                  child: Text(
                                    ntd.tenHienThi.isNotEmpty
                                        ? ntd.tenHienThi[0].toUpperCase()
                                        : 'N',
                                    style: const TextStyle(
                                      fontSize: 28,
                                      fontWeight: FontWeight.bold,
                                      color: AppColors.primary,
                                    ),
                                  ),
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Padding(
                              padding: const EdgeInsets.only(top: 26),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    ntd.tenHienThi,
                                    style: const TextStyle(
                                      fontSize: 18,
                                      fontWeight: FontWeight.bold,
                                      color: AppColors.heading,
                                    ),
                                  ),
                                  const SizedBox(height: 4),
                                  Container(
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 8,
                                      vertical: 3,
                                    ),
                                    decoration: BoxDecoration(
                                      color: AppColors.lightGreenSurface,
                                      borderRadius: BorderRadius.circular(6),
                                    ),
                                    child: Text(
                                      ntd.linhVucHienThi,
                                      style: const TextStyle(
                                        fontSize: 12,
                                        fontWeight: FontWeight.w600,
                                        color: AppColors.primary,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    Wrap(
                      spacing: 14,
                      runSpacing: 6,
                      children: [
                        Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            const Icon(Icons.location_on_outlined,
                                size: 15, color: AppColors.mutedText),
                            const SizedBox(width: 4),
                            Text(
                              ntd.diaChiHienThi,
                              style: const TextStyle(
                                  fontSize: 12, color: AppColors.bodyText),
                            ),
                          ],
                        ),
                        if (ntd.ngayDangKy != null)
                          Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              const Icon(Icons.calendar_today_outlined,
                                  size: 14, color: AppColors.mutedText),
                              const SizedBox(width: 4),
                              Text(
                                'Tham gia: ${DateFormat('MM/yyyy').format(ntd.ngayDangKy!)}',
                                style: const TextStyle(
                                    fontSize: 12, color: AppColors.bodyText),
                              ),
                            ],
                          ),
                      ],
                    ),
                    if (!isOwner && ntd.maUser > 0) ...[
                      const SizedBox(height: 14),
                      SizedBox(
                        width: double.infinity,
                        child: FilledButton.icon(
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (_) => ChatDetailScreen(
                                  receiverId: ntd.maUser.toString(),
                                  receiverName: ntd.tenHienThi,
                                  avatarUrl: ntd.hinhAnhHienThi,
                                ),
                              ),
                            );
                          },
                          icon: const Icon(Icons.chat_bubble_outline, size: 18),
                          label: const Text('Nhắn tin với Nhà tuyển dụng'),
                          style: FilledButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            padding: const EdgeInsets.symmetric(vertical: 12),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10),
                            ),
                          ),
                        ),
                      ),
                    ],
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),

        // Strip 6 chỉ số thống kê hoạt động (đồng bộ Web Profile.cshtml)
        GridView.count(
          crossAxisCount: 3,
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          mainAxisSpacing: 8,
          crossAxisSpacing: 8,
          childAspectRatio: 1.35,
          children: [
            _buildMetricBox(
              '${diemTB.toStringAsFixed(1)} ★',
              'Đánh giá ($soLuotDG)',
              color: Colors.amber.shade800,
            ),
            _buildMetricBox(
              '${(p?.tyLeHaiLong ?? 100).toStringAsFixed(0)}%',
              'Tỷ lệ hài lòng',
              color: AppColors.primary,
            ),
            _buildMetricBox(
              '${p?.tongTinTuyenDung ?? 0}',
              'Tổng tin đăng',
            ),
            _buildMetricBox(
              '${p?.tinDangTuyen ?? 0}',
              'Đang mở tuyển',
              color: AppColors.primary,
            ),
            _buildMetricBox(
              '${p?.soHopDongHoanThanh ?? 0}',
              'HĐ hoàn thành',
            ),
            _buildMetricBox(
              '${p?.soFreelancerHopTac ?? 0}',
              'SV đã hợp tác',
            ),
          ],
        ),
        const SizedBox(height: 16),

        // Giới thiệu doanh nghiệp
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.surface,
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: AppColors.border),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'Giới thiệu về Nhà tuyển dụng',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 10),
              Text(
                ntd.gioiThieuHienThi,
                style: const TextStyle(
                  fontSize: 13.5,
                  height: 1.55,
                  color: AppColors.bodyText,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),

        // Thông tin liên hệ
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.surface,
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: AppColors.border),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'Thông tin doanh nghiệp & Liên hệ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 12),
              _infoRow('Người đại diện', ntd.nguoiDaiDien),
              _infoRow('Lĩnh vực', ntd.linhVucHienThi),
              if (ntd.emailUser != null && ntd.emailUser!.isNotEmpty)
                _infoRow('Email', ntd.emailUser!),
              if (ntd.sdtUser != null && ntd.sdtUser!.isNotEmpty)
                _infoRow('Số điện thoại', ntd.sdtUser!),
              _infoRow('Địa chỉ', ntd.diaChiHienThi),
              if (ntd.link != null && ntd.link!.isNotEmpty)
                _infoRow('Website', ntd.link!),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildMetricBox(String value, String label, {Color? color}) {
    return Container(
      padding: const EdgeInsets.all(8),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Text(
            value,
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              color: color ?? AppColors.heading,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            label,
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(
              fontSize: 11,
              color: AppColors.mutedText,
            ),
          ),
        ],
      ),
    );
  }

  Widget _infoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 115,
            child: Text(
              label,
              style: const TextStyle(
                fontSize: 13,
                color: AppColors.mutedText,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w600,
                color: AppColors.heading,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildJobPostsTab(
    BuildContext context,
    List<NhaTuyenDungJobItemModel> jobs,
  ) {
    if (jobs.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(24),
          child: Text(
            'Nhà tuyển dụng này chưa có tin tuyển dụng nào.',
            style: TextStyle(color: AppColors.mutedText),
          ),
        ),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: jobs.length,
      separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final job = jobs[index];
        final isOpen = job.status.toLowerCase().contains('tuyển');

        return InkWell(
          borderRadius: BorderRadius.circular(14),
          onTap: () {
            if (job.maJob.isNotEmpty) {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => JobDetailScreen(maJob: job.maJob),
                ),
              );
            }
          },
          child: Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Text(
                        job.tieude,
                        style: const TextStyle(
                          fontSize: 15.5,
                          fontWeight: FontWeight.bold,
                          color: AppColors.heading,
                        ),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 8,
                        vertical: 3,
                      ),
                      decoration: BoxDecoration(
                        color: isOpen
                            ? AppColors.lightGreenSurface
                            : Colors.grey.shade200,
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        job.status,
                        style: TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.w600,
                          color: isOpen
                              ? AppColors.primary
                              : Colors.grey.shade700,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                Text(
                  'Thù lao: ${AppFormatters.formatCurrency(job.thulao)}',
                  style: const TextStyle(
                    fontSize: 13.5,
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
                if (job.mota != null && job.mota!.trim().isNotEmpty) ...[
                  const SizedBox(height: 6),
                  Text(
                    job.mota!,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontSize: 12.5,
                      color: AppColors.bodyText,
                    ),
                  ),
                ],
                if (job.danhSachKyNang.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 6,
                    runSpacing: 4,
                    children: job.danhSachKyNang
                        .take(4)
                        .map(
                          (k) => Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 8,
                              vertical: 2,
                            ),
                            decoration: BoxDecoration(
                              color: AppColors.background,
                              borderRadius: BorderRadius.circular(4),
                              border: Border.all(color: AppColors.border),
                            ),
                            child: Text(
                              k,
                              style: const TextStyle(
                                fontSize: 11,
                                color: AppColors.bodyText,
                              ),
                            ),
                          ),
                        )
                        .toList(),
                  ),
                ],
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildReviewsTab(
    BuildContext context,
    NhaTuyenDungModel ntd,
    bool isOwner,
  ) {
    final stats = _thongKeDanhGia;
    final total = stats?.tongLuotDanhGia ?? 0;

    if (total == 0) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.star_outline, size: 52, color: AppColors.mutedText),
              const SizedBox(height: 12),
              const Text(
                'Chưa có đánh giá nào từ Freelancer.',
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w600,
                  color: AppColors.heading,
                ),
              ),
              if (isOwner) ...[
                const SizedBox(height: 16),
                FilledButton.icon(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => const DanhGiaListScreen(),
                      ),
                    );
                  },
                  icon: const Icon(Icons.rate_review_outlined),
                  label: const Text('Công việc đã thuê & Đánh giá Freelancer'),
                  style: FilledButton.styleFrom(backgroundColor: AppColors.primary),
                ),
              ],
            ],
          ),
        ),
      );
    }

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        if (isOwner)
          Container(
            margin: const EdgeInsets.only(bottom: 16),
            width: double.infinity,
            child: FilledButton.icon(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) => const DanhGiaListScreen(),
                  ),
                );
              },
              icon: const Icon(Icons.rate_review_outlined),
              label: const Text('Công việc đã thuê & Đánh giá Freelancer'),
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                padding: const EdgeInsets.symmetric(vertical: 12),
              ),
            ),
          ),
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.surface,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: AppColors.border),
          ),
          child: Row(
            children: [
              Text(
                stats!.diemTrungBinh.toStringAsFixed(1),
                style: const TextStyle(
                  fontSize: 36,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: List.generate(
                        5,
                        (index) => Icon(
                          index < stats.diemTrungBinh.round()
                              ? Icons.star
                              : Icons.star_border,
                          size: 18,
                          color: Colors.amber,
                        ),
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '$total lượt đánh giá từ Freelancer',
                      style: const TextStyle(
                        fontSize: 12.5,
                        color: AppColors.mutedText,
                      ),
                    ),
                  ],
                ),
              ),
              if (total > stats.danhSach.length)
                TextButton(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => ManHinhDanhSachTatCaDanhGia(
                          maNhaTuyenDung: ntd.maNhaTuyenDung,
                          tenDoiTuong: ntd.tenHienThi,
                          laNhaTuyenDung: true,
                        ),
                      ),
                    );
                  },
                  child: const Text('Xem tất cả'),
                ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        ...stats.danhSach.map(
          (item) => Container(
            margin: const EdgeInsets.only(bottom: 12),
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(10),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Expanded(
                      child: Text(
                        item.tenNguoiDanhGia,
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          color: AppColors.heading,
                        ),
                      ),
                    ),
                    Row(
                      children: [
                        const Icon(Icons.star, size: 15, color: Colors.amber),
                        const SizedBox(width: 2),
                        Text(
                          '${item.soSaoTongThe}',
                          style: const TextStyle(fontWeight: FontWeight.bold),
                        ),
                      ],
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  item.tenCongViec,
                  style: const TextStyle(
                    fontSize: 12,
                    color: AppColors.mutedText,
                  ),
                ),
                if ((item.nhanXet ?? '').trim().isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Text(
                    item.nhanXet!,
                    style: const TextStyle(
                      fontSize: 13,
                      color: AppColors.bodyText,
                    ),
                  ),
                ],
              ],
            ),
          ),
        ),
      ],
    );
  }
}
