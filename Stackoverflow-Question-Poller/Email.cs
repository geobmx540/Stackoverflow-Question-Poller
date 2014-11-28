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
            var smtpServer = ConfigurationManager.AppSettings["stmp_server"];
            var emailFrom = ConfigurationManager.AppSettings["email_from"];
            var emailFromCred = ConfigurationManager.AppSettings["email_from_cred"];
            var emailTo = ConfigurationManager.AppSettings["email_to"];
            var isBodyHtml = bool.Parse(ConfigurationManager.AppSettings["is_body_html"]);
            var stmpClientPort = Int32.Parse(ConfigurationManager.AppSettings["smtp_client_port"]);
                
            using (var smtpClient = new SmtpClient(smtpServer))
            {
                var mail = new MailMessage {From = new MailAddress(emailFrom), Subject = title, IsBodyHtml = isBodyHtml};
                mail.To.Add(emailTo);
                mail.Body = body + "\n\n" + ReturnLink(link);
                smtpClient.Port = stmpClientPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(emailFrom, emailFromCred);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);
            }
        }

        private static String ReturnLink(string link)
        {
            return String.Format("<a href='{0}'>{0}</a>", link);
        }

    }
}
