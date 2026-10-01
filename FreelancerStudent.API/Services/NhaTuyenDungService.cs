using System.Reflection.Metadata.Ecma335;
using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Helper;
using FreelancerStudent.API.Models;
using FreelancerStudent.API.Repositories.Interfaces;
using FreelancerStudent.API.Services.Interfaces;

namespace FreelancerStudent.API.Services
{
    public class NhaTuyenDungService : INhaTuyenDungService
    {
        private readonly INhaTuyenDungRepository _nhaTuyenDungRepository; //Sử dụng để gọi các dữ liệu đucợ laays từ database để service xử lý

        public NhaTuyenDungService(INhaTuyenDungRepository nhaTuyenDungRepository)
        {
            _nhaTuyenDungRepository = nhaTuyenDungRepository;
        }


        //
        public async Task<List<NhaTuyenDung_ResponseDTO>> layDanhSachNhaTuyenDungAsync()
        {
            var ds_layTuSQL = await _nhaTuyenDungRepository.layTatCaNhaTuyenDungAsync();

            var dsResult = ds_layTuSQL.Select(ntd => new NhaTuyenDung_ResponseDTO
            {
                maNhaTuyenDung = ntd.maNhaTuyenDung,
                maUser = ntd.maUser,
                hotenUser = ntd.User?.hotenUser,
                emailUser = ntd.User?.emailUser,
                sdtUser = ntd.User?.sdtUser,
                tencongty = ntd.tencongty,
                linhvuc = ntd.linhvuc,
                diachi = ntd.diachi,
                gioithieu = ntd.gioithieu,

                logo = ntd.logo,
                sosaodanhgia = ntd.sosaodanhgia,
                trangthai = ntd.trangthai,
                ngayDangKy = ntd.ngayDangKy
            }).ToList();

            return dsResult;
        }

        /*
        

          

        */



    }
}