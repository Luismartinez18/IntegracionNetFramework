using IntegrationWS.Data;
using IntegrationWS.DTOs;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace IntegrationWS.Controllers
{
    public class OpportunityToQuoteController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody] PresupuestoDTO presupuestoDTO)
        {
            try
            {

                Pedido pedido = new Pedido();

                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(presupuestoDTO);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();
                }

                string respuesta = string.Empty;
                using (DevelopmentDbContext db_bnrd = new DevelopmentDbContext())
                {
                    respuesta = db_bnrd.Database.SqlQuery<string>($"EXEC VerficicarExistenciaFactura 1, 'P{presupuestoDTO.QuoteNumber}'").FirstOrDefault();
                }

                if (string.IsNullOrEmpty(respuesta))
                {
                    CompanyKey companyKey;
                    Context context;
                    SalesQuote salesOrder;
                    SalesDocumentTypeKey salesOrderType;
                    CustomerKey customerKey;
                    BatchKey batchKey;
                    ItemKey orderedItem;
                    Quantity orderedAmount;
                    Policy salesOrderCreatePolicy;
                    // Create an instance of the service
                    DynamicsGPClient wsDynamicsGP = new DynamicsGPClient();

                    // Create a context
                    context = new Context();

                    // Specify which company to use (sample company)
                    companyKey = new CompanyKey();
                    companyKey.Id = (presupuestoDTO.companyKeyId);

                    // Set up the context object
                    context.OrganizationKey = companyKey;


                    // Create a sales order object
                    salesOrder = new SalesQuote();


                    // Create a sales document type key for the sales order
                    salesOrderType = new SalesDocumentTypeKey();
                    salesOrderType.Type = SalesDocumentType.Quote;

                    // Populate the document type key of the sales order object
                    salesOrder.DocumentTypeKey = salesOrderType;

                    // Create a customer key
                    customerKey = new CustomerKey();
                    customerKey.Id = presupuestoDTO.NumeroDeLaCuenta;

                    // Set the customer key property of the sales order object
                    salesOrder.CustomerKey = customerKey;

                    // Create a batch key
                    batchKey = new BatchKey();
                    batchKey.Id = $"P-COT{DateTime.Now.ToString("ddMMyyyy")}";

                    //Set Vendor
                    SalespersonKey salespersonKey = new SalespersonKey();
                    salespersonKey.Id = "00086"; //<-- Colocando a Hector como vendedor fijo

                    salesOrder.SalespersonKey = salespersonKey;


                    //Agregando comentario
                    var comentario = $@"Número de cotización: {presupuestoDTO.QuoteNumber}";

                    CommentKey commentKey = new CommentKey();
                    commentKey.Id = $"P{presupuestoDTO.QuoteNumber}";

                    salesOrder.CommentKey = commentKey;
                    salesOrder.Comment = comentario;

                    // Set the batch key property of the sales order object
                    salesOrder.BatchKey = batchKey;

                    //creando lista de SalesOrderLine
                    List<SalesQuoteLine> orders = new List<SalesQuoteLine>();

                    //Iterando los productos recibidos
                    int cont = 1;
                    foreach (var producto in presupuestoDTO.presupuesto_producto)
                    {

                        //verificar producto inactivos
                        string Inactive = string.Empty;
                        using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
                            Inactive = db_dev.Database.SqlQuery<string>($"EXEC SP_GP_VerificarExistenciaProducto '{producto.Codigo.Trim()}'").FirstOrDefault();

                        if (Inactive == "1")
                            throw new Exception($"El producto {producto.NombreDeProducto} esta inactivo.");

                        SalesQuoteLine salesOrderLine = new SalesQuoteLine();

                        // Create an item key
                        orderedItem = new ItemKey();
                        orderedItem.Id = producto.Codigo;

                        // Set the item key property of the sales order line object
                        salesOrderLine.ItemKey = orderedItem;

                        salesOrderLine.Key = new SalesLineKey();

                        salesOrderLine.Key.LineSequenceNumber = 16384 * cont;

                        // Create a sales order quantity object
                        orderedAmount = new Quantity();
                        orderedAmount.Value = producto.Cantidad;

                        // Set the quantity of the sales order line object
                        salesOrderLine.Quantity = orderedAmount;

                        WarehouseKey whKey = new WarehouseKey();
                        whKey.Id = "PRINCIPAL";
                        whKey.CompanyKey = companyKey;
                        salesOrderLine.WarehouseKey = whKey;
                        //salesOrderLine.UofM = "UND";

                        //Aquí aplico el precio
                        salesOrderLine.UnitPrice = new MoneyAmount { Value = producto.PrecioUnitario };
                        salesOrderLine.ItemDescription = producto.Descripcion;

                        //Aquí aplico los descuentos
                        if (producto.Descuento > 0)
                        {
                            salesOrderLine.Discount = new MoneyPercentChoice() { Item = new Percent() { Value = producto.Descuento } };
                        }

                        orders.Add(salesOrderLine);

                        cont++;
                    }

                    //Agregando el almacen a la cabecera
                    salesOrder.WarehouseKey = new WarehouseKey { Id = "PRINCIPAL", CompanyKey = companyKey };

                    // Add the sales order line array to the sales order
                    salesOrder.Lines = orders.ToArray();

                    // Get the create policy for the sales order object
                    //salesOrderCreatePolicy = wsDynamicsGP.GetPolicyByOperation("CreateSalesInvoice", context);
                    salesOrderCreatePolicy = wsDynamicsGP.GetPolicyByOperation("CreateSalesQuote", context);

                    // Create the sales order
                    wsDynamicsGP.CreateSalesQuote(salesOrder, context, salesOrderCreatePolicy);

                    // Close the service
                    if (wsDynamicsGP.State != CommunicationState.Faulted)
                    {
                        wsDynamicsGP.Close();
                    }

                    using (DevelopmentDbContext db_bnrd = new DevelopmentDbContext())
                    {
                        respuesta = db_bnrd.Database.SqlQuery<string>($"EXEC VerficicarExistenciaFactura 1, 'P{presupuestoDTO.QuoteNumber}'").FirstOrDefault();
                    }
                }

                return Content(HttpStatusCode.Created, respuesta.Trim());
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message.ToString());
            }
        }
    }
}
