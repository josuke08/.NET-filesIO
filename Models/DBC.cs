using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{
    public class DBC : DbContext
    {
        public DBC() : base("GED2") => Database.SetInitializer<DBC>(null);

        public virtual DbSet<Utilisateur> Utilisateurs { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Repertoire> Repertoires { get; set; }
        public virtual DbSet<Revision> Revisions { get; set; }

    }
   
}