namespace IntegrationWS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActualizandoNombresTablaPedido : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pedidoes", "Cliente__c", c => c.String());
            AddColumn("dbo.Pedidoes", "Producto__c", c => c.String());
            AddColumn("dbo.Pedidoes", "Monto__c", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Pedidoes", "Cliente");
            DropColumn("dbo.Pedidoes", "Producto");
            DropColumn("dbo.Pedidoes", "Monto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pedidoes", "Monto", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pedidoes", "Producto", c => c.String());
            AddColumn("dbo.Pedidoes", "Cliente", c => c.String());
            DropColumn("dbo.Pedidoes", "Monto__c");
            DropColumn("dbo.Pedidoes", "Producto__c");
            DropColumn("dbo.Pedidoes", "Cliente__c");
        }
    }
}
