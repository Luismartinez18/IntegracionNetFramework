using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntegrationWS.Data
{
    public class BnrdDbContext : DbContext
    {
        public BnrdDbContext()
            : base("BNRD")
        {

        }
    }    
}