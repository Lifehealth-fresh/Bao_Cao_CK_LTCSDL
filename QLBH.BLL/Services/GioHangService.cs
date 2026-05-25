using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;
using QLBH.DAL.Helpers;
using Microsoft.Data.SqlClient;

namespace QLBH.BLL.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly IRepository<GioHang> _gioHangRepo;
        private readonly IRepository<ChiTietGioHang> _chiTietRepo;
        private readonly IRepository<KhachHang> _khachHangRepo;
        private readonly IRepository<BienTheSanPham> _bienTheRepo;
        private readonly IMapper _mapper;
        private readonly SqlHelper _sqlHelper;
        private readonly ILogger<GioHangService> _logger;

        public GioHangService(
            IRepository<GioHang> gioHangRepo,
            IRepository<ChiTietGioHang> chiTietRepo,
            IRepository<KhachHang> khachHangRepo,
            IRepository<BienTheSanPham> bienTheRepo,
            IMapper mapper,
            SqlHelper sqlHelper,
            ILogger<GioHangService> logger)
        {
            _gioHangRepo = gioHangRepo;
            _chiTietRepo = chiTietRepo;
            _khachHangRepo = khachHangRepo;
            _bienTheRepo = bienTheRepo;
            _mapper = mapper;
            _sqlHelper = sqlHelper;
            _logger = logger;
        }

        public async Task<GioHangDto?> GetCartByKhachHangIdAsync(int khachHangId)
        {
            var gioHang = await _gioHangRepo.GetAll()
                .Include(x => x.ChiTietGioHangs)
                    .ThenInclude(ct => ct.BienTheSanPham)
                        .ThenInclude(bt => bt.SanPham)
                .FirstOrDefaultAsync(x => x.KhachHangID == khachHangId);
            if (gioHang == null) return null;

            var khachHang = await _khachHangRepo.GetByIdAsync(khachHangId);
            var chiTiets = new List<ChiTietGioHangDto>();
            foreach (var ct in gioHang.ChiTietGioHangs)
            {
                var bienThe = ct.BienTheSanPham;
                if (bienThe == null) continue;
                chiTiets.Add(new ChiTietGioHangDto
                {
                    ChiTietID = ct.ChiTietID,
                    BienTheID = ct.BienTheID,
                    TenSanPham = bienThe.SanPham?.TenSanPham ?? "",
                    Size = bienThe.Size ?? "",
                    MauSac = bienThe.MauSac ?? "",
                    DonGia = bienThe.Gia,
                    SoLuong = ct.SoLuong
                });
            }
            return new GioHangDto
            {
                GioHangID = gioHang.GioHangID,
                KhachHangID = khachHangId,
                TenKhachHang = khachHang?.HoTen ?? "",
                ChiTietGioHangs = chiTiets
            };
        }
        public async Task<bool> AddToCartAsync(AddToCartDto dto)
        {
            // Kiểm tra biến thể tồn tại, trạng thái và tồn kho
            var bienThe = await _bienTheRepo.GetAll()
                .Include(x => x.SanPham)
                .FirstOrDefaultAsync(x => x.BienTheID == dto.BienTheID);
            if (bienThe == null)
                throw new ApplicationException("Sản phẩm không tồn tại.");

            if (bienThe.TrangThai != true)
                throw new ApplicationException("Sản phẩm này hiện không khả dụng để bán.");

            if (bienThe.SanPham != null && SanPhamTrangThai.IsNgungBan(bienThe.SanPham.TrangThai))
                throw new ApplicationException("Sản phẩm này hiện không khả dụng để bán.");

            if (bienThe.SoLuongTon < dto.SoLuong)
                throw new ApplicationException($"Chỉ còn {bienThe.SoLuongTon} sản phẩm trong kho.");

            var parameters = new[]
            {
            new SqlParameter("@KhachHangID", dto.KhachHangID),
            new SqlParameter("@BienTheID", dto.BienTheID),
            new SqlParameter("@SoLuong", dto.SoLuong)
            };
            await _sqlHelper.ExecuteStoredProcedureAsync("sp_ThemVaoGio", parameters);
            return true;
        }

        public async Task<bool> UpdateQuantityAsync(UpdateCartItemDto dto)
        {
            // Lấy chi tiết giỏ hàng kèm theo biến thể
            var item = await _chiTietRepo.GetAll()
                .Include(x => x.BienTheSanPham)
                .FirstOrDefaultAsync(x => x.ChiTietID == dto.ChiTietID);

            if (item == null) return false;

            // KIỂM TRA TỒN KHO
            var bienThe = item.BienTheSanPham;
            if (bienThe == null)
                throw new ApplicationException("Sản phẩm không tồn tại.");

            if (bienThe.SoLuongTon < dto.SoLuong)
                throw new ApplicationException($"Chỉ còn {bienThe.SoLuongTon} sản phẩm trong kho. Không thể cập nhật số lượng {dto.SoLuong}.");

            // Cập nhật số lượng
            item.SoLuong = dto.SoLuong;
            _chiTietRepo.Update(item);
            await _chiTietRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItemAsync(int chiTietId)
        {
            var item = await _chiTietRepo.GetByIdAsync(chiTietId);
            if (item == null) return false;
            _chiTietRepo.Delete(item);
            await _chiTietRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int khachHangId)
        {
            var gioHang = await _gioHangRepo.GetAll().FirstOrDefaultAsync(x => x.KhachHangID == khachHangId);
            if (gioHang == null) return false;
            var items = await _chiTietRepo.FindAsync(x => x.GioHangID == gioHang.GioHangID);
            foreach (var item in items)
                _chiTietRepo.Delete(item);
            await _chiTietRepo.SaveChangesAsync();
            return true;
        }
    }
}