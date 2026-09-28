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

        public async Task<JobPost> themJobPostAsync(JobPost job)
        {
            await _context.JobPosts.AddAsync(job);
            await _context.SaveChangesAsync();
            return job;
        }

    }
}