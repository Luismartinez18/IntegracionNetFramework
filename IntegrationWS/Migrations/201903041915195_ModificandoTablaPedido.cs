namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModificandoTablaPedido : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pedidoes", "jsonCompleto", c => c.String());
            DropColumn("dbo.Pedidoes", "SalesforceId");
            DropColumn("dbo.Pedidoes", "DynamicsId");
            DropColumn("dbo.Pedidoes", "Cliente__c");
            DropColumn("dbo.Pedidoes", "Producto__c");
            DropColumn("dbo.Pedidoes", "Monto__c");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pedidoes", "Monto__c", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pedidoes", "Producto__c", c => c.String());
            AddColumn("dbo.Pedidoes", "Cliente__c", c => c.String());
            AddColumn("dbo.Pedidoes", "DynamicsId", c => c.String());
            AddColumn("dbo.Pedidoes", "SalesforceId", c => c.String());
            DropColumn("dbo.Pedidoes", "jsonCompleto");
        }
    }
}
