using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class JobPostService : IJobPostService
    {
        // Repository xử lý bảng JobPost
        private readonly IJobPostRepository _jopPostRepository;

        // Repository xử lý bảng NhaTuyenDung
        private readonly INhaTuyenDungRepository _nhaTuyenDungRepository;


        // Inject cả 2 Repository
        public JobPostService(
            IJobPostRepository jopPostRepository,
            INhaTuyenDungRepository nhaTuyenDungRepository)
        {
            _jopPostRepository = jopPostRepository;
            _nhaTuyenDungRepository = nhaTuyenDungRepository;
        }


        // =========================================================
        // LẤY DANH SÁCH JOB POST
        // =========================================================
        public async Task<List<JobPost_ReponseDTO>> layDanhSachJobPostAsync()
        {
            var layDS_JobSQL =
                await _jopPostRepository.layTatCaJobPostAsync();

            var dsResult = layDS_JobSQL.Select(job =>
                new JobPost_ReponseDTO
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
                    thoigiandukienhoanthanh =
                        job.thoigiandukienhoanthanh,
                    status = job.status,
                    soluongtuyen = job.soluongtuyen
                }
            ).ToList();

            return dsResult;
        }


        // =========================================================
        // TẠO JOB POST
        // =========================================================
        public async Task<JobPost_ReponseDTO> taoJobPostAsync(
            JobPost_RequestDTO request)
        {
            // -----------------------------------------------------
            // 1. Kiểm tra hạn hoàn thành
            // -----------------------------------------------------
            if (request.thoigiandukienhoanthanh <= DateTime.Now)
            {
                throw new Exception(
                    "Thời gian hoàn thành dự kiến phải lớn hơn thời gian hiện tại!"
                );
            }


            // -----------------------------------------------------
            // 2. Từ maUser tìm Nhà tuyển dụng tương ứng
            // -----------------------------------------------------
            var nhaTuyenDung =
                await _nhaTuyenDungRepository
                    .layNhaTuyenDungTheoMaUserAsync(request.maUser);

            if (nhaTuyenDung == null)
            {
                throw new Exception(
                    "Không tìm thấy thông tin Nhà tuyển dụng của tài khoản này!"
                );
            }


            // -----------------------------------------------------
            // 3. Tự sinh mã Job
            // -----------------------------------------------------
            string maJobTuSinh =
                "JOB" + DateTime.Now.ToString("yyMMddHHmmssfff");


            // -----------------------------------------------------
            // 4. Tạo JobPost
            // -----------------------------------------------------
            var jobMoi = new JobPost
            {
                maJob = maJobTuSinh,

                // Không lấy trực tiếp từ request nữa.
                // maUser -> NhaTuyenDung -> maNhaTuyenDung
                maNhaTuyenDung = nhaTuyenDung.maNhaTuyenDung,

                tieude = request.tieude.Trim(),

                mota = request.mota.Trim(),

                kynangyeucau =
                    request.kynangyeucau?.Trim(),

                thulao = request.thulao,

                soluongtuyen = request.soluongtuyen,

                thoigiandukienhoanthanh =
                    request.thoigiandukienhoanthanh,

                fileDinhKem =
                    request.fileDinhKem ?? string.Empty,

                // Tạm thời phí đăng bài = 0
                // Sau này Admin sẽ quản lý phần này
                phiDangBai = 0,

                // Thời điểm tạo bài
                thoigiandangtuyen = DateTime.Now,

                // Trạng thái mặc định
                status = "DangTuyen"
            };


            // -----------------------------------------------------
            // 5. Repository lưu xuống Database
            // -----------------------------------------------------
            var luuJob =
                await _jopPostRepository.themJobPostAsync(jobMoi);


            // -----------------------------------------------------
            // 6. Chuyển Model -> ResponseDTO
            // -----------------------------------------------------
            return new JobPost_ReponseDTO
            {
                maJob = luuJob.maJob,

                maNhaTuyenDung =
                    luuJob.maNhaTuyenDung,

                tieude = luuJob.tieude,

                mota = luuJob.mota,

                kynangyeucau =
                    luuJob.kynangyeucau,

                thulao = luuJob.thulao,

                fileDinhKem =
                    luuJob.fileDinhKem,

                phiDangBai =
                    luuJob.phiDangBai,

                thoigiandangtuyen =
                    luuJob.thoigiandangtuyen,

                thoigiandukienhoanthanh =
                    luuJob.thoigiandukienhoanthanh,

                status = luuJob.status,

                soluongtuyen =
                    luuJob.soluongtuyen
            };
        }
    }
}