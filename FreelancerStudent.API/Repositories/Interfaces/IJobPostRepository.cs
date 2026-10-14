using FreelancerStudent.API.Models;

namespace FreelancerStudent.API.Repositories.Interfaces
{
    public interface IJobPostRepository
    {
        //Lấy tất cả JobPost
        Task<List<JobPost>> layTatCaJobPostAsync();

        //XS
        Task<JobPost?> layJobPostTheoMaAsync(string maJob);

        Task<JobPost> themJobPostAsync(JobPost job);


        //Tuấn
        Task<List<UngTuyen>> layTatCaDanhSachUngTuyenVaoJobPost_TheoNTD(int maNhaTuyenDung); //lấy ds ứng tuyển theo nhà tuyển dụng 
        //ở phía ntd xem ds ứng tuyển mà freelancer gửi sang


        //Tuấn
        Task<List<UngTuyen>> layTatCaDanhSachUngTuyenVaoJobPost_TheoFreelancerStudents(int maFreelancerStudents); //lấy ds ứng tuyển theo freelancer
        //ở phía freelancer có thể xem là đã nộp tuyển vào đâu

        //Tuấn
        Task<bool> capNhatTrangThaiUngTuyenAsync(int maUngTuyen, string trangthai);

        //Tuấn
        Task<UngTuyen> themUngTuyenAsync(UngTuyen ungTuyen);

        //Tuấn
        Task<List<JobPost>> layJobPostTheoMaNTDAsync(int maNhaTuyenDung);



        //Tuấn
        Task<bool> capNhatJobPostAsync(JobPost job);

    }
}