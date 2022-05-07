using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{
    public class Revision
    {
        [Key]
        public int version { get; set; }
        
        [Required]
        public string nom { get; set; }

        [Required]
        public DateTime DateDepos { get; set; } = DateTime.Now;

        [ForeignKey("Document")]
        public int parent { get; set; }
        public virtual Document Document { get; set; }

        [ForeignKey("Utilisateur")]
        public int? autheur { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }
    }
}