using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCCCC.Repositories
{
    public class CanHoRepository : ICanHoRepository
    {
        private readonly ApplicationDbContext _context;

        public CanHoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CanHo>> GetAllAsync()
        {
            return await _context.CanHos.ToListAsync();
        }

        public async Task<IEnumerable<CanHo>> GetAllWithDetailsAsync()
        {
            return await _context.CanHos
                .Include(c => c.HinhAnhCanHos)
                .Include(c => c.ChungCu)
                .ToListAsync();
        }

        public async Task<CanHo> GetByIdAsync(int id)
        {
            return await _context.CanHos.FindAsync(id);
        }

        public async Task<CanHo> GetByIdWithDetailsAsync(int id)
        {
            return await _context.CanHos
                .Include(c => c.HinhAnhCanHos)
                .Include(c => c.ChungCu)
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task UpdateAsync(CanHo canHo)
        {
            _context.CanHos.Update(canHo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var canHo = await _context.CanHos.FindAsync(id);
            if (canHo != null)
            {
                _context.CanHos.Remove(canHo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteImagesAsync(ICollection<HinhAnhCanHo> images)
        {
            if (images != null && images.Any())
            {
                foreach (var image in images.ToList())
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.DuongDan.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    _context.HinhAnhCanHos.Remove(image);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}