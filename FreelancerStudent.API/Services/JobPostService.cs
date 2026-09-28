using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class JobPostService : IJobPostService
    {
        private readonly IJobPostRepository _jopPostRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý

        public JobPostService(IJobPostRepository nhaTuyenDungRepository)
        {
            _jopPostRepository = nhaTuyenDungRepository;
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

            //Tự động sinh mã VARCHARR cho maJob
            string maJobTuSinh = "JOB" + DateTime.Now.ToString("yyMMddHHmmsss");

            //Tạo model JobPost để chuẩn bị lưu xuống Database
            var jobMoi = new JobPost
            {
                maJob = maJobTuSinh,
                maNhaTuyenDung = request.maNhaTuyenDung,
                tieude = request.tieude.Trim(),
                mota = request.mota.Trim(),
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

    }
}