using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLBH.BLL.Constants;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;

namespace QLBH.BLL.Services
{
    public class BienTheSanPhamService : IBienTheSanPhamService
    {
        private readonly IRepository<BienTheSanPham> _bienTheRepo;
        private readonly IRepository<SanPham> _sanPhamRepo;
        private readonly IMapper _mapper;

        public BienTheSanPhamService(
            IRepository<BienTheSanPham> bienTheRepo,
            IRepository<SanPham> sanPhamRepo,
            IMapper mapper)
        {
            _bienTheRepo = bienTheRepo;
            _sanPhamRepo = sanPhamRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BienTheSanPhamDto>> GetAllAsync()
        {
            var list = await _bienTheRepo.GetAll()
                .Include(x => x.SanPham)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BienTheSanPhamDto>>(list);
        }

        public async Task<IEnumerable<BienTheSanPhamDto>> GetBySanPhamIdAsync(int sanPhamId)
        {
            var list = await _bienTheRepo.GetAll()
                .Include(x => x.SanPham)
                .Where(x => x.SanPhamID == sanPhamId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BienTheSanPhamDto>>(list);
        }

        public async Task<BienTheSanPhamDto?> GetByIdAsync(int id)
        {
            var entity = await _bienTheRepo.GetByIdAsync(id);
            if (entity == null) return null;
            return _mapper.Map<BienTheSanPhamDto>(entity);
        }

        public async Task<BienTheSanPhamDto> CreateAsync(CreateBienTheSanPhamDto dto)
        {
            var sanPham = await _sanPhamRepo.GetByIdAsync(dto.SanPhamID);
            if (sanPham == null)
                throw new ApplicationException("Sản phẩm không tồn tại.");

            var entity = _mapper.Map<BienTheSanPham>(dto);
            entity.TrangThai = !SanPhamTrangThai.IsNgungBan(sanPham.TrangThai);
            await _bienTheRepo.AddAsync(entity);
            await _bienTheRepo.SaveChangesAsync();
            return _mapper.Map<BienTheSanPhamDto>(entity);
        }

        public async Task<BienTheSanPhamDto?> UpdateAsync(UpdateBienTheSanPhamDto dto)
        {
            var sanPham = await _sanPhamRepo.GetByIdAsync(dto.SanPhamID);
            if (sanPham != null && SanPhamTrangThai.IsNgungBan(sanPham.TrangThai) && dto.TrangThai)
                throw new ApplicationException("Không thể kích hoạt biến thể khi sản phẩm đang ngưng hoạt động.");

            var entity = await _bienTheRepo.GetByIdAsync(dto.BienTheID);
            if (entity == null) return null;
            _mapper.Map(dto, entity);
            _bienTheRepo.Update(entity);
            await _bienTheRepo.SaveChangesAsync();
            return _mapper.Map<BienTheSanPhamDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _bienTheRepo.GetByIdAsync(id);
            if (entity == null) return false;
            _bienTheRepo.Delete(entity);
            await _bienTheRepo.SaveChangesAsync();
            return true;
        }
    }
}