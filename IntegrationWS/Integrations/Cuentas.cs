using IntegrationWS.Data;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Utils
{
    public class Cuentas : ICuentas
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<AccountSf> _sobjectCRUD;
        private readonly ISobjectCRUD<AccountTeamMember> _sobjectCRUD2;
        private readonly string sobject;

        public Cuentas(IAuthToSalesforce authToSalesforce, ISobjectCRUD<AccountSf> sobjectCRUD, ISobjectCRUD<AccountTeamMember> sobjectCRUD2, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            _sobjectCRUD2 = sobjectCRUD2;
            sobject = "Account";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            AccountSf account = getOne(Id);

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, account, sobject);

            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery(loginResult, account, Id, sobject);

                Account cuenta = new Account();
                cuenta.DynamicsId = Id;
                cuenta.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Account.Add(cuenta);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL);

                if (result2 != "Ok")
                {
                    return result2;
                }

                return "actualizado";
            }

            if (result.Contains("errorCode"))
            {
                return result;
            }

            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL)
        {
            AccountSf account = getOne(Id);
            string salesforceID = string.Empty;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Account.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, account, salesforceID, sobject);
           

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


        public AccountSf getOne(string Id)
        {
            AccountSf cuenta = new AccountSf();            

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                cuenta = db_dev.Database.SqlQuery<AccountSf>($"SP_GPSalesforce_Accounts_Id '{Id}'").FirstOrDefault();
                
                //Para credito suspendido
                if(cuenta.Cuenta_desactivada__c == "0")
                {
                    cuenta.Cuenta_desactivada__c = "false";
                }
                else
                {
                    cuenta.Cuenta_desactivada__c = "true";
                }

                //Para cuenta inactiva
                if (cuenta.Cuenta_inactiva__c == "0")
                {
                    cuenta.Cuenta_inactiva__c = "false";
                }
                else
                {
                    cuenta.Cuenta_inactiva__c = "true";
                }

                //Aquí realizo la carga de los equipos de cuentas
                List<AccountTeamMember> accountTeamMemberList = db_dev.Database.SqlQuery<AccountTeamMember>($"EXEC DEVELOPMENT.[dbo].[SP_GPSalesforce_AccountTeamMember_V2] '{Id}'").ToList();

                foreach(var item in accountTeamMemberList)
                {
                    if (item.TeamMemberRole.Contains("Diagnóstica"))
                    {
                        cuenta.Representante_de_ventas_Diagnostica__c = item.UserId;
                    }
                    else if (item.TeamMemberRole.Contains("Médica"))
                    {
                        cuenta.Representante_de_ventas_M_dica__c = item.UserId;
                    }
                    else if (item.TeamMemberRole.Contains("Hospitalaria"))
                    {
                        cuenta.Representante_de_ventas_Hospitalaria__c = item.UserId;
                    }
                }
            }

            return cuenta;                
        }
    }
}