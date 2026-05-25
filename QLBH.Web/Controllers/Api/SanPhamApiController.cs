using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamApiController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamApiController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] int? danhMucId,
            [FromQuery] bool chiLayHoatDong = true)
        {
            var list = await _sanPhamService.GetAllAsync(keyword, danhMucId, chiLaySanPhamHoatDong: chiLayHoatDong);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _sanPhamService.GetByIdAsync(id);
            if (item == null) return NotFound();
            if (item.BienTheSanPhams != null)
                item.BienTheSanPhams = item.BienTheSanPhams.Where(bt => bt.TrangThai).ToList();
            return Ok(item);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSanPhamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _sanPhamService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.SanPhamID }, result);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSanPhamDto dto)
        {
            if (id != dto.SanPhamID) return BadRequest();
            try
            {
                var result = await _sanPhamService.UpdateAsync(dto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "NhanVien")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sanPhamService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // Biến thể
        [HttpGet("bienthe/sanpham/{sanPhamId}")]
        public async Task<IActionResult> GetAllBienThe(int sanPhamId)
        {
            var list = await _sanPhamService.GetAllBienTheAsync(sanPhamId);
            return Ok(list);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPost("bienthe")]
        public async Task<IActionResult> CreateBienThe([FromBody] CreateBienTheSanPhamDto dto)
        {
            var result = await _sanPhamService.CreateBienTheAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPut("bienthe/{id}")]
        public async Task<IActionResult> UpdateBienThe(int id, [FromBody] UpdateBienTheSanPhamDto dto)
        {
            if (id != dto.BienTheID) return BadRequest();
            var result = await _sanPhamService.UpdateBienTheAsync(dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpDelete("bienthe/{id}")]
        public async Task<IActionResult> DeleteBienThe(int id)
        {
            var result = await _sanPhamService.DeleteBienTheAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}