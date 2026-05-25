using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLBH.BLL.DTOs;
using QLBH.DAL.Entities;
using QLBH.DAL.Repositories;

namespace QLBH.BLL.Services
{
    public class DanhMucService : IDanhMucService
    {
        private readonly IRepository<DanhMuc> _danhMucRepo;
        private readonly IMapper _mapper;

        public DanhMucService(IRepository<DanhMuc> danhMucRepo, IMapper mapper)
        {
            _danhMucRepo = danhMucRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DanhMucDto>> GetAllAsync()
        {
            var list = await _danhMucRepo.GetAll().ToListAsync();
            return _mapper.Map<IEnumerable<DanhMucDto>>(list);
        }

        public async Task<DanhMucDto?> GetByIdAsync(int id)
        {
            var entity = await _danhMucRepo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<DanhMucDto>(entity);
        }

        public async Task<DanhMucDto> CreateAsync(CreateDanhMucDto dto)
        {
            var entity = _mapper.Map<DanhMuc>(dto);
            entity.TrangThai = true;
            await _danhMucRepo.AddAsync(entity);
            await _danhMucRepo.SaveChangesAsync();
            return _mapper.Map<DanhMucDto>(entity);
        }

        public async Task<DanhMucDto?> UpdateAsync(UpdateDanhMucDto dto)
        {
            var entity = await _danhMucRepo.GetByIdAsync(dto.DanhMucID);
            if (entity == null) return null;
            _mapper.Map(dto, entity);
            _danhMucRepo.Update(entity);
            await _danhMucRepo.SaveChangesAsync();
            return _mapper.Map<DanhMucDto>(entity);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var entity = await _danhMucRepo.GetByIdAsync(id);
            if (entity == null) return (false, "Danh mục không tồn tại.");

            // Kiểm tra xem danh mục có sản phẩm nào không
            var hasProducts = await _danhMucRepo.GetDbContext().Set<SanPham>()
                .AnyAsync(s => s.DanhMucID == id);

            if (hasProducts)
            {
                // Xóa mềm: chỉ vô hiệu hóa (giả sử cột TrangThai kiểu bool hoặc string)
                entity.TrangThai = false;   // hoặc "Ngung" nếu dùng string
                _danhMucRepo.Update(entity);
                await _danhMucRepo.SaveChangesAsync();
                return (true, "Danh mục đang có sản phẩm nên chỉ được vô hiệu hóa. Muốn xóa hẳn, hãy xóa các sản phẩm trước.");
            }

            else
            {
                // Xóa cứng
                _danhMucRepo.Delete(entity);
                await _danhMucRepo.SaveChangesAsync();
                return (true, "Xóa danh mục thành công.");
            }
        }
    }
    
}