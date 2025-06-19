using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace QLCCCC.Repositories
{
    public class HoaDonDichVuRepository : IHoaDonDichVuRepository
    {
        private readonly ApplicationDbContext _context;

        public HoaDonDichVuRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<HoaDonDichVu> GetAll()
        {
            return _context.HoaDonDichVus
                .Include(hd => hd.CanHo)
                .Include(hd => hd.ChungCu)
                .Include(hd => hd.HoaDonDichVu_DichVus)
                .ToList();
        }

        public HoaDonDichVu? GetById(int id)
        {
            return _context.HoaDonDichVus
                .Include(hd => hd.CanHo)
                .Include(hd => hd.ChungCu)
                .Include(hd => hd.HoaDonDichVu_DichVus)
                .FirstOrDefault(hd => hd.ID == id);
        }

        public void Add(HoaDonDichVu hoaDon)
        {
            if (hoaDon == null) throw new ArgumentNullException(nameof(hoaDon));

            _context.HoaDonDichVus.Add(hoaDon);
            Save();
        }
        public async Task AddAsync(HoaDonDichVu hoaDon)
        {
            await _context.HoaDonDichVus.AddAsync(hoaDon);
            await _context.SaveChangesAsync();
        }
        public void Update(HoaDonDichVu hoaDon)
        {
            if (hoaDon == null) throw new ArgumentNullException(nameof(hoaDon));

            _context.HoaDonDichVus.Update(hoaDon);
            Save();
        }

        public void Delete(int id)
        {
            var hoaDon = _context.HoaDonDichVus.Find(id);
            if (hoaDon != null)
            {
                _context.HoaDonDichVus.Remove(hoaDon);
                Save();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}