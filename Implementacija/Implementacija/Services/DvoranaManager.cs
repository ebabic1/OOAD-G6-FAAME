using Implementacija.Data;
using Implementacija.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implementacija.Services
{
    public class DvoranaManager : IDvoranaManager
    {
        private readonly ApplicationDbContext _db;
        public DvoranaManager(ApplicationDbContext db) => _db = db;
        public async Task<IEnumerable<Dvorana>> GetAll() => await _db.Dvorane.ToListAsync();
    }
}
