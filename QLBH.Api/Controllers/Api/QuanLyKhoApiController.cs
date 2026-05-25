using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.DAL.Data;
using System.Security.Claims;

namespace QLBH.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "NhanVien,Admin")]
    public class QuanLyKhoApiController : ControllerBase
    {
        private readonly IQuanLyKhoService _khoService;
        private readonly AppDbContext _context;

        public QuanLyKhoApiController(IQuanLyKhoService khoService, AppDbContext context)
        {
            _khoService = khoService;
            _context = context;
        }

        [HttpGet("phieu-nhap")]
        public async Task<IActionResult> GetPhieuNhap([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _khoService.GetPhieuNhapAsync(tuNgay, denNgay, page, pageSize);
            return Ok(result);
        }

        [HttpGet("phieu-nhap/{id}")]
        public async Task<IActionResult> GetPhieuNhapById(int id)
        {
            var phieu = await _khoService.GetPhieuNhapByIdAsync(id);
            if (phieu == null) return NotFound();
            var chiTiet = await _khoService.GetChiTietPhieuNhapAsync(id);
            return Ok(new { phieu, chiTiet });
        }

        [HttpPost("phieu-nhap")]
        public async Task<IActionResult> TaoPhieuNhap([FromBody] TaoPhieuNhapDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.GhiChu?.Length > 500)
                return BadRequest(new { message = "Ghi chú tối đa 500 ký tự." });
            if (model.ChiTiet == null || !model.ChiTiet.Any())
                return BadRequest(new { message = "Vui lòng thêm ít nhất một sản phẩm." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(nv => nv.TaiKhoanID.ToString() == userId);
            if (nhanVien == null)
                return Unauthorized(new { message = "Không xác định được nhân viên." });

            try
            {
                var newId = await _khoService.TaoPhieuNhapAsync(model, nhanVien.NhanVienID);
                return CreatedAtAction(nameof(GetPhieuNhapById), new { id = newId }, new { phieuNhapId = newId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("bien-the-hoat-dong")]
        public async Task<IActionResult> GetBienTheHoatDong()
        {
            var bienTheList = await _context.BienTheSanPhams
                .Include(bt => bt.SanPham)
                .Where(bt => bt.TrangThai && bt.SanPham.TrangThai == SanPhamTrangThai.HoatDong)
                .Select(bt => new { bt.BienTheID, bt.SanPham.TenSanPham, bt.Size, bt.MauSac })
                .ToListAsync();

            return Ok(bienTheList);
        }
    }
}
