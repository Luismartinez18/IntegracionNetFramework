using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class Contratos : IContratos
    {

        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;        
        private readonly ISobjectCRUD<ContractSf> _sobjectCRUD;
        private readonly string sobject;

        public Contratos(IAuthToSalesforce authToSalesforce, 
                         ISobjectCRUD<ContractSf> sobjectCRUD, 
                         IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Contract";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            ContractSf contract = getOne(Id, loginResult);

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, contract, sobject);
            var SalesforceId = string.Empty;

            //var logoutResult = await _authToSalesforce.Logout(authToken, serviceURL);
            if (result.Contains("DUPLICATE"))
            {
                SalesforceId = await _sobjectCRUD.rawQuery2(loginResult, contract, Id, sobject);

                Contrato contracto = new Contrato();
                contracto.DynamicsId = Id;
                contracto.SalesforceId = SalesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Contrato.Add(contracto);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL, SalesforceId);

                if (result2 != "Ok")
                {
                    return result2;
                }

                return "actualizado";
            }
            else if (!result.Contains("errorCode"))
            {
                JObject obj2 = JObject.Parse(result);
                SalesforceId = (string)obj2["id"];

                Contrato contrato = new Contrato();
                contrato.DynamicsId = Id;
                contrato.SalesforceId = SalesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Contrato.Add(contrato);
                    db.SaveChanges();
                }
            }

            if (result.Contains("errorCode"))
            {
                return result;
            }

            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL, string SalesforceId)
        {
            ContractSf contract = getOne(Id, loginResult);        

            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, contract, SalesforceId, sobject);

            if (result != "Ok")
            {
                return result;
            }

            return "Ok";
        }

        public async Task<string> delete(string loginResult, string Id)
        {

            var result = await _sobjectCRUD.deleteSobjectByIdAsync(loginResult, Id, sobject);
            
            if (result == "Ok")
            {
                return "Ok";
            }
            
            return result;
        }

        public ContractSf getOne(string Id, string loginResult)
        {
            ContractSf contract = new ContractSf();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                contract = db_dev.Database.SqlQuery<ContractSf>($"SP_GPSalesforce_Contract_ByContnbr '{Id}'").FirstOrDefault();
            }

            return contract;
        }
    }
}