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
    public class QuoteController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody] CaseDTO caseDTO)
        {
            try
            {

                Pedido pedido = new Pedido();

                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(caseDTO);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();
                }                

                
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
                companyKey.Id = (caseDTO.companyKeyId);

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
                customerKey.Id = caseDTO.NumeroDeLaCuenta;

                // Set the customer key property of the sales order object
                salesOrder.CustomerKey = customerKey;

                // Create a batch key
                batchKey = new BatchKey();
                batchKey.Id = $"COT{DateTime.Now.ToString("ddMMyyyy")}";

                //Set Vendor
                SalespersonKey salespersonKey = new SalespersonKey();
                salespersonKey.Id = "00086"; //<-- Colocando a Hector como vendedor fijo

                salesOrder.SalespersonKey = salespersonKey;


                //Agregando comentario
                var comentario = $@"Numero de caso: {caseDTO.CaseNumber}, Serie del equipo: {caseDTO.Nmerodeserie}, Nombre del activo: {caseDTO.Nombre_del_activo},";

                if (caseDTO.OrdenesConCitas != null)
                {
                    //Creando string para comentario en factura
                    foreach (var item in caseDTO.OrdenesConCitas)
                    {
                        comentario += $" [Orden: {item.WorkOrderNumber}";

                        foreach (var item2 in item.ServiceAppointments)
                        {
                            comentario += $", (Cita: {item2.AppointmentNumber}, Recurso: {item2.Recurso_asignado__c})";
                        }

                        comentario += "].";
                    }
                }

                CommentKey commentKey = new CommentKey();
                commentKey.Id = $"C{caseDTO.CaseNumber}";

                salesOrder.CommentKey = commentKey;
                salesOrder.Comment = comentario;

                // Set the batch key property of the sales order object
                salesOrder.BatchKey = batchKey;                

                //creando lista de SalesOrderLine
                List<SalesQuoteLine> orders = new List<SalesQuoteLine>();

                //Iterando los productos recibidos
                int cont = 1;
                foreach (var product in caseDTO.products)
                {

                    SalesQuoteLine salesOrderLine = new SalesQuoteLine();
                    // Create an item key
                    orderedItem = new ItemKey();
                    orderedItem.Id = product.C_digo_del_producto__c;

                    // Set the item key property of the sales order line object
                    salesOrderLine.ItemKey = orderedItem;

                    salesOrderLine.Key = new SalesLineKey();

                    salesOrderLine.Key.LineSequenceNumber = 16384 * cont;

                    // Create a sales order quantity object
                    orderedAmount = new Quantity();
                    orderedAmount.Value = product.QuantityConsumed__c;

                    // Set the quantity of the sales order line object
                    salesOrderLine.Quantity = orderedAmount;

                    WarehouseKey whKey = new WarehouseKey();

                    //Agregando el almacen a la linea
                    if (product.C_digo_del_producto__c == "serv000001" || product.C_digo_del_producto__c == "serv000050" || product.C_digo_del_producto__c == "serv000002")
                    {
                        whKey.Id = "PRINCIPAL";
                        whKey.CompanyKey = companyKey;
                        salesOrderLine.WarehouseKey = whKey;
                        salesOrderLine.UofM = "UND";

                        //Redondeando a .5 o a valores enteros
                        Quantity valorTruncado = new Quantity();
                        valorTruncado.Value = Math.Truncate(Convert.ToDecimal(salesOrderLine.Quantity.Value));

                        if (salesOrderLine.Quantity.Value != valorTruncado.Value)
                        {
                            decimal diff = salesOrderLine.Quantity.Value - valorTruncado.Value;

                            if (diff > 0.5m)
                            {
                                salesOrderLine.Quantity.Value = valorTruncado.Value + 1m;
                            }
                            else
                            {
                                salesOrderLine.Quantity.Value = valorTruncado.Value + 0.5m;
                            }
                        }
                    }
                    else
                    {
                        whKey.Id = product.Locker__c;
                        whKey.CompanyKey = companyKey;
                        salesOrderLine.WarehouseKey = whKey;
                    }

                    decimal? monto;

                    using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
                    {
                        if (product.C_digo_del_producto__c == "serv000001" || product.C_digo_del_producto__c == "serv000050" || product.C_digo_del_producto__c == "serv000002")
                        {
                            monto = db_dev.Database.SqlQuery<decimal?>($"EXEC SP_GP_BuscarPrecioProducto '{product.C_digo_del_producto__c.Trim()}', {orderedAmount.Value}, '{caseDTO.NumeroDeLaCuenta}'").FirstOrDefault();
                            orderedAmount.Value = 1;
                        }
                        else
                        {
                            monto = db_dev.Database.SqlQuery<decimal?>($"BuscarPrecioEnListaAsignada '{caseDTO.NumeroDeLaCuenta}', '{product.C_digo_del_producto__c}', '{product.QuantityUnitOfMeasure__c}', {product.QuantityConsumed__c}").FirstOrDefault();
                        }

                        if (monto != null)
                        {
                            salesOrderLine.UnitPrice = new MoneyAmount { Value = (decimal)monto };

                            if (product.Descuento__c > 0)
                            {
                                salesOrderLine.Discount = new MoneyPercentChoice() { Item = new Percent() { Value = product.Descuento__c } };
                            }
                        }
                        else if (monto == null)
                        {
                            throw new Exception($"El producto {product.C_digo_del_producto__c} no tiene precio asignado.");
                        }
                    }

                    orders.Add(salesOrderLine);

                    cont++;                    
                }

                if (caseDTO.CargosAdicionales != null)
                {
                    foreach (var cargoAdicional in caseDTO.CargosAdicionales)
                    {
                        SalesQuoteLine salesOrderLine = new SalesQuoteLine();

                        // Create an item key
                        orderedItem = new ItemKey();
                        orderedItem.Id = cargoAdicional.Codigo;

                        // Set the item key property of the sales order line object
                        salesOrderLine.ItemKey = orderedItem;

                        salesOrderLine.Key = new SalesLineKey();

                        salesOrderLine.Key.LineSequenceNumber = 16384 * cont;

                        // Create a sales order quantity object
                        orderedAmount = new Quantity();
                        orderedAmount.Value = cargoAdicional.Cantidad;

                        // Set the quantity of the sales order line object
                        salesOrderLine.Quantity = orderedAmount;

                        WarehouseKey whKey = new WarehouseKey();
                        whKey.Id = "PRINCIPAL";
                        whKey.CompanyKey = companyKey;
                        salesOrderLine.WarehouseKey = whKey;
                        salesOrderLine.UofM = "UND";

                        decimal? monto;

                        if (cargoAdicional.Codigo != "serv000025" && cargoAdicional.Codigo != "serv000013")
                        {
                            using (DevelopmentDbContext db_dev = new DevelopmentDbContext())
                            {
                                monto = db_dev.Database.SqlQuery<decimal?>($"BuscarPrecioEnListaAsignada '{caseDTO.NumeroDeLaCuenta}', '{cargoAdicional.Codigo}', 'UND', {cargoAdicional.Cantidad}").FirstOrDefault();

                                if (monto != null)
                                {
                                    salesOrderLine.UnitPrice = new MoneyAmount { Value = (decimal)monto };

                                }
                                else if (monto == null)
                                {
                                    throw new Exception($"El cargo adicional {cargoAdicional.Codigo} no tiene precio asignado.");
                                }
                            }
                        }

                        //Cargo de viaje
                        if (cargoAdicional.Codigo == "serv000025")
                        {
                            salesOrderLine.UnitPrice = new MoneyAmount { Value = cargoAdicional.PrecioUnitario };
                        }

                        //Productos no inventariados.
                        if (cargoAdicional.Codigo == "serv000013")
                        {
                            salesOrderLine.UnitPrice = new MoneyAmount { Value = cargoAdicional.PrecioUnitario };
                            salesOrderLine.ItemDescription = cargoAdicional.Descripcion;
                        }

                        //Aquí aplico los descuentos
                        if (cargoAdicional.Descuento > 0)
                        {
                            salesOrderLine.Discount = new MoneyPercentChoice() { Item = new Percent() { Value = cargoAdicional.Descuento } };
                        }

                        orders.Add(salesOrderLine);

                        cont++;
                    }
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

                string respuesta = string.Empty;

                using (DevelopmentDbContext db_bnrd = new DevelopmentDbContext())
                {
                    respuesta = db_bnrd.Database.SqlQuery<string>($"EXEC VerficicarExistenciaFactura 1, 'C{caseDTO.CaseNumber}'").FirstOrDefault();
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
