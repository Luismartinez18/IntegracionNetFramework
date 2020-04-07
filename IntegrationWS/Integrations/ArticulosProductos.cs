using IntegrationWS.Data;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class ArticulosProductos : IArticulosProductos
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<ProductItem> _sobjectCRUD;
        private readonly ISobjectCRUD<ProductItemToUpdate> _sobjectCRUD2;
        private readonly string sobject;

        public ArticulosProductos(IAuthToSalesforce authToSalesforce, ISobjectCRUD<ProductItem> sobjectCRUD, ISobjectCRUD<ProductItemToUpdate> sobjectCRUD2, IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            _sobjectCRUD2 = sobjectCRUD2;
            sobject = "ProductItem";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL, string table)
        {
            ProductItem productItem = new ProductItem(); 

            if (table == "IV00102")
            {
                productItem = getOne(Id);
            }
            else if (table == "IV00200")
            {
                productItem = getOne200(Id);
            }
            else if (table == "IV00300")
            {
                productItem = getOne300(Id);
            }


            if (productItem.Id_External__c == null)
            {
                return "Vacio";
            }

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, productItem, sobject);
            
            if (result.Contains("DUPLICATE"))
            {
                var salesforceId = await _sobjectCRUD.rawQuery(loginResult, productItem, Id, sobject);

                ArticuloProducto articuloProducto = new ArticuloProducto();
                articuloProducto.DynamicsId = Id;
                articuloProducto.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.ArticuloProducto.Add(articuloProducto);
                    db.SaveChanges();
                }

                var result2 = await update(Id, loginResult, authToken, serviceURL, table);

                if (result2 != "Ok")
                {
                    return result2;
                }

                return "actualizado";
            }
            else if (result.Contains("errorCode"))
            {
                return result;
            }
            
            return result;
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL, string table)
        {
            ProductItem productItem = new ProductItem();

            if (table == "IV00102")
            {
                productItem = getOne(Id);
            }
            else if (table == "IV00200")
            {
                productItem = getOne200(Id);
            }
            else if (table == "IV00300")
            {
                productItem = getOne300(Id);
            }

            if (productItem.Id_External__c == null)
            {
                return "Vacio";
            }

            string salesforceID = string.Empty;
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.ArticuloProducto.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            ProductItemToUpdate productItemToUpdate = new ProductItemToUpdate();
            productItemToUpdate.QuantityOnHand = productItem.QuantityOnHand;
            productItemToUpdate.Fecha_de_vencimiento_Lote__c = productItem.Fecha_de_vencimiento_Lote__c;


            var result = await _sobjectCRUD2.updateSobjectByIdAsync(loginResult, productItemToUpdate, salesforceID, sobject);
            
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

        public ProductItem getOne(string Id)
        {
            ////Inicializando el objeto que voy a enviar a salesforce
            ProductItem productItem = new ProductItem();            

            string item = Id.Substring(0, Id.IndexOf('|')).Trim();            

            //Buscando ubicacion en el string
            var posicion = Id.IndexOf('|');
            var posicion2 = Id.IndexOf('|') + 1;
            var text2 = Id.Substring(posicion2).Trim();
            var posicion3 = text2.IndexOf('|');
            var ubicacion = text2.Substring(0, posicion3).Trim();
            var productId = Id.Substring(0, Id.IndexOf('|')).Trim();

            ProductItemInDynamics productItemFromDb = new ProductItemInDynamics();
            using (BnrdDbContext db_bnrd = new BnrdDbContext())
            {
                productItemFromDb = db_bnrd.Database.SqlQuery<ProductItemInDynamics>($"SELECT ITEMNMBR, (QTYONHND - ATYALLOC) AS 'QTYONHND' FROM IV00102 WHERE ITEMNMBR = '{item}' AND LOCNCODE = '{ubicacion}'").FirstOrDefault();
                if (productItemFromDb == null)
                    return productItem;
                productItemFromDb.BASEUOFM = db_bnrd.Database.SqlQuery<string>($"select BASEUOFM from IV40201 where UOMSCHDL = (select UOMSCHDL from IV00101 where ITEMNMBR = '{item}')").FirstOrDefault();
            }

            if (productItemFromDb.QTYONHND >= 0)
            {
                productItem.Id_External__c = Id;
                productItem.QuantityOnHand = productItemFromDb.QTYONHND;


                using (ApplicationDbContext Db = new ApplicationDbContext())
                {
                    productItem.Product2Id = Db.Productos.Where(x => x.DynamicsId == productId).Select(x => x.SalesforceId).FirstOrDefault();
                    productItem.LocationId = Db.Ubicaciones.Where(x => x.DynamicsId == ubicacion).Select(x => x.SalesforceId).FirstOrDefault();
                }
                productItem.QuantityUnitOfMeasure = productItemFromDb.BASEUOFM.Trim();

                return productItem;
            }

            return productItem;
        }

        public ProductItem getOne200(string Id)
        {
            ////Inicializando el objeto que voy a enviar a salesforce
            ProductItem productItem = new ProductItem();

            

            //Buscando ubicacion en el string
            var posicion = Id.IndexOf('|');
            var posicion2 = Id.IndexOf('|') + 1;
            var text2 = Id.Substring(posicion2).Trim();
            var posicion3 = text2.IndexOf('|');
            var ubicacion = text2.Substring(0, posicion3).Trim();
            var productId = Id.Substring(0, Id.IndexOf('|')).Trim();
            var infoLotOrSerie = text2.Substring(posicion3 + 1).Trim();

            ProductItemInDynamics productItemFromDb = new ProductItemInDynamics();
            using (BnrdDbContext db_bnrd = new BnrdDbContext())
            {
                var numberReturned = db_bnrd.Database.SqlQuery<byte?>($"SELECT SERLNSLD FROM IV00200 WHERE ITEMNMBR = '{productId}' AND LOCNCODE = '{ubicacion}' AND SERLNMBR = '{infoLotOrSerie}'").FirstOrDefault();

                if(numberReturned == 0)
                {
                    productItemFromDb.QTYONHND = 1;
                }
                else
                {
                    productItemFromDb.QTYONHND = 0;
                }

                productItemFromDb.BASEUOFM = db_bnrd.Database.SqlQuery<string>($"select BASEUOFM from IV40201 where UOMSCHDL = (select UOMSCHDL from IV00101 where ITEMNMBR = '{productId}')").FirstOrDefault();
            }

            if (productItemFromDb.QTYONHND >= 0)
            {
                productItem.Id_External__c = Id;
                productItem.QuantityOnHand = productItemFromDb.QTYONHND;


                using (ApplicationDbContext Db = new ApplicationDbContext())
                {
                    productItem.Product2Id = Db.Productos.Where(x => x.DynamicsId == productId).Select(x => x.SalesforceId).FirstOrDefault();
                    productItem.LocationId = Db.Ubicaciones.Where(x => x.DynamicsId == ubicacion).Select(x => x.SalesforceId).FirstOrDefault();
                }
                productItem.QuantityUnitOfMeasure = productItemFromDb.BASEUOFM.Trim();

                productItem.Serie__c = true;
                productItem.N_mero_de_serie__c = infoLotOrSerie.Trim();
                productItem.SerialNumber = Id;

                return productItem;
            }

            return productItem;
        }

        public ProductItem getOne300(string Id)
        {
            ////Inicializando el objeto que voy a enviar a salesforce
            ProductItem productItem = new ProductItem();

            //Buscando ubicacion en el string
            var posicion = Id.IndexOf('|');
            var posicion2 = Id.IndexOf('|') + 1;
            var text2 = Id.Substring(posicion2).Trim();
            var posicion3 = text2.IndexOf('|');
            var ubicacion = text2.Substring(0, posicion3).Trim();
            var productId = Id.Substring(0, Id.IndexOf('|')).Trim();
            var infoLotOrSerie = text2.Substring(posicion3 + 1).Trim();

            ProductItemInDynamics productItemFromDb = new ProductItemInDynamics();
            using (BnrdDbContext db_bnrd = new BnrdDbContext())
            {
                productItemFromDb = db_bnrd.Database.SqlQuery<ProductItemInDynamics>($"SELECT TOP 1 EXPNDATE FROM IV00300 WHERE ITEMNMBR = '{productId}' AND LOCNCODE = '{ubicacion}' AND LOTNUMBR = '{infoLotOrSerie}'").FirstOrDefault();
                if(productItemFromDb != null)
                {
                    productItemFromDb.QTYONHND = db_bnrd.Database.SqlQuery<decimal>($"SELECT (SUM(QTYRECVD) - SUM(QTYSOLD) - SUM(ATYALLOC)) AS 'QTYONHND' FROM IV00300 WHERE ITEMNMBR = '{productId}' AND LOCNCODE = '{ubicacion}' AND LOTNUMBR = '{infoLotOrSerie}'").FirstOrDefault();
                    productItem.QuantityOnHand = productItemFromDb.QTYONHND;

                    productItemFromDb.BASEUOFM = db_bnrd.Database.SqlQuery<string>($"select BASEUOFM from IV40201 where UOMSCHDL = (select UOMSCHDL from IV00101 where ITEMNMBR = '{productId}')").FirstOrDefault();
                    
                    productItem.QuantityUnitOfMeasure = productItemFromDb.BASEUOFM.Trim();
                }
                else
                {
                    productItem.QuantityOnHand = 0;

                    try
                    {
                        productItem.QuantityUnitOfMeasure = db_bnrd.Database.SqlQuery<string>($"select BASEUOFM from IV40201 where UOMSCHDL = (select UOMSCHDL from IV00101 where ITEMNMBR = '{productId}')").FirstOrDefault().Trim();
                    }
                    finally
                    {
                        productItem.QuantityUnitOfMeasure = "UND";
                    }
                }
                
                
            }

            productItem.Id_External__c = Id;

            using (ApplicationDbContext Db = new ApplicationDbContext())
            {
                productItem.Product2Id = Db.Productos.Where(x => x.DynamicsId == productId).Select(x => x.SalesforceId).FirstOrDefault();
                productItem.LocationId = Db.Ubicaciones.Where(x => x.DynamicsId == ubicacion).Select(x => x.SalesforceId).FirstOrDefault();
            }
           
            productItem.Lote__c = true;
            productItem.Numero_de_lote__c = infoLotOrSerie.Trim();

            if (productItemFromDb != null)
            {
                productItem.Fecha_de_vencimiento_Lote__c = productItemFromDb.EXPNDATE;
            }
            else
            {
                productItem.Fecha_de_vencimiento_Lote__c = new DateTime(1900, 1, 1);
            }

            productItem.SerialNumber = Id;


            return productItem;
        }
    }
}