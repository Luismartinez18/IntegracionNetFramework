namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoAccountTeamMemberTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EquiposDeCuentas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        AccountId = c.String(),
                        TeamMemberRole = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EquiposDeCuentas");
        }
    }
}
