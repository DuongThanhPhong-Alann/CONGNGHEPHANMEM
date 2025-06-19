using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;

namespace QLCCCC.Repositories
{
    public class ChungCuRepository : IChungCuRepository
    {
        private readonly ApplicationDbContext _context;

        public ChungCuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChungCu>> GetAllAsync()
        {
            return await _context.ChungCus
                .Include(c => c.HinhAnhChungCus) // Đảm bảo load hình ảnh
                .ToListAsync();
        }

        public async Task<ChungCu?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ChungCus
                .Include(c => c.HinhAnhChungCus) // Load hình ảnh chi tiết
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<ChungCu?> GetByIdAsync(int id)
        {
            return await _context.ChungCus.FindAsync(id);
        }

        public async Task AddAsync(ChungCu chungCu)
        {
            _context.ChungCus.Add(chungCu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChungCu chungCu)
        {
            _context.ChungCus.Update(chungCu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var chungCu = await _context.ChungCus
                .Include(c => c.HinhAnhChungCus) // Load hình ảnh liên quan
                .FirstOrDefaultAsync(c => c.ID == id);

            if (chungCu == null)
            {
                throw new Exception("Không tìm thấy chung cư!");
            }

            // Xóa hình ảnh trước khi xóa chung cư
            _context.HinhAnhChungCus.RemoveRange(chungCu.HinhAnhChungCus);

            // Xóa chung cư sau
            _context.ChungCus.Remove(chungCu);

            await _context.SaveChangesAsync();
        }
    }
}
