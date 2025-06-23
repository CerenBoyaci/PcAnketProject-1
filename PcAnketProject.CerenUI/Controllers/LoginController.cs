using PcAnketProject.CerenUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PcAnketProject.CerenUI.Controllers
{
    public class LoginController : Controller
    {


        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        public LoginController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _apiKey = configuration["ApiSettings:PauIskurApiKey"];
        }

        [Route("Giris")]
        public IActionResult Giris()
        {
            //RedirectToAction("Index","Home");
            //return RedirectToAction("Index", "Home");
            return View();
        }


        [HttpPost]
        [Route("Giris")]
        public async Task<IActionResult> Giris(KullaniciGirisKontrolRequest girisKontrolRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(girisKontrolRequest);
            }

            // API'nin endpoint URL'si
            girisKontrolRequest.apikey = "";
            string apiUrl = $"{_apiBaseUrl}PusulaLogin/KullaniciDogrula";
            string apiKey = _apiKey;

            var jsonContent = JsonSerializer.Serialize(girisKontrolRequest);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("PauIskurApiKey", apiKey);

                var response = await _httpClient.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var kullaniciData = JsonSerializer.Deserialize<KullaniciDogrulaResponse>(responseData);

                    if (kullaniciData != null)
                    {
                        int _kullaniciId = int.Parse(kullaniciData.nufusID);

                        HttpContext.Session.SetInt32("KullaniciID", _kullaniciId);
                        HttpContext.Session.SetString("KuAdi", kullaniciData.nufusID);
                        ViewData["KullaniciAdi"] = kullaniciData.nufusID;

                        // KullaniciYetki API'sine **GET** isteği gönder
                        string yetkiApiUrl = $"{_apiBaseUrl}PusulaLogin/KullaniciYetki/{_kullaniciId}";

                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("PauIskurApiKey", apiKey);

                        var yetkiResponse = await _httpClient.GetAsync(yetkiApiUrl);

                        if (yetkiResponse.IsSuccessStatusCode)
                        {
                            var yetkiResponseData = await yetkiResponse.Content.ReadAsStringAsync();
                            var yetkiResult = JsonSerializer.Deserialize<KullaniciYetkiResponse>(yetkiResponseData);

                            if (yetkiResult != null)
                            {
                                HttpContext.Session.SetInt32("KullaniciYetki", yetkiResult.yetki);
                                HttpContext.Session.SetString("KullaniciDogrulamaDurumu", "True");

                                return yetkiResult.yetki switch
                                {
                                    0 => RedirectToAction("Belgeler", "Home"), // Öğrenci
                                    1 => RedirectToAction("List", "Inventory"), // Komisyon
                                    2 => RedirectToAction("ProtokolKontenjanListesi", "Komisyon"), // Yönetici
                                    99 => RedirectToAction("ProtokolKontenjanListesi", "Komisyon"), // Admin
                                    _ => RedirectToAction("YetkisizErisim", "Home")
                                };
                            }
                        }

                        ModelState.AddModelError(string.Empty, "Yetki bilgisi alınamadı.");
                        return View(girisKontrolRequest);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Kullanıcı doğrulama başarısız.");
                        return View(girisKontrolRequest);
                    }
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API Hatası: {errorMessage}");
                    return View(girisKontrolRequest);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Giriş Başarısız.");
                return View(girisKontrolRequest);
            }
        }

        [HttpGet]
        [Route("KomisyonGiris")]
        public IActionResult KomisyonGiris()
        {
            return View();
        }


        public IActionResult YetkisizErisim()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            // Session verilerini temizle
            HttpContext.Session.Clear();

            // Kullanıcıyı giriş sayfasına yönlendir
            return RedirectToAction("Index", "Home");
        }

        public class KullaniciDogrulaResponse
        {
            public bool success { get; set; }
            public string nufusID { get; set; }
            public string kullaniciAdi { get; set; }
        }

    }
}
