using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using WebApplication2.CoockieSession;

namespace WebApplication2.Attributes
{
    //Атрибут авторизации пользователя
    public class AuthAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Request.Cookies["_AUTH"] == null)
            {
                return false;
            }
            else
            {
                httpContext.Session["userKey"] = httpContext.Request.Cookies["_AUTH"].Value.ToString();
                return true;
            }
        }

    }
}
