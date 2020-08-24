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
    public class EntradaDelCatalogoDePrecios : IEntradaDelCatalogoDePrecios
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<PricebookEntry> _sobjectCRUD;
        private readonly ISobjectCRUD<PricebookEntryToUpdate> _sobjectCRUD2;
        private readonly ISobjectCRUD<PricebookEntryToDelete> _sobjectCRUD3;
        private readonly string sobject;

        public EntradaDelCatalogoDePrecios(IAuthToSalesforce authToSalesforce, 
                                           ISobjectCRUD<PricebookEntry> sobjectCRUD, 
                                           ISobjectCRUD<PricebookEntryToUpdate> sobjectCRUD2,
                                           ISobjectCRUD<PricebookEntryToDelete> sobjectCRUD3, 
                                           IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            _sobjectCRUD2 = sobjectCRUD2;
            _sobjectCRUD3 = sobjectCRUD3;
            sobject = "PricebookEntry";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            PricebookEntry pricebookEntry = getOne(Id);

            if(pricebookEntry == null)
            {
                return "No existe";
            }

            if(pricebookEntry.UnitPrice == null && pricebookEntry.Product2Id != null)
            {
                return $"El precio unitario del producto {Id} no puede ser nulo";
            }

            PricebookEntry pricebookEntry1 = getOne(Id);
            PricebookEntry pricebookEntry2 = getOne(Id);
            pricebookEntry1.Pricebook2Id = "01s15000000fINcAAM";
            pricebookEntry2.Pricebook2Id = "01s15000002AnLIAA0";

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, pricebookEntry, sobject);
            string errorMsj = string.Empty;
            if (result.Contains("errorCode"))
            {
                JArray jsonArray = JArray.Parse(result);
                errorMsj = jsonArray[0].ToString();
                JObject obj3 = JObject.Parse(errorMsj);
                errorMsj = (string)obj3["errorCode"];
            }
                      

            if (errorMsj == "STANDARD_PRICE_NOT_DEFINED")
            {
                result = await _sobjectCRUD.addSobjectAsync(loginResult, pricebookEntry1, sobject);
                result = await _sobjectCRUD.addSobjectAsync(loginResult, pricebookEntry2, sobject);
                result = await _sobjectCRUD.addSobjectAsync(loginResult, pricebookEntry, sobject);
            }



            if (result.Contains("Esta definición de precios ya existe en esta lista de precios"))
            {
                var salesforceId = await _sobjectCRUD.PricebookentryId(loginResult, pricebookEntry.Product2Id, pricebookEntry.Pricebook2Id, sobject);

                Entrada_del_catalogo_de_precios entrada_del_catalogo_de_precios = new Entrada_del_catalogo_de_precios();
                entrada_del_catalogo_de_precios.DynamicsId = Id;
                entrada_del_catalogo_de_precios.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Entrada_del_catalogo_de_precios.Add(entrada_del_catalogo_de_precios);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL, salesforceId);

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

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL, string salesforceId)
        {
            PricebookEntry pricebookEntry = getOne(Id);
            if(pricebookEntry!=null)
                pricebookEntry.CurrencyIsoCode = null;

            if(string.IsNullOrEmpty(salesforceId))
            {                
                salesforceId = await _sobjectCRUD.PricebookentryId(loginResult, pricebookEntry.Product2Id, pricebookEntry.Pricebook2Id, sobject);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Database.ExecuteSqlCommand($"DELETE FROM IntegrationWS.[dbo].[Entrada_del_catalogo_de_precios] WHERE DynamicsId = '{Id}'");

                    if(salesforceId.Count() == 18)
                    {
                        Entrada_del_catalogo_de_precios ecp = new Entrada_del_catalogo_de_precios();
                        ecp.DynamicsId = Id;
                        ecp.SalesforceId = salesforceId;
                        db.Entrada_del_catalogo_de_precios.Add(ecp);
                        db.SaveChanges();
                    }
                }                
            }

            PricebookEntryToUpdate pricebookEntryToUpdate = new PricebookEntryToUpdate();
            pricebookEntryToUpdate.IsActive = pricebookEntry.IsActive;
            pricebookEntryToUpdate.UnitPrice = pricebookEntry.UnitPrice;

            var result = await _sobjectCRUD2.updateSobjectByIdAsync(loginResult, pricebookEntryToUpdate, salesforceId, sobject);

            if (result != "Ok")
            {
                if (result.Contains("DELETED") || result.Contains("no data found"))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Database.ExecuteSqlCommand($"DELETE FROM IntegrationWS.[dbo].[Entrada_del_catalogo_de_precios] WHERE DynamicsId = '{Id}'");
                    }
                }

                return result;
            }

            return "Ok";
        }

        public async Task<string> delete(string loginResult, string salesforceId, string Id)
        {
            PricebookEntry pricebookEntry = getOne(Id);

            if (salesforceId == null)
            {
                try
                {
                    salesforceId = await _sobjectCRUD.PricebookentryId(loginResult, pricebookEntry.Product2Id, pricebookEntry.Pricebook2Id, sobject);
                    if(salesforceId == null)
                    {
                        return "Ok";
                    }
                }
                catch(Exception e)
                {
                    if (e.Message.ToString().Contains("Index was out of range."))
                    {
                        return "Ok";
                    }
                    else if(e.Message.ToString().Contains("Object reference"))
                    {
                        return "Ok";
                    }
                }
                
            }

            var result = await _sobjectCRUD.deleteSobjectByIdAsync(loginResult, salesforceId, sobject);

            if (result == "Ok")
            {
                return "Ok";
            }

            if (result.Contains("El registro archivado aún puede verse en estos productos"))
            {
                PricebookEntryToDelete pricebookEntryToDelete = new PricebookEntryToDelete();
                pricebookEntryToDelete.IsActive = 0;

                result = await _sobjectCRUD3.updateSobjectByIdAsync(loginResult, pricebookEntryToDelete, salesforceId, sobject);
            }

            return result;
        }


        public PricebookEntry getOne(string Id)
        {
            PricebookEntry pricebookEntry = new PricebookEntry();

            var PRCSHID = Id.Substring(0, Id.IndexOf('|')).Trim();
            var posicion = Id.IndexOf('|');
            var posicion2 = Id.IndexOf('|') + 1;
            var ITEMNMBR = Id.Substring(posicion2).Trim();

            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
            {
                pricebookEntry = db_dev.Database.SqlQuery<PricebookEntry>($"SP_GPSalesforce_Integracion_PricebookEntry_V2 '{PRCSHID}', '{ITEMNMBR}'").FirstOrDefault();                               

                if (pricebookEntry == null)
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        var product = db.Productos.Where(x => x.DynamicsId == ITEMNMBR).FirstOrDefault();
                        if (product == null)
                        {
                            General_Audit general_Audit = new General_Audit();

                            if(db_dev.General_Audit.Where(x => x.DynamicsId == ITEMNMBR && x.Activity == "INSERT").FirstOrDefault() != null)
                            {
                                return pricebookEntry;
                            }

                            general_Audit.Activity = "INSERT";
                            general_Audit.DateOfChanged = DateTime.Now;
                            general_Audit.DoneBy = "integracionGP";
                            general_Audit.DynamicsId = ITEMNMBR;
                            general_Audit.HasChanged = 1;
                            general_Audit.TableName = "IV00101";
                            db_dev.General_Audit.Add(general_Audit);
                            db_dev.SaveChanges();
                        }
                    }
                }
            }

            return pricebookEntry;
        }
    }
}