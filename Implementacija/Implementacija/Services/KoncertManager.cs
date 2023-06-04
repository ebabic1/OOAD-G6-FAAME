using Implementacija.Data;
using Implementacija.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implementacija.Services
{
    public class KoncertManager : IKoncertManager
    {
        private readonly ApplicationDbContext _db;
        public KoncertManager(ApplicationDbContext db) => _db = db;
        public async Task<IEnumerable<Koncert>> GetAll() => await _db.Koncerti.ToListAsync();

    }
    
}
