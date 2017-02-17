using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

using WebApplication2.Attributes;
using WebApplication2.Models;
using WebApplication2.Infrastructure;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        _Book book;
        _Readership readership;
        _BooksHistory history = new _BooksHistory();
        BooksContainer booksContainer;
        Sender sender;
        public static bool IsAuthenticate = false;


        public ActionResult Index()
        {
            //Определяем, зарегистрирован ли пользователь,
            //и если да, то узнаем брал ли он книгу
                if (HttpContext.Request.Cookies["_AUTH"] != null)
            {
                readership = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString());
                ViewBag.IsBook = readership.BookId;
                IsAuthenticate = true;
            }
            else if (HttpContext.Request.Cookies["_ADM"] != null)
            {
                return RedirectToAction("IndexAdm", "Admin");
            }
            BooksReading();
            BooksContainer tempVar = new BooksContainer(booksContainer.GetAccount());
            return View(tempVar);
        }

        [HttpPost]
        [AuthAttribute]
        public ActionResult Index(string selectionBook, string returnBook)
        {
            readership = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString());
            sender = new Sender();
            int id;
            if (selectionBook != null)
            {
                id = int.Parse(selectionBook);
                DecrementSelected(int.Parse(selectionBook));
                BooksReading();
                BooksContainer tempVar = new BooksContainer(booksContainer.GetAccount());
                AddBooksToTheUser(id);
                history.AddBooksHistory(id, readership);
                ViewBag.IsBook = id;
                sender.SendMail(readership.Email);
                return View(tempVar);

            }
            else if (returnBook != null)
            {
                //Возвращаем книгу - устанавливаем у юзера id-книги
                // в "-1"
                id = int.Parse(returnBook);
                AddBooksToTheUser(-1);
                SetAvailable(id);
                history.BooksReturn(readership);
                ViewBag.IsBook = -1;
                BooksReading();
                BooksContainer tempVar = new BooksContainer(booksContainer.GetAccount());
                return View(tempVar);
            }
            return View();
        }

        //Разлогиниваемя
        public ActionResult LogOut()
        {
            if (HttpContext.Request.Cookies["_AUTH"] != null)
            {
                var cookie = new HttpCookie("_AUTH")
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                Response.Cookies.Add(cookie);
                IsAuthenticate = false;
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Request.Cookies["_ADM"] != null)
            {
                var cookie = new HttpCookie("_ADM")
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [AuthAttribute]
        public ViewResult BooksAvailable(string toggle)
        {
            //Получаем доступные на данный момент книги
            if (toggle == "Books available")
            {
                ViewBag.IsBook = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString()).BookId;
                return View(AllAvailable());
            }
            //Получаем взятые пользователем книги
            else if (toggle == "Taken books")
            {
                return View(TakenBooks(readership));
            }
            return View();
        }

        #region DataWorking
        //Считываем из базы наименования книг
        public BooksContainer BooksReading()
        {
            booksContainer = new BooksContainer();

            using (SqlConnection connection =
                new SqlConnection(ConnectionStrings.DBQuery))
            {
                SqlCommand command = new SqlCommand(ConnectionStrings.BooksQuery, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        book = new _Book();
                        book.BookName = reader[0].ToString();
                        book.Authors = reader[1].ToString();
                        book.Total = int.Parse(reader[2].ToString());
                        book.Available = int.Parse(reader[3].ToString());
                        book.Id = int.Parse(reader[4].ToString());
                        book.LinkPhoto = reader[5].ToString();
                        booksContainer.Add(book);
                    }
                    reader.Close();
                    return booksContainer;
                }
                catch (Exception ex)
                {
                    try
                    {
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                        writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                        writer.Close();
                    }
                    catch(NullReferenceException refExc)
                    {
                        Console.WriteLine(refExc.Message);
                    }
                    return null;
                }
            }
        }
        //Декремент колличества доступных книг
        void DecrementSelected(int Id)
        {
            int? available = GetAvailable(Id);
            if (available != null && available != 0)
            {
                string updateData = string.Format("Update Books Set Available = '{0}' Where Id = '{1}'", available - 1, Id);
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.DBQuery))
                {
                    try
                    {
                        SqlCommand commandUpd = new SqlCommand(updateData, connection);
                        connection.Open();
                        commandUpd.ExecuteNonQuery();
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
        //Получаем доступное колличество экземпляров выбранной книги
        int? GetAvailable(int id)
        {
            using (SqlConnection connection =
                new SqlConnection(ConnectionStrings.DBQuery))
            {
                SqlCommand command = new SqlCommand(string.Format("SELECT *From [Books] Where Id = '{0}'", id), connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    // 3-м от нуля будет наличие книг в библиотеке
                    int available = int.Parse(reader[3].ToString());
                    reader.Close();
                    return available;
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

        #endregion

        //Добавляем книгу пользователю 
        void AddBooksToTheUser(int id)
        {
            using (SqlConnection connection =
               new SqlConnection(ConnectionStrings.DBQuery))
            {
                try
                {
                    SqlCommand command = new SqlCommand(string.Format("Update Readership Set BookId = '{0}' Where AuthKey = '{1}'", id, Session["userKey"]), connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(Server.MapPath("~/App_Data/ErrorLogDataBase.txt"), true);
                    writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                    writer.Close();
                }
            }
        }

        //Взял ли пользователь книгу,
        //если нет, значение id будет "-1"
        _Readership GetUserData(string userKey)
        {
            readership = new _Readership();
            using (SqlConnection connection =
               new SqlConnection(ConnectionStrings.DBQuery))
            {
                try
                {
                    SqlCommand command = new SqlCommand(string.Format("Select *From Readership Where AuthKey = '{0}'", userKey), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    readership.Login = reader[0].ToString();
                    readership.Email = reader[1].ToString();
                    readership.CookieVal = userKey;
                    readership.BookId = int.Parse(reader[3].ToString());
                    // 3-м от нуля будет id-книги взятой пользователем
                    int id = int.Parse(reader[3].ToString());
                    reader.Close();
                    return readership;
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

        //Возвращаем книгу в базу
        void SetAvailable(int id)
        {
            int? available = GetAvailable(id);
            if (available != null && available != 0)
            {
                string updateData = string.Format("Update Books Set Available = '{0}' Where Id = '{1}'", available + 1, id);
                try
                {
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
        }

        //Получить доступные книги
        public BooksContainer AllAvailable()
        {
            booksContainer = new BooksContainer();

            using (SqlConnection connection =
                new SqlConnection(ConnectionStrings.DBQuery))
            {
                SqlCommand command = new SqlCommand(ConnectionStrings.BooksAvailable, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        book = new _Book();
                        book.BookName = reader[0].ToString();
                        book.Authors = reader[1].ToString();
                        book.Total = int.Parse(reader[2].ToString());
                        book.Available = int.Parse(reader[3].ToString());
                        book.LinkPhoto = reader[5].ToString();
                        booksContainer.Add(book);
                    }
                    reader.Close();
                    return booksContainer;
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

        //Получить взятые книги
        public BooksContainer TakenBooks(_Readership reader)
        {
            if (HttpContext.Request.Cookies["_AUTH"] != null)
            {
                reader = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString());
                ViewBag.IsBook = readership.BookId;
                booksContainer = BooksReading();
                foreach (_Book book in booksContainer)
                {
                    if (book.Id == reader.BookId)
                        return new BooksContainer().Add(book);
                }
            }
            //Для админа отобразим книги, находящиеся в данное время "на руках"
            else if (HttpContext.Request.Cookies["_ADM"] != null)
            {
                reader = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString());
                booksContainer = BooksReading();
                foreach (_Book book in booksContainer)
                {
                    if (book.Id == reader.BookId)
                        return new BooksContainer().Add(book);
                }
            }
            return null;
        }

        public ActionResult SortByName()
        {
            booksContainer = BooksReading();
            booksContainer.GetAccount().Sort();
            ViewBag.IsBook = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString()).BookId;
            return View(booksContainer);
        }

        public ActionResult SortByAuthors()
        {
            booksContainer = BooksReading();
            booksContainer.GetAccount().Sort(new SortByAuthors());
            ViewBag.IsBook = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString()).BookId;
            return View(booksContainer);
        }

    }

}
