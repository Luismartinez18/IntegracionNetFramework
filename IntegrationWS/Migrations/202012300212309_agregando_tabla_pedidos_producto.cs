namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agregando_tabla_pedidos_producto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Producto_de_pedido",
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
            DropTable("dbo.Producto_de_pedido");
        }
    }
}
