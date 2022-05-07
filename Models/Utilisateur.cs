using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{
    public class Utilisateur
    {
        [Key]
        public int userID { get; set; } // GUID

        [Required] 
        [MinLength(4), MaxLength(100)]
        public string Nom { get; set; }

        [Required]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        [EmailAddress]
        public string AdresseMail { get; set; }

        [Required]
        [MaxLength(100)]

        public string MotDePasse { get; set; }
    }
}