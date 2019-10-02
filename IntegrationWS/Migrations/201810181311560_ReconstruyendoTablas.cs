namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReconstruyendoTablas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransferenciaProductoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ubicacions",
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
            DropTable("dbo.Ubicacions");
            DropTable("dbo.TransferenciaProductoes");
        }
    }
}
