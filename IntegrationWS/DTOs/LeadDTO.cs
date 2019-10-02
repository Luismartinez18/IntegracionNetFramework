using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class LeadDTO
    {
        public string Id { get; set; }
        public string Company { get; set; }
        public string RNC__c { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Tel_fono_2__c { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }


        //este campo debe ser editado
        public string Correo_Ejecutivo_de_Ventas__c { get; set; }
        public string Ejecutivo_de_ventas_DD__c { get; set; }
        public string Ejecutivo_de_ventas_DH__c { get; set; }
        public string Ejecutivo_de_ventas_DM__c { get; set; }
        public string Id_de_clase__c { get; set; }
        public string CurrencyIsoCode { get; set; }

        //este campo debe ser editado
        public string Nombre_de_la_lista__c { get; set; }
        public string Calle__c { get; set; }
        public string N_mero__c { get; set; }
        public string Sector__c { get; set; }
        public string Ciudad__c { get; set; }
        public string Pa_s__c { get; set; }
        public string Region__c { get; set; }
        public string Provincia__c { get; set; }
        public string Plan_de_impuesto__c { get; set; }
        public string Tipo_de_NCF__c { get; set; }
        public string Condiciones_de_pago__c { get; set; }
        public string Destino_de_env_o__c { get; set; }
        public string Facturar_a__c { get; set; }
        public string Estado_de_cuenta__c { get; set; }                                     
    }
}