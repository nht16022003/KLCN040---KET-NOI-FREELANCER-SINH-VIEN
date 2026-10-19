import 'package:flutter/material.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_bai_dang_tuyen_dung.dart';
import '../../services/dich_vu_cong_viec.dart';
import '../chat/man_hinh_chi_tiet_chat.dart';
import 'bang_ung_tuyen_cong_viec.dart';

/// Chi tiết công việc: dữ liệu thật từ `GET api/JobPost/ChiTietJobPost/{maJob}`.
class JobDetailScreen extends StatefulWidget {
  final JobPostModel job;

  const JobDetailScreen({super.key, required this.job});

  @override
  State<JobDetailScreen> createState() => _JobDetailScreenState();
}

class _JobDetailScreenState extends State<JobDetailScreen> {
  final JobService _jobService = JobService();

  late JobPostModel _job;
  bool _dangTai = false;
  String? _loi;

  @override
  void initState() {
    super.initState();
    _job = widget.job;
    _taiChiTiet();
  }

  Future<void> _taiChiTiet() async {
    final effectiveMaJob = widget.job.maJob.trim().isNotEmpty
        ? widget.job.maJob.trim()
        : widget.job.id.trim();

    if (effectiveMaJob.isEmpty) return;

    setState(() {
      _dangTai = true;
      _loi = null;
    });

    final res = await _jobService.layChiTietJobPost(effectiveMaJob);
    if (!mounted) return;
    setState(() {
      _dangTai = false;
      if (res.isSuccess && res.data != null && res.data!.title.isNotEmpty) {
        // Giữ lại các trường gợi ý nếu có từ danh sách trước đó
        final chiTietMoi = res.data!;
        _job = JobPostModel(
          maJob: chiTietMoi.maJob.isNotEmpty ? chiTietMoi.maJob : widget.job.maJob,
          maNhaTuyenDung: chiTietMoi.maNhaTuyenDung.isNotEmpty
              ? chiTietMoi.maNhaTuyenDung
              : widget.job.maNhaTuyenDung,
          tieude: chiTietMoi.tieude.isNotEmpty ? chiTietMoi.tieude : widget.job.tieude,
          mota: chiTietMoi.mota.isNotEmpty ? chiTietMoi.mota : widget.job.mota,
          kynangyeucau: chiTietMoi.kynangyeucau.isNotEmpty
              ? chiTietMoi.kynangyeucau
              : widget.job.kynangyeucau,
          thulao: chiTietMoi.thulao > 0 ? chiTietMoi.thulao : widget.job.thulao,
          status: chiTietMoi.status.isNotEmpty ? chiTietMoi.status : widget.job.status,
          soluongtuyen: chiTietMoi.soluongtuyen > 0
              ? chiTietMoi.soluongtuyen
              : widget.job.soluongtuyen,
          nganSachMin: chiTietMoi.nganSachMin ?? widget.job.nganSachMin,
          nganSachMax: chiTietMoi.nganSachMax ?? widget.job.nganSachMax,
          tenNhaTuyenDung: chiTietMoi.tenNhaTuyenDung ?? widget.job.tenNhaTuyenDung,
          avatarNhaTuyenDung:
              chiTietMoi.avatarNhaTuyenDung ?? widget.job.avatarNhaTuyenDung,
          ngayHetHan: chiTietMoi.ngayHetHan ?? widget.job.ngayHetHan,
          doPhuHop: widget.job.doPhuHop ?? chiTietMoi.doPhuHop,
          lyDoGoiY: widget.job.lyDoGoiY ?? chiTietMoi.lyDoGoiY,
          kyNangTrungKhop: widget.job.kyNangTrungKhop.isNotEmpty
              ? widget.job.kyNangTrungKhop
              : chiTietMoi.kyNangTrungKhop,
        );
      } else if (!res.isSuccess) {
        _loi = res.message;
      }
    });
  }

  Future<void> _ungTuyen() async {
    final daUngTuyen = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => ApplyJobBottomSheet(job: _job),
    );
    if (daUngTuyen == true && mounted) {
      Navigator.pop(context, true);
    }
  }

  String _thuLao() {
    if (_job.nganSachMin != null && _job.nganSachMax != null) {
      return '${AppFormatters.formatCurrency(_job.nganSachMin!)} - '
          '${AppFormatters.formatCurrency(_job.nganSachMax!)}';
    }
    if (_job.budget > 0) {
      return AppFormatters.formatCurrency(_job.budget);
    }
    return 'Thoả thuận';
  }

  @override
  Widget build(BuildContext context) {
    final statusStr = _job.trangThai.toLowerCase();
    final dangTuyen = statusStr == 'dangtuyen' || statusStr.isEmpty;
    final tenNtd = _job.tenNhaTuyenDung ?? 'Doanh nghiệp tuyển dụng';
    final phanTramPhuHop = _job.phanTramPhuHop;

    final danhSachKyNang = _job.kyNangYeuCau.isNotEmpty
        ? _job.kyNangYeuCau
            .split(RegExp(r'[,;]'))
            .map((s) => s.trim())
            .where((s) => s.isNotEmpty)
            .toList()
        : <String>[];

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text(
          'Chi tiết công việc',
          style: TextStyle(
            fontWeight: FontWeight.bold,
            color: AppColors.heading,
            fontSize: 18,
          ),
        ),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
        bottom: _dangTai
            ? const PreferredSize(
                preferredSize: Size.fromHeight(2),
                child: LinearProgressIndicator(
                  color: AppColors.primary,
                  backgroundColor: AppColors.lightGreenSurface,
                ),
              )
            : null,
      ),
      body: RefreshIndicator(
        color: AppColors.primary,
        onRefresh: _taiChiTiet,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            if (_loi != null)
              Container(
                margin: const EdgeInsets.only(bottom: 12),
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: AppColors.error.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: AppColors.error.withValues(alpha: 0.3)),
                ),
                child: Text(
                  _loi!,
                  style: const TextStyle(color: AppColors.error, fontSize: 13),
                ),
              ),

            // Khối 1: Header Công việc & Nhà tuyển dụng
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(16),
                border: Border.all(color: AppColors.border),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withValues(alpha: 0.04),
                    blurRadius: 8,
                    offset: const Offset(0, 3),
                  ),
                ],
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Tên công việc + Badge phù hợp
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Expanded(
                        child: Text(
                          _job.tieuDe,
                          style: const TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                            color: AppColors.heading,
                            height: 1.3,
                          ),
                        ),
                      ),
                      if (phanTramPhuHop != null) ...[
                        const SizedBox(width: 8),
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 8,
                            vertical: 4,
                          ),
                          decoration: BoxDecoration(
                            color: AppColors.lightGreenSurface,
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(color: AppColors.primaryLight),
                          ),
                          child: Text(
                            '$phanTramPhuHop% phù hợp',
                            style: const TextStyle(
                              fontSize: 11,
                              fontWeight: FontWeight.bold,
                              color: AppColors.primaryDark,
                            ),
                          ),
                        ),
                      ],
                    ],
                  ),
                  const SizedBox(height: 12),

                  // Nhà tuyển dụng
                  Row(
                    children: [
                      CircleAvatar(
                        radius: 20,
                        backgroundColor: AppColors.lightGreenSurface,
                        backgroundImage: _job.avatarNhaTuyenDung != null &&
                                _job.avatarNhaTuyenDung!.trim().isNotEmpty
                            ? NetworkImage(_job.hinhAnhHienThi)
                            : null,
                        child: _job.avatarNhaTuyenDung == null ||
                                _job.avatarNhaTuyenDung!.trim().isEmpty
                            ? Text(
                                tenNtd.isNotEmpty ? tenNtd[0].toUpperCase() : 'C',
                                style: const TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.primary,
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
                              tenNtd,
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.bold,
                                color: AppColors.heading,
                              ),
                            ),
                            const Text(
                              'Nhà tuyển dụng xác thực',
                              style: TextStyle(
                                fontSize: 11,
                                color: AppColors.bodyText,
                              ),
                            ),
                          ],
                        ),
                      ),
                      if (_job.maNhaTuyenDung.isNotEmpty)
                        OutlinedButton(
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (_) => ChatDetailScreen(
                                  receiverId: _job.maNhaTuyenDung,
                                  receiverName: tenNtd,
                                  avatarUrl: _job.hinhAnhHienThi,
                                ),
                              ),
                            );
                          },
                          style: OutlinedButton.styleFrom(
                            foregroundColor: AppColors.primary,
                            side: const BorderSide(color: AppColors.primary),
                            padding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 6,
                            ),
                            textStyle: const TextStyle(fontSize: 12),
                          ),
                          child: const Text('Nhắn tin'),
                        ),
                    ],
                  ),

                  // Khung gợi ý lý do
                  if (_job.lyDoGoiY != null && _job.lyDoGoiY!.trim().isNotEmpty) ...[
                    const SizedBox(height: 12),
                    Container(
                      width: double.infinity,
                      padding: const EdgeInsets.all(10),
                      decoration: BoxDecoration(
                        color: AppColors.lightGreenSurface,
                        borderRadius: BorderRadius.circular(8),
                        border: Border.all(color: AppColors.border),
                      ),
                      child: Text(
                        _job.lyDoGoiY!,
                        style: const TextStyle(
                          fontSize: 12,
                          color: AppColors.primaryDark,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ),
                  ],
                ],
              ),
            ),

            const SizedBox(height: 16),

            // Khối 2: Thù lao & Thông tin cơ bản
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(16),
                border: Border.all(color: AppColors.border),
              ),
              child: Column(
                children: [
                  Row(
                    children: [
                      _TheChiTietItem(
                        tieuDe: 'Thù lao / Ngân sách',
                        giaTri: _thuLao(),
                        noiBat: true,
                      ),
                      const SizedBox(width: 12),
                      _TheChiTietItem(
                        tieuDe: 'Số lượng tuyển',
                        giaTri: '${_job.soLuongTuyen} người',
                      ),
                    ],
                  ),
                  const SizedBox(height: 14),
                  Row(
                    children: [
                      _TheChiTietItem(
                        tieuDe: 'Hạn nộp / Dự kiến',
                        giaTri: _job.ngayHetHan ?? 'Chưa xác định',
                      ),
                      const SizedBox(width: 12),
                      _TheChiTietItem(
                        tieuDe: 'Trạng thái',
                        giaTri: dangTuyen ? 'Đang tuyển' : 'Đã đóng',
                      ),
                    ],
                  ),
                ],
              ),
            ),

            const SizedBox(height: 16),

            // Khối 3: Mô tả công việc
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: AppColors.surface,
                borderRadius: BorderRadius.circular(16),
                border: Border.all(color: AppColors.border),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Mô tả công việc',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppColors.heading,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    _job.moTa.isNotEmpty
                        ? _job.moTa
                        : 'Chưa có mô tả chi tiết cho công việc này.',
                    style: const TextStyle(
                      fontSize: 14,
                      height: 1.6,
                      color: AppColors.bodyText,
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(height: 16),

            // Khối 4: Kỹ năng yêu cầu
            if (danhSachKyNang.isNotEmpty)
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: AppColors.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Kỹ năng yêu cầu',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: AppColors.heading,
                      ),
                    ),
                    const SizedBox(height: 10),
                    Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: danhSachKyNang.map((kn) {
                        final daKhop = _job.kyNangTrungKhop.contains(kn);
                        return Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 10,
                            vertical: 5,
                          ),
                          decoration: BoxDecoration(
                            color: daKhop
                                ? AppColors.lightGreenSurface
                                : AppColors.surface,
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(
                              color: daKhop
                                  ? AppColors.primaryLight
                                  : AppColors.border,
                            ),
                          ),
                          child: Text(
                            kn,
                            style: TextStyle(
                              fontSize: 12,
                              fontWeight:
                                  daKhop ? FontWeight.bold : FontWeight.w500,
                              color: daKhop
                                  ? AppColors.primaryDark
                                  : AppColors.bodyText,
                            ),
                          ),
                        );
                      }).toList(),
                    ),
                  ],
                ),
              ),

            const SizedBox(height: 80),
          ],
        ),
      ),
      bottomNavigationBar: SafeArea(
        child: Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.surface,
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.05),
                blurRadius: 8,
                offset: const Offset(0, -2),
              ),
            ],
          ),
          child: SizedBox(
            height: 48,
            child: ElevatedButton(
              onPressed: dangTuyen ? _ungTuyen : null,
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.primary,
                foregroundColor: Colors.white,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                elevation: 0,
              ),
              child: Text(
                dangTuyen ? 'Ứng tuyển công việc này' : 'Công việc đã đóng tuyển',
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 15,
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _TheChiTietItem extends StatelessWidget {
  final String tieuDe;
  final String giaTri;
  final bool noiBat;

  const _TheChiTietItem({
    required this.tieuDe,
    required this.giaTri,
    this.noiBat = false,
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: noiBat ? AppColors.lightGreenSurface : AppColors.surface,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(
            color: AppColors.border,
          ),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              tieuDe,
              style: TextStyle(
                fontSize: 11,
                color: noiBat ? AppColors.primary : AppColors.bodyText,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              giaTri,
              style: TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.bold,
                color: noiBat ? AppColors.primaryDark : AppColors.heading,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
