using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;
using QLBH.DAL.Helpers;
using Microsoft.Data.SqlClient;

namespace QLBH.BLL.Services
{
    public class ThanhToanService : IThanhToanService
    {
        private readonly IRepository<ThanhToan> _thanhToanRepo;
        private readonly IMapper _mapper;
        private readonly SqlHelper _sqlHelper;
        private readonly ILogger<ThanhToanService> _logger;

        public ThanhToanService(IRepository<ThanhToan> thanhToanRepo, IMapper mapper, SqlHelper sqlHelper, ILogger<ThanhToanService> logger)
        {
            _thanhToanRepo = thanhToanRepo;
            _mapper = mapper;
            _sqlHelper = sqlHelper;
            _logger = logger;
        }

        public async Task<IEnumerable<ThanhToanDto>> GetPendingPaymentsAsync()
        {
            try
            {
                var list = await _thanhToanRepo.GetAll()
                    .Where(x => x.TrangThai == "ChoDuyet")
                    .Include(x => x.DonHang)
                    .ThenInclude(dh => dh!.KhachHang)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<ThanhToanDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách thanh toán chờ duyệt");
                throw new ApplicationException("Không thể tải danh sách thanh toán.");
            }
        }

        public async Task<bool> ProcessPaymentAsync(ConfirmPaymentDto dto)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@ThanhToanID", dto.ThanhToanID),
                    new SqlParameter("@NhanVienID", dto.NhanVienID),
                    new SqlParameter("@ChapNhan", dto.ChapNhan ? 1 : 0),
                    new SqlParameter("@GhiChu", string.IsNullOrEmpty(dto.GhiChu) ? DBNull.Value : (object)dto.GhiChu)
                };
                await _sqlHelper.ExecuteStoredProcedureAsync("sp_XacNhanThanhToan", parameters);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý thanh toán: {@Dto}", dto);
                throw new ApplicationException("Xử lý thanh toán thất bại. Vui lòng thử lại.");
            }
        }

        public async Task<IEnumerable<ThanhToanDto>> GetHistoryByKhachHangIdAsync(int khachHangId)
        {
            try
            {
                var list = await _thanhToanRepo.GetAll()
                    .Where(x => x.DonHang != null && x.DonHang.KhachHangID == khachHangId)
                    .Include(x => x.DonHang)
                    .OrderByDescending(x => x.NgayXuLy)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<ThanhToanDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy lịch sử thanh toán của khách {KhachHangId}", khachHangId);
                throw new ApplicationException("Không thể tải lịch sử thanh toán.");
            }
        }
    }
}