using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace ProjetGED.Models
{
  
    public class Repertoire
    {
        [Key] 
        public int rep { get; set; }
        public int? parent { get; set; }


        [Required]
        public DateTime DateDepos { get; set; } = DateTime.Now;

        [Required]
        [MinLength(4), MaxLength(50)]
        public string NomRep { get; set; }

        [ForeignKey("Utilisateur")]
        public int? proprietaire { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }

        public bool isDowloadable { get; set; }
        public bool isEditable { get; set; }
        public bool isVisible { get; set; }


    }
}