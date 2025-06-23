using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcAnketProject.Core.Dto;
using PcAnketProject.Data.Repository;
using PcAnketProject.Service;

namespace PcAnketProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KullaniciController : ControllerBase
    {
        private readonly KullaniciService _service;

        public KullaniciController(KullaniciService service)
        {
            _service = service;
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
            var user = await _service.GetByIdAsync(id);
            return user != null ? Ok(user) : NotFound("Kullanıcı bulunamadı.");
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Kullanici kullanici)
        {
            var id = await _service.AddAsync(kullanici);
            return Ok(new { Id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Kullanici kullanici)
        {
            kullanici.ID = id;
            var success = await _service.UpdateAsync(kullanici);
            return success ? Ok("Kullanıcı başarıyla güncellendi.") : NotFound("Güncellenecek kullanıcı bulunamadı.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? Ok("Kullanıcı silindi.") : NotFound("Silinecek kullanıcı bulunamadı.");
        }
    }
}

