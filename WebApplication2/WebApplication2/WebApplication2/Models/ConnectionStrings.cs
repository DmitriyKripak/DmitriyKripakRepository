using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public static class ConnectionStrings
    {
        public static string DBQuery = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
           C:\Users\Дмитрий\Documents\Visual Studio 2015\Projects\WebApplication2\WebApplication2\App_Data\LibraryEvents.mdf;
           Integrated Security=True;";

        public static string BooksQuery =
            "SELECT BookName, Authors, Total, Available, Id, LinkPhoto from dbo.Books ";

        public static string BooksAvailable =
           "SELECT BookName, Authors, Total, Available, Id, LinkPhoto from dbo.Books Where Available > 0";


        public static string UsersQuery = 
        "SELECT Login, Email, AuthKey from dbo.Readership ";
    }
}