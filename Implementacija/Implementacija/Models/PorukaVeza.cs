﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Implementacija.Models
{
    public class PorukaVeza
    {
        
        [Key]
        public int Id { get; set; }
        /*
        [ForeignKey("Korisnik")]
        public int posiljalacId { get; set; }

        [ForeignKey("Korisnik")]
        public int primalacId { get; set; }
    
        [ForeignKey("Poruka")]
        public int porukaId { get; set; }
        
        */
        public PorukaVeza() { }
    }
}