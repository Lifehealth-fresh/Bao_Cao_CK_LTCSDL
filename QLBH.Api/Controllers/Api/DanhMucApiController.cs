using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucApiController : ControllerBase
    {
        private readonly IDanhMucService _danhMucService;

        public DanhMucApiController(IDanhMucService danhMucService)
        {
            _danhMucService = danhMucService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _danhMucService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _danhMucService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDanhMucDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _danhMucService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.DanhMucID }, result);
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDanhMucDto dto)
        {
            if (id != dto.DanhMucID) return BadRequest();
            var result = await _danhMucService.UpdateAsync(dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _danhMucService.DeleteAsync(id);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }
    }
}
