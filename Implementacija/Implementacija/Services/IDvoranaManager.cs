using Implementacija.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implementacija.Services
{
    public interface IDvoranaManager
    {
        public Task<IEnumerable<Dvorana>> GetAll();
    }
}
