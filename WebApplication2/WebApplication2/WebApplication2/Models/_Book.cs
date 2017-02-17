using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class _Book
    {
        public string BookName{ get; set; }
        public string Authors { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }
        public int Id { get; set; }
        public string LinkPhoto { get; set; }
    }
}