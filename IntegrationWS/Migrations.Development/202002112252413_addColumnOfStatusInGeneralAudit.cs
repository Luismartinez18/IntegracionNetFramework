namespace IntegrationWS.Migrations.Development
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addColumnOfStatusInGeneralAudit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.General_Audit", "Error", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.General_Audit", "Error");
        }
    }
}
