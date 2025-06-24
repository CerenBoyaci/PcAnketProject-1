using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using PcAnketProject.Core.Dto;
using PcAnketProject.Service;
using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace PcAnketProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResimController : ControllerBase
    {
        private readonly ResimService _service;
        private readonly IWebHostEnvironment _env;

        public ResimController(ResimService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] ResimYukleDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Dosya yüklenmedi.");

            var uploadsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.File.CopyToAsync(stream);

            var resim = new Resim
            {
                DosyaAdi = dto.File.FileName,
                DosyaYolu = $"uploads/{fileName}",
                YuklenmeTarihi = DateTime.Now
            };

            var id = await _service.AddAsync(resim);

            return Ok(new { Id = id, Path = resim.DosyaYolu });
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var resim = await _service.GetByIdAsync(id);
            return resim != null ? Ok(resim) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? Ok("Silindi") : NotFound("Bulunamadı");
        }

        [HttpGet("img/{id}")]
        public async Task<IActionResult> GetImage(int id, [FromQuery] int? width, [FromQuery] int? height)
        {
            var resim = await _service.GetByIdAsync(id);
            if (resim == null)
                return NotFound();

            var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", resim.DosyaYolu.Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            using var image = await Image.LoadAsync(fullPath);

            if (width.HasValue || height.HasValue)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(width ?? 0, height ?? 0)
                }));
            }

            using var ms = new MemoryStream();
            await image.SaveAsJpegAsync(ms);
            return File(ms.ToArray(), "image/jpeg");
        }


    }
}