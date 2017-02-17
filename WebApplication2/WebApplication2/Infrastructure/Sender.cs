using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Net.Mail;
using WebApplication2.Models;


namespace WebApplication2.Infrastructure
{
    //Класс для отправки сообщений пользователю
    public class Sender
    {
        const string SMTP = "smtp.mail.ru";
        const string EMAIL = "SimpleLibrary@mail.ru";
        const string PASSWORD = "Simp_LB85";
        const string CAPTION = "Remainder from the LibraryReaders";
        const string MESSAGE = "We remind you that you borrowed a book in our library. Do not forget to return it.";


        public void SendMail(string mailto, string attachFile = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(EMAIL);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = CAPTION;
                mail.Body = MESSAGE;
                if (!string.IsNullOrEmpty(attachFile))
                    mail.Attachments.Add(new Attachment(attachFile));
                SmtpClient client = new SmtpClient();
                client.Host = SMTP;
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(EMAIL.Split('@')[0], PASSWORD);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }
    }
   
}