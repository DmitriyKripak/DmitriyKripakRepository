using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

using WebApplication2.Attributes;
using WebApplication2.Models;
using WebApplication2.CoockieSession;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        _Book book;
        BooksContainer booksContainer;
        _Readership readership;

        public ViewResult Index()
        {
            //Определяем, зарегистрирован ли пользователь,
            //и если да, то узнаем брал ли он книгу
            if (HttpContext.Request.Cookies["_AUTH"] != null)
            {
                readership = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString());
                ViewBag.IsBook = readership.BookId;
            }
            BooksReading();
            BooksContainer tempVar = new BooksContainer(booksContainer.GetAccount());
            return View(tempVar);
        }

        [HttpPost]
        [AuthAttribute]
        public ViewResult Index(string selectionBook, string returnBook)
        {
            //Найти в табл Session["userKey"] и добавить книгу 
            if (selectionBook != null)
            {
                DecrementSelected(int.Parse(selectionBook));
                BooksReading();
                BooksContainer tempVar = new BooksContainer(booksContainer.GetAccount());
                AddBooksToTheUser(int.Parse(selectionBook));
                ViewBag.IsBook = int.Parse(selectionBook);
                return View(tempVar);

            }
            else if (returnBook != null)
            {
                //Возвращаем книгу - устанавливаем id-книги
                // в "-1"
                AddBooksToTheUser(-1);
                SetAvailable(int.Parse(returnBook));
                ViewBag.IsBook = -1;
                BooksReading();
                BooksContainer tempVar = new BooksContainer(booksContainer.GetAccount());
                return View(tempVar);
            }
            return View();
        }

        //________Проверить фильтр Taken Books!!!!!
        public ViewResult BooksAvailable(string toggle)
        {
            if (toggle == "Books available")
                return View(AllAvailable());
            else if (toggle == "Taken books")//(toggle == "Taken books" && readership != null)
                return View(TakenBooks(readership));
            return View();
        }

        #region DataWorking
        //считываем из базы наименования книг
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
                    //throw new Exception(ex.Message);
                    Console.WriteLine(ex.Message);
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
                    SqlCommand commandUpd = new SqlCommand(updateData, connection);
                    connection.Open();
                    commandUpd.ExecuteNonQuery();
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
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            #endregion
        }
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
                    Console.WriteLine(ex.Message);
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
                    Console.WriteLine(ex.Message);
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
                    Console.WriteLine(ex.Message);
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
                    //throw new Exception(ex.Message);
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
        //Получить взятые книги
        public BooksContainer TakenBooks(_Readership reader)
        {
            reader = GetUserData(HttpContext.Request.Cookies["_AUTH"].Value.ToString());
            booksContainer = BooksReading();
            foreach (_Book book in booksContainer)
            {
                if (book.Id == reader.BookId)
                    return new BooksContainer().Add(book);

            }
            return null;
        }
    }

}
