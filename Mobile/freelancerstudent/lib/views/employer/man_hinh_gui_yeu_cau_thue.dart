import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../models/mo_hinh_bai_dang_tuyen_dung.dart';
import '../../models/mo_hinh_ung_thue.dart';
import '../../viewmodels/viewmodel_cong_viec.dart';
import '../../viewmodels/viewmodel_ung_thue.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';
import 'man_hinh_quan_ly_yeu_cau_thue_ntd.dart';

class ManHinhGuiYeuCauThue extends StatefulWidget {
  final int maFreelancerStudent;
  final String tenFreelancer;
  final String? avatarFreelancer;
  final String? chuyenNganh;
  final double? thulaoMacDinh;

  const ManHinhGuiYeuCauThue({
    super.key,
    required this.maFreelancerStudent,
    required this.tenFreelancer,
    this.avatarFreelancer,
    this.chuyenNganh,
    this.thulaoMacDinh,
  });

  @override
  State<ManHinhGuiYeuCauThue> createState() => _ManHinhGuiYeuCauThueState();
}

class _ManHinhGuiYeuCauThueState extends State<ManHinhGuiYeuCauThue> {
  final _formKey = GlobalKey<FormState>();
  final _tieuDeController = TextEditingController();
  final _moTaController = TextEditingController();
  final _nganSachController = TextEditingController();

  final currencyFormatter = NumberFormat('#,###', 'vi_VN');
  final dateFormatter = DateFormat('dd/MM/yyyy');

  bool _chonTuJobCoSan = true;
  JobPostModel? _jobDuocChon;
  DateTime? _thoiHanDuKien;

  @override
  void initState() {
    super.initState();
    if (widget.thulaoMacDinh != null && widget.thulaoMacDinh! > 0) {
      _nganSachController.text = widget.thulaoMacDinh!.toStringAsFixed(0);
    }
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _taiDanhSachJob();
    });
  }

  void _taiDanhSachJob() {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      context.read<JobViewModel>().taiDanhSachJobCuaToi(user.userId);
    }
  }

  @override
  void dispose() {
    _tieuDeController.dispose();
    _moTaController.dispose();
    _nganSachController.dispose();
    super.dispose();
  }

  void _onSelectJob(JobPostModel job) {
    setState(() {
      _jobDuocChon = job;
      _tieuDeController.text = job.tieude;
      _moTaController.text = job.mota;
      if (job.thulao != null && job.thulao! > 0) {
        _nganSachController.text = job.thulao!.toStringAsFixed(0);
      }
      _thoiHanDuKien = job.thoigiandukienhoanthanh ?? job.hannophoso;
    });
  }

  Future<void> _chonNgay() async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: _thoiHanDuKien ?? now.add(const Duration(days: 7)),
      firstDate: now,
      lastDate: now.add(const Duration(days: 365)),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: AppColors.primary,
              onPrimary: Colors.white,
              onSurface: AppColors.heading,
            ),
          ),
          child: child!,
        );
      },
    );
    if (picked != null) {
      setState(() => _thoiHanDuKien = picked);
    }
  }

  Future<void> _guiYeuCau() async {
    if (!_formKey.currentState!.validate()) return;

    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng đăng nhập để tiếp tục.')),
      );
      return;
    }

    final nganSach = double.tryParse(
      _nganSachController.text.replaceAll(RegExp(r'[^0-9]'), ''),
    );

    final req = GuiYeuCauThueRequest(
      maUserNTD: user.userId,
      maFreelancerStudent: widget.maFreelancerStudent,
      maJob: _chonTuJobCoSan ? _jobDuocChon?.maJob : null,
      tieuDeCongViec: _tieuDeController.text.trim(),
      moTaCongViec: _moTaController.text.trim(),
      nganSachDeNghi: nganSach,
      thoiHanDuKien: _thoiHanDuKien,
    );

    final vm = context.read<UngThueViewModel>();
    final ok = await vm.guiYeuCauThue(req);

    if (!mounted) return;

    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            vm.thongBaoThanhCong ??
                'Gửi yêu cầu thuê tới ${widget.tenFreelancer} thành công!',
          ),
          backgroundColor: AppColors.primary,
        ),
      );
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (_) => const ManHinhQuanLyYeuCauThueNTD(),
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(vm.thongBaoLoi ?? 'Không thể gửi yêu cầu thuê.'),
          backgroundColor: Colors.red[700],
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final avatar = widget.avatarFreelancer != null &&
            widget.avatarFreelancer!.isNotEmpty
        ? widget.avatarFreelancer!
        : 'https://ui-avatars.com/api/?name=${Uri.encodeComponent(widget.tenFreelancer)}&background=E8F5E9&color=1B5E36';

    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFC),
      appBar: AppBar(
        title: const Text(
          'Thuê Freelancer trực tiếp',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        backgroundColor: AppColors.primary,
        elevation: 0,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Thông tin ứng viên được chọn
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: Colors.white,
                  border: Border.all(color: const Color(0xFFE2E8F0)),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    ClipRRect(
                      borderRadius: BorderRadius.circular(24),
                      child: Image.network(
                        avatar,
                        width: 48,
                        height: 48,
                        fit: BoxFit.cover,
                        errorBuilder: (ctx, err, stack) => Container(
                          width: 48,
                          height: 48,
                          color: Colors.grey[200],
                          child: const Icon(Icons.person, color: Colors.grey),
                        ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            widget.tenFreelancer,
                            style: const TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: AppColors.heading,
                            ),
                          ),
                          if (widget.chuyenNganh != null &&
                              widget.chuyenNganh!.isNotEmpty) ...[
                            const SizedBox(height: 2),
                            Text(
                              widget.chuyenNganh!,
                              style: const TextStyle(
                                fontSize: 13,
                                color: AppColors.primary,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                          ],
                        ],
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 20),

              // Chọn cách thuê
              const Text(
                'Hình thức yêu cầu',
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.bold,
                  color: AppColors.heading,
                ),
              ),
              const SizedBox(height: 8),

              Row(
                children: [
                  Expanded(
                    child: InkWell(
                      onTap: () => setState(() => _chonTuJobCoSan = true),
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                            vertical: 10, horizontal: 12),
                        decoration: BoxDecoration(
                          color: _chonTuJobCoSan
                              ? const Color(0xFFE8F5E9)
                              : Colors.white,
                          border: Border.all(
                            color: _chonTuJobCoSan
                                ? AppColors.primary
                                : const Color(0xFFCBD5E1),
                            width: _chonTuJobCoSan ? 1.5 : 1,
                          ),
                          borderRadius: BorderRadius.circular(6),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(
                              _chonTuJobCoSan
                                  ? Icons.radio_button_checked
                                  : Icons.radio_button_off,
                              size: 18,
                              color: _chonTuJobCoSan
                                  ? AppColors.primary
                                  : Colors.grey,
                            ),
                            const SizedBox(width: 6),
                            const Flexible(
                              child: Text(
                                'Từ bài đăng có sẵn',
                                style: TextStyle(
                                  fontSize: 13,
                                  fontWeight: FontWeight.w600,
                                ),
                                overflow: TextOverflow.ellipsis,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: InkWell(
                      onTap: () => setState(() {
                        _chonTuJobCoSan = false;
                        _jobDuocChon = null;
                      }),
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                            vertical: 10, horizontal: 12),
                        decoration: BoxDecoration(
                          color: !_chonTuJobCoSan
                              ? const Color(0xFFE8F5E9)
                              : Colors.white,
                          border: Border.all(
                            color: !_chonTuJobCoSan
                                ? AppColors.primary
                                : const Color(0xFFCBD5E1),
                            width: !_chonTuJobCoSan ? 1.5 : 1,
                          ),
                          borderRadius: BorderRadius.circular(6),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(
                              !_chonTuJobCoSan
                                  ? Icons.radio_button_checked
                                  : Icons.radio_button_off,
                              size: 18,
                              color: !_chonTuJobCoSan
                                  ? AppColors.primary
                                  : Colors.grey,
                            ),
                            const SizedBox(width: 6),
                            const Flexible(
                              child: Text(
                                'Dự án riêng mới',
                                style: TextStyle(
                                  fontSize: 13,
                                  fontWeight: FontWeight.w600,
                                ),
                                overflow: TextOverflow.ellipsis,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                ],
              ),

              const SizedBox(height: 16),

              // Nếu chọn từ Job có sẵn -> dropdown các Job đang tuyển
              if (_chonTuJobCoSan) ...[
                Consumer<JobViewModel>(
                  builder: (context, jobVM, _) {
                    final jobs = jobVM.danhSachJobCuaToi
                        .where((j) => j.status == 'DangTuyen')
                        .toList();

                    if (jobVM.dangTai) {
                      return const Center(
                        child: Padding(
                          padding: EdgeInsets.all(8.0),
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                      );
                    }

                    if (jobs.isEmpty) {
                      return Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: const Color(0xFFFEF3C7),
                          borderRadius: BorderRadius.circular(6),
                          border: Border.all(color: const Color(0xFFF59E0B)),
                        ),
                        child: const Text(
                          'Bạn hiện chưa có bài đăng tuyển dụng nào đang mở. Bạn có thể chọn "Dự án riêng mới" bên trên để tạo công việc trực tiếp cho Freelancer.',
                          style: TextStyle(
                            fontSize: 12.5,
                            color: Color(0xFF92400E),
                          ),
                        ),
                      );
                    }

                    return DropdownButtonFormField<JobPostModel>(
                      value: _jobDuocChon,
                      isExpanded: true,
                      decoration: const InputDecoration(
                        labelText: 'Chọn bài tuyển dụng *',
                        border: OutlineInputBorder(),
                        contentPadding:
                            EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                      ),
                      hint: const Text('Chọn một bài đăng của bạn'),
                      items: jobs.map((j) {
                        return DropdownMenuItem<JobPostModel>(
                          value: j,
                          child: Text(
                            j.tieude,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: const TextStyle(fontSize: 14),
                          ),
                        );
                      }).toList(),
                      onChanged: (j) {
                        if (j != null) _onSelectJob(j);
                      },
                      validator: (v) {
                        if (_chonTuJobCoSan && v == null) {
                          return 'Vui lòng chọn bài tuyển dụng';
                        }
                        return null;
                      },
                    );
                  },
                ),
                const SizedBox(height: 14),
              ],

              // Tiêu đề công việc
              TextFormField(
                controller: _tieuDeController,
                decoration: const InputDecoration(
                  labelText: 'Tiêu đề công việc / dự án *',
                  hintText: 'Ví dụ: Thiết kế giao diện ứng dụng di động',
                  border: OutlineInputBorder(),
                  contentPadding:
                      EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập tiêu đề công việc';
                  }
                  return null;
                },
              ),

              const SizedBox(height: 14),

              // Mô tả công việc / Lời mời
              TextFormField(
                controller: _moTaController,
                maxLines: 4,
                decoration: const InputDecoration(
                  labelText: 'Mô tả công việc & Lời mời cộng tác *',
                  hintText:
                      'Nêu rõ yêu cầu dự án, các đầu việc cần làm và thời gian mong muốn...',
                  border: OutlineInputBorder(),
                  contentPadding: EdgeInsets.all(14),
                  alignLabelWithHint: true,
                ),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) {
                    return 'Vui lòng nhập mô tả công việc';
                  }
                  return null;
                },
              ),

              const SizedBox(height: 14),

              // Ngân sách đề nghị & Thời hạn dự kiến
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _nganSachController,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(
                        labelText: 'Ngân sách đề nghị (đ) *',
                        hintText: '1.000.000',
                        border: OutlineInputBorder(),
                        contentPadding:
                            EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                      ),
                      validator: (val) {
                        if (val == null || val.trim().isEmpty) {
                          return 'Nhập ngân sách';
                        }
                        return null;
                      },
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: InkWell(
                      onTap: _chonNgay,
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 12, vertical: 13),
                        decoration: BoxDecoration(
                          border: Border.all(color: Colors.grey.shade400),
                          borderRadius: BorderRadius.circular(4),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Flexible(
                              child: Text(
                                _thoiHanDuKien != null
                                    ? dateFormatter.format(_thoiHanDuKien!)
                                    : 'Hạn dự kiến',
                                style: TextStyle(
                                  fontSize: 14,
                                  color: _thoiHanDuKien != null
                                      ? AppColors.heading
                                      : Colors.grey.shade600,
                                ),
                                overflow: TextOverflow.ellipsis,
                              ),
                            ),
                            const Icon(Icons.calendar_today,
                                size: 18, color: AppColors.primary),
                          ],
                        ),
                      ),
                    ),
                  ),
                ],
              ),

              const SizedBox(height: 28),

              // Nút gửi yêu cầu
              Consumer<UngThueViewModel>(
                builder: (context, vm, _) {
                  return SizedBox(
                    width: double.infinity,
                    height: 48,
                    child: ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        elevation: 0,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(6),
                        ),
                      ),
                      onPressed: vm.dangXuLy ? null : _guiYeuCau,
                      child: vm.dangXuLy
                          ? const SizedBox(
                              width: 22,
                              height: 22,
                              child: CircularProgressIndicator(
                                strokeWidth: 2.5,
                                color: Colors.white,
                              ),
                            )
                          : const Text(
                              'Gửi yêu cầu thuê',
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                    ),
                  );
                },
              ),
            ],
          ),
        ),
      ),
    );
  }
}

