using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.DTOs;
using QLBH.BLL.Services;
using QLBH.DAL.Entities;
using QLBH.DAL.Helpers;
using QLBH.DAL.Repositories;
using System.Data;

public class DonHangService : IDonHangService
{
    private readonly IRepository<DonHang> _donHangRepo;
    private readonly IRepository<ChiTietDonHang> _chiTietRepo;
    private readonly IRepository<KhachHang> _khachHangRepo;
    private readonly IMapper _mapper;
    private readonly SqlHelper _sqlHelper;
    private readonly IGioHangService _gioHangService;   // Thêm
    private readonly IVoucherService _voucherService;   // Thêm
    private readonly ILogger<DonHangService> _logger;

    public DonHangService(
        IRepository<DonHang> donHangRepo,
        IRepository<ChiTietDonHang> chiTietRepo,
        IRepository<KhachHang> khachHangRepo,
        IMapper mapper,
        SqlHelper sqlHelper,
        IGioHangService gioHangService,    // Thêm
        IVoucherService voucherService,    // Thêm
        ILogger<DonHangService> logger)
    {
        _donHangRepo = donHangRepo;
        _chiTietRepo = chiTietRepo;
        _khachHangRepo = khachHangRepo;
        _mapper = mapper;
        _sqlHelper = sqlHelper;
        _gioHangService = gioHangService;
        _voucherService = voucherService;
        _logger = logger;
    }

    public async Task<DonHangDto> CreateOrderAsync(CreateDonHangDto dto)
    {
        try
        {
            var cart = await _gioHangService.GetCartByKhachHangIdAsync(dto.KhachHangID);
            if (cart == null || !cart.ChiTietGioHangs.Any())
                throw new ApplicationException("Giỏ hàng trống.");

            var parameters = new[]
            {
            new SqlParameter("@KhachHangID", dto.KhachHangID),
            new SqlParameter("@TenNguoiNhan", dto.TenNguoiNhan),
            new SqlParameter("@SoDienThoai", dto.SoDienThoai),
            new SqlParameter("@DiaChi", dto.DiaChi),
            new SqlParameter("@MaVoucher", string.IsNullOrEmpty(dto.MaVoucher) ? DBNull.Value : (object)dto.MaVoucher),
            new SqlParameter("@GhiChu", dto.GhiChu ?? string.Empty),  // ✅ sửa ở đây
            new SqlParameter("@PhuongThucThanhToan", dto.PhuongThucThanhToan),
            new SqlParameter("@DonHangID", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };
            await _sqlHelper.ExecuteStoredProcedureAsync("sp_TaoDonHangTuGio", parameters);
            var donHangId = (int)parameters.Last().Value;
            return await GetByIdAsync(donHangId) ?? throw new Exception("Không thể tạo đơn hàng");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi tạo đơn hàng: {@Dto}", dto);
            throw new ApplicationException("Đặt hàng thất bại. Vui lòng kiểm tra lại giỏ hàng và tồn kho.");
        }
    }


    public async Task<IEnumerable<DonHangDto>> GetAllAsync()
        {
            try
            {
                var list = await _donHangRepo.GetAll()
                    .Include(x => x.KhachHang)
                    .Include(x => x.ChiTietDonHangs)
                    .OrderByDescending(x => x.NgayDat)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<DonHangDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách đơn hàng");
                throw new ApplicationException("Không thể tải danh sách đơn hàng.");
            }
        }

        public async Task<DonHangDto?> GetByIdAsync(int donHangId, int? khachHangId = null)
        {
            try
            {
                var query = _donHangRepo.GetAll()
                    .Include(x => x.KhachHang)
                    .Include(x => x.ChiTietDonHangs)
                    .Where(x => x.DonHangID == donHangId);
                if (khachHangId.HasValue)
                    query = query.Where(x => x.KhachHangID == khachHangId.Value);
                var entity = await query.FirstOrDefaultAsync();
                return entity == null ? null : _mapper.Map<DonHangDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy chi tiết đơn hàng {DonHangId}", donHangId);
                throw new ApplicationException("Không thể tải chi tiết đơn hàng.");
            }
        }

        public async Task<IEnumerable<DonHangDto>> GetHistoryByKhachHangIdAsync(int khachHangId)
        {
            try
            {
                var list = await _donHangRepo.GetAll()
                    .Include(x => x.KhachHang)
                    .Include(x => x.ChiTietDonHangs)
                    .Where(x => x.KhachHangID == khachHangId)
                    .OrderByDescending(x => x.NgayDat)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<DonHangDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy lịch sử đơn hàng của khách {KhachHangId}", khachHangId);
                throw new ApplicationException("Không thể tải lịch sử đơn hàng.");
            }
        }

        public async Task<IEnumerable<DonHangDto>> SearchAsync(string? keyword, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            try
            {
                var query = _donHangRepo.GetAll()
                    .Include(x => x.KhachHang)
                    .Include(x => x.ChiTietDonHangs)
                    .AsQueryable();
                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(x => x.DonHangID.ToString().Contains(keyword) ||
                                              (x.KhachHang != null && x.KhachHang.HoTen.Contains(keyword)));
                if (!string.IsNullOrWhiteSpace(trangThai))
                    query = query.Where(x => x.TrangThai == trangThai);
                if (tuNgay.HasValue)
                    query = query.Where(x => x.NgayDat >= tuNgay.Value);
                if (denNgay.HasValue)
                    query = query.Where(x => x.NgayDat <= denNgay.Value);
                var list = await query.OrderByDescending(x => x.NgayDat).ToListAsync();
                return _mapper.Map<IEnumerable<DonHangDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tìm kiếm đơn hàng với keyword={Keyword}, trangThai={TrangThai}", keyword, trangThai);
                throw new ApplicationException("Tìm kiếm đơn hàng thất bại.");
            }
        }

        public async Task<bool> UpdateTrangThaiAsync(int donHangId, string trangThaiMoi)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@DonHangID", donHangId),
                    new SqlParameter("@TrangThaiMoi", trangThaiMoi)
                };
                await _sqlHelper.ExecuteStoredProcedureAsync("sp_CapNhatTrangThaiDonHang", parameters);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật trạng thái đơn hàng {DonHangId} thành {TrangThaiMoi}", donHangId, trangThaiMoi);
                throw new ApplicationException("Cập nhật trạng thái đơn hàng thất bại.");
            }
        }
    }
