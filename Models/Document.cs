using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{
    public class Document
    {
        [Key]
        public int doc { get; set; }
        [Required]
        public string titre { get; set; }
        [Required]
        public DateTime DateDepos { get; set; } = DateTime.Now;

        [ForeignKey("Utilisateur")]
        public int UserID { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }

        [ForeignKey("Repertoire")]
        public int? repository { get; set; }
        public virtual Repertoire Repertoire { get; set; }

        public bool isDowloadable { get; set; }
        public bool isEditable { get; set; }
        public bool isVisible { get; set; }
    }
}