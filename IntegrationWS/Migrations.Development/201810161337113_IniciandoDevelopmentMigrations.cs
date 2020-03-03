namespace IntegrationWS.Migrations.Development
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IniciandoDevelopmentMigrations : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.General_Audit",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            TableName = c.String(maxLength: 50),
            //            DynamicsId = c.String(maxLength: 100),
            //            Activity = c.String(maxLength: 50),
            //            DoneBy = c.String(maxLength: 100),
            //            HasChanged = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.General_Audit_History",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            TableName = c.String(maxLength: 50),
            //            DynamicsId = c.String(maxLength: 100),
            //            Activity = c.String(maxLength: 50),
            //            DoneBy = c.String(maxLength: 100),
            //            DateOfChanged = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.General_Audit_History");
            //DropTable("dbo.General_Audit");
        }
    }
}
