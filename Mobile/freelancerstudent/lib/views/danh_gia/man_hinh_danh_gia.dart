import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../core/constants/mau_sac_ung_dung.dart';
import '../../core/utils/dinh_dang_du_lieu.dart';
import '../../models/mo_hinh_danh_gia.dart';
import '../../viewmodels/viewmodel_danh_gia.dart';
import '../../viewmodels/viewmodel_xac_thuc.dart';

class DanhGiaListScreen extends StatefulWidget {
  const DanhGiaListScreen({super.key});

  @override
  State<DanhGiaListScreen> createState() => _DanhGiaListScreenState();
}

class _DanhGiaListScreenState extends State<DanhGiaListScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _taiDanhSach();
    });
  }

  void _taiDanhSach() {
    final user = context.read<AuthViewModel>().currentUser;
    if (user != null) {
      context.read<DanhGiaViewModel>().taiDanhSachCongViecDaThue(user.userId);
    }
  }

  void _moFormDanhGia(HopDongDanhGiaModel hd) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
      builder: (ctx) => Padding(
        padding: EdgeInsets.only(
          bottom: MediaQuery.of(ctx).viewInsets.bottom,
        ),
        child: FormDanhGiaWidget(
          maHD: hd.maHD,
          tenCongViec: hd.tenCongViec,
          tenDoiTac: hd.tenDoiTac,
          vaiTroDoiTac: hd.vaiTroDoiTac,
          danhGiaBanDau: hd.maDanhGia != null
              ? DanhGiaModel(
                  maDanhGia: hd.maDanhGia!,
                  maHD: hd.maHD,
                  tenNguoiDanhGia: '',
                  tenCongViec: hd.tenCongViec,
                  soSaoTongThe: hd.soSao ?? 5,
                  soSaoChatLuong: hd.soSaoChatLuong,
                  soSaoDungHan: hd.soSaoDungHan,
                  soSaoGiaoTiep: hd.soSaoGiaoTiep,
                  nhanXet: hd.nhanXet,
                  coTheChinhSua: hd.coTheChinhSuaDanhGia,
                )
              : null,
          onThanhCong: () {
            Navigator.pop(ctx);
            _taiDanhSach();
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Lưu đánh giá thành công!')),
            );
          },
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<DanhGiaViewModel>();

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Đánh giá công việc & đối tác'),
        shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
      ),
      body: RefreshIndicator(
        onRefresh: () async => _taiDanhSach(),
        child: Builder(
          builder: (context) {
            if (vm.dangTai && vm.danhSachCongViec.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }

            if (vm.thongBaoLoi != null && vm.danhSachCongViec.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        vm.thongBaoLoi!,
                        textAlign: TextAlign.center,
                        style: const TextStyle(color: AppColors.error),
                      ),
                      const SizedBox(height: 12),
                      OutlinedButton(
                        onPressed: _taiDanhSach,
                        style: OutlinedButton.styleFrom(
                          shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.zero,
                          ),
                        ),
                        child: const Text('Thử lại'),
                      ),
                    ],
                  ),
                ),
              );
            }

            if (vm.danhSachCongViec.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Text(
                        'Chưa có công việc nào đủ điều kiện đánh giá.\nChỉ các hợp đồng đã hoàn thành mới có thể đánh giá.',
                        textAlign: TextAlign.center,
                        style: TextStyle(color: AppColors.bodyText),
                      ),
                      const SizedBox(height: 12),
                      OutlinedButton(
                        onPressed: _taiDanhSach,
                        style: OutlinedButton.styleFrom(
                          shape: const RoundedRectangleBorder(
                            borderRadius: BorderRadius.zero,
                          ),
                        ),
                        child: const Text('Tải lại'),
                      ),
                    ],
                  ),
                ),
              );
            }

            return ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: vm.danhSachCongViec.length,
              separatorBuilder: (_, _) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final item = vm.danhSachCongViec[index];
                return _buildItemHopDong(item);
              },
            );
          },
        ),
      ),
    );
  }

  Widget _buildItemHopDong(HopDongDanhGiaModel item) {
    final daDanhGia = item.trangThaiDanhGia == 'DaDanhGia' || item.maDanhGia != null;

    return Container(
      decoration: BoxDecoration(
        color: AppColors.surface,
        border: Border.all(color: AppColors.border),
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Expanded(
                child: Text(
                  item.tenCongViec,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: AppColors.heading,
                  ),
                ),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                decoration: BoxDecoration(
                  color: daDanhGia ? Colors.green.shade50 : Colors.amber.shade50,
                  border: Border.all(
                    color: daDanhGia ? Colors.green : Colors.amber.shade700,
                  ),
                ),
                child: Text(
                  daDanhGia ? 'Đã đánh giá' : 'Chờ đánh giá',
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                    color: daDanhGia ? Colors.green.shade800 : Colors.amber.shade900,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            'Đối tác: ${item.tenDoiTac} (${item.vaiTroDoiTac})',
            style: const TextStyle(fontSize: 14, color: AppColors.bodyText),
          ),
          Text(
            'Mã HĐ: ${item.maHD}',
            style: const TextStyle(fontSize: 13, color: Colors.grey),
          ),
          if (item.tongTien != null)
            Text(
              'Giá trị: ${AppFormatters.formatCurrency(item.tongTien)}',
              style: const TextStyle(fontSize: 13, color: Colors.grey),
            ),
          const SizedBox(height: 12),
          if (daDanhGia) ...[
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.grey.shade50,
                border: Border.all(color: Colors.grey.shade200),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Đánh giá của bạn: ${item.soSao ?? 0}/5 sao',
                    style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13),
                  ),
                  if ((item.nhanXet ?? '').isNotEmpty)
                    Text(
                      'Nhận xét: ${item.nhanXet}',
                      style: const TextStyle(fontSize: 13, color: AppColors.bodyText),
                    ),
                ],
              ),
            ),
            const SizedBox(height: 8),
          ],
          Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              if (item.coTheDanhGia)
                FilledButton(
                  onPressed: () => _moFormDanhGia(item),
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                  ),
                  child: const Text('Viết đánh giá'),
                )
              else if (item.coTheChinhSuaDanhGia)
                OutlinedButton(
                  onPressed: () => _moFormDanhGia(item),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppColors.primary,
                    side: const BorderSide(color: AppColors.primary),
                    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                  ),
                  child: const Text('Chỉnh sửa đánh giá'),
                )
              else
                const Text(
                  'Hết hạn chỉnh sửa',
                  style: TextStyle(fontSize: 12, color: Colors.grey),
                ),
            ],
          ),
        ],
      ),
    );
  }
}

class FormDanhGiaWidget extends StatefulWidget {
  final String maHD;
  final String tenCongViec;
  final String tenDoiTac;
  final String vaiTroDoiTac;
  final DanhGiaModel? danhGiaBanDau;
  final VoidCallback onThanhCong;

  const FormDanhGiaWidget({
    super.key,
    required this.maHD,
    required this.tenCongViec,
    required this.tenDoiTac,
    required this.vaiTroDoiTac,
    this.danhGiaBanDau,
    required this.onThanhCong,
  });

  @override
  State<FormDanhGiaWidget> createState() => _FormDanhGiaWidgetState();
}

class _FormDanhGiaWidgetState extends State<FormDanhGiaWidget> {
  int _soSaoTongThe = 5;
  int _soSaoChatLuong = 5;
  int _soSaoDungHan = 5;
  int _soSaoGiaoTiep = 5;
  final TextEditingController _nhanXetCtrl = TextEditingController();
  bool _dangGui = false;
  String? _loi;

  @override
  void initState() {
    super.initState();
    if (widget.danhGiaBanDau != null) {
      _soSaoTongThe = widget.danhGiaBanDau!.soSaoTongThe;
      _soSaoChatLuong = widget.danhGiaBanDau!.soSaoChatLuong ?? 5;
      _soSaoDungHan = widget.danhGiaBanDau!.soSaoDungHan ?? 5;
      _soSaoGiaoTiep = widget.danhGiaBanDau!.soSaoGiaoTiep ?? 5;
      _nhanXetCtrl.text = widget.danhGiaBanDau!.nhanXet ?? '';
    }
  }

  @override
  void dispose() {
    _nhanXetCtrl.dispose();
    super.dispose();
  }

  Future<void> _gui() async {
    final user = context.read<AuthViewModel>().currentUser;
    if (user == null) {
      setState(() => _loi = 'Vui lòng đăng nhập trước khi đánh giá.');
      return;
    }

    setState(() {
      _dangGui = true;
      _loi = null;
    });

    final vm = context.read<DanhGiaViewModel>();
    bool thanhCong = false;

    if (widget.danhGiaBanDau != null && widget.danhGiaBanDau!.maDanhGia > 0) {
      thanhCong = await vm.capNhatDanhGia(
        maDanhGia: widget.danhGiaBanDau!.maDanhGia,
        maUser: user.userId,
        maHD: widget.maHD,
        soSaoTongThe: _soSaoTongThe,
        soSaoChatLuong: _soSaoChatLuong,
        soSaoDungHan: _soSaoDungHan,
        soSaoGiaoTiep: _soSaoGiaoTiep,
        nhanXet: _nhanXetCtrl.text.trim(),
      );
    } else {
      thanhCong = await vm.guiDanhGia(
        maUser: user.userId,
        maHD: widget.maHD,
        soSaoTongThe: _soSaoTongThe,
        soSaoChatLuong: _soSaoChatLuong,
        soSaoDungHan: _soSaoDungHan,
        soSaoGiaoTiep: _soSaoGiaoTiep,
        nhanXet: _nhanXetCtrl.text.trim(),
      );
    }

    if (!mounted) return;
    setState(() => _dangGui = false);

    if (thanhCong) {
      widget.onThanhCong();
    } else {
      setState(() => _loi = vm.thongBaoLoi ?? 'Có lỗi xảy ra khi lưu đánh giá.');
    }
  }

  Widget _chonSoSao(String tieuDe, int giaTriHienTai, ValueChanged<int> onChon) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Expanded(
            child: Text(
              tieuDe,
              style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w500),
            ),
          ),
          Row(
            children: List.generate(5, (index) {
              final sao = index + 1;
              final daChon = sao <= giaTriHienTai;
              return InkWell(
                onTap: () => onChon(sao),
                child: Container(
                  margin: const EdgeInsets.symmetric(horizontal: 2),
                  padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 4),
                  decoration: BoxDecoration(
                    color: daChon ? Colors.amber.shade100 : Colors.grey.shade100,
                    border: Border.all(
                      color: daChon ? Colors.amber.shade700 : Colors.grey.shade300,
                    ),
                  ),
                  child: Text(
                    '$sao*',
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: daChon ? Colors.amber.shade900 : Colors.grey.shade600,
                    ),
                  ),
                ),
              );
            }),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      color: AppColors.surface,
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisSize: MainAxisSize.min,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    widget.danhGiaBanDau != null ? 'Chỉnh sửa đánh giá' : 'Đánh giá đối tác',
                    style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                ),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  style: TextButton.styleFrom(
                    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                  ),
                  child: const Text('Đóng'),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              'Công việc: ${widget.tenCongViec}',
              style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w500),
            ),
            Text(
              'Đối tác: ${widget.tenDoiTac} (${widget.vaiTroDoiTac})',
              style: const TextStyle(fontSize: 13, color: AppColors.bodyText),
            ),
            const Divider(height: 24),
            _chonSoSao('Đánh giá tổng thể', _soSaoTongThe, (v) => setState(() => _soSaoTongThe = v)),
            _chonSoSao('Chất lượng công việc', _soSaoChatLuong, (v) => setState(() => _soSaoChatLuong = v)),
            _chonSoSao('Đúng thời hạn', _soSaoDungHan, (v) => setState(() => _soSaoDungHan = v)),
            _chonSoSao('Giao tiếp & hợp tác', _soSaoGiaoTiep, (v) => setState(() => _soSaoGiaoTiep = v)),
            const SizedBox(height: 12),
            TextField(
              controller: _nhanXetCtrl,
              maxLines: 4,
              decoration: const InputDecoration(
                labelText: 'Nhận xét chi tiết',
                hintText: 'Chia sẻ trải nghiệm làm việc cùng đối tác...',
                border: OutlineInputBorder(borderRadius: BorderRadius.zero),
              ),
            ),
            if (_loi != null) ...[
              const SizedBox(height: 8),
              Text(
                _loi!,
                style: const TextStyle(color: AppColors.error, fontSize: 13),
              ),
            ],
            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              height: 44,
              child: FilledButton(
                onPressed: _dangGui ? null : _gui,
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  shape: const RoundedRectangleBorder(borderRadius: BorderRadius.zero),
                ),
                child: Text(_dangGui ? 'Đang lưu...' : 'Gửi đánh giá'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
