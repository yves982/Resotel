using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ResotelApp.Utils
{
    class Mailer
    {
        private static SmtpClient _smtpClient;
        private static MailMessage _msg;
        private static string _host;
        private static string _sender;

        static Mailer()
        {
            _host = ConfigurationManager.AppSettings["SmtpHost"];
            _sender = ConfigurationManager.AppSettings["SmtpFrom"];
        }

        public static async Task Send(string subject, string message, string to, bool isHtml = false)
        {
            _msg = new MailMessage(_sender, to);
            _msg.IsBodyHtml = isHtml;
            _msg.Body = message;
            _msg.Subject = subject;
            using (_smtpClient = new SmtpClient(_host, 25))
            {
                await _smtpClient.SendMailAsync(_msg);
            }
        }
    }
}
