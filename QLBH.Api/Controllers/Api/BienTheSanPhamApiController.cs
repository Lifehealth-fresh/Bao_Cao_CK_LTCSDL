using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BienTheSanPhamApiController : ControllerBase
    {
        private readonly IBienTheSanPhamService _bienTheService;

        public BienTheSanPhamApiController(IBienTheSanPhamService bienTheService)
        {
            _bienTheService = bienTheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? sanPhamId)
        {
            if (sanPhamId.HasValue)
            {
                var byProduct = await _bienTheService.GetBySanPhamIdAsync(sanPhamId.Value);
                return Ok(byProduct);
            }
            var list = await _bienTheService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _bienTheService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBienTheSanPhamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _bienTheService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.BienTheID }, result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "NhanVien")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBienTheSanPhamDto dto)
        {
            if (id != dto.BienTheID) return BadRequest();
            try
            {
                var result = await _bienTheService.UpdateAsync(dto);
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
            var result = await _bienTheService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
