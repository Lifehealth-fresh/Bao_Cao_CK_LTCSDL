using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;

namespace QLBH.BLL.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IRepository<Voucher> _voucherRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<VoucherService> _logger;

        public VoucherService(IRepository<Voucher> voucherRepo, IMapper mapper, ILogger<VoucherService> logger)
        {
            _voucherRepo = voucherRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<VoucherDto>> GetAllAsync()
        {
            try
            {
                var list = await _voucherRepo.GetAll().ToListAsync();
                return _mapper.Map<IEnumerable<VoucherDto>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách voucher");
                throw new ApplicationException("Không thể tải danh sách voucher.");
            }
        }

        public async Task<VoucherDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _voucherRepo.GetByIdAsync(id);
                return entity == null ? null : _mapper.Map<VoucherDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy voucher {VoucherId}", id);
                throw new ApplicationException("Không thể tải thông tin voucher.");
            }
        }

        public async Task<VoucherDto> CreateAsync(CreateVoucherDto dto)
        {
            try
            {
                var entity = _mapper.Map<Voucher>(dto);
                entity.DaSuDung = 0;
                await _voucherRepo.AddAsync(entity);
                await _voucherRepo.SaveChangesAsync();
                return _mapper.Map<VoucherDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo voucher mới: {@Dto}", dto);
                throw new ApplicationException("Tạo voucher thất bại. Kiểm tra mã voucher trùng lặp.");
            }
        }

        public async Task<VoucherDto?> UpdateAsync(UpdateVoucherDto dto)
        {
            try
            {
                var entity = await _voucherRepo.GetByIdAsync(dto.VoucherID);
                if (entity == null) return null;
                _mapper.Map(dto, entity);
                _voucherRepo.Update(entity);
                await _voucherRepo.SaveChangesAsync();
                return _mapper.Map<VoucherDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật voucher {VoucherId}", dto.VoucherID);
                throw new ApplicationException("Cập nhật voucher thất bại.");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _voucherRepo.GetByIdAsync(id);
                if (entity == null) return false;
                _voucherRepo.Delete(entity);
                await _voucherRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa voucher {VoucherId}", id);
                throw new ApplicationException("Xóa voucher thất bại.");
            }
        }

        public async Task<List<VoucherDto>> GetAvailableVouchersAsync(decimal tongTamTinh)
        {
            var now = DateTime.Now;
            var vouchers = await _voucherRepo.GetAll()
                .Where(v => v.TrangThai == true
                            && v.NgayBatDau <= now
                            && v.NgayKetThuc >= now
                            && v.DaSuDung < v.SoLuong
                            && v.DonToiThieu <= tongTamTinh)
                .OrderBy(v => v.GiaTriGiam)
                .ToListAsync();
            return _mapper.Map<List<VoucherDto>>(vouchers);
        }

        public async Task<VoucherDto?> ValidateVoucherAsync(string maVoucher, decimal tongTamTinh)
        {
            var now = DateTime.Now;
            var voucher = await _voucherRepo.GetAll()
                .FirstOrDefaultAsync(v => v.MaVoucher == maVoucher
                                          && v.TrangThai == true
                                          && v.NgayBatDau <= now
                                          && v.NgayKetThuc >= now
                                          && v.DaSuDung < v.SoLuong
                                          && v.DonToiThieu <= tongTamTinh);
            return voucher == null ? null : _mapper.Map<VoucherDto>(voucher);
        }
    }


}