namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoNuevoCampo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EquiposDeCuentas", "SalesforceId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EquiposDeCuentas", "SalesforceId");
        }
    }
}
