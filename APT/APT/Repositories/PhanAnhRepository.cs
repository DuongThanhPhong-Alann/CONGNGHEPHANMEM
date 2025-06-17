using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLCCCC.Repositories
{
    public class PhanAnhRepository : IPhanAnhRepository
    {
        private readonly ApplicationDbContext _context;

        public PhanAnhRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhanAnh>> GetAllAsync()
        {
            return await _context.PhanAnhs.Include(p => p.NguoiDung).ToListAsync();
        }

        public async Task<PhanAnh?> GetByIdAsync(int id)
        {
            return await _context.PhanAnhs.Include(p => p.NguoiDung).FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task AddAsync(PhanAnh phanAnh)
        {
            await _context.PhanAnhs.AddAsync(phanAnh);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PhanAnh phanAnh)
        {
            _context.PhanAnhs.Update(phanAnh);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var phanAnh = await _context.PhanAnhs.FindAsync(id);
            if (phanAnh != null)
            {
                _context.PhanAnhs.Remove(phanAnh);
                await _context.SaveChangesAsync();
            }
        }
    }
}
