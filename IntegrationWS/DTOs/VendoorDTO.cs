using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class VendorDTO
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string CheckTitle { get; set; }
        public string ClassId { get; set; }
        public string Contact { get; set; }
        public string DirLine1 { get; set; }
        public string DirLine2 { get; set; }
        public string DirLine3 { get; set; }
        public string City { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Fax { get; set; }
        public string TaxPlan { get; set; }
        public string ShippingMethod { get; set; }
        public string ZipCode { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string BankAccount { get; set; }
        public string BankAccountBPD { get; set; }
        public string BankAccountBHDL { get; set; }
        public string CurrencyId { get; set; }
        public string RateType { get; set; }
        public string PaymentTerms { get; set; }
        public int? DiscountGracePeriod { get; set; }
        public int? ExpirationDateGracePeriod { get; set; }
        public string PaymentPriority { get; set; }
        public double MinimumOrderAmount {get; set; }
        public double CommercialDiscount { get; set; }
        public string NCFClasification { get; set; }
        public string Rnc { get; set; }
        public string CheckBookId { get; set; }
        public string TaxType { get; set; }
        public string PaymentFor { get; set; }
        public double? MinimumPayment { get; set; }
        public double? MaxBillAmount { get; set; }
        public bool MaxBillUnlimited { get; set; }
        public decimal CreditLimit { get; set; }
        public bool CreditUnlimited { get; set; }
        public decimal Cancellations { get; set; }
        public bool CancellationsUnlimited { get; set; }
        public bool? VendorRevalue { get; set; }
        public string DynamicId { get; set; }
        public string UpsZone { get; set; }
    }
}