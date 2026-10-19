import 'package:flutter_test/flutter_test.dart';
import 'package:freelancerstudent/models/mo_hinh_chinh_sua_ho_so_freelancer.dart';
import 'package:freelancerstudent/models/mo_hinh_ho_so_freelancer.dart';

void main() {
  group('FreelancerProfileEditModel', () {
    test('parses profile edit DTO JSON', () {
      final model = FreelancerProfileEditModel.fromJson({
        'maFreelancerStudents': 12,
        'maUser': 34,
        'tenUser': 'Nguyen Van A',
        'sdt': '0900000000',
        'email': 'a@example.com',
        'avatar': '/avatars/a.png',
        'maChuyenNganh': 'CNTT',
        'tenChuyenNganh': 'Công nghệ thông tin',
        'tenTruong': 'Đại học A',
        'diaDiemFreelancerStudent': 'TP. Hồ Chí Minh',
        'namThu': 3,
        'GPA': 3.6,
        'nienKhoa': '2022-2026',
        'gioithieu': 'Giới thiệu',
        'kyNangCoBan': 'Flutter',
        'ngonNgu': 'Tiếng Việt',
        'trangthaiNhanViec': true,
        'chiPhiTu': 500000,
        'danhSachKyNang': ['Dart', 'Flutter'],
      });

      expect(model.maFreelancerStudents, 12);
      expect(model.maUser, 34);
      expect(model.tenUser, 'Nguyen Van A');
      expect(model.gpa, 3.6);
      expect(model.trangthaiNhanViec, isTrue);
      expect(model.chiPhiTu, 500000);
      expect(model.danhSachKyNang, ['Dart', 'Flutter']);
    });

    test('creates edit model from freelancer profile', () {
      const profile = FreelancerProfileModel(
        maUser: 34,
        hoTen: 'Nguyen Van A',
        email: 'a@example.com',
        sdt: '0900000000',
        maFreelancerStudents: 12,
        maChuyenNganh: 'CNTT',
        tenChuyenNganh: 'Công nghệ thông tin',
        tenTruong: 'Đại học A',
        diaDiemFreelancerStudent: 'TP. Hồ Chí Minh',
        namThu: 3,
        gpa: 3.6,
        nienKhoa: '2022-2026',
        gioiThieu: 'Giới thiệu',
        trangThaiNhanViec: true,
        chiPhiTu: 500000,
        skills: ['Dart', 'Flutter'],
        projects: [],
        proofs: [],
        jobPosts: [],
      );

      final model = FreelancerProfileEditModel.fromProfile(profile);

      expect(model.maUser, profile.maUser);
      expect(model.maFreelancerStudents, profile.maFreelancerStudents);
      expect(model.tenUser, profile.hoTen);
      expect(model.gpa, profile.gpa);
      expect(model.danhSachKyNang, profile.skills);
    });

    test('serializes fields using the API DTO keys', () {
      const model = FreelancerProfileEditModel(
        maFreelancerStudents: 12,
        maUser: 34,
        tenUser: 'Nguyen Van A',
        gpa: 3.6,
        danhSachKyNang: ['Dart'],
      );

      expect(model.toJson(), {
        'maFreelancerStudents': 12,
        'maUser': 34,
        'tenUser': 'Nguyen Van A',
        'sdt': null,
        'email': null,
        'avatar': null,
        'maChuyenNganh': '',
        'tenChuyenNganh': '',
        'tenTruong': '',
        'diaDiemFreelancerStudent': '',
        'namThu': 0,
        'GPA': 3.6,
        'nienKhoa': '',
        'gioithieu': null,
        'kyNangCoBan': null,
        'ngonNgu': null,
        'trangthaiNhanViec': true,
        'chiPhiTu': null,
        'danhSachKyNang': ['Dart'],
      });
    });
  });
}
