using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;

namespace QLCCCC.Repositories
{
    public class CuDanRepository : ICuDanRepository
    {
        private readonly ApplicationDbContext _context;

        public CuDanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CuDan>> GetAllAsync()
        {
            return await _context.CuDans
                .Include(cd => cd.NguoiDung)
                .Include(cd => cd.CanHo)
                .Include(cd => cd.ChungCu)  
                .ToListAsync();
        }

        public async Task<CuDan?> GetByIdAsync(int id)
        {
            return await _context.CuDans
                .Include(cd => cd.NguoiDung)
                .Include(cd => cd.CanHo)
                .Include(cd => cd.ChungCu)  // Bao gồm ChungCu để lấy thông tin
                .FirstOrDefaultAsync(cd => cd.ID == id);
        }
        public async Task<CuDan> GetByNguoiDungIdAsync(int nguoiDungId)
        {
            return await _context.CuDans
                                 .FirstOrDefaultAsync(c => c.ID_NguoiDung == nguoiDungId);
        }
        public async Task AddAsync(CuDan cuDan)
        {
            // Kiểm tra nếu NguoiDung đã tồn tại
            var nguoiDung = await _context.NguoiDungs.FindAsync(cuDan.ID_NguoiDung);
            if (nguoiDung == null)
            {
                throw new Exception("NguoiDung không tồn tại.");
            }

            // Thiết lập mối quan hệ giữa CuDan và NguoiDung
            cuDan.NguoiDung = nguoiDung;

            // Thêm CuDan vào cơ sở dữ liệu
            _context.CuDans.Add(cuDan);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(CuDan cuDan)
        {
            _context.CuDans.Update(cuDan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cuDan = await GetByIdAsync(id);
            if (cuDan != null)
            {
                _context.CuDans.Remove(cuDan);
                await _context.SaveChangesAsync();
            }
        }
    }
}
