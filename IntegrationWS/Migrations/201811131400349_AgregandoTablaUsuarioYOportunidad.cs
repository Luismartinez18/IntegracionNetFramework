namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoTablaUsuarioYOportunidad : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Oportunidads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Usuarios");
            DropTable("dbo.Oportunidads");
        }
    }
}
