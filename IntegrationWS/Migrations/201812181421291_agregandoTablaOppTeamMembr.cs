namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agregandoTablaOppTeamMembr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EquiposDeOportunidads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamMemberRole = c.String(maxLength: 100),
                        OpportunityId = c.String(maxLength: 100),
                        UserId = c.String(maxLength: 100),
                        SalesforceId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EquiposDeOportunidads");
        }
    }
}
