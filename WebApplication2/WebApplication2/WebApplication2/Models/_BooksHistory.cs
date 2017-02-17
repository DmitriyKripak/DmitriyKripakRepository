using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class _BooksHistory
    {
        public string WhoTook { get; set; }
        public string DateOfIssue { get; set; }
        public string DateOfReturn { get; set; }

        public void ShowHistory(_Book book)
        {
            //TODO: показать данные из таблицы истории
            //по значению book
        }
    }
}