using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Implementacija.Models
{
    public class Iznajmljivac
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Korisnik")]
        public int korisnikId { get; set; }
        public Korisnik korisnik { get; set; }
        public Iznajmljivac() { }
    }
}
