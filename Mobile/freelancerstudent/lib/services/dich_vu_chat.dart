import '../core/constants/diem_cuoi_api.dart';
import '../core/network/trinh_khach_api.dart';
import '../core/network/phan_hoi_api.dart';
import '../core/utils/tien_ich_loi_api.dart';
import '../models/mo_hinh_tin_nhan_chat.dart';
import '../models/mo_hinh_phong_chat.dart';

class ChatService {
  final ApiClient _apiClient = ApiClient();

  Map<String, dynamic> _body(dynamic data) =>
      data is Map<String, dynamic> ? data : <String, dynamic>{};

  /// Lấy danh sách các phòng chat của User
  Future<ApiResponse<List<PhongChatModel>>> layDanhSachPhong(int maUser) async {
    try {
      final res = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.danhSachPhongChat}/$maUser',
      );
      final body = _body(res.data);
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<PhongChatModel>>(
        isSuccess: true,
        data: rawItems
            .whereType<Map<String, dynamic>>()
            .map(PhongChatModel.fromJson)
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<PhongChatModel>>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không tải được danh sách phòng chat.'),
      );
    }
  }

  /// Tạo hoặc mở phòng chat với đối phương (Client <-> Freelancer)
  Future<ApiResponse<PhongChatModel>> taoHoacLayPhong({
    required int maUserClient,
    required int maFreelancerStudent,
    String? maJob,
  }) async {
    try {
      final res = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.taoHoacLayPhongChat,
        {
          'maUserClient': maUserClient,
          'maFreelancerStudent': maFreelancerStudent,
          if (maJob != null && maJob.isNotEmpty) 'maJob': maJob,
        },
      );
      final body = _body(res.data);
      final data = body['data'];
      if (data is Map<String, dynamic>) {
        return ApiResponse<PhongChatModel>(
          isSuccess: true,
          data: PhongChatModel.fromJson(data),
          message: body['message']?.toString() ?? 'Lấy phòng chat thành công',
        );
      }
      return ApiResponse<PhongChatModel>(
        isSuccess: false,
        message: body['message']?.toString() ?? 'Không tạo được phòng chat.',
      );
    } catch (e) {
      return ApiResponse<PhongChatModel>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Lỗi khi mở phòng chat.'),
      );
    }
  }

  /// Lấy lịch sử tin nhắn trong phòng & đánh dấu đã đọc
  Future<ApiResponse<List<ChatMessageModel>>> layLichSuTinNhan(
    int maPhongChat,
    int maUser,
  ) async {
    try {
      final res = await _apiClient.get<Map<String, dynamic>>(
        '${ApiEndpoints.lichSuTinNhan}/$maPhongChat',
        queryParameters: {'maUser': maUser},
      );
      final body = _body(res.data);
      final rawItems = body['data'] is List ? body['data'] as List : <dynamic>[];

      return ApiResponse<List<ChatMessageModel>>(
        isSuccess: true,
        data: rawItems
            .whereType<Map<String, dynamic>>()
            .map(ChatMessageModel.fromJson)
            .toList(),
      );
    } catch (e) {
      return ApiResponse<List<ChatMessageModel>>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không tải được lịch sử tin nhắn.'),
      );
    }
  }

  /// Gửi tin nhắn mới qua REST API
  Future<ApiResponse<ChatMessageModel>> guiTinNhan({
    required int maPhongChat,
    required int maNguoiGui,
    required String noiDung,
    String? fileDinhKem,
    String loaiTinNhan = 'Text',
  }) async {
    try {
      final res = await _apiClient.post<Map<String, dynamic>>(
        ApiEndpoints.guiTinNhan,
        {
          'maPhongChat': maPhongChat,
          'maNguoiGui': maNguoiGui,
          'noiDung': noiDung,
          'fileDinhKem': ?fileDinhKem,
          'loaiTinNhan': loaiTinNhan,
        },
      );
      final body = _body(res.data);
      final data = body['data'];
      if (data is Map<String, dynamic>) {
        return ApiResponse<ChatMessageModel>(
          isSuccess: true,
          data: ChatMessageModel.fromJson(data),
          message: body['message']?.toString() ?? 'Gửi tin nhắn thành công!',
        );
      }
      return ApiResponse<ChatMessageModel>(
        isSuccess: false,
        message: body['message']?.toString() ?? 'Gửi tin nhắn thất bại.',
      );
    } catch (e) {
      return ApiResponse<ChatMessageModel>(
        isSuccess: false,
        message: layThongBaoLoiApi(e, macDinh: 'Không thể gửi tin nhắn.'),
      );
    }
  }
}
