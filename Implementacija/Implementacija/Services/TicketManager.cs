using Implementacija.Data;
using Implementacija.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Implementacija.Services
{
    public class TicketManager : ITicketManager
    {
        private readonly ApplicationDbContext _db;
        public TicketManager(ApplicationDbContext db) => _db = db;
        public async Task<IEnumerable<RezervacijaKarte>> GetAll() => await _db.RezervacijaKarata.ToListAsync();
        public IEnumerable<RezervacijaKarte> GetOwned(string currentID)
        {
            return _db.RezervacijaKarata.Where(rez => rez.obicniKorisnikId == currentID);
        }

    }
}
