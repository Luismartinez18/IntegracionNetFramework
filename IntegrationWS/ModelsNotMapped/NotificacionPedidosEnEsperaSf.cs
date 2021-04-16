using IntegrationWS.Migrations.Development;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class NotificacionPedidosEnEsperaSf
    {
        public int Cantidad_Pedido_Espera__c { get; set; }
        public int Cantidad_Recibida_Recepcion__c { get; set; }
        public string Codigo_Producto__c { get; set; }
        public string Division__c { get; set; }
        public DateTime Fecha_Pedido_Espera__c { get; set; }
        public string Nombre_Cliente__c { get; set; }
        public string Nombre_Producto__c { get; set; }
        public string Numero_Cliente__c { get; set; }
        public string Numero_Pedido_Espera__c { get; set; }
        public string Numero_Recepcion__c { get; set; }
        public string Unidad_Medida_Pedido_Espera__c { get; set; }
        public string Unidad_Medida_Recibida_Recepcion__c { get; set; }
        public string Usuario_Creacion_Pedido_Espera__c { get; set; }
        public string Usuario_Creacion_Salesforce__c { get; set; }
        public string Pedido_en_espera__c { get; set; }
        public string Producto__c { get; set; }
        public string Cuenta__c { get; set; }
    }
}