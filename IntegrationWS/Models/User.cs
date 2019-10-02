using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public byte[] Passwordhash { get; set; }

        public byte[] Passwordsalt { get; set; }
    }
}