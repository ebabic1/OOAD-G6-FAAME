﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Implementacija.Models
{
    public class Poruka
    {
        [Key]
        public int Id { get; set; }
        public string sadrzaj { get; set; }

        public Poruka() { }

    }
}