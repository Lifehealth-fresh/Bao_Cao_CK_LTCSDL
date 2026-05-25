using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.DAL.Data;
using System.Security.Claims;

namespace QLBH.Web.Controllers
{
    [Authorize(Roles = "NhanVien,Admin")]
    public class QuanLyKhoController : Controller
    {
        private readonly IQuanLyKhoService _khoService;
        private readonly AppDbContext _context;
        private readonly ILogger<QuanLyKhoController> _logger;

        public QuanLyKhoController(IQuanLyKhoService khoService, AppDbContext context, ILogger<QuanLyKhoController> logger)
        {
            _khoService = khoService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay, int page = 1)
        {
            const int pageSize = 10;
            var result = await _khoService.GetPhieuNhapAsync(tuNgay, denNgay, page, pageSize);
            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;
            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var phieuNhap = await _khoService.GetPhieuNhapByIdAsync(id);
            if (phieuNhap == null) return NotFound();
            var chiTiet = await _khoService.GetChiTietPhieuNhapAsync(id);
            ViewBag.ChiTiet = chiTiet;
            return View(phieuNhap);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách biến thể sản phẩm để dropdown
            var bienTheList = await _context.BienTheSanPhams
                .Include(bt => bt.SanPham)
                .Where(bt => bt.TrangThai && bt.SanPham.TrangThai == SanPhamTrangThai.HoatDong)
                .Select(bt => new { bt.BienTheID, bt.SanPham.TenSanPham, bt.Size, bt.MauSac })
                .ToListAsync();
            ViewBag.BienTheList = bienTheList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaoPhieuNhapDto model)
        {
            if (model.GhiChu?.Length > 500)
                ModelState.AddModelError(nameof(model.GhiChu), "Ghi chú tối đa 500 ký tự.");

            if (model.ChiTiet == null || !model.ChiTiet.Any())
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm.");

            for (int i = 0; i < model.ChiTiet?.Count; i++)
            {
                var ct = model.ChiTiet[i];
                if (ct.BienTheID <= 0)
                    ModelState.AddModelError($"ChiTiet[{i}].BienTheID", "Chọn sản phẩm");
                if (ct.SoLuong <= 0)
                    ModelState.AddModelError($"ChiTiet[{i}].SoLuong", "Số lượng phải lớn hơn 0");
                if (ct.GiaNhap <= 0)
                    ModelState.AddModelError($"ChiTiet[{i}].GiaNhap", "Giá nhập phải lớn hơn 0");
            }

            if (!ModelState.IsValid)
            {
                await LoadBienTheList();
                return View(model);
            }

            try
            {
                // Lấy NhanVienID từ claim (giả sử claim NameIdentifier là TaiKhoanID)
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(nv => nv.TaiKhoanID.ToString() == userId);
                if (nhanVien == null)
                {
                    TempData["Error"] = "Không xác định được nhân viên. Vui lòng đăng nhập lại.";
                    return RedirectToAction(nameof(Index));
                }

                var newId = await _khoService.TaoPhieuNhapAsync(model, nhanVien.NhanVienID);
                TempData["Success"] = $"Tạo phiếu nhập #{newId} thành công!";
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo phiếu nhập");
                TempData["Error"] = "Lỗi: " + ex.Message;
                await LoadBienTheList();
                return View(model);
            }
        }

        private async Task LoadBienTheList()
        {
            var bienTheList = await _context.BienTheSanPhams
                .Include(bt => bt.SanPham)
                .Where(bt => bt.TrangThai && bt.SanPham.TrangThai == SanPhamTrangThai.HoatDong)
                .Select(bt => new { bt.BienTheID, bt.SanPham.TenSanPham, bt.Size, bt.MauSac })
                .ToListAsync();
            ViewBag.BienTheList = bienTheList;
        }


    
    }
}
