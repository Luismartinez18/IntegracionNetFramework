namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoTablaAppState : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AppStates");
        }
    }
}
