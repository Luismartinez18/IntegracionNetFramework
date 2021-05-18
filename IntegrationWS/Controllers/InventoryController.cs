
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web.Http;
using IntegrationWS.Data;
using IntegrationWS.DTOs;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Models;

namespace IntegrationWS.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        // GET: Inventory
        public IHttpActionResult Index(StockPlanDTO stockplan)
        {
            return Ok();
        }
        [Route("api/Inventory/InventoryTransaction")]
        [HttpPost]
        public IHttpActionResult InventoryTransaction( StockPlanDTO stockplan)
        {
            try
            {
                var respuesta = string.Empty;
                string NextSecuence = string.Empty;
                using (BnrdDbContextToRemove db_bnrd = new BnrdDbContextToRemove())
                {
              
                    respuesta = db_bnrd.Database.SqlQuery<string>($"select dbo.GetSequenceVariation()").FirstOrDefault();
       
                    if (!string.IsNullOrEmpty(respuesta))
                    {
                        NextSecuence = db_bnrd.Database.SqlQuery<string>($"exec NextDocmuneto 1,'{respuesta}'").FirstOrDefault();
                        if (!string.IsNullOrEmpty(NextSecuence))
                            db_bnrd.Database.ExecuteSqlCommand($"exec sp_UpdateSequenceVariation '{NextSecuence}'");
                    }
                }
                if (!string.IsNullOrEmpty(NextSecuence) && !string.IsNullOrEmpty(respuesta))
                {
                    CompanyKey companyKey = new CompanyKey { Id = 1 };
                    Context context = new Context() { OrganizationKey = companyKey };
                    InventoryVariance variance = new InventoryVariance();
                    BatchKey batchKey;
                    ItemKey itemKey;
                    Quantity quantity;
                    Policy policy;
                    DynamicsGPClient wsDynamicsGP = new DynamicsGPClient();
                    variance.Key = new InventoryKey();
                    variance.Key.Id = respuesta;
                    batchKey = new BatchKey();
                    batchKey.Id = stockplan.StockPlanId;
                    variance.BatchKey = batchKey;
                    variance.Date = stockplan.CreatedDate.GetValueOrDefault();
                    List<InventoryVarianceLine> inventoryVarianceLines = new List<InventoryVarianceLine>();
                    decimal counted = 16384;
                    foreach (var item in stockplan.Detail)
                    {

                        var varancialine = new InventoryVarianceLine();
                        itemKey = new ItemKey();
                        itemKey.Id = item.ItemNumber;
                        varancialine.ItemKey = itemKey;

                        varancialine.Key = new InventoryLineKey();
                        varancialine.Key.SequenceNumber = Convert.ToInt32(item.counted);
                        quantity = new Quantity();
                        quantity.Value = item.Variation;
                        varancialine.Quantity = quantity;
                        varancialine.WarehouseKey = new WarehouseKey();
                        varancialine.WarehouseKey.Id = stockplan.WarehouseId;
                        varancialine.UofM = item.UnitOfMeasure;
                        varancialine.UnitCost = new MoneyAmount();
                        varancialine.UnitCost.Value = item.UnitCost;
                        var SerieLot = item.Detail;
                        List<InventoryLot> inventoryLotKeys = new List<InventoryLot>();
                        List<InventorySerial> inventorySerialBases = new List<InventorySerial>();
                        foreach (var itemlot in item.Detail)
                        {
                            if (item.TypeitemNumber == 3)
                            {
                                var itemlotkey = new InventoryLot();
                                itemlotkey.LotNumber = itemlot.LotNumber.Trim();
                                itemlotkey.Key = new InventoryLotKey();
                                itemlotkey.Key.SequenceNumber = itemlot.DateSEQNumber;
                                itemlotkey.Key.InventoryLineKey = new InventoryLineKey();
                                itemlotkey.Key.InventoryLineKey.SequenceNumber = Convert.ToInt32(decimal.Round(item.counted,0));
                                itemlotkey.Quantity = new Quantity();
                                itemlotkey.Quantity.Value = itemlot.Variation;
                                itemlotkey.ReceivedDate = itemlot.DateReceived.Value.Date;
                                itemlotkey.ExpirationDate = itemlot.ExpirationDate.Value.Date;
                                inventoryLotKeys.Add(itemlotkey);
                            }
                            else if (item.TypeitemNumber == 2)
                            {
                                var itemseriakey = new InventorySerial();
                                itemseriakey.SerialNumber = itemlot.LotNumber.Trim();
                                itemseriakey.Key = new InventorySerialKey();
                                itemseriakey.Key.SequenceNumber = itemlot.DateSEQNumber;
                                itemseriakey.Key.InventoryLineKey = new InventoryLineKey();
                                itemseriakey.Key.InventoryLineKey.SequenceNumber = item.counted;
                                inventorySerialBases.Add(itemseriakey);
                            }
                        }
                        if (item.TypeitemNumber == 3)
                            varancialine.Lots = inventoryLotKeys.ToArray();
                        if (item.TypeitemNumber == 2)
                            varancialine.Serials = inventorySerialBases.ToArray();

                        inventoryVarianceLines.Add(varancialine);
                        counted = counted + 16384;
                    }
                    variance.Lines = inventoryVarianceLines.ToArray();
                    policy = wsDynamicsGP.GetPolicyByOperation("CreateInventoryVariance", context);
                    wsDynamicsGP.CreateInventoryVariance(variance, context, policy);
                    if (wsDynamicsGP.State != CommunicationState.Faulted)
                        wsDynamicsGP.Close();
                    return Ok("Plan de conteo Procesado");
                }
                else return BadRequest("No se puede obtener numero de variacion ");
              
            }
            catch (SecurityNegotiationException se)
            {
                return BadRequest(se.Message);
            }
            catch (FaultException<ExceptionDetail> fe)
            {
                return BadRequest(fe.Detail?.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }
    }
}