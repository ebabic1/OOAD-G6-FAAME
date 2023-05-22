using System.ComponentModel.DataAnnotations;

namespace Implementacija.Models
{
    public class Korisnik
    {
        [Key]
        public int Id { get; set; }
        public string imeIPrezime { get; set; }
        public string email { get; set; }
        public string lozinka { get; set; }
        public Korisnik() { }
    }
}
