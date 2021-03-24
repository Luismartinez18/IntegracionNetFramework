namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agregando_tabla_notificacion_pedido_espera : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notificacion_de_pedido_espera",
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
            DropTable("dbo.Notificacion_de_pedido_espera");
        }
    }
}
