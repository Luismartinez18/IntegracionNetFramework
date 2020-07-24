using IntegrationWS.DTOs;
using IntegrationWS.DynamicsGPService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Extensions
{
    public static class ProductDTOExtensions
    {
        private static Dictionary<string, ItemType> itemTypes;
        public static Dictionary<string,ItemType> ItemTypes
        {
            get
            {
                if(itemTypes == null)
                {
                    itemTypes = new Dictionary<string, ItemType>();
                    itemTypes.Add("Inventario ventas", ItemType.SalesItem);
                    itemTypes.Add("Kits", ItemType.Kit);
                    itemTypes.Add("Cargos misceláneos", ItemType.MiscellaneousCharges);
                    itemTypes.Add("Servicios", ItemType.Service);
                    itemTypes.Add("Honor, fijos", ItemType.FlatFee);
                }
                return itemTypes;
            }
        }
        private static Dictionary<string, PurchasingTaxBasis> purchaseTaxOptions;
        public static Dictionary<string,PurchasingTaxBasis> PurchaseTaxOptions
        {
            get
            {
                if(purchaseTaxOptions == null)
                {
                    purchaseTaxOptions = new Dictionary<string, PurchasingTaxBasis>();
                    purchaseTaxOptions.Add("Gravable",PurchasingTaxBasis.Taxable);
                    purchaseTaxOptions.Add("No gravable", PurchasingTaxBasis.Nontaxable);
                    purchaseTaxOptions.Add("Basar en proveedores", PurchasingTaxBasis.BasedOnVendor);
                }
                return purchaseTaxOptions;
            }
        }
        private static Dictionary<string, SalesTaxBasis> salesTaxOptions;
        public static Dictionary<string, SalesTaxBasis> SalesTaxOptions
        {
            get
            {
                if (salesTaxOptions == null)
                {
                    salesTaxOptions = new Dictionary<string, SalesTaxBasis>();
                    salesTaxOptions.Add("Gravable", SalesTaxBasis.Taxable);
                    salesTaxOptions.Add("No gravable", SalesTaxBasis.Nontaxable);
                    salesTaxOptions.Add("Basar en clientes", SalesTaxBasis.BasedOnCustomer);
                }
                return salesTaxOptions;
            }
        }
        private static Dictionary<string, ABCCode> aBCCodes;
        public static Dictionary<string, ABCCode> ABCCodes
        {
            get
            {
                if (aBCCodes == null)
                {
                    aBCCodes = new Dictionary<string, ABCCode>();
                    aBCCodes.Add("A", ABCCode.A);
                    aBCCodes.Add("B", ABCCode.B);
                    aBCCodes.Add("C", ABCCode.C);
                }
                return aBCCodes;
            }
        }
        public static ItemType? GetItemType(this ProductDTO productDTO)
        {
            if (productDTO.ProductType == "Descontinuado")
                return null;
            if (ItemTypes.ContainsKey(productDTO.ProductType))
                return ItemTypes[productDTO.ProductType];
            else
                throw new Exception($"{productDTO.ProductType} is not a correct value to the field ProductType.");
        }
        public static PurchasingTaxBasis GetPurchasingTaxBasis(this ProductDTO productDTO)
        {
            if (PurchaseTaxOptions.ContainsKey(productDTO.PurchaseTaxesOption))
                return PurchaseTaxOptions[productDTO.PurchaseTaxesOption];
            else
                throw new Exception($"{productDTO.PurchaseTaxesOption} is not a correct value to the field {nameof(productDTO.PurchaseTaxesOption)}.");
        }
        public static SalesTaxBasis GetSalesTaxBasis(this ProductDTO productDTO)
        {
            if (SalesTaxOptions.ContainsKey(productDTO.SaleTaxesOption))
                return SalesTaxOptions[productDTO.SaleTaxesOption];
            else
                throw new Exception($"{productDTO.PurchaseTaxesOption} is not a correct value to the field {nameof(productDTO.SaleTaxesOption)}.");
        }
        public static ABCCode GetABCCode(this ProductDTO productDTO)
        {
            if (ABCCodes.ContainsKey(productDTO.ABCCode))
                return ABCCodes[productDTO.ABCCode];
            else
                throw new Exception($"{productDTO.PurchaseTaxesOption} is not a correct value to the field {nameof(productDTO.SaleTaxesOption)}.");
        }
    }
}