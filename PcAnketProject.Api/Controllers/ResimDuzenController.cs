using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcAnketProject.Core.Dto;
using PcAnketProject.Service;

namespace PcAnketProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResimDuzenController : ControllerBase
    {
        private readonly ResimDuzenService _service;

        public ResimDuzenController(ResimDuzenService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            foreach (var item in result)
            {
                Console.WriteLine($"ID: {item.ID}, Aktif: {item.Aktif}");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ResimDuzen dto)
        {
            var id = await _service.AddAsync(dto);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResimDuzen dto)
        {
            if (id != dto.ID)
                return BadRequest("ID uyuşmuyor");

            var success = await _service.UpdateAsync(dto);
            return success ? Ok("Güncellendi") : NotFound("Bulunamadı");
        }





    }
}
