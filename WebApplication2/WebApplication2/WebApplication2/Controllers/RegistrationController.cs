using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using WebApplication2.Models;
using System.Data.SqlClient;
using WebApplication2.CoockieSession;

namespace WebApplication2.Controllers
{
    public class RegistrationController : Controller
    {
        const string CONNECTION_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
           C:\Users\Дмитрий\Documents\Visual Studio 2015\Projects\WebApplication2\WebApplication2\App_Data\LibraryEvents.mdf;
           Integrated Security=True;";
        //string SQL_QUERY = string.Format("Insert Into [Readership]" +
        //           "(Login, Email, BookId, AuthKey) Values(@Login, @Email, @BookId, @AuthKey)");
        string SQL_QUERY = string.Format("Insert Into [Readership]" +
                   "(Login, Email, AuthKey) Values(@Login, @Email, @AuthKey)");
        CookieSet _setCook = new CookieSet();

        public ActionResult RegForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegForm(_Readership reader)
        {
            reader.CookieVal = _setCook.CookieGet().Value.ToString();
            Session["userKey"] = reader.CookieVal;
            HttpContext.Response.Cookies.Add(_setCook.CookieGet());
            AddUser(CONNECTION_STRING, SQL_QUERY, reader);
            // return View();
            return RedirectToAction("Index", "Home");
        }

        //        if (Request.Cookies["_AUTH"] == null)
        //            {
        //                Response.Cookies.Add(cookie);
        //                Session["UserKey"] = cookie.Value;
        //                uData.CookieValue = Session["UserKey"].ToString();
        //                return View();
        //    }
        //            else
        //            {
        //                Session["UserKey"] = Request.Cookies["_AUTH"].Value;
        //                return View();
        //}

        public void AddUser(string connectionString, string sql, _Readership reader)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    cmd.Parameters.AddWithValue("@Login", reader.Login);
                    cmd.Parameters.AddWithValue("@Email", reader.Email);
                    //if (reader.BookName != null)
                    //    cmd.Parameters.AddWithValue("@BookName", reader.BookName);
                    cmd.Parameters.AddWithValue("@AuthKey", reader.CookieVal);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}