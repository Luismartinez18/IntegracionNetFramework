namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EliminandoTablas : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.TransferenciaProductos");
            DropTable("dbo.Ubicaciones");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Ubicaciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransferenciaProductos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
