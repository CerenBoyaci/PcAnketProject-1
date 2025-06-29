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

            // Debug: Gelen duzenler'i kontrol et
            foreach (var duzen in duzenler)
            {
                Console.WriteLine($"Duzen ID: {duzen.ID}, ResimID: {duzen.ResimID}, Aktif: {duzen.Aktif}");
            }

            var model = resimler.Select(resim =>
            {
                var duzen = duzenler.Where(d => d.ResimID == resim.ID).OrderByDescending(d => d.ID).FirstOrDefault();
                return new ResimVeDuzenDto
                {
                    ResimID = resim.ID,
                    DosyaYolu = resim.DosyaYolu,
                    DuzenID = duzen?.ID,
                    Baslik = duzen?.Baslik,
                    Rolu = duzen?.Rolu,
                    Genislik = duzen?.Genislik,
                    Yukseklik = duzen?.Yukseklik,
                    Aktif = duzen?.Aktif ?? true
                };
            }).ToList();

            // Debug: Model içindeki Aktif değerlerini kontrol et
            foreach (var item in model)
            {
                Console.WriteLine($"Model ResimID: {item.ResimID}, DuzenID: {item.DuzenID}, Aktif: {item.Aktif}");
            }

            return View("~/Views/Resim/Duzenle.cshtml", model);
        }

        [HttpPost("Duzenle")]
        public async Task<IActionResult> Duzenle(List<ResimVeDuzenDto> model, int? silinenIndex)
        {
            if (silinenIndex.HasValue)
            {
                var item = model[silinenIndex.Value];

                if (item.DuzenID.HasValue)
                {
                    // Resmi pasif hale getir (veri tabanına gönder!)
                    var duzenDto = new ResimDuzen
                    {
                        ID = item.DuzenID.Value,
                        ResimID = item.ResimID,
                        Baslik = item.Baslik,
                        Rolu = item.Rolu,
                        Genislik = item.Genislik,
                        Yukseklik = item.Yukseklik,
                        Aktif = false // Kritik yer!
                    };

                    await _http.PutAsJsonAsync($"ResimDuzen/{item.DuzenID}", duzenDto);
                }

                // Tekrar Duzenle view'ine dön
                return RedirectToAction("Duzenle");
            }

            // Diğer normal kayıt işlemleri
            foreach (var item in model)
            {
                var duzenDto = new ResimDuzen
                {
                    ID = item.DuzenID ?? 0,
                    ResimID = item.ResimID,
                    Baslik = item.Baslik,
                    Rolu = item.Rolu,
                    Genislik = item.Genislik,
                    Yukseklik = item.Yukseklik,
                    Aktif = item.Aktif
                };

                if (item.DuzenID.HasValue)
                    await _http.PutAsJsonAsync($"ResimDuzen/{item.DuzenID}", duzenDto);
                else if (!string.IsNullOrWhiteSpace(item.Baslik) || !string.IsNullOrWhiteSpace(item.Rolu))
                    await _http.PostAsJsonAsync("ResimDuzen", duzenDto);
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
                    Yukseklik = duzen?.Yukseklik,
                    Aktif = duzen?.Aktif ?? true
                };
            }).Where(x => !string.IsNullOrWhiteSpace(x.Rolu) && x.Aktif).ToList();

            return View("~/Views/Resim/Sayfa.cshtml", model);
        }


        


        

        [HttpPost("Sil")]
        public async Task<IActionResult> Sil(int duzenId)
        {
            var duzenler = await _http.GetFromJsonAsync<List<ResimDuzen>>("ResimDuzen");
            var duzen = duzenler.FirstOrDefault(d => d.ID == duzenId);

            if (duzen != null)
            {
                duzen.Aktif = false;
                await _http.PutAsJsonAsync($"ResimDuzen/{duzen.ID}", duzen);
            }

            return RedirectToAction("Duzenle");
        }



    }
}
