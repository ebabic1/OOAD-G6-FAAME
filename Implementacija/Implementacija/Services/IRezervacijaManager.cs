using Implementacija.Models;
using System.Threading.Tasks;

namespace Implementacija.Services
{
    public interface IRezervacijaManager
    {
        public Task<double> calculatePrice(TipMjesta t, int koncertId);
    }
}
