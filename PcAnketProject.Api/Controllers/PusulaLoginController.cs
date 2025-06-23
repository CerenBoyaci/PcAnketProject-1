using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcAnketProject.Core.Dto;
using PcAnketProject.Service;

namespace PcAnketProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PusulaLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PusulaAuthService _pusulaAuthService;

        public PusulaLoginController(PusulaAuthService pusulaAuthService, IConfiguration configuration)
        {
            _pusulaAuthService = pusulaAuthService;
            _configuration = configuration;
        }


        [HttpPost("KullaniciDogrula")]
        public async Task<IActionResult> KullaniciDogrula([FromHeader(Name = "PauIskurApiKey")] string apiKey, [FromBody] PusulaLoginRequest request)
        {

            var validApiKey = _configuration["PauIskurApiKey"];
            if (apiKey != validApiKey)
            {
                return Unauthorized("E005 Hata Oluştu");
            }
            var (isValid, NufusID) = await _pusulaAuthService.KullaniciDogrula(request.username, request.password);

            return Ok(new
            {
                Success = isValid,
                NufusID = NufusID
            });
        }

        [HttpPost("KullaniciBilgisi")]
        public async Task<IActionResult> KullaniciBilgisi(
    [FromHeader(Name = "PauIskurApiKey")] string apiKey,
    [FromBody] int nufusID)
        {
            var validApiKey = _configuration["PauIskurApiKey"];
            if (apiKey != validApiKey)
            {
                return Unauthorized("E005 Hata Oluştu");
            }

            // NufusID'ye göre Ad ve Soyad bilgilerini al
            var (nufusIDString, ad, soyad) = await _pusulaAuthService.KullaniciBilgi(nufusID);

            if (string.IsNullOrEmpty(ad) && string.IsNullOrEmpty(soyad))
            {
                return NotFound(new { Success = false, Message = "Kullanıcı bulunamadı" });
            }

            return Ok(new
            {
                Success = true,
                NufusID = nufusIDString,
                Ad = ad,
                Soyad = soyad
            });
        }

        [HttpGet("KullaniciYetki/{nufusID}")]
        public async Task<IActionResult> KullaniciYetki(
            [FromHeader(Name = "PauIskurApiKey")] string apiKey,
            [FromRoute] int nufusID)
        {
            var validApiKey = _configuration["PauIskurApiKey"];
            if (apiKey != validApiKey)
            {
                return Unauthorized(new { Success = false, Message = "E005 Hata Oluştu" });
            }

            var kullanici = await _pusulaAuthService.GetKullaniciYetki(nufusID);

            if (!kullanici.HasValue)
            {
                return NotFound(new { Success = false, Message = "Kullanıcı bulunamadı" });
            }

            return Ok(new
            {
                Success = true,
                NufusID = nufusID,
                Yetki = kullanici.Value.Yetki,
                AdSoyad = kullanici.Value.AdSoyad
            });
        }





    }
}
