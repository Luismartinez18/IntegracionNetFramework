using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class General_Audit_History
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string TableName { get; set; }

        [StringLength(100)]
        public string DynamicsId { get; set; }

        [StringLength(50)]
        public string Activity { get; set; }

        [StringLength(100)]
        public string DoneBy { get; set; }

        public DateTime DateOfChanged { get; set; }
    }
}