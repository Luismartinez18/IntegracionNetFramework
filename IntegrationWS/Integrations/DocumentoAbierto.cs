using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class DocumentoAbierto : IDocumentoAbierto
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<DocumentoAbiertoSf> _sobjectCRUD;
        private readonly string sobject;

        public DocumentoAbierto(IAuthToSalesforce authToSalesforce, ISobjectCRUD<DocumentoAbiertoSf> sobjectCRUD, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            sobject = "Documentos_abiertos__c";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            DocumentoAbiertoSf docAbiertoSf = getOne(Id);

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, docAbiertoSf, sobject);

            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery(loginResult, docAbiertoSf, Id, sobject);

                Documento_Abierto documentoAbierto = new Documento_Abierto();
                documentoAbierto.DynamicsId = Id;
                documentoAbierto.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.DocumentoAbierto.Add(documentoAbierto);
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
            DocumentoAbiertoSf docAbiertoSf = getOne(Id);
            string salesforceID = string.Empty;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.DocumentoAbierto.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, docAbiertoSf, salesforceID, sobject);


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


        public DocumentoAbiertoSf getOne(string Id)
        {
            DocumentoAbiertoSf docAbiertoSf = new DocumentoAbiertoSf();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                docAbiertoSf = db_dev.Database.SqlQuery<DocumentoAbiertoSf>($"EXEC SP_GP_Documento_Abierto '{Id}'").FirstOrDefault();
            }

            return docAbiertoSf;
        }
    }
}