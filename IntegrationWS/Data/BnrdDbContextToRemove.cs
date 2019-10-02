using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntegrationWS.Data
{
    public class BnrdDbContextToRemove : DbContext
    {
        //Este DbContext lo uso para que se conecte al Development de prueba para realizar pruebas con la creacion de factura
        public BnrdDbContextToRemove()
            : base("DEVELOPMENTTOREMOVE")
        {

        }
    }
}