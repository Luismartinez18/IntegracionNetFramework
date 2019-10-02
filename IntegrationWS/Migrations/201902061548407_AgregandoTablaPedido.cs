namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoTablaPedido : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pedidoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(),
                        DynamicsId = c.String(),
                        Cliente = c.String(),
                        Producto = c.String(),
                        Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Pedidoes");
        }
    }
}
