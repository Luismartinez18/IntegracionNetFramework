using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntegrationWS.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe contener entre 8 y 20 caracteres")]
        public string Password { get; set; }
    }
}