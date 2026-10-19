import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../core/utils/tien_ich_loi_api.dart';
import '../models/mo_hinh_task_cong_viec.dart';

/// Gọi `api/TaskCongViec` — mọi thao tác ghi đều gửi `maUser` thật để Backend kiểm tra quyền.
class TaskCongViecService {
  final ApiClient _apiClient = ApiClient();

  Map<String, dynamic> _body(dynamic data) =>
      data is Map<String, dynamic> ? data : <String, dynamic>{};

  Future<ApiResponse<TienDoDuAnModel>> layTienDoDuAn(
    String maHD,
    int maUser,
  ) async {
    try {
      final res = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.taskCongViec}/hop-dong/$maHD',
        queryParameters: {'maUser': maUser},
      );
      final body = _body(res.data);
      final data = body['data'];
      if (data is! Map<String, dynamic>) {
        return ApiResponse<TienDoDuAnModel>(
          isSuccess: false,
          message: 'Dữ liệu tiến độ không hợp lệ.',
        );
      }
      return ApiResponse<TienDoDuAnModel>(
        isSuccess: true,
        data: TienDoDuAnModel.fromJson(data),
      );
    } catch (e) {
      return ApiResponse<TienDoDuAnModel>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không tải được tiến độ dự án.'),
      );
    }
  }

  Future<ApiResponse<bool>> themTask({
    required String maHD,
    required int maUser,
    required String tenTask,
    required String moTa,
    int? maGiaiDoan,
    String? linkSanPham,
    DateTime? hanHoanThanh,
  }) async {
    try {
      final res = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.taskCongViec,
        {
          'maHD': maHD,
          'tenTask': tenTask,
          'moTa': moTa,
          'maGiaiDoan': maGiaiDoan,
          'linkSanPham': linkSanPham,
          'hanHoanThanh': hanHoanThanh?.toIso8601String(),
        },
        queryParameters: {'maUser': maUser},
      );
      final body = _body(res.data);
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Thêm đầu việc thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không thể thêm đầu việc.'),
      );
    }
  }

  Future<ApiResponse<bool>> capNhatTask({
    required TaskCongViecModel task,
    required int maUser,
    required String tenTask,
    required String moTa,
    int? maGiaiDoan,
    String? linkSanPham,
    DateTime? hanHoanThanh,
  }) async {
    try {
      final res = await _apiClient.put<Map<String, dynamic>>(
        '${ApiEndpoints.taskCongViec}/${task.maTask}',
        {
          'maTask': task.maTask,
          'maGiaiDoan': maGiaiDoan,
          'tenTask': tenTask,
          'moTa': moTa,
          'linkSanPham': linkSanPham,
          'hanHoanThanh': hanHoanThanh?.toIso8601String(),
          'trangThai': task.trangThai,
        },
        queryParameters: {'maUser': maUser},
      );
      final body = _body(res.data);
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Cập nhật đầu việc thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không thể cập nhật đầu việc.'),
      );
    }
  }

  Future<ApiResponse<bool>> capNhatTrangThai({
    required int maTask,
    required int maUser,
    required String trangThai,
  }) async {
    try {
      final res = await _apiClient.patch<Map<String, dynamic>>(
        '${ApiEndpoints.taskCongViec}/$maTask/trang-thai',
        {'maTask': maTask, 'trangThai': trangThai},
        queryParameters: {'maUser': maUser},
      );
      final body = _body(res.data);
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Cập nhật trạng thái thành công!',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không thể cập nhật trạng thái.'),
      );
    }
  }

  Future<ApiResponse<bool>> xoaTask({
    required int maTask,
    required int maUser,
  }) async {
    try {
      final res = await _apiClient.delete<Map<String, dynamic>>(
        '${ApiEndpoints.taskCongViec}/$maTask',
        queryParameters: {'maUser': maUser},
      );
      final body = _body(res.data);
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ?? 'Đã xóa đầu việc.',
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không thể xóa đầu việc.'),
      );
    }
  }

  Future<ApiResponse<bool>> danhGiaGiaiDoan({
    required String maHD,
    required int maUser,
    int? maGiaiDoan,
    required String trangThai,
  }) async {
    try {
      final res = await _apiClient.post<Map<String, dynamic>>(
        '${ApiEndpoints.taskCongViec}/hop-dong/$maHD/danh-gia-giai-doan',
        {
          'maHD': maHD,
          'maGiaiDoan': maGiaiDoan,
          'trangThai': trangThai,
        },
        queryParameters: {'maUser': maUser},
      );
      final body = _body(res.data);
      return ApiResponse<bool>(
        isSuccess: body['success'] == true,
        data: true,
        message: body['message']?.toString() ??
            (trangThai == 'Dat'
                ? 'Đã xác nhận đạt yêu cầu!'
                : 'Đã cập nhật trạng thái chưa đạt.'),
      );
    } catch (e) {
      return ApiResponse<bool>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không thể đánh giá giai đoạn.'),
      );
    }
  }
}
