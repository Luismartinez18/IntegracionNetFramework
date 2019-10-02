namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoAsset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DynamicsId = c.String(),
                        SalesforceId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Assets");
        }
    }
}
