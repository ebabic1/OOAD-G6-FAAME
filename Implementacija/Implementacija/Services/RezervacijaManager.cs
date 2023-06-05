using Implementacija.Models;
using System.Threading.Tasks;

namespace Implementacija.Services
{
    public class RezervacijaManager : IRezervacijaManager
    {
        public async Task<double> calculatePrice(TipMjesta t, int koncertId)
        {
            if (t == TipMjesta.VIP) return 300;
            else if (t == TipMjesta.TRIBINA) return 200;
            else return 100;
        }
    }
}
