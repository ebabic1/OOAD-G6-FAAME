using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Implementacija.Models
{
    public class Recenzija
    {
        [Key]
        public int Id { get; set; }
        public double rating { get; set; }
        public string komentar { get; set; }
        [ForeignKey("Izvodjac")]
        public int izvodjacId { get; set; }
        public Izvodjac izvodjac { get; set; }
        public Recenzija() { }
    }
}
