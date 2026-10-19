import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/diem_cuoi_api.dart';
import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_ung_tuyen.dart';
import '../../services/dich_vu_ung_tuyen.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import '../employer/man_hinh_tao_hop_dong.dart';
import '../hop_dong/man_hinh_chi_tiet_hop_dong.dart';
import '../profile/man_hinh_ho_so.dart';

class ApplicantListScreen extends StatefulWidget {
  final String? maJob;
  final String? tieuDeJob;

  const ApplicantListScreen({super.key, this.maJob, this.tieuDeJob});

  @override
  State<ApplicantListScreen> createState() => _ApplicantListScreenState();
}

class _ApplicantListScreenState extends State<ApplicantListScreen> {
  final ApplicationService _service = ApplicationService();
  bool _isLoading = false;
  String? _errorMessage;
  List<ApplicationModel> _applicants = <ApplicationModel>[];
  final Set<String> _updatingIds = <String>{};

  String _nhanTrangThaiHopDong(String? tt) {
    switch (tt) {
      case 'ChoXacNhan':
        return 'Chờ xác nhận';
      case 'ChoKyQuy':
        return 'Chờ ký quỹ';
      case 'DangThucHien':
        return 'Đang thực hiện';
      case 'HoanThanh':
        return 'Hoàn thành';
      case 'DaHuy':
        return 'Đã hủy';
      default:
        return tt ?? 'Đã tạo HĐ';
    }
  }

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadApplicants();
    });
  }

  Future<void> _loadApplicants() async {
    final userId = context.read<AuthViewModel>().currentUser?.userId;
    if (userId == null || userId <= 0) {
      setState(() {
        _errorMessage = 'Vui lòng đăng nhập lại để xem danh sách ứng viên.';
      });
      return;
    }

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    final response = await _service.getMyApplications(userId: userId);

    if (!mounted) return;

    setState(() {
      _isLoading = false;
      if (response.isSuccess) {
        var list = response.data ?? <ApplicationModel>[];
        if (widget.maJob != null && widget.maJob!.isNotEmpty) {
          list = list.where((a) => a.jobId == widget.maJob || a.maJob == widget.maJob).toList();
        }
        _applicants = list;
        if (_applicants.isEmpty) {
          _errorMessage = response.message.isNotEmpty
              ? response.message
              : (widget.maJob != null
                  ? 'Chưa có ứng viên nào ứng tuyển cho công việc này.'
                  : 'Chưa có ứng viên nào ứng tuyển.');
        }
      } else {
        _errorMessage = response.message;
      }
    });
  }

  String _statusLabel(String status) {
    final value = status.trim();
    if (value.isEmpty) return 'Chờ duyệt';
    final lower = value.toLowerCase();
    if (lower == 'chapnhan' ||
        lower.contains('approved') ||
        lower.contains('accept')) {
      return 'Phê duyệt';
    }
    if (lower == 'tuchoi' ||
        lower.contains('reject') ||
        lower.contains('denied')) {
      return 'Từ chối';
    }
    return 'Chờ duyệt';
  }

  bool _isPending(ApplicationModel applicant) {
    final status = _statusLabel(applicant.status);
    return status == 'Chờ duyệt';
  }

  Future<void> _updateStatus(
    ApplicationModel applicant,
    String nextStatus,
  ) async {
    if (_updatingIds.contains(applicant.id)) return;
    setState(() => _updatingIds.add(applicant.id));

    final response = await _service.updateApplicationStatus(
      applicant.id,
      nextStatus,
    );

    if (!mounted) return;
    setState(() => _updatingIds.remove(applicant.id));

    if (response.isSuccess) {
      final index = _applicants.indexWhere((item) => item.id == applicant.id);
      if (index == -1) return;
      setState(() {
        _applicants[index] = applicant.copyWith(status: nextStatus);
      });

      final label = nextStatus == 'ChapNhan'
          ? 'đã được phê duyệt'
          : 'bị từ chối';
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Đã gửi thông báo cho freelancer: $label.')),
      );
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          response.message.isEmpty
              ? 'Không thể cập nhật trạng thái ứng viên.'
              : response.message,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Text(
          widget.tieuDeJob != null
              ? 'Ứng viên: ${widget.tieuDeJob}'
              : 'Danh sách ứng viên',
          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 17),
        ),
        backgroundColor: AppColors.surface,
        foregroundColor: AppColors.heading,
        elevation: 0.5,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadApplicants,
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: _loadApplicants,
        child: Builder(
          builder: (_) {
            if (_isLoading && _applicants.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }

            if (_errorMessage != null && _applicants.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Text(
                    _errorMessage!,
                    textAlign: TextAlign.center,
                    style: const TextStyle(color: AppColors.error),
                  ),
                ),
              );
            }

            return ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: _applicants.length,
              itemBuilder: (context, index) {
                final applicant = _applicants[index];
                final isPending = _isPending(applicant);
                final statusText = _statusLabel(applicant.status);
                final freelancerId = int.tryParse(applicant.applicantId);

                return Card(
                  margin: const EdgeInsets.only(bottom: 12),
                  child: Padding(
                    padding: const EdgeInsets.all(14),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            InkWell(
                              borderRadius: BorderRadius.circular(28),
                              onTap: freelancerId == null || freelancerId <= 0
                                  ? null
                                  : () => Navigator.push(
                                      context,
                                      MaterialPageRoute(
                                        builder: (_) => ProfileScreen(
                                          freelancerId: freelancerId,
                                          readOnly: true,
                                        ),
                                      ),
                                    ),
                              child: CircleAvatar(
                                backgroundColor: AppColors.lightGreenSurface,
                                backgroundImage:
                                    applicant.avatar?.trim().isNotEmpty == true
                                    ? NetworkImage(
                                        ApiEndpoints.dinhDangUrlAnh(
                                          applicant.avatar,
                                          tenMacDinh:
                                              applicant.tenUser ?? 'Freelancer',
                                        ),
                                      )
                                    : null,
                                child:
                                    applicant.avatar?.trim().isNotEmpty == true
                                    ? null
                                    : const Icon(
                                        Icons.person,
                                        color: AppColors.bodyText,
                                      ),
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: InkWell(
                                onTap: freelancerId == null || freelancerId <= 0
                                    ? null
                                    : () => Navigator.push(
                                        context,
                                        MaterialPageRoute(
                                          builder: (_) => ProfileScreen(
                                            freelancerId: freelancerId,
                                            readOnly: true,
                                          ),
                                        ),
                                      ),
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(
                                      applicant.tenUser?.trim().isNotEmpty ==
                                              true
                                          ? applicant.tenUser!
                                          : 'Freelancer #${applicant.applicantId}',
                                      style: const TextStyle(
                                        fontWeight: FontWeight.bold,
                                        fontSize: 17,
                                        color: AppColors.heading,
                                      ),
                                    ),
                                    const SizedBox(height: 4),
                                    Text(
                                      statusText,
                                      style: TextStyle(
                                        color: statusText == 'Phê duyệt'
                                            ? AppColors.success
                                            : statusText == 'Từ chối'
                                            ? AppColors.error
                                            : AppColors.warning,
                                        fontWeight: FontWeight.w600,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 12),
                        Text(
                          applicant.tieude.isEmpty
                              ? 'Mã công việc: ${applicant.jobId}'
                              : 'Công việc: ${applicant.tieude}',
                          style: const TextStyle(fontWeight: FontWeight.w600, color: AppColors.heading),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          applicant.coverLetter.isNotEmpty
                              ? applicant.coverLetter
                              : 'Không có lời giới thiệu.',
                          style: const TextStyle(
                            color: AppColors.bodyText,
                            height: 1.5,
                          ),
                        ),
                        const SizedBox(height: 12),
                        if (isPending)
                          Row(
                            children: [
                              Expanded(
                                child: ElevatedButton.icon(
                                  onPressed: _updatingIds.contains(applicant.id)
                                      ? null
                                      : () => _updateStatus(
                                          applicant,
                                          'ChapNhan',
                                        ),
                                  icon: const Icon(Icons.check),
                                  label: const Text('Phê duyệt'),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: AppColors.primary,
                                    foregroundColor: Colors.white,
                                  ),
                                ),
                              ),
                              const SizedBox(width: 8),
                              Expanded(
                                child: ElevatedButton.icon(
                                  onPressed: _updatingIds.contains(applicant.id)
                                      ? null
                                      : () =>
                                            _updateStatus(applicant, 'TuChoi'),
                                  icon: const Icon(Icons.close),
                                  label: const Text('Từ chối'),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: AppColors.error,
                                    foregroundColor: Colors.white,
                                  ),
                                ),
                              ),
                            ],
                          )
                        else
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Chip(
                                label: Text(
                                  statusText,
                                  style: TextStyle(
                                    color: statusText == 'Phê duyệt'
                                        ? AppColors.success
                                        : AppColors.error,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                                backgroundColor: statusText == 'Phê duyệt'
                                    ? AppColors.lightGreenSurface
                                    : AppColors.error.withValues(alpha: 0.1),
                                side: BorderSide(
                                  color: statusText == 'Phê duyệt'
                                      ? AppColors.border
                                      : AppColors.error.withValues(alpha: 0.3),
                                ),
                              ),
                              if (statusText == 'Phê duyệt')
                                if (applicant.daCoHopDong)
                                  Row(
                                    mainAxisSize: MainAxisSize.min,
                                    children: [
                                      Chip(
                                        avatar: const Icon(Icons.assignment_turned_in, size: 16, color: AppColors.primary),
                                        label: Text(
                                          _nhanTrangThaiHopDong(applicant.trangThaiHopDong),
                                          style: const TextStyle(
                                            color: AppColors.primaryDark,
                                            fontWeight: FontWeight.w600,
                                            fontSize: 12,
                                          ),
                                        ),
                                        backgroundColor: AppColors.lightGreenSurface,
                                        side: const BorderSide(color: AppColors.border),
                                      ),
                                      if ((applicant.maHopDong ?? '').isNotEmpty) ...[
                                        const SizedBox(width: 4),
                                        IconButton(
                                          tooltip: 'Xem hợp đồng',
                                          icon: const Icon(Icons.arrow_forward_ios, size: 14, color: AppColors.primary),
                                          onPressed: () async {
                                            await Navigator.push(
                                              context,
                                              MaterialPageRoute(
                                                builder: (_) => HopDongDetailScreen(maHD: applicant.maHopDong!),
                                              ),
                                            );
                                            if (mounted) _loadApplicants();
                                          },
                                        ),
                                      ],
                                    ],
                                  )
                                else
                                  Row(
                                    mainAxisSize: MainAxisSize.min,
                                    children: [
                                      if (applicant.trangThaiHopDong == 'DaHuy' && (applicant.maHopDong ?? '').isNotEmpty) ...[
                                        ActionChip(
                                          avatar: const Icon(Icons.error_outline, size: 14, color: AppColors.error),
                                          label: const Text(
                                            'Bị từ chối',
                                            style: TextStyle(
                                              color: AppColors.error,
                                              fontWeight: FontWeight.w600,
                                              fontSize: 12,
                                            ),
                                          ),
                                          backgroundColor: AppColors.error.withValues(alpha: 0.1),
                                          side: BorderSide(color: AppColors.error.withValues(alpha: 0.3)),
                                          onPressed: () async {
                                            await Navigator.push(
                                              context,
                                              MaterialPageRoute(
                                                builder: (_) => HopDongDetailScreen(maHD: applicant.maHopDong!),
                                              ),
                                            );
                                            if (mounted) _loadApplicants();
                                          },
                                        ),
                                        const SizedBox(width: 8),
                                      ],
                                      ElevatedButton.icon(
                                        onPressed: () async {
                                          await Navigator.push(
                                            context,
                                            MaterialPageRoute(
                                              builder: (_) => TaoHopDongScreen(
                                                applicant: applicant,
                                              ),
                                            ),
                                          );
                                          if (mounted) _loadApplicants();
                                        },
                                        icon: Icon(
                                          applicant.trangThaiHopDong == 'DaHuy'
                                              ? Icons.refresh
                                              : Icons.description_outlined,
                                          size: 16,
                                        ),
                                        label: Text(
                                          applicant.trangThaiHopDong == 'DaHuy'
                                              ? 'Tạo lại'
                                              : 'Tạo hợp đồng',
                                        ),
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor: AppColors.primary,
                                          foregroundColor: Colors.white,
                                          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                                        ),
                                      ),
                                    ],
                                  ),
                            ],
                          ),
                      ],
                    ),
                  ),
                );
              },
            );
          },
        ),
      ),
    );
  }
}
