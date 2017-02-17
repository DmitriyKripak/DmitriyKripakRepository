using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class _Readership
    {
        [Required]
        public string Login     { get; set; }
        [Required]
        public string Email     { get; set; }
        public string CookieVal { get; set; }
        public int BookId { get; set; }
    }
}