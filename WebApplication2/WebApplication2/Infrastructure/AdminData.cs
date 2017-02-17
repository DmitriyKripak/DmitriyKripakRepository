using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Infrastructure
{
    //Данные авторизации администратора
    public static class AdminData
    {
        public static string Name { get; set; }
        public static string Email { get; set; }
        public static string CookieVal { get; set; }

        static AdminData()
        {
            Name      = "AdminVasya";
            Email     = "vasya@mail.adm";
            CookieVal = "BorodatyiAdmin";
        }
    }
}