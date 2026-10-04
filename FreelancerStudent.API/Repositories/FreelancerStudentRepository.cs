using FreelancerStudent.API.Data;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerStudent.API.Repositories
{
    public class FreelancerStudentRepository : IFreelancerStudentRepository
    {
        private readonly ApplicationDBContext _context;

        public FreelancerStudentRepository(ApplicationDBContext context)
        {
            _context = context;

        }


        public async Task<List<FreelancerStudents>> layTatCaFreelancerStudentsAsync()
        {
            var ketqua = await _context.FreelancerStudents.Include(f => f.User).Include(f => f.ChuyenNganh).ToListAsync();
            return ketqua;
        }

        //Tuấn
        public async Task<FreelancerStudents?> layFreelancerStudent_TheoMaUser(int maUser)
        {
            var freelancerStudent = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maUser == maUser);
            return freelancerStudent;
        }

        //XS
        public async Task<FreelancerStudents?> layTheoMaFreelancerStudents(int maFreelancerStudents)
        {
            var freelancerStudent = await _context.FreelancerStudents.FirstOrDefaultAsync(f => f.maFreelancerStudents == maFreelancerStudents);
            return freelancerStudent;
        }
    }
}