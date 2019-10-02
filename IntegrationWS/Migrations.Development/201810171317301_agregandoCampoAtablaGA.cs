namespace IntegrationWS.Migrations.Development
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agregandoCampoAtablaGA : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.General_Audit", "DateOfChanged", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.General_Audit", "DateOfChanged");
        }
    }
}
