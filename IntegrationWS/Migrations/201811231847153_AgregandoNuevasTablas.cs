namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoNuevasTablas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Entrada_del_catalogo_de_precios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Lista_De_Precios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Producto_de_oportunidad",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Producto_de_oportunidad");
            DropTable("dbo.Lista_De_Precios");
            DropTable("dbo.Entrada_del_catalogo_de_precios");
        }
    }
}
