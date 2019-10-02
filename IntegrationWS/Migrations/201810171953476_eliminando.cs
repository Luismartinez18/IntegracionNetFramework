namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eliminando : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Productos");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Productos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesforceId = c.String(maxLength: 100),
                        DynamicsId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
