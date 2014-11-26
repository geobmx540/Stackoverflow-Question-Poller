using System;
using System.Net.Mail;
using System.Configuration;

namespace Stackoverflow_Question_Poller
{
    class Email
    {
        public static void SendEmail(string title, string body, string link)
        {
            //get configured settings
            string smtpServer = ConfigurationManager.AppSettings["stmp_server"];
            string emailFrom = ConfigurationManager.AppSettings["email_from"];
            string emailFromCred = ConfigurationManager.AppSettings["email_from_cred"];
            string emailTo = ConfigurationManager.AppSettings["email_to"];
            bool isBodyHtml = bool.Parse(ConfigurationManager.AppSettings["is_body_html"]);
            int stmpClientPort = Int32.Parse(ConfigurationManager.AppSettings["smtp_client_port"]);
                
            using (var smtpClient = new SmtpClient(smtpServer))
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = title;
                mail.IsBodyHtml = isBodyHtml;
                mail.Body = body + "\n\n" + link;
                smtpClient.Port = stmpClientPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(emailFrom, emailFromCred);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);
            }
        }

    }
}
