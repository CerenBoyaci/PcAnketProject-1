using Microsoft.AspNetCore.Mvc;
using PcAnketProject.CerenUI.Models;
using System.Net.Http;
using System.Text.Json;

namespace PcAnketProject.CerenUI.Controllers
{
    public class CerenController : Controller
    {
        private readonly HttpClient _httpClient;
         public CerenController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task <IActionResult> Index()
        {
            string apiUrl = $"https://localhost:7211/api/Kullanici"; // API URL

            _httpClient.DefaultRequestHeaders.Clear();


            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var protokolKontenjanListesi = JsonSerializer.Deserialize<List<cerendto>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(protokolKontenjanListesi);
                }
                else
                {
                    ViewData["ErrorMessage"] = "API'den veri alınamadı.";
                    return View(new List<cerendto>());
                }
            }
            catch (Exception ex)
            {         
                ViewData["ErrorMessage"] = "Bağlantı sırasında hata oluştu.";
                return View(new List<cerendto>());
            }
        }
    }
}
