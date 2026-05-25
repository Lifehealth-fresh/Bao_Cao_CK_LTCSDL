using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.DAL.Data;
using System.Security.Claims;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangApiController : ControllerBase
    {
        private readonly IDonHangService _donHangService;
        private readonly AppDbContext _context;

        public DonHangApiController(IDonHangService donHangService, AppDbContext context)
        {
            _donHangService = donHangService;
            _context = context;
        }

        private int GetTaiKhoanId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        private int? GetKhachHangIdFromToken()
        {
            var taiKhoanId = GetTaiKhoanId();
            if (taiKhoanId <= 0) return null;
            return _context.KhachHangs.Where(x => x.TaiKhoanID == taiKhoanId).Select(x => (int?)x.KhachHangID).FirstOrDefault();
        }

        [Authorize(Roles = "KhachHang")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateDonHangDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });

            dto.KhachHangID = khachHangId.Value;

            try
            {
                var result = await _donHangService.CreateOrderAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var list = await _donHangService.GetAllAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpGet("{donHangId}")]
        public async Task<IActionResult> GetOrderById(int donHangId)
        {
            int? khachHangId = null;
            if (User.IsInRole("KhachHang"))
            {
                khachHangId = GetKhachHangIdFromToken();
                if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });
            }

            var order = await _donHangService.GetByIdAsync(donHangId, khachHangId);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [Authorize(Roles = "KhachHang")]
        [HttpGet("history")]
        public async Task<IActionResult> GetMyHistory()
        {
            var khachHangId = GetKhachHangIdFromToken();
            if (!khachHangId.HasValue) return Unauthorized(new { message = "Không xác định được khách hàng." });

            var list = await _donHangService.GetHistoryByKhachHangIdAsync(khachHangId.Value);
            return Ok(list);
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? keyword, [FromQuery] string? trangThai, [FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
        {
            var list = await _donHangService.SearchAsync(keyword, trangThai, tuNgay, denNgay);
            return Ok(new
            {
                keywordGuide = "keyword = mã đơn hàng hoặc tên khách hàng (có thể để trống)",
                items = list
            });
        }

        [Authorize(Roles = "NhanVien,Admin")]
        [HttpPut("status/{donHangId}")]
        public async Task<IActionResult> UpdateStatus(int donHangId, [FromBody] string trangThaiMoi)
        {
            if (string.IsNullOrWhiteSpace(trangThaiMoi)) return BadRequest(new { message = "Trạng thái mới không được để trống." });
            var result = await _donHangService.UpdateTrangThaiAsync(donHangId, trangThaiMoi);
            if (!result) return NotFound();
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }
    }
}
