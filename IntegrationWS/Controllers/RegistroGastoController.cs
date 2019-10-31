using IntegrationWS.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IntegrationWS.WSMovilDGII;
using System.ServiceModel;
using IntegrationWS.Utils;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Models;
using IntegrationWS.Data;
using System.Web.Script.Serialization;
using IntegrationWS.ModelsNotMapped;

namespace IntegrationWS.Controllers
{
    [RoutePrefix("api/registrogasto")]
    public class RegistroGastoController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult get([FromBody] RegistroGastoDTO registroGastoDTO)
        {
            try
            {
                if(registroGastoDTO == null)
                {
                    ModelState.AddModelError("Message", "El body no debe ser nulo.");
                    return BadRequest(ModelState);
                }

                Pedido pedido = new Pedido();

                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(registroGastoDTO);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var client = new WSMovilDGIISoapClient();

                var responseRNC = client.GetContribuyentes(registroGastoDTO.RNC__c.Trim(), 0, 0, 0, "");

                if (responseRNC == "0")
                {
                    ModelState.AddModelError("Message", "El RNC no es valido");

                    if (client.State != CommunicationState.Faulted)
                    {
                        client.Close();
                    }

                    return BadRequest(ModelState);
                }

                var responseNCF = client.GetNCF(registroGastoDTO.RNC__c.Trim(), registroGastoDTO.NCF__c.Trim(), "");

                if (responseNCF == "0")
                {
                    ModelState.AddModelError("Message", "El NCF no es valido");

                    if (client.State != CommunicationState.Faulted)
                    {
                        client.Close();
                    }

                    return BadRequest(ModelState);
                }

                if (client.State != CommunicationState.Faulted)
                {
                    client.Close();
                }

                var caja = string.Empty;

                switch (registroGastoDTO.Departamento__c)
                {
                    case "Ingeniería y Aplicaciones": caja = "CC02"; break;
                }

                var response1 = new ResponseCreatePayableInvoice();

                using (BnrdDbContextToRemove db_dev = new BnrdDbContextToRemove())
                {
                    response1 = db_dev.Database.SqlQuery<ResponseCreatePayableInvoice>($"BNRD_SP_CreatePayableInvoce '{registroGastoDTO.RNC__c}', '{caja}'").FirstOrDefault();
                }

                //Crear registro en GP
                CompanyKey companyKey;
                Context context;
                PayablesDocumentKey invoiceKey;
                BatchKey batchKey;
                VendorKey vendorKey;
                PayablesInvoice payablesInvoice;
                Policy payablesInvoiceCreatePolicy;

                // Create an instance of the service
                DynamicsGPClient wsDynamicsGP = new DynamicsGPClient();

                // Create a context with which to call the service
                context = new Context();

                // Specify which company to use (sample company)
                companyKey = new CompanyKey();
                companyKey.Id = (1);

                // Set up the context object
                context.OrganizationKey = (OrganizationKey)companyKey;

                //EL SP DE DADUS VA AQUI PARA OBTENER EL CPB , EL BATCH Y EL VENDORID
                // Create a payables document key object to identify the payables invoice
                invoiceKey = new PayablesDocumentKey();
                var CPB = $"CPB{Convert.ToInt32(response1.CPB.Substring(3).Trim()) + 1}";
                invoiceKey.Id = CPB;//<-- Aqui va el CPB

                context = new Context();
                companyKey = new CompanyKey();
                companyKey.Id = 1;
                context.OrganizationKey = companyKey;

                // Create a batch key
                batchKey = new BatchKey();
                //batchKey.Id = $"CC2{DateTime.Now.ToString("ddMMyyyy")}";
                batchKey.Id = response1.LOTE.Trim(); //<-- Aqui va el batch

                // Create a vendor key
                vendorKey = new VendorKey();
                vendorKey.Id = response1.PROVEEDOR.Trim(); //<-- Aquí va el vendor ID 

                // Create a money amount object for the transaction
                MoneyAmount purchaseAmount = new MoneyAmount();
                purchaseAmount.Currency = "DOP";
                purchaseAmount.Value = registroGastoDTO.Monto__c;

                //string cuenta;

                //if (registroGastoDTO.Tipo_de_gasto_NFC__c == "02 - Gasto por trabajo, suministros, servicios")
                //{
                //    cuenta = "601010-01-04";
                //}
                //else
                //{
                //    cuenta = "";
                //}

                // setup my credit and debit values
                //MoneyAmount creditAmount = new MoneyAmount();
                //Amount aCredit = new Amount();
                //creditAmount.Currency = "DOP";
                //creditAmount.Value = 0m;
                //aCredit = creditAmount;

                //MoneyAmount debitAmount = new MoneyAmount();
                //Amount aDebit = new Amount();
                //debitAmount.Currency = "DOP";
                //debitAmount.Value = purchaseAmount.Value;
                //aDebit = debitAmount;

                ////Distribucion
                //PayablesDistribution[] distributions = new PayablesDistribution[1];
                //distributions[0] = new PayablesDistribution();
                //DistributionTypeKey disTypeKey = new DistributionTypeKey();
                //disTypeKey.Id = 6;
                //GLAccountNumberKey gKey = new GLAccountNumberKey();
                //gKey.Id = cuenta;

                //distributions[0].CompanyKey = companyKey;
                //distributions[0].DistributionTypeKey = disTypeKey;
                //distributions[0].GLAccountKey = gKey;
                //distributions[0].DebitAmount = aDebit;

                //AGREGANDO IMPUESTO
                PayablesTax[] payablesTaxs = default;
                decimal montoTaxes = 0m;

                if (registroGastoDTO.Propina_legal__c)
                {
                    purchaseAmount.Value = Math.Round(purchaseAmount.Value / 1.28m, 2);
                    montoTaxes = registroGastoDTO.Monto__c - purchaseAmount.Value;

                    payablesTaxs = new PayablesTax[2];
                    payablesTaxs[0] = new PayablesTax();
                    TaxDetailKey taxDetailKey = new TaxDetailKey();
                    taxDetailKey.CompanyKey = companyKey;
                    taxDetailKey.Id = "ITBISC";
                    PayablesTaxKey payablesTaxKey = new PayablesTaxKey();
                    payablesTaxKey.TaxDetailKey = taxDetailKey;
                    payablesTaxs[0].Key = payablesTaxKey;
                    payablesTaxs[0].PurchasesTaxAmount = purchaseAmount;
                    MoneyAmount taxAmount18 = new MoneyAmount();
                    taxAmount18.Currency = "DOP";
                    taxAmount18.Value = Math.Round(purchaseAmount.Value * 0.18m, 2);
                    payablesTaxs[0].TaxAmount = taxAmount18;

                    payablesTaxs[1] = new PayablesTax();
                    TaxDetailKey taxDetailKey1 = new TaxDetailKey();
                    taxDetailKey1.CompanyKey = companyKey;
                    taxDetailKey1.Id = "PROP_LEGAL_C";
                    PayablesTaxKey payablesTaxKey1 = new PayablesTaxKey();
                    payablesTaxKey1.TaxDetailKey = taxDetailKey1;
                    payablesTaxs[1].Key = payablesTaxKey1;
                    payablesTaxs[1].PurchasesTaxAmount = purchaseAmount;
                    MoneyAmount taxAmount10 = new MoneyAmount();
                    taxAmount10.Currency = "DOP";
                    taxAmount10.Value = montoTaxes - taxAmount18.Value;
                    payablesTaxs[1].TaxAmount = taxAmount10;
                }
                else
                {
                    purchaseAmount.Value = Math.Round(purchaseAmount.Value / 1.18m, 2);
                    montoTaxes = registroGastoDTO.Monto__c - purchaseAmount.Value;
                    //Math.Round(purchaseAmount.Value * 0.18m, 2);

                    payablesTaxs = new PayablesTax[1];
                    payablesTaxs[0] = new PayablesTax();      
                    TaxDetailKey taxDetailKey = new TaxDetailKey();
                    taxDetailKey.CompanyKey = companyKey;
                    taxDetailKey.Id = "ITBISC";
                    PayablesTaxKey payablesTaxKey = new PayablesTaxKey();
                    payablesTaxKey.TaxDetailKey = taxDetailKey;
                    payablesTaxs[0].Key = payablesTaxKey;
                    payablesTaxs[0].PurchasesTaxAmount = purchaseAmount;
                    MoneyAmount taxAmount18 = new MoneyAmount();
                    taxAmount18.Currency = "DOP";
                    taxAmount18.Value = registroGastoDTO.Monto__c - purchaseAmount.Value;
                    payablesTaxs[0].TaxAmount = taxAmount18;
                }


                //CREANDO PAGO
                MoneyAmount MontoNeto = new MoneyAmount();
                MontoNeto.Currency = "DOP";
                MontoNeto.Value = registroGastoDTO.Monto__c;

                PayablesCashDetail payablesCashDetail = new PayablesCashDetail();
                payablesCashDetail.DocumentId = "EFECTIVO";
                payablesCashDetail.Amount = MontoNeto;
                payablesCashDetail.Date = registroGastoDTO.Fecha_de_consumo__c;
                payablesCashDetail.Number = "PAG172275";

                BankAccountKey bankAccountKey = new BankAccountKey();
                bankAccountKey.CompanyKey = companyKey;
                bankAccountKey.Id = "CC02_DIA";

                payablesCashDetail.BankAccountKey = bankAccountKey;

                MoneyAmount montoTaxesTotal = new MoneyAmount();
                montoTaxesTotal.Currency = "DOP";
                montoTaxesTotal.Value = montoTaxes;

                PayablesPayment payablesPayment = new PayablesPayment();
                payablesPayment.Cash = payablesCashDetail;

                // Create a payables invoice object      
                payablesInvoice = new PayablesInvoice();
                payablesInvoice.Key = invoiceKey;
                payablesInvoice.BatchKey = batchKey;
                payablesInvoice.VendorKey = vendorKey;
                payablesInvoice.VendorDocumentNumber = registroGastoDTO.Numero_de_factura__c; //<-- Aqui el numero de Factura
                payablesInvoice.PurchasesAmount = purchaseAmount;
                payablesInvoice.TaxAmount = montoTaxesTotal;
                payablesInvoice.Date = registroGastoDTO.Fecha_de_consumo__c;
                payablesInvoice.Taxes = payablesTaxs;
                payablesInvoice.Payment = payablesPayment;

                //payablesInvoice.Distributions = distributions;

                //Policy
                payablesInvoiceCreatePolicy = wsDynamicsGP.GetPolicyByOperation("CreatePayablesInvoice", context);

                // Create the sales order
                wsDynamicsGP.CreatePayablesInvoice(payablesInvoice, context, payablesInvoiceCreatePolicy);

                // Close the service
                if (wsDynamicsGP.State != CommunicationState.Faulted)
                {
                    wsDynamicsGP.Close();
                }
                
                using (BnrdDbContextToRemove db_dev = new BnrdDbContextToRemove())
                {
                    var fecha = registroGastoDTO.Fecha_de_consumo__c.ToString("yyyy.MM.dd");
                    var TipoGastoNCF = registroGastoDTO.Tipo_de_gasto_NFC__c.Substring(0, 2).Trim();
                    //db_dev.Database.ExecuteSqlCommand($"BNRD_SP_CreatePayableInvocePayment '{CPB}', {registroGastoDTO.Monto__c}, '{fecha}', '{TipoGastoNCF}'");
                    db_dev.Database.ExecuteSqlCommand($"BNRD_SP_RegistrarNCFaCPB '{response1.PROVEEDOR}', '{registroGastoDTO.Numero_de_factura__c}', '{registroGastoDTO.NCF__c}', '{registroGastoDTO.Tipo_de_gasto_NFC__c.Substring(0, 2).Trim()}'");
                }

                return Content(HttpStatusCode.Created, invoiceKey.Id);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("Message", e.Message.ToString());
                return BadRequest(ModelState);
            }           
        }
    }
}
