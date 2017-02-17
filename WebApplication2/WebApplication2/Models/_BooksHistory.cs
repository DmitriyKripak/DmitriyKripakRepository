using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;


namespace WebApplication2.Models
{

    public class _BooksHistory
    {
        public string WhoTook { get; set; }
        public string DateOfIssue { get; set; }
        public string DateOfReturn { get; set; }
        List<_BooksHistory> historyContainer = new List<_BooksHistory>();

        //Пишем историю книги
        public void AddBooksHistory(int id, _Readership reader)
        {
            string insert = "Insert Into BooksHistory(Id, Login, DateOfIssue) Values(@Id, @Login, @DateOfIssue)";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.DBQuery))
                {
                    using (SqlCommand cmd = new SqlCommand(insert, connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Login", reader.Login);
                        cmd.Parameters.AddWithValue("@DateOfIssue", DateTime.Now.ToShortDateString());
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\Dmitriy\Downloads\WebApplication2\WebApplication2\WebApplication2\App_Data\ErrorLogDataBase.txt", true);
                writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                writer.Close();
            }
        }
        //Посмотреть историю книги
        public List<_BooksHistory> ShowBooksHistory(int id)
        {
            string showHistory = string.Format("SELECT *From BooksHistory Where Id = '{0}'", id);// Login, DateOfIssue, DateOfReturn
            _BooksHistory bookHistory;
            using (SqlConnection connection =
              new SqlConnection(ConnectionStrings.DBQuery))
            {
                SqlCommand command = new SqlCommand(showHistory, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        bookHistory = new _BooksHistory();
                        bookHistory.WhoTook = reader[1].ToString();
                        bookHistory.DateOfIssue = reader[2].ToString();
                        bookHistory.DateOfReturn = reader[3].ToString();
                        historyContainer.Add(bookHistory);
                    }
                    reader.Close();
                    return historyContainer;
                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\Dmitriy\Downloads\WebApplication2\WebApplication2\WebApplication2\App_Data\ErrorLogDataBase.txt", true);
                    writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                    writer.Close();
                    return null;
                }
            }
            
        }
        //Вернуть книгу
        public void BooksReturn(_Readership reader)
        {
            string insert = string.Format("Update BooksHistory Set DateOfReturn = '{0}' Where Login = '{1}'", DateTime.Now.ToShortDateString(), reader.Login);

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.DBQuery))
                {
                    using (SqlCommand cmd = new SqlCommand(insert, connection))
                    {
                        connection.Open();
                        
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\Dmitriy\Downloads\WebApplication2\WebApplication2\WebApplication2\App_Data\ErrorLogDataBase.txt", true);
                writer.WriteLine(string.Format("Date : " + DateTime.Now.ToString() + " Error : " + ex.Message));
                writer.Close();
            }
        }
    }
}