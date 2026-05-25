using Microsoft.AspNetCore.Mvc;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.Web.Models;
using System.Diagnostics;

namespace QLBH.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ISanPhamService sanPhamService, ILogger<HomeController> logger)
        {
            _sanPhamService = sanPhamService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhams = await _sanPhamService.GetAllAsync(null, null, null, null, chiLaySanPhamHoatDong: true);
            return View(sanPhams.Take(8));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message)
        {
            ViewBag.ErrorMessage = message;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}