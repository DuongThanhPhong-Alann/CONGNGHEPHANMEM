using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLCCCC.Repositories
{
    public class TinTucRepository : ITinTucRepository
    {
        private readonly ApplicationDbContext _context;

        public TinTucRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả tin tức
        public async Task<IEnumerable<TinTuc>> GetAllAsync()
        {
            return await _context.TinTucs.ToListAsync();
        }

        // Lấy tin tức theo ID
        public async Task<TinTuc?> GetByIdAsync(int id)
        {
            return await _context.TinTucs.FindAsync(id);
        }

        // Thêm tin tức mới
        public async Task AddAsync(TinTuc tinTuc)
        {
            await _context.TinTucs.AddAsync(tinTuc);
            await _context.SaveChangesAsync();
        }

        // Cập nhật tin tức
        public async Task UpdateAsync(TinTuc tinTuc)
        {
            _context.TinTucs.Update(tinTuc);
            await _context.SaveChangesAsync();
        }

        // Xóa tin tức theo ID
        public async Task DeleteAsync(int id)
        {
            var tinTuc = await _context.TinTucs.FindAsync(id);
            if (tinTuc != null)
            {
                _context.TinTucs.Remove(tinTuc);
                await _context.SaveChangesAsync();
            }
        }
    }
}