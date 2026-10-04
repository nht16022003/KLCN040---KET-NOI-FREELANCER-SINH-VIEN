using FreelancerStudent.Web.Models;
using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface IJobPostWebService
    {
        Task<ApiReponse<List<JobPostViewModel>>> layDanhSachJobPost();

        //XS
        Task<ApiReponse<JobDetailViewModel>> layChiTietJobPost(string maJob);



        //Tuấn Anh
        Task<ApiReponse<JobPostViewModel>> taoJobPost(
           JobPostViewModel model
       );

        //Tuấn
        Task<ApiReponse<List<UngTuyenViewModel>>> layDSUngTuyenTheoMaUser(int maUser);
    }
}