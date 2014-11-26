using System;
using System.Net.Mail; //for SmtpClient
using System.Configuration; //for ConfigurationManager

namespace Stackoverflow_Question_Poller
{
    class Email
    {
        public static void sendEmail(string title, string body, string link)
        {
            try
            {
                //get configured settings
                string smtpServer = ConfigurationManager.AppSettings["stmp_server"];
                string emailFrom = ConfigurationManager.AppSettings["email_from"];
                string emailFromCred = ConfigurationManager.AppSettings["email_from_cred"];
                string emailTo = ConfigurationManager.AppSettings["email_to"];
                
                SmtpClient smtpClient = new SmtpClient(smtpServer);
                var mail = new MailMessage();
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = title;
                mail.IsBodyHtml = true;
                mail.Body = body + "\n\n" + link;
                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(emailFrom, emailFromCred);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);
            }
            catch (SmtpFailedRecipientException)
            {
                Console.Write("ERROR: Invalid recipient");
            }
            catch (Exception ex)
            {
                Console.Write("ERROR: Failed to send message: {0}", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

    }
}
