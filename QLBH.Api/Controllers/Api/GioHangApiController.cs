using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.DAL.Data;
using System.Security.Claims;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "KhachHang")]
    public class GioHangApiController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;
        private readonly AppDbContext _context;

        public GioHangApiController(IGioHangService gioHangService, AppDbContext context)
        {
            _gioHangService = gioHangService;
            _context = context;
        }

        private int? GetKhachHangIdFromToken()
        {
            var tkClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(tkClaim, out var taiKhoanId)) return null;
            return _context.KhachHangs.Where(x => x.TaiKhoanID == taiKhoanId).Select(x => (int?)x.KhachHangID).FirstOrDefault();
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyCart()
        {
            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });

            var cart = await _gioHangService.GetCartByKhachHangIdAsync(khachHangId.Value);
            if (cart == null) return NotFound(new { message = "Không tìm thấy giỏ hàng" });
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });
            dto.KhachHangID = khachHangId.Value;

            var result = await _gioHangService.AddToCartAsync(dto);
            if (!result) return BadRequest(new { message = "Thêm vào giỏ thất bại" });
            return Ok(new { message = "Đã thêm sản phẩm vào giỏ" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });

            var belongsToUser = await _context.ChiTietGioHangs
                .Include(x => x.GioHang)
                .AnyAsync(x => x.ChiTietID == dto.ChiTietID && x.GioHang.KhachHangID == khachHangId.Value);

            if (!belongsToUser) return Forbid();

            var result = await _gioHangService.UpdateQuantityAsync(dto);
            if (!result) return NotFound(new { message = "Chi tiết giỏ hàng không tồn tại" });
            return Ok(new { message = "Cập nhật số lượng thành công" });
        }

        [HttpDelete("remove/{chiTietId}")]
        public async Task<IActionResult> RemoveItem(int chiTietId)
        {
            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });

            var belongsToUser = await _context.ChiTietGioHangs
                .Include(x => x.GioHang)
                .AnyAsync(x => x.ChiTietID == chiTietId && x.GioHang.KhachHangID == khachHangId.Value);

            if (!belongsToUser) return Forbid();

            var result = await _gioHangService.RemoveItemAsync(chiTietId);
            if (!result) return NotFound(new { message = "Chi tiết giỏ hàng không tồn tại" });
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearMyCart()
        {
            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });

            var result = await _gioHangService.ClearCartAsync(khachHangId.Value);
            if (!result) return NotFound(new { message = "Không tìm thấy giỏ hàng" });
            return NoContent();
        }
    }
}
