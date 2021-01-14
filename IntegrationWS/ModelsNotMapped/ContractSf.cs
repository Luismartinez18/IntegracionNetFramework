using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ContractSf
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Term { get; set; }
        public string OwnerId { get; set; }
        //public string Status { get; set; } //invest
        public string Description { get; set; }
        public string Tipo_de_contrato__c { get; set; }
        public string Tipo_de_servicio__c { get; set; }
        public string RecordTypeId { get; set; }
        //public string RootServiceContractId { get; set; }
        public string Estado__c { get; set; }
        public string Pricebook2Id { get; set; }
        public int Supendido__c { get; set; }
        public string ParentServiceContractId { get; set; }
        public string Numero_de_contrato_de_Dynamics__c { get; set; }
    }
}