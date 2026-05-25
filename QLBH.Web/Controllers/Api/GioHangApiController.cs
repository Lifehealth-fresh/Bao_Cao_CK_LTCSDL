using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "KhachHang")]
    public class GioHangApiController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangApiController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        [HttpGet("{khachHangId}")]
        public async Task<IActionResult> GetCart(int khachHangId)
        {
            var cart = await _gioHangService.GetCartByKhachHangIdAsync(khachHangId);
            if (cart == null) return NotFound(new { message = "Không tìm thấy giỏ hàng" });
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _gioHangService.AddToCartAsync(dto);
            if (!result) return BadRequest(new { message = "Thêm vào giỏ thất bại" });
            return Ok(new { message = "Đã thêm sản phẩm vào giỏ" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _gioHangService.UpdateQuantityAsync(dto);
            if (!result) return NotFound(new { message = "Chi tiết giỏ hàng không tồn tại" });
            return Ok(new { message = "Cập nhật số lượng thành công" });
        }

        [HttpDelete("remove/{chiTietId}")]
        public async Task<IActionResult> RemoveItem(int chiTietId)
        {
            var result = await _gioHangService.RemoveItemAsync(chiTietId);
            if (!result) return NotFound(new { message = "Chi tiết giỏ hàng không tồn tại" });
            return NoContent();
        }

        [HttpDelete("clear/{khachHangId}")]
        public async Task<IActionResult> ClearCart(int khachHangId)
        {
            var result = await _gioHangService.ClearCartAsync(khachHangId);
            if (!result) return NotFound(new { message = "Không tìm thấy giỏ hàng" });
            return NoContent();
        }
    }
}