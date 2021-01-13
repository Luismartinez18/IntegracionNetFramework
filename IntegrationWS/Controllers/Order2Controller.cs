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
    public class Order2Controller : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody] OpportunitiesDTO opportunitiesDTO)
        {
            try
            {
                Pedido pedido = new Pedido();

                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(opportunitiesDTO);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();
                }

                var respuesta = string.Empty;

                using (BnrdDbContextToRemove db_bnrd = new BnrdDbContextToRemove())
                {
                    respuesta = db_bnrd.Database.SqlQuery<string>($"EXEC VerficicarExistenciaPedido 1, '{opportunitiesDTO.Id}'").FirstOrDefault();

                    if (string.IsNullOrEmpty(respuesta))
                    {
                        respuesta = db_bnrd.Database.SqlQuery<string>($"EXEC VerficicarExistenciaPedido 3, '{opportunitiesDTO.Id}'").FirstOrDefault();
                    }
                }

                if (string.IsNullOrEmpty(respuesta))
                {
                    CompanyKey companyKey;
                    Context context;
                    SalesOrder salesOrder;
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
                    companyKey.Id = 1;

                    // Set up the context object
                    context.OrganizationKey = companyKey;


                    // Create a sales order object
                    salesOrder = new SalesOrder();


                    // Create a sales document type key for the sales order
                    salesOrderType = new SalesDocumentTypeKey();
                    salesOrderType.Type = SalesDocumentType.Order;

                    // Populate the document type key of the sales order object
                    salesOrder.DocumentTypeKey = salesOrderType;

                    // Create a customer key
                    customerKey = new CustomerKey();
                    customerKey.Id = opportunitiesDTO.CodigoCliente;

                    // Set the customer key property of the sales order object
                    salesOrder.CustomerKey = customerKey;

                    // Create a batch key
                    batchKey = new BatchKey();
                    batchKey.Id = $"PED{DateTime.Now.ToString("ddMMyyyy")}";


                    CommentKey commentKey = new CommentKey();
                    commentKey.Id = opportunitiesDTO.Id;

                    salesOrder.CommentKey = commentKey;
                    salesOrder.Comment = opportunitiesDTO.Name;

                    // Set the batch key property of the sales order object
                    salesOrder.BatchKey = batchKey;

                    //creando lista de SalesOrderLine
                    List<SalesOrderLine> orders = new List<SalesOrderLine>();

                    //Iterando los productos recibidos
                    int cont = 1;
                    foreach (var product in opportunitiesDTO.OpportunityLineItemModels)
                    {

                        SalesOrderLine salesOrderLine = new SalesOrderLine();
                        // Create an item key
                        orderedItem = new ItemKey();
                        orderedItem.Id = product.CodigoDeProducto;

                        // Set the item key property of the sales order line object
                        salesOrderLine.ItemKey = orderedItem;

                        salesOrderLine.Key = new SalesLineKey();

                        salesOrderLine.Key.LineSequenceNumber = 16384 * cont;

                        // Create a sales order quantity object
                        orderedAmount = new Quantity();
                        orderedAmount.Value = product.Cantidad;

                        // Set the quantity of the sales order line object
                        salesOrderLine.Quantity = orderedAmount;

                        WarehouseKey whKey = new WarehouseKey();

                        whKey.Id = opportunitiesDTO.Sucursal;
                        whKey.CompanyKey = companyKey;
                        salesOrderLine.WarehouseKey = whKey;

                        decimal? monto;

                        using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
                        {
                            monto = db_dev.Database.SqlQuery<decimal?>($"BuscarPrecioEnListaAsignada '{opportunitiesDTO.CodigoCliente}', '{product.CodigoDeProducto}', '{product.UnidadDeMedida}', {product.Cantidad}").FirstOrDefault();

                            if (monto != null)
                            {
                                salesOrderLine.UnitPrice = new MoneyAmount { Value = (decimal)monto };

                                if (product.Descuento > 0)
                                {
                                    salesOrderLine.Discount = new MoneyPercentChoice() { Item = new Percent() { Value = product.Descuento } };
                                }
                            }
                            else if (monto == null)
                            {
                                throw new Exception($"El producto {product.CodigoDeProducto} no tiene precio asignado.");
                            }
                        }

                        orders.Add(salesOrderLine);

                        cont++;
                    }

                    //Agregando el almacen a la cabecera
                    salesOrder.WarehouseKey = new WarehouseKey { Id = opportunitiesDTO.Sucursal, CompanyKey = companyKey };

                    // Add the sales order line array to the sales order
                    salesOrder.Lines = orders.ToArray();

                    // Get the create policy for the sales order object
                    //salesOrderCreatePolicy = wsDynamicsGP.GetPolicyByOperation("CreateSalesInvoice", context);
                    salesOrderCreatePolicy = wsDynamicsGP.GetPolicyByOperation("CreateSalesOrder", context);

                    // Create the sales order
                    wsDynamicsGP.CreateSalesOrder(salesOrder, context, salesOrderCreatePolicy);

                    // Close the service
                    if (wsDynamicsGP.State != CommunicationState.Faulted)
                    {
                        wsDynamicsGP.Close();
                    }

                    using (BnrdDbContextToRemove db_bnrd = new BnrdDbContextToRemove())
                    {
                        respuesta = db_bnrd.Database.SqlQuery<string>($"EXEC VerficicarExistenciaPedido 1, '{opportunitiesDTO.Id}'").FirstOrDefault();
                    }
                }

                return Ok(IntegrationResult.GetSuccessResult(respuesta.Trim()));
            }
            catch (FaultException<ExceptionDetail> fe)
            {
                return Content(HttpStatusCode.BadRequest, IntegrationResult.GetErrorResult(fe.Detail?.Message ?? fe.Message));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, IntegrationResult.GetErrorResult(e.ToString()));
            }
        }
    }
}
