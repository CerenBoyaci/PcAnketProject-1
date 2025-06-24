using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PcAnketProject.CerenUI.Models;
using System.Text;

namespace PcAnketProject.CerenUI.Controllers
{
    public class ResimController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResimController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7211/api/Resim");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<ResimDto>>(json);
                return View(data);
            }

            return View(new List<ResimDto>());
        }

        [HttpGet]
        public IActionResult Yukle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Yukle(IFormFile file)
        {
            var client = _httpClientFactory.CreateClient();
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

            var response = await client.PostAsync("https://localhost:7211/api/Resim", form);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Goruntule(int id, int? width, int? height)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7211/api/Resim/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<ResimDto>(json);

                ViewBag.Width = width ?? 300;
                ViewBag.Height = height ?? 300;

                return View(model);
            }

            return NotFound("Resim bulunamadı.");
        }

    }
}

