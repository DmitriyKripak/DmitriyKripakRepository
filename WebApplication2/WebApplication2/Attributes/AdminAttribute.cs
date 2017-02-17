using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using WebApplication2.Infrastructure;


namespace WebApplication2.Attributes
{
    //Атрибут авторизации с данными администратора
    public class AdminAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Request.Cookies["_ADM"] == null)
            {
                return false;
            }
            else if(httpContext.Request.Cookies["_ADM"].Value.ToString() == AdminData.CookieVal)
            {
                return true;
            }
            return false;
        }

    }
}