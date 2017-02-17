using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace WebApplication2.CoockieSession
{
    public class CookieSet
    {
        HttpCookie cookie;

        public CookieSet()
        {
            cookie = new HttpCookie("_AUTH")
            {
                Value = Guid.NewGuid().ToString(),
                Expires = DateTime.Now.AddYears(1)
            };
        }

        public HttpCookie CookieGet()
        {
            if (this.cookie != null)
                return cookie;
            else
                return null;
        }

        public HttpCookie CookieGet(string cookieVal)
        {
            if (this.cookie != null)
            {
                return cookie = new HttpCookie("_AUTH")
                {
                    Value = cookieVal,
                    Expires = DateTime.Now.AddYears(1)
                };
            }
            else
                return null;
        }

    }
}