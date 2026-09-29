using FreelancerStudent.API.DTOs.ReponsesDTO;
using FreelancerStudent.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerStudent.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("LayViTheoUser/{maUser}")]

        public async Task<IActionResult> LayViTheoUser(int maUser)
        {
            try
            {
                var request = new Wallet_RequestDTO { maUser = maUser };

                //Gọi service
                var ketquads = await _walletService.layViTheoMaUsersAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Lấy ví thành công!",
                    data = ketquads
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy ví: " + ex.Message
                });
            }
        }



    }
}