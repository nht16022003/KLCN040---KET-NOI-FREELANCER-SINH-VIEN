using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class JobPostService : IJobPostService
    {
        private readonly IJobPostRepository _jopPostRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý
        private readonly INhaTuyenDungRepository _ntd;

        private readonly IUserRepository _user;

        private readonly IFreelancerStudentRepository _free;



        public JobPostService(IJobPostRepository nhaTuyenDungRepository, INhaTuyenDungRepository ntd, IUserRepository user, IFreelancerStudentRepository free)
        {
            _jopPostRepository = nhaTuyenDungRepository;
            _ntd = ntd;
            _user = user;
            _free = free;
        }


        //
        public async Task<List<JobPost_ReponseDTO>> layDanhSachJobPostAsync()
        {
            var layDS_JobSQL = await _jopPostRepository.layTatCaJobPostAsync(); //Đang trả về là Models
            var dsResult = layDS_JobSQL.Select(job => new JobPost_ReponseDTO
            {
                maJob = job.maJob,
                maNhaTuyenDung = job.maNhaTuyenDung,
                tieude = job.tieude,
                mota = job.mota,
                kynangyeucau = job.kynangyeucau,
                thulao = job.thulao,
                fileDinhKem = job.fileDinhKem,
                phiDangBai = job.phiDangBai,
                thoigiandangtuyen = job.thoigiandangtuyen,
                thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                status = job.status,
                soluongtuyen = job.soluongtuyen
            }).ToList();

            return dsResult;
        }


        public async Task<JobPost_ReponseDTO> taoJobPostAsync(JobPost_RequestDTO request)
        {
            //Kiểm tra ngày hoàn thành phải sau ngày hiện tại
            if (request.thoigiandukienhoanthanh <= DateTime.Now)
            {
                throw new Exception("Thời gian hoàn thành dự kiến phải lớn hơn thời gian hiện tại!");
            }


            //Tuấn bổ sung:
            //Lấy thông tin mã nhà tuyển dụng dựa trên request.user

            var NTD_Theo_maUser = await _ntd.layNhaTuyenDungTheoMaUserAsync(request.maUser);

            if (NTD_Theo_maUser == null)
            {
                throw new Exception($"Không tìm thấy hồ sơ Nhà tuyển dụng của User ID {request.maUser}!");
            }


            //Gán mã ntd theo mã user
            int maNTD_TheoUser = NTD_Theo_maUser!.maNhaTuyenDung;


            //Tự động sinh mã VARCHARR cho maJob
            string maJobTuSinh = "JOB" + DateTime.Now.ToString("yyMMddHHmmsss");

            //Tạo model JobPost để chuẩn bị lưu xuống Database
            var jobMoi = new JobPost
            {
                maJob = maJobTuSinh,
                maNhaTuyenDung = maNTD_TheoUser, //sửa
                tieude = request.tieude.Trim(),
                mota = request.mota!.Trim(),
                kynangyeucau = request.kynangyeucau?.Trim(),
                thulao = request.thulao,
                soluongtuyen = request.soluongtuyen,
                thoigiandukienhoanthanh = request.thoigiandukienhoanthanh,
                fileDinhKem = request.fileDinhKem ?? string.Empty,
                phiDangBai = request.phiDangBai,
                thoigiandangtuyen = DateTime.UtcNow,
                status = "DangTuyen"
            };

            //Gọi repo để lưu
            var luuJob = await _jopPostRepository.themJobPostAsync(jobMoi);
            // 5. Chuyển Model thành DTO trả về cho Controller
            return new JobPost_ReponseDTO
            {
                maJob = luuJob.maJob,
                maNhaTuyenDung = luuJob.maNhaTuyenDung,
                tieude = luuJob.tieude,
                mota = luuJob.mota,
                kynangyeucau = luuJob.kynangyeucau,
                thulao = luuJob.thulao,
                fileDinhKem = luuJob.fileDinhKem,
                phiDangBai = luuJob.phiDangBai,
                thoigiandangtuyen = luuJob.thoigiandangtuyen,
                thoigiandukienhoanthanh = luuJob.thoigiandukienhoanthanh,
                status = luuJob.status,
                soluongtuyen = luuJob.soluongtuyen
            };
        }

        public async Task<List<UngTuyen_ReponseDTO>> layDanhSachUngTuyen_JobPost_TheoMaUser(int maUser)
        {
            if (maUser <= 0)
            {
                throw new Exception("Mã người dùng không hợp lệ!");
            }

            var user = await _user.timUserTheoMaUserAsync(maUser);
            if (user == null)
            {
                throw new Exception("Không tìm thấy người dùng!");
            }

            List<UngTuyen> dsUngTuyen = new List<UngTuyen>();

            if (user.maRole == 2)
            {
                var ntd = await _ntd.layNhaTuyenDungTheoMaUserAsync(user.maUser);
                if (ntd == null)
                {
                    throw new Exception("Chưa tìm thấy hồ sơ Nhà tuyển dụng của tài khoản này!");
                }

                dsUngTuyen = await _jopPostRepository.layTatCaDanhSachUngTuyenVaoJobPost_TheoNTD(ntd.maNhaTuyenDung);
            }
            else if (user.maRole == 1)
            {
                var freelacerstudent = await _free.layFreelancerStudent_TheoMaUser(maUser);
                if (freelacerstudent == null)
                {
                    throw new Exception("Chưa tìm thấy hồ sơ FreelancerStudent của tài khoản này!");
                }

                dsUngTuyen = await _jopPostRepository.layTatCaDanhSachUngTuyenVaoJobPost_TheoFreelancerStudents(freelacerstudent.maFreelancerStudents);
            }

            if (dsUngTuyen == null || !dsUngTuyen.Any())
            {
                return new List<UngTuyen_ReponseDTO>();
            }


            return dsUngTuyen.Select(u => new UngTuyen_ReponseDTO
            {
                maUngTuyen = u.maUngTuyen,
                maJob = u.maJob,
                tieude = u.JobPosts?.tieude ?? string.Empty,
                thulao = u.JobPosts?.thulao,
                mota = u.JobPosts?.mota,
                kynangyeucau = u.JobPosts?.kynangyeucau,
                soluongtuyen = u.JobPosts?.soluongtuyen,
                status = u.JobPosts?.status,
                phiDangBai = u.JobPosts?.phiDangBai ?? 0,
                thoigiandangtuyen = u.JobPosts?.thoigiandangtuyen ?? DateTime.UtcNow,
                thoigiandukienhoanthanh = u.JobPosts?.thoigiandukienhoanthanh ?? DateTime.UtcNow,

                //THÔNG TIN NHÀ TUYỂN DỤNG CỦA BÀI ĐĂNG
                maNhaTuyenDung = u.JobPosts?.maNhaTuyenDung ?? 0,
                tenCongTy = u.JobPosts?.NhaTuyenDung?.tencongty ?? u.JobPosts?.NhaTuyenDung?.User?.hotenUser ?? string.Empty,
                avatarNhaTuyenDung = u.JobPosts?.NhaTuyenDung?.User?.avatarUrl ?? "",

                // Thông tin ứng tuyển
                maFreelancerStudents = u.maFreelancerStudent,
                thuGioiThieu = u.thuGioiThieu,
                thulaoDeXuat = u.thulaoDeXuat,
                thoiGianHoanThanhDeXuat = u.thoiGianHoanThanhDeXuat,
                fileCV = u.fileCV,
                ngayUngTuyen = u.ngayUngTuyen,
                trangThaiUngTuyen = u.trangThaiUngTuyen,

                // Thông tin sinh viên nộp đơn
                maUser = u.FreelamcerStudents?.User?.maUser ?? 0,
                tenUser = u.FreelamcerStudents?.User?.hotenUser,
                emailUser = u.FreelamcerStudents?.User?.emailUser,
                sdtUser = u.FreelamcerStudents?.User?.sdtUser,
                avatar = u.FreelamcerStudents?.User?.avatarUrl,
                tenTruong = u.FreelamcerStudents?.tenTruong ?? string.Empty,
                GPA = u.FreelamcerStudents?.GPA ?? 0,
                namThu = u.FreelamcerStudents?.namThu ?? 1,
                tenChuyenNganh = u.FreelamcerStudents?.ChuyenNganh?.tenChuyenNganh ?? string.Empty
            }).ToList();

        }

        //Tuấn
        //Tuấn
        public async Task<bool> duyetUngTuyenAsync(DuyetUngTuyen_RequestDTO request)
        {
            if (request.maUngTuyen <= 0)
            {
                throw new Exception("Mã đơn ứng tuyển không hợp lệ!");
            }
            if (request.trangThai != "ChapNhan" && request.trangThai != "TuChoi")
            {
                throw new Exception("Trạng thái duyệt không hợp lệ! (Chỉ chấp nhận 'ChapNhan' hoặc 'TuChoi')");
            }

            var ketQua = await _jopPostRepository.capNhatTrangThaiUngTuyenAsync(request.maUngTuyen, request.trangThai);

            if (!ketQua)
            {
                throw new Exception("Không tìm thấy đơn ứng tuyển để cập nhật!");
            }
            return true;

        }

        public async Task<bool> nopDonUngTuyenAsync(UngTuyen_RequestDTO requets)
        {
            var sinhvien = await _free.layFreelancerStudent_TheoMaUser(requets.maUser);
            if (sinhvien == null)
            {
                throw new Exception("Chưa tìm thấy hồ sơ Sinh viên của bạn để ứng tuyển!");
            }


            var ungTuyenMoi = new UngTuyen
            {
                maJob = requets.maJob,
                maFreelancerStudent = sinhvien.maFreelancerStudents,
                thuGioiThieu = requets.thuGioiThieu.Trim(),
                thulaoDeXuat = requets.thulaoDeXuat,
                thoiGianHoanThanhDeXuat = requets.thoiGianHoanThanhDeXuat,
                fileCV = requets.fileCV,
                ngayUngTuyen = DateTime.UtcNow,
                trangThaiUngTuyen = "ChoDuyet"
            };

            await _jopPostRepository.themUngTuyenAsync(ungTuyenMoi);
            return true;
        }


        //Tuấn
        public async Task<List<JobPost_ReponseDTO>> layJobPostTheoMaUserAsync(int maUser)
        {
            var ntd = await _ntd.layNhaTuyenDungTheoMaUserAsync(maUser);
            if (ntd == null) return new List<JobPost_ReponseDTO>();
            var ds = await _jopPostRepository.layJobPostTheoMaNTDAsync(ntd.maNhaTuyenDung);
            return ds.Select(job => new JobPost_ReponseDTO
            {
                maJob = job.maJob,
                maNhaTuyenDung = job.maNhaTuyenDung,
                tieude = job.tieude,
                mota = job.mota,
                kynangyeucau = job.kynangyeucau,
                thulao = job.thulao,
                soluongtuyen = job.soluongtuyen,
                thoigiandangtuyen = job.thoigiandangtuyen,
                thoigiandukienhoanthanh = job.thoigiandukienhoanthanh,
                status = job.status
            }).ToList();
        }


        //Tuấn
        public async Task<bool> capNhatJobPostAsync(JobPost_RequestDTO reqeuest)
        {
            var job = await _jopPostRepository.layJobPostTheoMaAsync(reqeuest.maJob);
            if (job == null) throw new Exception("Không tìm thấy bài tuyển dụng!");
            job.tieude = reqeuest.tieude.Trim();
            job.mota = reqeuest.mota.Trim();
            job.kynangyeucau = reqeuest.kynangyeucau?.Trim();
            job.thulao = reqeuest.thulao;
            job.soluongtuyen = reqeuest.soluongtuyen;
            job.thoigiandukienhoanthanh = reqeuest.thoigiandukienhoanthanh;
            return await _jopPostRepository.capNhatJobPostAsync(job);
        }

    }
}