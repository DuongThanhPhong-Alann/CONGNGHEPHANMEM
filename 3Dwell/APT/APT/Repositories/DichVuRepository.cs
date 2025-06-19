using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;

namespace QLCCCC.Repositories
{
    public class DichVuRepository : IDichVuRepository
    {
        private readonly ApplicationDbContext _context;

        public DichVuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DichVu>> GetAllAsync()
        {
            return await _context.DichVus.ToListAsync();
        }

        public async Task<DichVu?> GetByIdAsync(int id)
        {
            return await _context.DichVus.FindAsync(id);
        }

        public async Task AddAsync(DichVu dichVu)
        {
            _context.DichVus.Add(dichVu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DichVu dichVu)
        {
            _context.DichVus.Update(dichVu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dichVu = await GetByIdAsync(id);
            if (dichVu != null)
            {
                _context.DichVus.Remove(dichVu);
                await _context.SaveChangesAsync();
            }
        }
    }
}
