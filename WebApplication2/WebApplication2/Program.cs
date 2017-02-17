//Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="C:\Users\Дмитрий\Documents\Visual Studio 2015\Projects\ExtractFromDataBase\ExtractFromDataBase\SampleDatabase.mdf";Integrated Security=True;Connect Timeout=30
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ExtractingData
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
           @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
           c:\users\дмитрий\documents\visual studio 2015\Projects\ExtractingData\ExtractingData\SampleDatabase.mdf;
           Integrated Security=True;Connect Timeout=30";

            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT CustomerID, CompanyName, ContactName, ContactTitle from dbo.Customers ";


            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                //command.Parameters.AddWithValue("@pricePoint", paramValue);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}\t{3}",
                            reader[0], reader[1], reader[2], reader[3]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }
        }
    }
}
