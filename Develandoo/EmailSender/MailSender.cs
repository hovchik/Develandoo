using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Develandoo.EmailSender
{//Oriented only for Gmail
    public class MailSender
    {
        private MailCredentional _credentials;
        public MailSender(MailCredentional cred)
        {
            _credentials = cred;
        }
        public Task SendAsync(string Subject, string Body,string Destination)
        {
            return Task.Run(() =>
            {
                MailMessage mess = new System.Net.Mail.MailMessage();
                string fromEmail = _credentials.Email;
                string fromPW = _credentials.Password;
                string toEmail = Destination;
                mess.From = new MailAddress(fromEmail);
                mess.To.Add(toEmail);
                mess.Subject = Subject;
                mess.Body = Body;
                mess.IsBodyHtml = true;
                mess.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(fromEmail, fromPW);

                    smtpClient.Send(mess.From.ToString(), mess.To.ToString(),
                                    mess.Subject, mess.Body);
                }
            });
        }
    }
}