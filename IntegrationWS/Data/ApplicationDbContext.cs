using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace IntegrationWS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Si se desea eliminar la pluralización de las tablas de sql hay que descomentar la siguiente linea 
            //y aplicar la migracion
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<TransferenciaProducto> TransferenciaProducto { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<ArticuloProducto> ArticuloProducto { get; set; }
        public DbSet<ErrorLogs> ErrorLogs { get; set; }
        public DbSet<AppState> AppState { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Oportunidad> Oportunidad { get; set; }
        public DbSet<Pedidos> Pedido { get; set; }
        public DbSet<Contrato> Contrato { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Entrada_del_catalogo_de_precios> Entrada_del_catalogo_de_precios { get; set; }
        public DbSet<Producto_de_oportunidad> Producto_de_oportunidad { get; set; }
        public DbSet<Producto_de_pedido> Producto_de_pedido { get; set; }
        public DbSet<Lista_De_Precios> Lista_De_Precios { get; set; }
        public DbSet<EquiposDeCuenta> AccountTeamMember { get; set; }
        public DbSet<EquiposDeOportunidad> EquiposDeOportunidad { get; set; }
        public DbSet<Asset> Asset { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ViolacionesFEFO> ViolacionesFEFO { get; set; }
        public DbSet<Documento_Abierto> DocumentoAbierto { get; set; }
    }
}