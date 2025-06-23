using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PcAnketProject.CerenUI.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace PcAnketProject.CerenUI.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KullaniciController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        
        public async Task<IActionResult> Kullanici()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7211/api/Kullanici");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<KullaniciDto>>(jsonData);
                return View(values); 
            }

            return View(new List<KullaniciDto>()); 
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Ekle(KullaniciDto model)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7211/api/Kullanici", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Kullanici");
            }

            return View();
        }

        
        public async Task<IActionResult> Sil(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7211/api/Kullanici/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Kullanici");
            }

            return RedirectToAction("Kullanici");
        }

        
        // güncelleme get
        [HttpGet]
        public async Task<IActionResult> Guncelle(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7211/api/Kullanici/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<KullaniciDto>(jsonData);
                return View(value);
            }

            return RedirectToAction("Kullanici");
        }

        
        // güncelleme post
        [HttpPost]
        public async Task<IActionResult> Guncelle(KullaniciDto model)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7211/api/Kullanici/{model.ID}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Kullanici");
            }

            return View(model);
        }


    }
}
