using IntegrationWS.Data;
using IntegrationWS.DTOs;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace IntegrationWS.Controllers
{
    public class VendorController : ApiController
    {
        [HttpGet(),Route("vendor/{id}")]
        [AllowAnonymous]
        public IHttpActionResult Get(string id)
        {
            using (DynamicsGPClient wsDynamicsGP = new DynamicsGPClient())
            {
                CompanyKey companyKey = new CompanyKey();
                Context context = new Context();
                companyKey.Id = 1;
                context.OrganizationKey = companyKey;
                var key = new VendorKey();
                key.Id = id;
                var vendor = wsDynamicsGP.GetVendorByKey(key,context);
                return Ok(vendor);
            }
        }
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody] VendorDTO vendorDTO)
        {
            try
            {
                Pedido pedido = new Pedido();

                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(vendorDTO);

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
                    Vendor vendor = default;
                    companyKey.Id = 1;
                    context.OrganizationKey = companyKey;
                    VendorKey vendorKey = new VendorKey();
                    string vendorId = string.Empty;
                    string CountryId = string.Empty;
                    bool isInsert = string.IsNullOrEmpty(vendorDTO.DynamicId);
                    if (!isInsert)
                    {
                        vendorKey.Id = vendorDTO.DynamicId;
                        try
                        {
                            vendor = wsDynamicsGP.GetVendorByKey(vendorKey, context);
                            vendorId = vendor.Key.Id;
                        }
                        catch (Exception e)
                        {
                            return Ok(IntegrationResult.GetNotFoundResult($"Ha ocurrido un error al buscar el registro solicitado. {e.ToString()}"));
                        }
                    }
                    else
                    {
                        vendor = new Vendor();
                        using (BnrdDbContextToRemove db_dev = new BnrdDbContextToRemove())
                        {
                            vendorId = db_dev.Database.SqlQuery<string>($"EXEC GetSequenceVendorId").FirstOrDefault();
                            vendorKey.Id = vendorId;
                            vendor.Key = vendorKey;
                        }
                    }






                    // Set properties for the new customer
                    if (!string.IsNullOrEmpty(vendorDTO.Name))
                        vendor.Name = vendorDTO.Name;
                    if (!string.IsNullOrEmpty(vendorDTO.ShortName))
                        vendor.ShortName = vendorDTO.ShortName;
                    if(!string.IsNullOrEmpty(vendorDTO.CheckTitle))
                        vendor.CheckName = vendorDTO.CheckTitle;
                    if (!string.IsNullOrEmpty(vendorDTO.ClassId))
                    {
                        vendor.ClassKey = new VendorClassKey();
                        vendor.ClassKey.Id = vendorDTO.ClassId;
                    }

                    //RNC
                    string RNC = Regex.Replace(vendorDTO.Rnc, @"[^0-9]", "");
                    if (RNC.Length != 9 && RNC.Length != 11)
                        throw new Exception("El RNC debe ser de 9 o 13 caracteres.");

                    using (BnrdDbContext db_bnrd = new BnrdDbContext())
                    {
                        //El RNC es solo numeros por las RegEx es decir bloquea el sql injection
                        var CustFromDb = db_bnrd.Database.SqlQuery<string>($"SELECT VENDORID " +
                                                                       $"FROM BNRD.dbo.PM00200 " +
                                                                       $"WHERE TXRGNNUM = @p0 AND " +
                                                                       $"TXRGNNUM <> '000000000'", RNC).FirstOrDefault()?.Trim();

                        if (!string.IsNullOrEmpty(CustFromDb) && CustFromDb != vendorId)
                            throw new Exception($"RNC duplicado con el Proveedor numero: {CustFromDb}");
                    }

                    vendor.TaxRegistrationNumber = RNC;
                    vendor.UserDefined1 = RNC;

                    //MONEDA
                    if (!string.IsNullOrEmpty(vendorDTO.CurrencyId))
                    {
                        vendor.CurrencyKey = new CurrencyKey();
                        vendor.CurrencyKey.ISOCode = vendorDTO.CurrencyId;
                    }
                    if (!string.IsNullOrEmpty(vendorDTO.BankAccount))
                    {
                        vendor.BankAccountKey = new BankAccountKey();
                        vendor.BankAccountKey.CompanyKey = companyKey;
                        vendor.BankAccountKey.Id = vendorDTO.BankAccount;
                    }
                    if(!string.IsNullOrEmpty(vendorDTO.PaymentPriority))
                        vendor.PaymentPriority = vendorDTO.PaymentPriority;

                    // Customer Address Key
                    VendorAddressKey vendorAddressKey = new VendorAddressKey();
                    vendorAddressKey.VendorKey = vendorKey;
                    vendorAddressKey.Id = "PRINCIPAL";
                    // Customer Address List with Contact Person
                    VendorAddress[] vendorAddresses = new VendorAddress[1];
                    vendorAddresses[0] = new VendorAddress();
                    vendorAddresses[0].Key = vendorAddressKey;
                    vendorAddresses[0].Line1 = vendorDTO.DirLine1;
                    vendorAddresses[0].Line2 = vendorDTO.DirLine2;
                    vendorAddresses[0].Line3 = vendorDTO.DirLine3;
                    vendorAddresses[0].City = vendorDTO.City;
                    vendorAddresses[0].State = vendorDTO.StateProvince;
                    vendorAddresses[0].CountryRegion = vendorDTO.Country;
                    vendorAddresses[0].CountryRegionCodeKey = new CountryRegionCodeKey();
                    vendorAddresses[0].CountryRegionCodeKey.Id = CountryId;
                    vendorAddresses[0].PostalCode = vendorDTO.ZipCode;

                    vendorAddresses[0].Phone1 = new PhoneNumber();
                    vendorAddresses[0].Phone1.Value = vendorDTO.Phone1;

                    vendorAddresses[0].Phone2 = new PhoneNumber();
                    vendorAddresses[0].Phone2.Value = vendorDTO.Phone2;

                    vendorAddresses[0].Phone3 = new PhoneNumber();
                    vendorAddresses[0].Phone3.Value = vendorDTO.Phone3;

                    vendorAddresses[0].Fax = new PhoneNumber();
                    vendorAddresses[0].Fax.Value = vendorDTO.Fax;

                    vendorAddresses[0].ContactPerson = vendorDTO.Contact;
                    vendor.DefaultAddressKey = vendorAddressKey;
                    vendor.Addresses = vendorAddresses;
                    vendor.MinimumOrderAmount = new MoneyAmount();
                    vendor.MinimumOrderAmount.Currency = vendorDTO.CurrencyId;
                    vendor.MinimumOrderAmount.Value = Convert.ToDecimal(vendorDTO.MinimumOrderAmount);
                    vendor.PaymentPriority = vendorDTO.PaymentPriority;
                    PaymentTermsKey paymentTermsKey = new PaymentTermsKey();
                    paymentTermsKey.CompanyKey = companyKey;
                    paymentTermsKey.Id = vendorDTO.PaymentTerms;
                    vendor.PaymentTermsKey = paymentTermsKey;
                    vendor.PaymentPriority = vendorDTO.PaymentPriority;
                    vendor.RateTypeKey = new RateTypeKey();
                    vendor.RateTypeKey.CompanyKey = companyKey;
                    vendor.RateTypeKey.Id = vendorDTO.RateType;

                    // Get the create policy for the customer
                    var vendorPolicy = wsDynamicsGP.GetPolicyByOperation("CreateVendor", context);
                    //Upsert vendor
                    if (isInsert)
                        wsDynamicsGP.CreateVendor(vendor, context, vendorPolicy);
                    else
                        wsDynamicsGP.UpdateVendor(vendor, context, vendorPolicy);

                    // Close the service
                    if (wsDynamicsGP.State != CommunicationState.Faulted)
                    {
                        wsDynamicsGP.Close();
                    }

                    using (var db = new DevelopmentDbContext())
                    {
                        int opcionLimiteCredito = vendorDTO.CreditUnlimited ? 1 : vendorDTO.CreditLimit == 0 ? 0 : 2;
                        int opcionCancelaciones = vendorDTO.CancellationsUnlimited ? 1 : vendorDTO.Cancellations == 0 ? 0 : 2;
                        var result = db.Database.SqlQuery<IntegrationResult>("EXEC INS_SF_OpcionesMaestroProveedores_BNRD @VendorId, @BancoBPD, @BancoLEON, @ClasificacionNCF, @OpcionLimiteCredito, @MontoLimiteCredito, @OpcioneCancelaciones,@MontoCancelaciones",
                            new SqlParameter("@VendorId",vendorId), 
                            new SqlParameter("@BancoBPD", (object)vendorDTO.BankAccountBPD??DBNull.Value),
                            new SqlParameter("@BancoLEON", (object)vendorDTO.BankAccountBHDL??DBNull.Value), 
                            new SqlParameter("@ClasificacionNCF", (object)vendorDTO.NCFClasification??DBNull.Value),
                            new SqlParameter("@OpcionLimiteCredito", (object)opcionLimiteCredito??DBNull.Value),
                            new SqlParameter("@MontoLimiteCredito", (object)vendorDTO.CreditLimit??DBNull.Value), 
                            new SqlParameter("@OpcioneCancelaciones", (object)opcionCancelaciones??DBNull.Value),
                            new SqlParameter("@MontoCancelaciones",(object)vendorDTO.Cancellations??DBNull.Value)).FirstOrDefault();
                        
                    }
                    return Ok(IntegrationResult.GetSuccessResult(vendorId));
                }
            }
            catch (Exception e)
            {
                return Ok(IntegrationResult.GetErrorResult(e.ToString()));
            }
        }
    }
}
