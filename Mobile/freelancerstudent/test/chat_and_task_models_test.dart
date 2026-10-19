import 'package:flutter_test/flutter_test.dart';
import 'package:freelancerstudent/models/mo_hinh_tin_nhan_chat.dart';
import 'package:freelancerstudent/models/mo_hinh_bai_dang_tuyen_dung.dart';
import 'package:freelancerstudent/models/mo_hinh_phong_chat.dart';
import 'package:freelancerstudent/models/mo_hinh_task_cong_viec.dart';

void main() {
  group('Chat and Task Model Unit Tests', () {
    test('PhongChatModel parses from backend JSON correctly', () {
      final json = {
        'maPhongChat': 10,
        'maUserClient': 5,
        'tenClient': 'FPT Software',
        'avatarClient': '/uploads/avatars/fpt.png',
        'maFreelancerStudent': 2,
        'maUserFreelancer': 8,
        'tenFreelancer': 'Nguyen Van SV',
        'avatarFreelancer': null,
        'maJob': 'JOB_001',
        'tieuDeJob': 'Lap trinh Flutter',
        'thulaoJob': 15000000,
        'tinNhanCuoi': 'Chao ban',
        'thoiGianTinNhanCuoi': '2026-10-18T09:00:00Z',
        'soTinNhanChuaDoc': 3,
        'trangThai': 'Active',
        'ngayTao': '2026-10-15T08:00:00Z',
      };

      final model = PhongChatModel.fromJson(json);

      expect(model.maPhongChat, 10);
      expect(model.maUserClient, 5);
      expect(model.tenDoiPhuong(8), 'FPT Software'); // Freelancer xem client
      expect(model.tenDoiPhuong(5), 'Nguyen Van SV'); // Client xem freelancer
      expect(model.soTinNhanChuaDoc, 3);
      expect(model.maJob, 'JOB_001');
    });

    test('ChatMessageModel parses from backend JSON correctly', () {
      final json = {
        'maTinNhan': 101,
        'maPhongChat': 10,
        'maNguoiGui': 8,
        'tenNguoiGui': 'Nguyen Van SV',
        'avatarNguoiGui': null,
        'noiDung': 'Em da cap nhat tien do a',
        'fileDinhKem': null,
        'loaiTinNhan': 'Text',
        'daDoc': true,
        'ngayGui': '2026-10-18T09:10:00Z',
      };

      final msg = ChatMessageModel.fromJson(json);

      expect(msg.maTinNhan, 101);
      expect(msg.maPhongChat, 10);
      expect(msg.maNguoiGui, 8);
      expect(msg.noiDung, 'Em da cap nhat tien do a');
      expect(msg.daDoc, true);
      // Backward compatibility getters
      expect(msg.id, '101');
      expect(msg.senderId, '8');
      expect(msg.content, 'Em da cap nhat tien do a');
    });

    test('TienDoDuAnModel and TaskCongViecModel parse correctly', () {
      final json = {
        'maHD': 'HD_001',
        'trangThaiHopDong': 'DangThucHien',
        'ngayBatDauHopDong': '2026-10-01T00:00:00Z',
        'ngayKetThucHopDong': '2026-10-31T00:00:00Z',
        'tongSoTask': 3,
        'soTaskChuaThucHien': 1,
        'soTaskDangThucHien': 1,
        'soTaskHoanThanh': 1,
        'tyLeHoanThanh': 33.33,
        'coQuyenCapNhat': true,
        'danhSachTask': [
          {
            'maTask': 1,
            'maHD': 'HD_001',
            'tenTask': 'Thiet ke man hinh Mobile',
            'moTa': 'Dung Figma hoac Flutter UI',
            'trangThai': 'HoanThanh',
          },
          {
            'maTask': 2,
            'maHD': 'HD_001',
            'tenTask': 'Ket noi API Tien do',
            'moTa': 'Su dung TaskCongViec API',
            'trangThai': 'DangThucHien',
          },
        ],
      };

      final tienDo = TienDoDuAnModel.fromJson(json);

      expect(tienDo.maHD, 'HD_001');
      expect(tienDo.tyLeHoanThanh, 33.33);
      expect(tienDo.coQuyenCapNhat, true);
      expect(tienDo.danhSachTask.length, 2);
      expect(tienDo.danhSachTask[0].tenTask, 'Thiet ke man hinh Mobile');
      expect(tienDo.danhSachTask[0].trangThai, 'HoanThanh');
      expect(tienDo.danhSachTask[1].trangThai, 'DangThucHien');
    });

    test('JobPostModel parses job recommendation fields correctly', () {
      final json = {
        'maJob': 'JOB_Flutter_01',
        'tieude': 'Lập trình ứng dụng Flutter cho Startup',
        'mota': 'Cần bạn sinh viên có kỹ năng Flutter, Dart và REST API',
        'thulao': 12000000,
        'ngaydang': '2026-10-18T08:00:00Z',
        'kynangyeucau': 'Flutter, Dart, REST API',
        'trangthai': 'DangTuyen',
        'doPhuHop': 0.85,
        'lyDoGoiY': 'Trùng khớp 2 kỹ năng: Flutter, Dart. Phù hợp chuyên ngành CNTT.',
        'kyNangTrungKhop': ['Flutter', 'Dart'],
      };

      final job = JobPostModel.fromJson(json);

      expect(job.id, 'JOB_Flutter_01');
      expect(job.title, 'Lập trình ứng dụng Flutter cho Startup');
      expect(job.budget, 12000000);
      expect(job.doPhuHop, 0.85);
      expect(job.phanTramPhuHop, 85);
      expect(job.lyDoGoiY, contains('Trùng khớp 2 kỹ năng: Flutter, Dart'));
      expect(job.kyNangTrungKhop, containsAll(['Flutter', 'Dart']));

      final jobFromPercent = JobPostModel.fromJson({...json, 'doPhuHop': 33.0});
      expect(jobFromPercent.phanTramPhuHop, 33);

      final jobOverMax = JobPostModel.fromJson({...json, 'doPhuHop': 120.0});
      expect(jobOverMax.phanTramPhuHop, 100);
    });
  });
}
