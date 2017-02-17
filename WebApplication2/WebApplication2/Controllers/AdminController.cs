using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebApplication2.Attributes;
using WebApplication2.Models;
using System.Data.SqlClient;

namespace WebApplication2.Controllers
{
    public class AdminController : Controller
    {
        BooksContainer container;
        _Readership readership;
        List<_Readership> listReaders;
        Dictionary<_Readership, string> whoTookInfo;
        HomeController hCtrl = new HomeController();

        [AdminAttribute]
        public ActionResult IndexAdm()
        {
            container = hCtrl.BooksReading();
            return View(container);
        }

        [AdminAttribute]
        [HttpPost]
        public ActionResult ShowHistory(string WatchHistory, string Delete)
        {

            if (WatchHistory != null)
            {
                _BooksHistory history = new _BooksHistory();
                return View(history.ShowBooksHistory(int.Parse(WatchHistory)));
            }
            else
                return null;
        }

        [AdminAttribute]
        [HttpPost]
        public ActionResult ChangeQuantity(string сhangeQuantity, string nameSubmit)
        {
            int quant;
            try
            {
                if (сhangeQuantity != null && сhangeQuantity != string.Empty)
                {
                    quant = int.Parse(сhangeQuantity);
                    string updateData = string.Format("Update Books Set Total = '{0}', Available = '{0}' Where Id = '{1}'", quant, int.Parse(nameSubmit));
                    using (SqlConnection connection = new SqlConnection(ConnectionStrings.DBQuery))
                    {
                        SqlCommand commandUpd = new SqlCommand(updateData, connection);
                        connection.Open();
                        commandUpd.ExecuteNonQuery();
                    }

                    return View(quant);
                }
            }
            catch (Exception ex)
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                writer.Close();
            }

            return View();
        }

        [AdminAttribute]
        [HttpPost]
        public ActionResult DeleteBook(string Delete)
        {
            if (Delete != null)
            {
                try
                {
                    int id = int.Parse(Delete);
                    string updateData = string.Format("Delete Books Where Id = '{0}'", id);
                    using (SqlConnection connection = new SqlConnection(ConnectionStrings.DBQuery))
                    {
                        SqlCommand commandUpd = new SqlCommand(updateData, connection);
                        connection.Open();
                        commandUpd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                    writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                    writer.Close();
                }
            }

            return View();
        }

        public ActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, _Book book)
        {
            if (upload != null)
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    string linkPhoto = string.Format("~/Resources/Photo/" + fileName);
                    upload.SaveAs(Server.MapPath(linkPhoto));
                    AddBookToDB(book, linkPhoto);
                }
                catch(Exception ex)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                    writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                    writer.Close();
                }
            }
            return RedirectToAction("IndexAdm");
        }

        public ActionResult LogOut()
        {
            var cookie = new HttpCookie("_ADM")
            {
                Expires = DateTime.Now.AddDays(-1d)
            };
            Response.Cookies.Add(cookie);
            return RedirectToAction("RegForm", "Registration");
        }

        [AdminAttribute]
        public ActionResult TakenBooks()
        {
            whoTookInfo = new Dictionary<_Readership, string>();
            List<_Readership> tempList = GetAllUser();
            foreach (_Readership r in tempList)
            {
                //По узерам определить какие книги взяли, извлечь из таблицы "LinkPhoto"
                whoTookInfo.Add(r, GetLinkPhoto(r.BookId));
            }
            return View(whoTookInfo);
        }
       
        //Получаем всех зарегистрированных пользователей
        List<_Readership> GetAllUser()
        {
            listReaders = new List<_Readership>();
            using (SqlConnection connection =
               new SqlConnection(ConnectionStrings.DBQuery))
            {
                try
                {
                    SqlCommand command = new SqlCommand(string.Format("Select *From Readership Where BookId != '-1'"), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        readership = new _Readership();
                        readership.Login = reader[0].ToString();
                        readership.Email = reader[1].ToString();
                        readership.CookieVal = reader[2].ToString();
                        readership.BookId = int.Parse(reader[3].ToString());
                        listReaders.Add(readership);
                    }
                    reader.Close();
                    return listReaders;
                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                    writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                    writer.Close();
                    return null;
                }
            }
        }

        //Получаем ссылку на изображение книги
        string GetLinkPhoto(int id)
        {
            string link = string.Empty;
            using (SqlConnection connection =
                new SqlConnection(ConnectionStrings.DBQuery))
            {
                SqlCommand command = new SqlCommand(String.Format("SELECT *From [Books] LinkPhoto Where Id = '{0}'", id), connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    link = reader[5].ToString();
                    reader.Close();
                    return link;
                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                    writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                    writer.Close();
                    return null;
                }
            }
        }

        public ActionResult SortByName()
        {
            container = hCtrl.BooksReading();
            container.GetAccount().Sort();
            return View(container);
        }

        public ActionResult SortByAuthors()
        {
            container = hCtrl.BooksReading();
            container.GetAccount().Sort( new SortByAuthors());
            return View(container);
        }

        public void AddBookToDB(_Book book, string linkPhoto)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings.DBQuery))
            {
                using (SqlCommand cmd = new SqlCommand(ConnectionStrings.AddBook, connection))
                {
                    try
                    {
                        connection.Open();

                        cmd.Parameters.AddWithValue("@BookName", book.BookName);
                        cmd.Parameters.AddWithValue("@Authors", book.Authors);
                        cmd.Parameters.AddWithValue("@Total", book.Total);
                        cmd.Parameters.AddWithValue("@Available", book.Total);
                        cmd.Parameters.AddWithValue("@LinkPhoto", linkPhoto);

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                        writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                        writer.Close();
                    }
                }
            }
        }
       
    }
}
