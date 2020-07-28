using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string DynamicId { get; set; }
        public string ShortDescription { get; set; }
        public string GenericDescription { get; set; }
        public string ProductType { get; set; }
        public string ValuationMethod { get; set; }
        public string ClassId { get; set; }
        public int DecimalsQuantity { get; set; }
        public string SaleTaxesOption { get; set; }
        public string PurchaseTaxesOption { get; set; }
        public string SaleTaxesPlanId { get; set; }
        public string PurchaseTaxesPlanId { get; set; }
        public string UOMPlanId { get; set; }
        public double? StandardCost { get; set; }
        public double? ActualCost { get; set; }
        public string ABCCode { get; set; }
        public string SubstituteItem1 { get; set; }
        public string SubstituteItem2 { get; set; }
        public string CategoryDivision { get; set; }
        public string CategoryClass { get; set; }
        public string Category { get; set; }
        public string CategoryType { get; set; }
        public string CategorySource { get; set; }
        public string CategoryXRT { get; set; }
    }
}