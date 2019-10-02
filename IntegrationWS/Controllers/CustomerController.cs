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
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace IntegrationWS.Controllers
{
    public class CustomerController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody] LeadDTO leadDTO)
        {
            try
            {
                Pedido pedido = new Pedido();

                pedido.jsonCompleto = new JavaScriptSerializer().Serialize(leadDTO);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();
                }
                
                CompanyKey companyKey;
                Context context;
                Customer customer;
                CustomerKey customerKey;
                Policy customerPolicy;
                // Create an instance of the service
                DynamicsGPClient wsDynamicsGP = new DynamicsGPClient();
                // Create a context with which to call the web service
                context = new Context();
                // Specify which company to use (lesson company)
                companyKey = new CompanyKey();
                companyKey.Id = (1);
                // Set up the context
                context.OrganizationKey = (OrganizationKey)companyKey;
                // Create a new customer object
                customer = new Customer();
                // Create a customer key
                customerKey = new CustomerKey();

                string CustormerId = string.Empty;
                string CountryId = string.Empty;
                string CodigoVendedor = string.Empty;
                using (BnrdDbContextToRemove db_dev = new BnrdDbContextToRemove())
                {
                    CustormerId = db_dev.Database.SqlQuery<string>($"EXEC GetSequenceCustomerId").FirstOrDefault();
                    CountryId = db_dev.Database.SqlQuery<string>($"EXEC GetCountryCode '{leadDTO.Pa_s__c}'").FirstOrDefault();
                    CodigoVendedor = db_dev.Database.SqlQuery<string>($"EXEC ObtenerCodigoVendedor '{leadDTO.Correo_Ejecutivo_de_Ventas__c}'").FirstOrDefault();
                }

                customerKey.Id = CustormerId;
                customer.Key = customerKey;

                // Set properties for the new customer
                customer.Name = leadDTO.Company;

                //Id de clase
                customer.ClassKey = new CustomerClassKey();
                customer.ClassKey.Id = leadDTO.Id_de_clase__c;

                //RNC
                string RNC = Regex.Replace(leadDTO.RNC__c, @"[^0-9]", "");

                if(RNC.Length != 9 && RNC.Length != 11)
                    throw new Exception("El RNC debe ser de 9 o 13 caracteres.");

                string CustFromDb = string.Empty;                

                using (BnrdDbContext db_bnrd = new BnrdDbContext())
                {
                    //El RNC es solo numeros por las RegEx es decir bloquea el sql injection
                    CustFromDb = db_bnrd.Database.SqlQuery<string>($"SELECT CUSTNMBR " +
                                                                   $"FROM BNRD.dbo.RM00101 " +
                                                                   $"WHERE TXRGNNUM = '{RNC}' AND " +
                                                                   $"TXRGNNUM <> '000000000'").FirstOrDefault();                    
                }

                if (CustFromDb != null)
                    throw new Exception($"RNC duplicado con el cliente numero: {CustFromDb}");
                
                customer.TaxRegistrationNumber = RNC;
                customer.UserDefined1 = RNC;

                if (string.IsNullOrEmpty(CodigoVendedor))
                    throw new Exception($"El ejecutivo de ventas registrado no existe en el ERP. Correo del ejecutivo: {leadDTO.Correo_Ejecutivo_de_Ventas__c}");

                //MONEDA
                customer.CurrencyKey = new CurrencyKey();
                customer.CurrencyKey.ISOCode = leadDTO.CurrencyIsoCode;                

                // Customer Address Key
                CustomerAddressKey customerAddressKey = new CustomerAddressKey();
                customerAddressKey.CustomerKey = customerKey;
                customerAddressKey.Id = "PRINCIPAL";
                // Customer Address List with Contact Person
                CustomerAddress[] customerAddresses = new CustomerAddress[1];
                customerAddresses[0] = new CustomerAddress();
                customerAddresses[0].Key = customerAddressKey;
                customerAddresses[0].Line1 = $"{leadDTO.Calle__c}, {leadDTO.N_mero__c}";
                customerAddresses[0].Line2 = "S/I";
                customerAddresses[0].Line3 = leadDTO.Sector__c;
                customerAddresses[0].City = leadDTO.Ciudad__c;
                customerAddresses[0].State = leadDTO.Provincia__c;
                customerAddresses[0].CountryRegion = leadDTO.Pa_s__c;
                customerAddresses[0].CountryRegionCodeKey = new CountryRegionCodeKey();
                customerAddresses[0].CountryRegionCodeKey.Id = CountryId;
                customerAddresses[0].PostalCode = "S/I";

                customerAddresses[0].Phone1 = new PhoneNumber();
                customerAddresses[0].Phone1.Value = leadDTO.Phone;

                customerAddresses[0].Phone2 = new PhoneNumber();
                customerAddresses[0].Phone2.Value = leadDTO.Tel_fono_2__c;

                customerAddresses[0].Phone3 = new PhoneNumber();
                customerAddresses[0].Phone3.Value = leadDTO.MobilePhone;

                customerAddresses[0].Fax = new PhoneNumber();
                customerAddresses[0].Fax.Value = leadDTO.Fax;

                customerAddresses[0].ContactPerson = leadDTO.Name;
                customer.DefaultAddressKey = customerAddressKey;
                customer.Addresses = customerAddresses;

                // Get the create policy for the customer
                customerPolicy = wsDynamicsGP.GetPolicyByOperation("CreateCustomer", context);
                
                // Create the customer
                wsDynamicsGP.CreateCustomer(customer, context, customerPolicy);

                // Close the service
                if (wsDynamicsGP.State != CommunicationState.Faulted)
                {
                    wsDynamicsGP.Close();
                }

                using(BnrdDbContextToRemove dev = new BnrdDbContextToRemove())
                {
                    dev.Database.ExecuteSqlCommand($"EXEC AddFieldsInfoToAccount " +
                                                   $"  '{CustormerId}'" +
                                                   $", '{leadDTO.Plan_de_impuesto__c}'" +
                                                   $", '{leadDTO.Correo_Ejecutivo_de_Ventas__c}'" +
                                                   $", '{leadDTO.Region__c}'" +
                                                   $", '{leadDTO.Condiciones_de_pago__c}'" +
                                                   $", '{leadDTO.Nombre_de_la_lista__c}'" +
                                                   $", '{leadDTO.Tipo_de_NCF__c}'" +
                                                   $", '{leadDTO.Ejecutivo_de_ventas_DD__c}'" +
                                                   $", '{leadDTO.Ejecutivo_de_ventas_DM__c}'" +
                                                   $", '{leadDTO.Ejecutivo_de_ventas_DH__c}'" +
                                                   $", '{leadDTO.Email}'" +
                                                   $", '{leadDTO.Website}'" +
                                                   $", '{leadDTO.Id_de_clase__c}'");
                }

                return Content(HttpStatusCode.Created, CustormerId);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message.ToString().Trim());
            }            
        }
    }
}
