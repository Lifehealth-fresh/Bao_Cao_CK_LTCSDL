using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBH.BLL.Services;
using QLBH.DAL.Data;
using System.Text;
using System.Xml.Linq;


namespace QLBH.Web.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class ThongKeController : Controller
    {
        private readonly IThongKeService _thongKeService;
        private readonly ILogger<ThongKeController> _logger;
        private readonly AppDbContext _context; // thêm dòng này

        // Sửa constructor
        public ThongKeController(IThongKeService thongKeService,
                                  ILogger<ThongKeController> logger,
                                  AppDbContext context) // inject DbContext
        {
            _thongKeService = thongKeService;
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var summary = await _thongKeService.GetDashboardSummaryAsync();
                return View(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi dashboard");
                TempData["Error"] = "Không thể tải dữ liệu thống kê.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> DoanhThuTheoNgay(DateTime? tuNgay, DateTime? denNgay)
        {
            var data = await _thongKeService.GetDoanhThuTheoNgayAsync(tuNgay, denNgay);
            return View(data);
        }

        public async Task<IActionResult> DoanhThuTheoThang(int? nam)
        {
            var data = await _thongKeService.GetDoanhThuTheoThangAsync(nam);
            return View(data);
        }

        public async Task<IActionResult> DoanhThuTheoDanhMuc()
        {
            var data = await _thongKeService.GetDoanhThuTheoDanhMucAsync();
            return View(data);
        }

        [HttpGet("ExportXml")]
        public async Task<IActionResult> ExportXml(string type = "ngay", DateTime? tuNgay = null, DateTime? denNgay = null, int? nam = null)
        {
            string xmlString = "";
            string fileName = "";
            switch (type.ToLower())
            {
                case "ngay":
                    xmlString = await _thongKeService.ExportDoanhThuTheoNgayXmlAsync(tuNgay, denNgay);
                    fileName = $"DoanhThuTheoNgay_{DateTime.Now:yyyyMMddHHmmss}.xml";
                    break;
                case "thang":
                    xmlString = await _thongKeService.ExportDoanhThuTheoThangXmlAsync(nam);
                    fileName = $"DoanhThuTheoThang_{DateTime.Now:yyyyMMddHHmmss}.xml";
                    break;
                case "danhmuc":
                    xmlString = await _thongKeService.ExportDoanhThuTheoDanhMucXmlAsync();
                    fileName = $"DoanhThuTheoDanhMuc_{DateTime.Now:yyyyMMddHHmmss}.xml";
                    break;
                case "dashboard":
                    xmlString = await _thongKeService.ExportDashboardXmlAsync();
                    fileName = $"Dashboard_{DateTime.Now:yyyyMMddHHmmss}.xml";
                    break;
                default:
                    return BadRequest("Loại báo cáo không hợp lệ.");
            }
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xmlString);
            return File(xmlBytes, "application/xml", fileName);
        }
        // Hiển thị danh sách sản phẩm sắp hết
        public async Task<IActionResult> DanhSachSanPhamSapHet()
        {
            var data = await _thongKeService.GetSanPhamSapHetAsync();
            return View(data);
        }

        // Hiển thị danh sách đơn hàng thành công
        public async Task<IActionResult> DanhSachDonHangThanhCong()
        {
            var data = await _thongKeService.GetDonHangThanhCongAsync();
            return View(data);
        }
    }
}