using Microsoft.AspNetCore.Mvc;
using PcAnketProject.CerenUI.Models;
using System.Net.Http.Json;

namespace PcAnketProject.CerenUI.Controllers
{
    [Route("Resim")]
    public class ResimDuzenController : Controller
    {
        private readonly HttpClient _http;

        public ResimDuzenController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7211/api/");
        }

        [HttpGet("Duzenle")]
        public async Task<IActionResult> Duzenle()
        {
            var resimler = await _http.GetFromJsonAsync<List<ResimDto>>("Resim");
            var duzenler = await _http.GetFromJsonAsync<List<ResimDuzen>>("ResimDuzen");

            var model = resimler.Select(resim =>
            {
                var duzen = duzenler.FirstOrDefault(d => d.ResimID == resim.ID);
                return new ResimVeDuzenDto
                {
                    ResimID = resim.ID,
                    DosyaYolu = resim.DosyaYolu,
                    DuzenID = duzen?.ID,
                    Baslik = duzen?.Baslik,
                    Rolu = duzen?.Rolu
                };
            }).ToList();

            return View("~/Views/Resim/Duzenle.cshtml", model);
        }

        [HttpPost("Duzenle")]
        public async Task<IActionResult> Duzenle(List<ResimVeDuzenDto> model)
        {
            foreach (var item in model)
            {
                // API’ye uygun model oluştur
                var duzenDto = new ResimDuzen
                {
                    ID = item.DuzenID ?? 0,
                    ResimID = item.ResimID,
                    Baslik = item.Baslik,
                    Rolu = item.Rolu,
                    Genislik = item.Genislik,
                    Yukseklik = item.Yukseklik
                };

                if (item.DuzenID.HasValue)
                {
                    await _http.PutAsJsonAsync($"ResimDuzen/{item.DuzenID}", duzenDto);
                }
                else if (!string.IsNullOrWhiteSpace(item.Baslik) || !string.IsNullOrWhiteSpace(item.Rolu))
                {
                    await _http.PostAsJsonAsync("ResimDuzen", duzenDto);
                }
            }

            return RedirectToAction("Duzenle");
        }


        [HttpGet("Sayfa")]
        public async Task<IActionResult> Sayfa()
        {
            var resimler = await _http.GetFromJsonAsync<List<ResimDto>>("Resim");
            var duzenler = await _http.GetFromJsonAsync<List<ResimDuzen>>("ResimDuzen");

            var model = resimler.Select(resim =>
            {
                var duzen = duzenler.FirstOrDefault(d => d.ResimID == resim.ID);
                return new ResimVeDuzenDto
                {
                    ResimID = resim.ID,
                    DosyaYolu = resim.DosyaYolu,
                    DuzenID = duzen?.ID,
                    Baslik = duzen?.Baslik,
                    Rolu = duzen?.Rolu,
                    Genislik = duzen?.Genislik,
                    Yukseklik = duzen?.Yukseklik
                };
            }).Where(x => !string.IsNullOrWhiteSpace(x.Rolu)).ToList();

            return View("~/Views/Resim/Sayfa.cshtml", model);
        }



    }
}
