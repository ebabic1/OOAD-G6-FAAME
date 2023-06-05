using Implementacija.Data;
using Implementacija.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Implementacija.Services
{
    public class KoncertManager : IKoncertManager
    {
        private readonly ApplicationDbContext _db;
        public KoncertManager(ApplicationDbContext db) => _db = db;
        public async Task<IEnumerable<Koncert>> GetAll() => await _db.Koncerti.ToListAsync();
        public IEnumerable<Koncert> GetRecommended()
        {
            return _db.Koncerti.Where(koncert => koncert.zanr == Zanr.HIPHOP);
        }
        public int GetRemainingSeats(Koncert koncert)
        {
            var rezDvorana = _db.RezervacijaDvorana.Where(rez => rez.izvodjacId == koncert.izvodjacId).FirstOrDefault();
            var dvorana = _db.Dvorane.Where(rez => rez.Id == rezDvorana.dvoranaId).FirstOrDefault();
            var count = _db.RezervacijaKarata.Where(rez => rez.koncertId == koncert.Id).Count();
            return dvorana.brojSjedista - count;
        } 

    }
    
}
