using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class _Book : IComparable
    {
        public string BookName{ get; set; }
        public string Authors { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }
        public int Id { get; set; }
        public string LinkPhoto { get; set; }

        #region Interfaces
        public int CompareTo(object obj)
        {
            _Book temp = obj as _Book;
            if (temp != null)
            {
                if (this.BookName.ToCharArray()[0] > temp.BookName.ToCharArray()[0])
                    return 1;
                if (this.BookName.ToCharArray()[0] < temp.BookName.ToCharArray()[0])
                    return -1;
                else
                    return 0;
            }
            else
                throw new ArgumentException("Argument is wrong!!!");
        }
        #endregion
    }

    public class SortByAuthors : IComparer<_Book>
    {
        public int Compare(_Book b1, _Book b2)
        {
            if (b1 != null && b2 != null)
                return string.Compare(b1.Authors, b2.Authors);
            else
                throw new ArgumentException("Incorrect parameters!!!");
        }
    }
}