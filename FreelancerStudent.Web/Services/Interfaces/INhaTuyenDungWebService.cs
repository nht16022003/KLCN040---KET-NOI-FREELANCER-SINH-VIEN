using FreelancerStudent.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.Web.Services.Interfaces
{
    public interface INhaTuyenDungWebService
    {
        Task<ApiReponse<List<NhaTuyenDungViewModel>>> layDanhSachNhaTuyenDungAsync();
        Task<ApiReponse<NhaTuyenDungProfileViewModel>> layProfileNhaTuyenDungAsync(int maNhaTuyenDung);
        Task<ApiReponse<NhaTuyenDungProfileViewModel>> layProfileNhaTuyenDungTheoMaUserAsync(int maUser);
    }
}