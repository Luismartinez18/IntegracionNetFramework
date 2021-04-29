using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class StockPlanDTO
    {
        public string DocumentNumber { get; set; }
        public string StockPlanId { get; set; }
        public string DescriptionPlan { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string WarehouseId { get; set; }
        public List<StockPlanDetailDTO> Detail { get; set; }

    }
    public class StockPlanDetailDTO
    {

        public string ItemNumber { get; set; }

        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Variation { get; set; }
        public short TypeitemNumber { get; set; }
        public List<StockPlanSerialLotDTO> Detail { get; set; }

    }

    public class StockPlanSerialLotDTO
    {
        public DateTime? DateReceived { get; set; }
        public int DateSEQNumber { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal Variation { get; set; }
    }

}