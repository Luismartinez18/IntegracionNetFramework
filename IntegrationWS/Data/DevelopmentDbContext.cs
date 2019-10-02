using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntegrationWS.Data
{
    public class DevelopmentDbContext : DbContext
    {
        public DevelopmentDbContext()
            :base("DEVELOPMENT")
        {

        }

        public DbSet<General_Audit> General_Audit { get; set; }
        public DbSet<General_Audit_History> General_Audit_History { get; set; }
    }
}