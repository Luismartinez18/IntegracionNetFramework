using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Utils
{
    /// <summary>
    /// Esta es la clase que se encarga de leer todo lo capturado por los triggers en la base de datos y trabajar como 
    /// orquestador para saber donde debe comportarse cada dato capturado.
    /// </summary>
    public class ReadGpTables : IReadGpTables
    {
        private readonly IProductos _productos;
        private readonly IArticulosProductos _articulosProductos;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly IUbicaciones _ubicaciones;
        private readonly ITransferenciasProductos _transferenciasProductos;
        private readonly ICuentas _cuentas;
        //private readonly ISobjectCRUD<ProductTransfer> _sobjectCRUD;
        private readonly IOportunidades _oportunidades;
        private readonly IContratos _contratos;
        private readonly IEntradaDelCatalogoDePrecios _entradaDelCatalogo;
        private readonly IListaDePrecios _listaDePrecios;
        private readonly IProductoDeOportunidad _productoDeOportunidad;
        private readonly IProductoConLoteUtils _productoConLoteUtils;
        private readonly IActivos _activos;
        private readonly IExcepcionesFEFO _excepcionesFEFO;
        private readonly IExcepcionesFEFODetalle _excepcionesFEFODetalle;
        private readonly IDocumentoAbierto _documentoAbierto;

        //Variables globales
        string authToken = string.Empty;
        string serviceURL = string.Empty;
        string loginResult = string.Empty;
        string logoutResult = string.Empty;

        public ReadGpTables(  IProductos productos,
                              IArticulosProductos articulosProductos,
                              IAuthToSalesforce authToSalesforce,
                              IUbicaciones ubicaciones,
                              ITransferenciasProductos transferenciasProductos,
                              //ISobjectCRUD<ProductTransfer> sobjectCRUD,
                              ICuentas cuentas,
                              IOportunidades oportunidades,
                              IEntradaDelCatalogoDePrecios entradaDelCatalogo,
                              IListaDePrecios listaDePrecios,
                              IProductoDeOportunidad productoDeOportunidad,
                              IProductoConLoteUtils productoConLoteUtils,
                              IActivos activos,
                              IExcepcionesFEFO excepcionesFEFO,
                              IExcepcionesFEFODetalle excepcionesFEFODetalle,
                              IDocumentoAbierto documentoAbierto,
                              IContratos contratos)
        {
            _productos = productos;
            _articulosProductos = articulosProductos;
            _authToSalesforce = authToSalesforce;
            _ubicaciones = ubicaciones;
            _transferenciasProductos = transferenciasProductos;
            //_sobjectCRUD = sobjectCRUD;
            _cuentas = cuentas;
            _oportunidades = oportunidades;
            _entradaDelCatalogo = entradaDelCatalogo;
            _listaDePrecios = listaDePrecios;
            _productoDeOportunidad = productoDeOportunidad;
            _productoConLoteUtils = productoConLoteUtils;
            _activos = activos;
            _excepcionesFEFO = excepcionesFEFO;
            _excepcionesFEFODetalle = excepcionesFEFODetalle;
            _documentoAbierto = documentoAbierto;
            _contratos = contratos;
        }
        public async Task Run()
        {
            bool state = true;
            var task2 = Task.Run(async () => {                
                while (state)
                {              
                    await ReadTables();
                    
                    await Task.Delay(TimeSpan.FromSeconds(10));

                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        //Si el estado es cambiado a false en la base de datos termina el bucle y la integración deja de correr
                        state = db.AppState.Find(1).State;
                    }
                }
            });
        }

        public async Task ReadTables()
        {         
            List<General_Audit> general_Audit_list = new List<General_Audit>();

            //Esto es para controlar los día que se ejecute el metodo
            //El mismo se ejecutará de 7AM hasta las 7PM todos los días de la semana cada 30 segundos
            DayOfWeek day = DateTime.Now.DayOfWeek;
            var date = DateTime.Now;
             
            if (date.Hour >= 4 && date.Hour <= 5)
            {
                authToken = string.Empty;
                return;
            }

            try
            {
                using (DevelopmentDbContext Db_Dev = new DevelopmentDbContext())
                {                    
                    General_Audit_History general_Audit_History = new General_Audit_History();

                    using (ApplicationDbContext Db = new ApplicationDbContext())
                    {
                        general_Audit_list = Db_Dev.General_Audit.Where(x => x.Error == false).ToList();
                        //general_Audit_list.Add(new General_Audit() { 
                        //    TableName = "SOP30200",
                        //    DynamicsId = "FAC325307",
                        //    Activity = "UPDATE",
                        //    DoneBy = "wburgos",
                        //    HasChanged = 1,
                        //    DateOfChanged =  DateTime.Now
                        //});

                        if (general_Audit_list.Count() != 0 && string.IsNullOrEmpty(authToken))
                        {
                            loginResult = await _authToSalesforce.Login();

                            if (loginResult == "Invalid login attempt to salesforce.")
                            {
                                throw new Exception("Invalid login attempt to salesforce.");
                            }

                            JObject obj = JObject.Parse(loginResult);
                            authToken = (string)obj["access_token"];
                            serviceURL = (string)obj["instance_url"];
                        }


                        foreach (General_Audit general_Audit in general_Audit_list)
                        {
                            try
                            {
                                //Productos
                                if (general_Audit.TableName == "IV00101")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Producto product = new Producto();
                                        product.DynamicsId = Db.Productos.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        if (product.DynamicsId == null)
                                        {
                                            var salesforceId = await _productos.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                product.SalesforceId = salesforceId;
                                                product.DynamicsId = general_Audit.DynamicsId;
                                                Db.Productos.Add(product);
                                                Db.SaveChanges();
                                            }
                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (product.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _productos.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }
                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Productos.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();

                                        if (objectFromDb != null)
                                        {
                                            var salesforceResponse = await _productos.delete(loginResult, objectFromDb.SalesforceId, general_Audit.DynamicsId);
                                            if (salesforceResponse == "Ok")
                                            {
                                                Db.Productos.Remove(objectFromDb);
                                                Db.SaveChanges();
                                            }

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }
                                        else {
                                            Db_Dev.General_Audit.Remove(general_Audit);
                                            general_Audit_History.TableName = general_Audit.TableName;
                                            general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                            general_Audit_History.Activity = general_Audit.Activity;
                                            general_Audit_History.DoneBy = general_Audit.DoneBy;
                                            general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                            Db_Dev.General_Audit_History.Add(general_Audit_History);
                                            Db_Dev.SaveChanges();
                                        }
                                        
                                    }
                                }

                                //Ubicaciones
                                if (general_Audit.TableName == "IV40700")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Ubicacion ubicacion = new Ubicacion();
                                        ubicacion.DynamicsId = Db.Ubicaciones.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        if (ubicacion.DynamicsId == null)
                                        {
                                            var salesforceId = await _ubicaciones.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                ubicacion.SalesforceId = salesforceId;
                                                ubicacion.DynamicsId = general_Audit.DynamicsId;
                                                Db.Ubicaciones.Add(ubicacion);
                                                Db.SaveChanges();
                                            }

                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (ubicacion.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _ubicaciones.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Ubicaciones.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                        var salesforceResponse = await _ubicaciones.delete(loginResult, objectFromDb.SalesforceId);
                                        if (salesforceResponse == "Ok")
                                        {
                                            Db.Ubicaciones.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Articulos de productos
                                if (general_Audit.TableName == "IV00102" || general_Audit.TableName == "IV00200" || general_Audit.TableName == "IV00300")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        ArticuloProducto articuloProducto = new ArticuloProducto();
                                        articuloProducto = Db.ArticuloProducto.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();

                                        if(articuloProducto != null)
                                        {
                                            if ((articuloProducto.SalesforceId == null || articuloProducto.SalesforceId == "") && articuloProducto.DynamicsId != null)
                                            {
                                                Db.ArticuloProducto.Remove(articuloProducto);
                                                Db.SaveChanges();

                                                articuloProducto = Db.ArticuloProducto.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                            }
                                        }

                                        if (articuloProducto == null)
                                        {
                                            var salesforceId = await _articulosProductos.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL, general_Audit.TableName);

                                            if (salesforceId == "Vacio")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                articuloProducto.SalesforceId = salesforceId;
                                                articuloProducto.DynamicsId = general_Audit.DynamicsId;
                                                Db.ArticuloProducto.Add(articuloProducto);
                                                Db.SaveChanges();
                                            }
                                        }
                                        else if (articuloProducto.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _articulosProductos.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL, general_Audit.TableName);

                                            if (salesforceResponse == "Vacio")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (salesforceResponse.Contains("INVALID_CROSS_REFERENCE_KEY"))
                                            {
                                                Db.ArticuloProducto.Remove(articuloProducto);
                                                Db.SaveChanges();
                                                continue;
                                            }

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.ArticuloProducto.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();

                                        if (objectFromDb == null)
                                        {
                                            Db_Dev.General_Audit.Remove(general_Audit);
                                            Db_Dev.SaveChanges();
                                            continue;
                                        }

                                        var salesforceResponse = await _articulosProductos.delete(loginResult, objectFromDb.SalesforceId);
                                        if (salesforceResponse == "Ok")
                                        {
                                            Db.ArticuloProducto.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse.Contains("errorCode"))
                                        {
                                            JArray jsonArray = JArray.Parse(salesforceResponse);
                                            salesforceResponse = jsonArray[0].ToString();
                                            JObject obj3 = JObject.Parse(salesforceResponse);
                                            salesforceResponse = (string)obj3["errorCode"];

                                            if (salesforceResponse == "ENTITY_IS_DELETED")
                                            {
                                                Db.ArticuloProducto.Remove(objectFromDb);
                                                Db.SaveChanges();
                                            }
                                        }


                                        if (salesforceResponse != "Ok" && salesforceResponse != "ENTITY_IS_DELETED")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Cuentas
                                if (general_Audit.TableName == "RM00101")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Account account = new Account();
                                        account.DynamicsId = Db.Account.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        if (account.DynamicsId == null)
                                        {
                                            var salesforceId = await _cuentas.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                account.SalesforceId = salesforceId;
                                                account.DynamicsId = general_Audit.DynamicsId;
                                                Db.Account.Add(account);
                                                Db.SaveChanges();
                                            }

                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (account.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _cuentas.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Account.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                        var salesforceResponse = await _cuentas.delete(loginResult, objectFromDb.SalesforceId);
                                        if (salesforceResponse == "Ok")
                                        {
                                            Db.Account.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Oportunidades
                                if (general_Audit.TableName == "SOP30200")
                                {
                                    General_Audit newForSOP30300 = new General_Audit();
                                    newForSOP30300.Activity = general_Audit.Activity;
                                    newForSOP30300.DateOfChanged = general_Audit.DateOfChanged;
                                    newForSOP30300.DoneBy = general_Audit.DoneBy;
                                    newForSOP30300.DynamicsId = general_Audit.DynamicsId;
                                    newForSOP30300.HasChanged = general_Audit.HasChanged;
                                    newForSOP30300.TableName = "SOP30300";

                                    General_Audit newForSOP10201 = new General_Audit();
                                    newForSOP10201.Activity = general_Audit.Activity;
                                    newForSOP10201.DateOfChanged = general_Audit.DateOfChanged;
                                    newForSOP10201.DoneBy = general_Audit.DoneBy;
                                    newForSOP10201.DynamicsId = general_Audit.DynamicsId;
                                    newForSOP10201.HasChanged = general_Audit.HasChanged;
                                    newForSOP10201.TableName = "SOP10201";

                                    Db_Dev.General_Audit.Add(newForSOP30300);
                                    Db_Dev.General_Audit.Add(newForSOP10201);
                                    Db_Dev.SaveChanges();

                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Oportunidad oportunidad = new Oportunidad();
                                        oportunidad.DynamicsId = Db.Oportunidad.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();
                                        oportunidad.SalesforceId = Db.Oportunidad.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.SalesforceId).FirstOrDefault();

                                        if (oportunidad.DynamicsId == null)
                                        {
                                            var salesforceId = await _oportunidades.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }else if (salesforceId.Contains("errorCode"))
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (oportunidad.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _oportunidades.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL, oportunidad.SalesforceId);

                                            if (salesforceResponse != "Ok")
                                            {
                                                JArray jsonArray = JArray.Parse(salesforceResponse);
                                                salesforceResponse = jsonArray[0].ToString();
                                                JObject obj3 = JObject.Parse(salesforceResponse);
                                                salesforceResponse = (string)obj3["errorCode"];

                                                if (salesforceResponse == "ENTITY_IS_DELETED")
                                                {
                                                    var opo = Db.Oportunidad.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                                    Db.Oportunidad.Remove(opo);
                                                    Db.SaveChanges();
                                                    continue;
                                                }
                                                else
                                                {
                                                    throw new Exception(salesforceResponse);
                                                }
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Oportunidad.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                        var salesforceResponse = await _oportunidades.delete(loginResult, objectFromDb.SalesforceId);
                                        if (salesforceResponse == "Ok")
                                        {                                            
                                            Db.Oportunidad.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Transferencia de productos
                                if (general_Audit.TableName == "IV30300")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        TransferenciaProducto transferencia = new TransferenciaProducto();
                                        transferencia.DynamicsId = Db.TransferenciaProducto.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();
                                        transferencia.SalesforceId = Db.TransferenciaProducto.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.SalesforceId).FirstOrDefault();

                                        if ((transferencia.SalesforceId == null || transferencia.SalesforceId == "") && transferencia.DynamicsId != null)
                                        {
                                            TransferenciaProducto transferenciaDelete = new TransferenciaProducto();
                                            transferenciaDelete.Id = Db.TransferenciaProducto.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.Id).FirstOrDefault();

                                            Db.Database.ExecuteSqlCommand($"SP_DeleteTransferenciaProducto '{transferenciaDelete.Id}'");

                                            transferencia.DynamicsId = Db.TransferenciaProducto.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();
                                        }

                                        if (transferencia.DynamicsId == null)
                                        {
                                            var salesforceId = await _transferenciasProductos.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                transferencia.SalesforceId = salesforceId;
                                                transferencia.DynamicsId = general_Audit.DynamicsId;
                                                Db.TransferenciaProducto.Add(transferencia);
                                                Db.SaveChanges();
                                            }

                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (transferencia.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _transferenciasProductos.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                    }
                                }

                                //Lista de precios Pricebook2
                                if (general_Audit.TableName == "SOP10110")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Lista_De_Precios listaDePrecios = new Lista_De_Precios();
                                        listaDePrecios.DynamicsId = Db.Lista_De_Precios.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        if (listaDePrecios.DynamicsId == null)
                                        {
                                            var salesforceId = await _listaDePrecios.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                listaDePrecios.SalesforceId = salesforceId;
                                                listaDePrecios.DynamicsId = general_Audit.DynamicsId;
                                                Db.Lista_De_Precios.Add(listaDePrecios);
                                                Db.SaveChanges();
                                            }

                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (listaDePrecios.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _listaDePrecios.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Lista_De_Precios.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                        var salesforceResponse = await _listaDePrecios.delete(loginResult, objectFromDb.SalesforceId);
                                        if (salesforceResponse == "Ok")
                                        {
                                            Db.Lista_De_Precios.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Entrada del catálogo de precios PricebookEntry
                                if (general_Audit.TableName == "IV10402")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Entrada_del_catalogo_de_precios entrada_del_catalogo_de_precios = new Entrada_del_catalogo_de_precios();
                                        entrada_del_catalogo_de_precios = Db.Entrada_del_catalogo_de_precios.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();

                                        if (entrada_del_catalogo_de_precios == null)
                                        {
                                            var salesforceId = await _entradaDelCatalogo.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId.Contains("El precio unitario del producto") || salesforceId.Contains("No existe"))
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = "ERROR";
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                            }

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                entrada_del_catalogo_de_precios.SalesforceId = salesforceId;
                                                entrada_del_catalogo_de_precios.DynamicsId = general_Audit.DynamicsId;
                                                Db.Entrada_del_catalogo_de_precios.Add(entrada_del_catalogo_de_precios);
                                                Db.SaveChanges();
                                            }

                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (entrada_del_catalogo_de_precios != null)
                                        {
                                            var salesforceResponse = await _entradaDelCatalogo.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL, entrada_del_catalogo_de_precios.SalesforceId);

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Entrada_del_catalogo_de_precios.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();

                                        string salesforceResponse;

                                        if (objectFromDb == null)
                                        {
                                            salesforceResponse = await _entradaDelCatalogo.delete(loginResult, null, general_Audit.DynamicsId);
                                        }
                                        else
                                        {
                                            salesforceResponse = await _entradaDelCatalogo.delete(loginResult, objectFromDb.SalesforceId, general_Audit.DynamicsId);
                                        }

                                        if (salesforceResponse == "Ok" && objectFromDb != null)
                                        {
                                            Db.Entrada_del_catalogo_de_precios.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Productos de oportunidad
                                if (general_Audit.TableName == "SOP30300")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Producto_de_oportunidad producto_de_oportunidad = new Producto_de_oportunidad();
                                        producto_de_oportunidad.DynamicsId = Db.Producto_de_oportunidad.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        var salesforceId = await _productoDeOportunidad.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                        if (salesforceId.Contains("versions 3.0 and higher must specify pricebook entry id"))
                                        {
                                            continue;
                                        }

                                        if (!salesforceId.Contains("field integrity exception"))
                                        {
                                            if (salesforceId.Contains("errorCode"))
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                    }
                                }

                                //Productos de oportunidad con lote
                                if (general_Audit.TableName == "SOP10201")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                       var salesforceId = await _productoConLoteUtils.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                        if (salesforceId.Contains("errorCode"))
                                        {
                                            throw new Exception(salesforceId);
                                        }
                                    }
                                }

                                //Activos
                                if (general_Audit.TableName == "FA00100")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Asset asset = new Asset();
                                        asset.DynamicsId = Db.Asset.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        if (asset.DynamicsId == null)
                                        {
                                            var salesforceId = await _activos.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "No existe")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = "No existe";
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                asset.SalesforceId = salesforceId;
                                                asset.DynamicsId = general_Audit.DynamicsId;
                                                Db.Asset.Add(asset);
                                                Db.SaveChanges();
                                            }
                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (asset.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _activos.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse == "No existe")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = "No existe";
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }
                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Asset.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                        var salesforceResponse = await _activos.delete(loginResult, objectFromDb.SalesforceId, general_Audit.DynamicsId);
                                        if (salesforceResponse == "Ok")
                                        {
                                            Db.Asset.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Excepciones FEFO
                                if (general_Audit.TableName == "BNRD_ViolacionesFEFO")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        ViolacionesFEFO violacion = new ViolacionesFEFO();
                                        violacion.DynamicsId = Db.ViolacionesFEFO.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        if (violacion.DynamicsId == null)
                                        {
                                            var salesforceId = await _excepcionesFEFO.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId.Contains("actualizado"))
                                            {
                                                var sfId = salesforceId.Substring(3).Trim();
                                                var detalleResult = await _excepcionesFEFODetalle.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL, sfId);

                                                if (!detalleResult.Contains("success") && detalleResult != "Ok" && detalleResult != "actualizado")
                                                {
                                                    continue;
                                                }

                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                violacion.SalesforceId = salesforceId;
                                                violacion.DynamicsId = general_Audit.DynamicsId;
                                                Db.ViolacionesFEFO.Add(violacion);
                                                Db.SaveChanges();

                                                var detalleResult = await _excepcionesFEFODetalle.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL, salesforceId);
                                                if (!detalleResult.Contains("success") && detalleResult != "Ok" && detalleResult != "actualizado")
                                                {
                                                    continue;
                                                }
                                                else if(salesforceId.Contains("errorCode"))
                                                {
                                                    throw new Exception(salesforceId);
                                                }
                                            }
                                            
                                        }

                                        else if (violacion.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _excepcionesFEFO.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse.Contains("la entidad está eliminada"))
                                            {
                                                using(ApplicationDbContext db = new ApplicationDbContext())
                                                {
                                                    var excepcion = db.ViolacionesFEFO.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                                    db.ViolacionesFEFO.Remove(excepcion);
                                                    db.SaveChanges();
                                                    continue;
                                                }
                                            }

                                            if (salesforceResponse.Contains("Ok"))
                                            {
                                                var sfId = salesforceResponse.Substring(3).Trim();
                                                var detalleResult = await _excepcionesFEFODetalle.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL, sfId);
                                                if (!detalleResult.Contains("success") && detalleResult != "Ok" && detalleResult != "actualizado")
                                                {
                                                    continue;
                                                }
                                            }

                                            

                                            if (!salesforceResponse.Contains("Ok"))
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                        else if (general_Audit.Activity == "DELETE")
                                        {
                                            var objectFromDb = Db.ViolacionesFEFO.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                            var salesforceResponse = await _excepcionesFEFO.delete(loginResult, objectFromDb.SalesforceId, general_Audit.DynamicsId);
                                            if (salesforceResponse == "Ok")
                                            {
                                                Db.ViolacionesFEFO.Remove(objectFromDb);
                                                Db.SaveChanges();
                                            }

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }
                                    }
                                    
                                }

                                //Excepciones FEFO
                                if (general_Audit.TableName == "BNRD_DetalleViolacionesFEFO")
                                {
                                    if (general_Audit.Activity == "DELETE")
                                    {
                                        var salesforceResponse = await _excepcionesFEFODetalle.delete(loginResult, general_Audit.DynamicsId);

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Documento abierto
                                if (general_Audit.TableName == "RM20101")
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Documento_Abierto docAbierto = new Documento_Abierto();
                                        docAbierto.DynamicsId = Db.DocumentoAbierto.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();

                                        if (docAbierto.DynamicsId == null)
                                        {
                                            var salesforceId = await _documentoAbierto.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }

                                            if (!salesforceId.Contains("errorCode"))
                                            {
                                                JObject obj2 = JObject.Parse(salesforceId);
                                                salesforceId = (string)obj2["id"];
                                                docAbierto.SalesforceId = salesforceId;
                                                docAbierto.DynamicsId = general_Audit.DynamicsId;
                                                Db.DocumentoAbierto.Add(docAbierto);
                                                Db.SaveChanges();
                                            }

                                            else
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (docAbierto.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _documentoAbierto.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceResponse != "Ok")
                                            {
                                                throw new Exception(salesforceResponse);
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {

                                        var objectFromDb = Db.DocumentoAbierto.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();

                                        string salesforceResponse = default;

                                        if (objectFromDb == null)
                                        {
                                            salesforceResponse = await _documentoAbierto.delete(loginResult, general_Audit.DynamicsId);
                                        }
                                        else
                                        {
                                            salesforceResponse = await _documentoAbierto.delete(loginResult, objectFromDb.SalesforceId);
                                        }

                                        if (salesforceResponse == "Ok" && objectFromDb != null)
                                        {
                                            Db.DocumentoAbierto.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                //Contrato
                                if (general_Audit.TableName == "SVC00600") 
                                {
                                    if (general_Audit.Activity == "INSERT" || general_Audit.Activity == "UPDATE")
                                    {
                                        Contrato contrato = new Contrato();
                                        contrato.DynamicsId = Db.Contrato.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.DynamicsId).FirstOrDefault();
                                        contrato.SalesforceId = Db.Contrato.Where(x => x.DynamicsId == general_Audit.DynamicsId).Select(x => x.SalesforceId).FirstOrDefault();

                                        if (contrato.DynamicsId == null)
                                        {
                                            var salesforceId = await _contratos.create(general_Audit.DynamicsId, loginResult, authToken, serviceURL);

                                            if (salesforceId == "actualizado")
                                            {
                                                Db_Dev.General_Audit.Remove(general_Audit);
                                                general_Audit_History.TableName = general_Audit.TableName;
                                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                                general_Audit_History.Activity = general_Audit.Activity;
                                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                                Db_Dev.SaveChanges();
                                                continue;
                                            }
                                            else if (salesforceId.Contains("errorCode"))
                                            {
                                                throw new Exception(salesforceId);
                                            }
                                        }
                                        else if (contrato.DynamicsId != null)
                                        {
                                            var salesforceResponse = await _contratos.update(general_Audit.DynamicsId, loginResult, authToken, serviceURL, contrato.SalesforceId);

                                            if (salesforceResponse != "Ok")
                                            {
                                                JArray jsonArray = JArray.Parse(salesforceResponse);
                                                salesforceResponse = jsonArray[0].ToString();
                                                JObject obj3 = JObject.Parse(salesforceResponse);
                                                salesforceResponse = (string)obj3["errorCode"];

                                                if (salesforceResponse == "ENTITY_IS_DELETED")
                                                {
                                                    var contr = Db.Contrato.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                                    Db.Contrato.Remove(contr);
                                                    Db.SaveChanges();
                                                    continue;
                                                }
                                                else
                                                {
                                                    throw new Exception(salesforceResponse);
                                                }
                                            }
                                        }

                                    }
                                    else if (general_Audit.Activity == "DELETE")
                                    {
                                        var objectFromDb = Db.Contrato.Where(x => x.DynamicsId == general_Audit.DynamicsId).FirstOrDefault();
                                        var salesforceResponse = await _contratos.delete(loginResult, objectFromDb.SalesforceId);
                                        if (salesforceResponse == "Ok")
                                        {
                                            Db.Contrato.Remove(objectFromDb);
                                            Db.SaveChanges();
                                        }

                                        if (salesforceResponse != "Ok")
                                        {
                                            throw new Exception(salesforceResponse);
                                        }
                                    }
                                }

                                Db_Dev.General_Audit.Remove(general_Audit);
                                general_Audit_History.TableName = general_Audit.TableName;
                                general_Audit_History.DynamicsId = general_Audit.DynamicsId;
                                general_Audit_History.Activity = general_Audit.Activity;
                                general_Audit_History.DoneBy = general_Audit.DoneBy;
                                general_Audit_History.DateOfChanged = general_Audit.DateOfChanged;
                                Db_Dev.General_Audit_History.Add(general_Audit_History);
                                Db_Dev.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                if (e.Message == "Ignorar")
                                {
                                    Db_Dev.General_Audit.Remove(general_Audit);
                                    Db_Dev.SaveChanges();
                                    continue;
                                }

                                if (e.Message == "Business object not found.")
                                {
                                    Db_Dev.General_Audit.Remove(general_Audit);
                                    Db_Dev.SaveChanges();
                                    continue;
                                }

                                string errorMsj = string.Empty;

                                var result = Db_Dev.General_Audit.FirstOrDefault(x => x.Id == general_Audit.Id);
                                if (result != null)
                                {
                                    result.Error = true;
                                    Db_Dev.SaveChanges();
                                }

                                try
                                {
                                    errorMsj = e.Message.ToString();
                                    JArray jsonArray = JArray.Parse(errorMsj);
                                    errorMsj = jsonArray[0].ToString();
                                    JObject obj3 = JObject.Parse(errorMsj);
                                    errorMsj = (string)obj3["errorCode"];
                                }
                                finally 
                                {
                                    //Simplemente que ejecute le siguiente bloque de codigo
                                }

                                if (errorMsj != "ENTITY_IS_DELETED")
                                {
                                    if (errorMsj == "INVALID_SESSION_ID")
                                    {
                                        loginResult = await _authToSalesforce.Login();

                                        if (loginResult == "Invalid login attempt to salesforce.")
                                        {
                                            throw new Exception("Invalid login attempt to salesforce.");
                                        }

                                        JObject obj = JObject.Parse(loginResult);
                                        authToken = (string)obj["access_token"];
                                        serviceURL = (string)obj["instance_url"];

                                        return;
                                    }

                                    ErrorLogs error = new ErrorLogs();
                                    error.Error = e.Message + " | " + general_Audit.DynamicsId;
                                    error.FechaDelError = DateTime.Now;
                                    Db.ErrorLogs.Add(error);
                                    Db.SaveChanges();
                                }
                                else if (errorMsj == "ENTITY_IS_DELETED" && general_Audit.Activity == "DELETE")
                                {
                                    Db_Dev.General_Audit.Remove(general_Audit);
                                    Db_Dev.SaveChanges();
                                    continue;
                                }

                            }
                        }
                    }     
                }
                if (general_Audit_list.Count() != 0 && (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday || (date.Hour >= 19 && date.Hour <= 7)))
                {
                    logoutResult = await _authToSalesforce.Logout(authToken, serviceURL);
                    authToken = string.Empty;
                }
            }
            catch (Exception e)
            {
                if (authToken != string.Empty)
                {
                    try
                    {
                        logoutResult = await _authToSalesforce.Logout(authToken, serviceURL);
                    }
                    catch (Exception ex)
                    {
                        using (ApplicationDbContext Db = new ApplicationDbContext())
                        {
                            ErrorLogs error = new ErrorLogs();
                            error.Error = ex.Message;
                            error.FechaDelError = DateTime.Now;
                            Db.ErrorLogs.Add(error);
                            Db.SaveChanges();
                        }
                    }
                    
                    authToken = string.Empty;
                }

                if (e.Message != "Invalid login attempt to salesforce." && (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday || (date.Hour >= 19 && date.Hour <= 7)))                   
                {
                    logoutResult = await _authToSalesforce.Logout(authToken, serviceURL);
                }

                using (ApplicationDbContext Db = new ApplicationDbContext())
                {
                    ErrorLogs error = new ErrorLogs();
                    error.Error = e.ToString();
                    error.FechaDelError = DateTime.Now;
                    Db.ErrorLogs.Add(error);
                    Db.SaveChanges();
                }
            }
        }
    }
}