using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class ComisionMedicaDTO
    {
        public string SalesPersonId { get; set; }
        public bool IsVaccines { get; set; }
        public string MonthOfTheYear { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public decimal Quote { get; set; }
        public decimal BaseAward { get; set; }
        public decimal TotalSales { get; set; }
        public decimal PercentageOfSales { get; set; }
        public decimal Award { get; set; }


        //public string SalesPersonId { get; set; }
        //public bool IsVaccines { get; set; }
        //public int Month { get; set; }
        //public int Year { get; set; }
        //public decimal Quote { get; set; }
        //public decimal BaseAward { get; set; }
        //public decimal TotalSales { get; set; }
        //public decimal PercentageOfSales { get; set; }
        //public decimal Award { get; set; }
    }
}