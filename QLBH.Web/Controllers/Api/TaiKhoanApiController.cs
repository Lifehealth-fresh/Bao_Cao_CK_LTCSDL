using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanApiController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;

        public TaiKhoanApiController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }

        [HttpPost("dangnhap")]
        public async Task<IActionResult> DangNhap([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _taiKhoanService.LoginAsync(request);
            if (result == null) return Unauthorized(new { message = "Sai email hoặc mật khẩu" });
            return Ok(result);
        }

        [HttpPost("dangky")]
        public async Task<IActionResult> DangKy([FromBody] DangKyRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _taiKhoanService.DangKyAsync(request);
            if (!success) return BadRequest(new { message = "Email đã tồn tại" });
            return Ok(new { message = "Đăng ký thành công" });
        }

        [Authorize(Roles = "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _taiKhoanService.GetAllAsync();
            return Ok(list);
        }

        [Authorize(Roles = "NhanVien")]
        [HttpGet("nguoidung")]
        public async Task<IActionResult> GetNguoiDungList()
        {
            var list = await _taiKhoanService.GetNguoiDungListAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _taiKhoanService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [Authorize]
        [HttpPut("thongtin")]
        public async Task<IActionResult> UpdateThongTin([FromBody] CapNhatThongTinDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _taiKhoanService.UpdateThongTinAsync(dto);
            if (!result) return NotFound();
            return Ok(new { message = "Cập nhật thành công" });
        }

        [Authorize]
        [HttpPut("doimatkhau")]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _taiKhoanService.DoiMatKhauAsync(dto);
            if (!result) return BadRequest(new { message = "Mật khẩu cũ không đúng" });
            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

        [Authorize(Roles = "NhanVien")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _taiKhoanService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}