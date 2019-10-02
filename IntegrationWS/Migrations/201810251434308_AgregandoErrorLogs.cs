namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoErrorLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FechaDelError = c.DateTime(nullable: false),
                        Error = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ErrorLogs");
        }
    }
}
