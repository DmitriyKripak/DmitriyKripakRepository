using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using WebApplication2.Models;
using System.Data.SqlClient;
using WebApplication2.CoockieSession;
using WebApplication2.Infrastructure;
using System.Threading;
using System.Text;

namespace WebApplication2.Controllers
{
    public class RegistrationController : Controller
    {
        
        CookieSet _setCook = new CookieSet();
        StringBuilder build = new StringBuilder();

        public ActionResult RegForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegForm(_Readership reader)
        {
            if (ModelState.IsValid)
            {
                //Не админ ли?_______
                if (reader.Login == AdminData.Name && reader.Email == AdminData.Email)
                {
                    HomeController.IsAuthenticate = true;
                    reader.CookieVal = AdminData.CookieVal;
                    var cookie = new HttpCookie("_ADM")
                    {
                        Value = AdminData.CookieVal,
                        Expires = DateTime.Now.AddYears(1)
                    };
                    HttpContext.Response.Cookies.Add(cookie);
                    return RedirectToAction("IndexAdm", "Admin");
                }
                //____________________
                else if (SearchSameUser(reader))
                {
                    HomeController.IsAuthenticate = true;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    HomeController.IsAuthenticate = true;
                    reader.CookieVal = _setCook.CookieGet().Value.ToString();
                    Session["userKey"] = reader.CookieVal;
                    HttpContext.Response.Cookies.Add(_setCook.CookieGet());
                    AddUser(ConnectionStrings.DBQuery, ConnectionStrings.AddNewReader, reader);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                //Даем возможность клиенту прочитать сообщение валидатора
                //и опять возвращаем ему форму регистрации
                Thread.Sleep(5000);
                return View();
            }
        }

        //Добавляем нового пользователя в базу
        public void AddUser(string connectionString, string sql, _Readership reader)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings.DBQuery))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();

                        cmd.Parameters.AddWithValue("@Login", reader.Login);
                        cmd.Parameters.AddWithValue("@Email", reader.Email);
                        cmd.Parameters.AddWithValue("@AuthKey", reader.CookieVal);

                        cmd.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                        writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                        writer.Close();
                    }
                }
            }
        }

        //Ищем в базе юзера с такими же данными
        bool SearchSameUser(_Readership readerShip)
        {

            using (SqlConnection connection =
                new SqlConnection(ConnectionStrings.DBQuery))
            {
                SqlCommand command = new SqlCommand(ConnectionStrings.UsersQuery, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (readerShip.Login == reader[0].ToString().Trim() && readerShip.Email == reader[1].ToString().Trim())
                        {
                            //Если данные совпадают - возвращаем пользователю ранее установленные куки
                            HttpContext.Response.Cookies.Add(new HttpCookie("_AUTH")
                            {
                                Value = reader[2].ToString().Trim(),
                                Expires = DateTime.Now.AddYears(1)
                            });
                            reader.Close();
                            return true;
                        }
                    }
                    reader.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                    writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                    writer.Close();
                    return false;
                }
            }
        }

    }
}