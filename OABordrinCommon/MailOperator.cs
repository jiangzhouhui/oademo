using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinCommon
{
    public class MailOperator
    {
        public static bool SendMail(string emailAddress, string subject, string body)
        {
            SmtpClient client = new SmtpClient();
            client.Host = System.Configuration.ConfigurationManager.AppSettings["MailHost"];
            client.Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MailPort"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(
                System.Configuration.ConfigurationManager.AppSettings["SendMailAddress"],
                System.Configuration.ConfigurationManager.AppSettings["SendMailPassword"]);

            MailMessage mm = new MailMessage(
                System.Configuration.ConfigurationManager.AppSettings["SendMailAddress"],
                emailAddress, subject, body);
            mm.SubjectEncoding = UTF8Encoding.UTF8;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.IsBodyHtml = true;

            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                client.Dispose();
            }
            return true;
        }

        public static bool SendMail(System.Collections.Generic.List<string> emailAddressCollection, string subject, string body)
        {
            SmtpClient client = new SmtpClient();
            client.Host = System.Configuration.ConfigurationManager.AppSettings["MailHost"];
            client.Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MailPort"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(
                System.Configuration.ConfigurationManager.AppSettings["SendMailAddress"],
                System.Configuration.ConfigurationManager.AppSettings["SendMailPassword"]);

            MailMessage mm = new MailMessage();
            mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["SendMailAddress"]);

            foreach (var item in emailAddressCollection)
            {
                mm.To.Add(item);
            }
            mm.Subject = subject;
            mm.Body = "<span style='font-family:Calibri'>" + body + "</span>";
            mm.SubjectEncoding = UTF8Encoding.UTF8;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.IsBodyHtml = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };
            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                client.Dispose();
            }
            return true;
        }
    }
}
