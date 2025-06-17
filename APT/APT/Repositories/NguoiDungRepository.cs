using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLCCCC.Repositories
{
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NguoiDung>> GetAllAsync()
        {
            return await _context.NguoiDungs
                .Include(nd => nd.CuDan)
                .ToListAsync();
        }

        public async Task<NguoiDung?> GetByIdAsync(int id)
        {
            return await _context.NguoiDungs.FindAsync(id);
        }

        public async Task<NguoiDung?> GetByPhoneNumberAsync(string soDienThoai)
        {
            return await _context.NguoiDungs.FirstOrDefaultAsync(u => u.SoDienThoai == soDienThoai);
        }

        public async Task<NguoiDung?> GetByEmailAsync(string email)
        {
            return await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(NguoiDung nguoiDung)
        {
            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NguoiDung nguoiDung)
        {
            _context.NguoiDungs.Update(nguoiDung);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var nguoiDung = await GetByIdAsync(id);
            if (nguoiDung != null)
            {
                _context.NguoiDungs.Remove(nguoiDung);
                await _context.SaveChangesAsync();
            }
        }
    }
}
