using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.ModelsNotMapped
{
    public class ordenConCita
    {
        public string WorkOrderNumber { get; set; }
        public List<citas> ServiceAppointments { get; set; }
    }
}