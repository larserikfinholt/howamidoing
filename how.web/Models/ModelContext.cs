using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace how.web.Models
{
    public class ModelContext : DbContext
    {
        // public ModelContext() : base("how") { }

        public DbSet<Goal> Goals { get; set; }
        public DbSet<DoneIt> DoneIts { get; set; }
    }
}