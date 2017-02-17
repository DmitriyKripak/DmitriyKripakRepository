using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections;

namespace WebApplication2.Models
{
    public class BooksContainer : IEnumerable
    {
        //Класс содержит коллекцию объектов _Book
        List<_Book> bookCollection;
        public BooksContainer()
        {
            bookCollection = new List<_Book>();
        }

        public BooksContainer(List<_Book> books)
        {
            bookCollection = new List<_Book>(books);
        }

        public void Show()
        {
            //TODO: Отобразить все объекты коллекции
        }

        public BooksContainer Add(_Book book)
        {
            bookCollection.Add(book);
            return this;
        }

        public List<_Book> GetAccount()
        {
            return this.bookCollection;
        }

        public IEnumerator GetEnumerator()
        {
            return this.bookCollection.GetEnumerator();
        }
    }
}