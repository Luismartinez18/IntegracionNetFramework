using IntegrationWS.Data;
using IntegrationWS.DTOs;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Extensions;
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
    [Authorize]
    public class ProductController : ApiController
    {
        [HttpPost]
        public IHttpActionResult post(ProductDTO productDTO)
        {
            try
            {
                Pedido pedido = new Pedido();

                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(productDTO);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();
                }

                // Create an instance of the service
                using (DynamicsGPClient wsDynamicsGP = new DynamicsGPClient())
                {
                    CompanyKey companyKey = new CompanyKey();
                    Context context = new Context();
                    SalesItem product = new SalesItem();
                    companyKey.Id = 1;
                    context.OrganizationKey = companyKey;
                    
                    ItemKey productKey = new ItemKey();
                    string productId = productDTO.DynamicId;
                    string CountryId = string.Empty;
                    //bool isInsert = string.IsNullOrEmpty(productDTO.DynamicId);
                    //if (!isInsert)
                    //{
                    //    productKey.Id = productDTO.DynamicId;
                    //    try
                    //    {
                    //        //product = wsDynamicsGP.GetSalesIte(productKey, context);

                    //        vendorId = product.Key.Id;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        ///return Ok(IntegrationResult.GetNotFoundResult($"Ha ocurrido un error al buscar el registro solicitado. {e.ToString()}"));
                    //    }
                    //}
                    //else
                    //{
                    //    product = new SalesItem();
                    //    //using (BnrdDbContextToRemove db_dev = new BnrdDbContextToRemove())
                    //    //{
                    //    //    vendorId = db_dev.Database.SqlQuery<string>($"EXEC GetSequenceVendorId").FirstOrDefault();
                    //    //    vendorKey.Id = vendorId;
                    //    //    product.Key = vendorKey;
                    //    //}
                    //}




                    product.Key = new ItemKey { CompanyKey = companyKey, Id = productDTO.DynamicId };
                    // Set properties for the new customer
                    if (!string.IsNullOrEmpty(productDTO.Name))
                        product.Description = productDTO.Name;
                    if (!string.IsNullOrEmpty(productDTO.GenericDescription))
                        product.GenericDescription = productDTO.GenericDescription;
                    if (!string.IsNullOrEmpty(productDTO.ShortDescription))
                        product.ShortDescription= productDTO.ShortDescription;
                    if (!string.IsNullOrEmpty(productDTO.ProductType))
                        product.Type = productDTO.GetItemType();
                    if (!string.IsNullOrEmpty(productDTO.ClassId))
                    {
                        product.ClassKey = new ItemClassKey();
                        product.ClassKey.Id = productDTO.ClassId;
                    }
                    
                    if(!string.IsNullOrEmpty(productDTO.PurchaseTaxesOption))
                        product.PurchaseTaxBasis = productDTO.GetPurchasingTaxBasis();
                    if(!string.IsNullOrEmpty(productDTO.SaleTaxesOption))
                        product.SalesTaxBasis = productDTO.GetSalesTaxBasis();
                    if (!string.IsNullOrEmpty(productDTO.SaleTaxesPlanId))
                        product.SalesTaxScheduleKey = new TaxScheduleKey() { CompanyKey = companyKey, Id = productDTO.SaleTaxesPlanId };
                    if (!string.IsNullOrEmpty(productDTO.PurchaseTaxesPlanId))
                        product.PurchaseTaxScheduleKey = new TaxScheduleKey() { CompanyKey = companyKey, Id = productDTO.PurchaseTaxesPlanId };
                    if(!string.IsNullOrEmpty(productDTO.UOMPlanId))
                        product.UofMScheduleKey = new UofMScheduleKey() { CompanyKey = companyKey, Id = productDTO.UOMPlanId };
                    if (productDTO.StandardCost != null)
                        product.StandardCost = new MoneyAmount { DecimalDigits = 2, Value = (decimal)productDTO.StandardCost };
                    if (productDTO.ActualCost != null)
                        product.CurrentCost = new MoneyAmount { DecimalDigits =2, Value =(decimal)productDTO.ActualCost};
                    if (!string.IsNullOrEmpty(productDTO.SubstituteItem1))
                        product.SubstituteItem1Key = new ItemKey { CompanyKey = companyKey,Id = productDTO.SubstituteItem1};
                    if (!string.IsNullOrEmpty(productDTO.SubstituteItem2))
                        product.SubstituteItem2Key = new ItemKey { CompanyKey = companyKey, Id = productDTO.SubstituteItem2 };
                    if (productDTO.ABCCode != null)
                        product.ABCCode = productDTO.GetABCCode();
                    if (!string.IsNullOrEmpty(productDTO.CategoryDivision))
                        product.UserCategoryList1 = productDTO.CategoryDivision;
                    if (!string.IsNullOrEmpty(productDTO.CategoryClass))
                        product.UserCategoryList2 = productDTO.CategoryClass;
                    if (!string.IsNullOrEmpty(productDTO.Category))
                        product.UserCategoryList3 = productDTO.Category;
                    if (!string.IsNullOrEmpty(productDTO.CategoryType))
                        product.UserCategoryList4 = productDTO.CategoryType;
                    if (!string.IsNullOrEmpty(productDTO.CategorySource))
                        product.UserCategoryList5 = productDTO.CategorySource;
                    if (!string.IsNullOrEmpty(productDTO.CategoryXRT))
                        product.UserCategoryList6 = productDTO.CategoryXRT;
                    product.UofMScheduleKey = new UofMScheduleKey { Id = "UND", CompanyKey = companyKey };
                    //product.UserCategoryList1 = 
                    // Get the create policy for the customer
                    var itemPolicy = wsDynamicsGP.GetPolicyByOperation("CreateSalesItem", context);
                    //Upsert product
                    wsDynamicsGP.CreateSalesItem(product, context, itemPolicy);

                    // Close the service
                    if (wsDynamicsGP.State != CommunicationState.Faulted)
                    {
                        wsDynamicsGP.Close();
                    }

                    return Ok(IntegrationResult.GetSuccessResult(productId));
                }
            }
            catch (Exception e)
            {
                return Ok(IntegrationResult.GetErrorResult(e.ToString()));
            }
        } 
    }
}
