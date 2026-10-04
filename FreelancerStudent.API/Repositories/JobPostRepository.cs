using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly ApplicationDBContext _context;

        public JobPostRepository(ApplicationDBContext context)
        {
            _context = context;

        }


        public async Task<List<JobPost>> layTatCaJobPostAsync()
        {
            var dsJobPost = await _context.JobPosts.ToListAsync();
            return dsJobPost;
        }

        public async Task<JobPost?> layJobPostTheoMaAsync(string maJob)
        {
            return await _context.JobPosts
                .FirstOrDefaultAsync(job => job.maJob == maJob);
        }

        public async Task<JobPost> themJobPostAsync(JobPost job)
        {
            await _context.JobPosts.AddAsync(job);
            await _context.SaveChangesAsync();
            return job;
        }

        //Tuấn
        public async Task<List<UngTuyen>> layTatCaDanhSachUngTuyenVaoJobPost_TheoNTD(int maNhaTuyenDung)
        {
            var dsUTuyen = await _context.UngTuyens.Include(u => u.JobPosts).
            Include(u => u.FreelamcerStudents).ThenInclude(f => f!.User).
            Where(u => u.JobPosts != null && u.JobPosts.maNhaTuyenDung == maNhaTuyenDung)
            .OrderByDescending(u => u.ngayUngTuyen).ToListAsync();
            return dsUTuyen;
        }

        //Tuấn
        public async Task<List<UngTuyen>> layTatCaDanhSachUngTuyenVaoJobPost_TheoFreelancerStudents(int maFreelancerStudents)
        {
            var dsUngTuyen = await _context.UngTuyens.Include(u => u.JobPosts)
            .ThenInclude(j => j!.NhaTuyenDung).Include(u => u.FreelamcerStudents).ThenInclude(f => f!.User)
            .Where(u => u.maFreelancerStudent == maFreelancerStudents).OrderByDescending(u => u.ngayUngTuyen).ToListAsync();

            return dsUngTuyen;
        }

    }
}