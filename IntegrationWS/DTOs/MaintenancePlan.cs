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
        public DateTime? VAR_FECHA_ULT_MANT { get; set; }
        [Required]
        public DateTime? VAR_FECHA_PROX_MANT { get; set; }

        public string Fabricante { get; set; }
        [Required]

        public string VAR_MODELO { get; set; }
        [Required]
        public string VAR_ACTIVO { get; set; }
        public string VAR_SERIE { get; set; }
        public string OwnerName { get; set; }


    }
    public class EquipmentinWorkshop
    {
        [Required]
        public string aId { get; set; }
        [Required]
        public DateTime? VAR_FECHA_PROX_MANT { get; set; }
        [Required]
        public DateTime? VAR_Date_Ingre { get; set; }

        public string Fabricante { get; set; }
        [Required]
        public string VAR_MODELO { get; set; }
        [Required]
        public string VAR_ACTIVO { get; set; }
        public string VAR_SERIE { get; set; }

        public string AccountName { get; set; }
        public string EQUIPO_EN_TALLER { get; set; }
        public string OwnerName { get; set; }
    }
    public class TechnicalEvaluation
    {
        public string aId { get; set; }
        [Required]
        public string Nombre_RST_Ingenier { get; set; }
        public string Nombre_RST_Aplicaciones { get; set; }
        [Required]
        public DateTime? Fecha_Evalucion_Ingenieria { get; set; }
        [Required]
        public DateTime? Fecha_Proximo_Mantenimiento { get; set; }
        [Required]
        public DateTime? Fecha_Evaluacion_Aplicacion { get; set; }
        public string Fabricante { get; set; }
        [Required]
        public string VAR_MODELO { get; set; }
        [Required]
        public string VAR_ACTIVO { get; set; }
        public string VAR_SERIE { get; set; }

    }
}