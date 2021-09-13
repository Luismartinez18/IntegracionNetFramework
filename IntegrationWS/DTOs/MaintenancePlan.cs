using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class PrintMaintenancePlan
    {
        [Required]
        public string aId { get; set; }
        [Required]
        public string VAR_ID_RST_Name_RST { get; set; }
        [Required]
        public DateTime? VAR_Date_Last_Mnt { get; set; }
        [Required]
        public DateTime? VAR_Date_Ingre { get; set; }

        public string Fabricante { get; set; }
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string ActiveName { get; set; }
        public string Serie { get; set; }
        [Required]
        public string Propiedad { get; set; }
        public string EQUIPO_EN_TALLER { get; set; }
    }
}