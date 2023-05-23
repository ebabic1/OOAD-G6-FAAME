using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Implementacija.Models
{
    public class ObicniKorisnik
    {
        [Key]
        public int Id { get; set; }
        public string imeIPrezime { get; set; }
        public string email { get; set; }
        public string lozinka { get; set; }
        public ObicniKorisnik() { }
    }
}
