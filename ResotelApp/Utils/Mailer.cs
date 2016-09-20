using System;
using System.Configuration;
using System.Globalization;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ResotelApp.Utils
{
    /// <summary>
    /// Handles Mailing for alert (less than 5 rooms available on any single day)
    /// </summary>
    class Mailer
    {
        private static SmtpClient _smtpClient;
        private static MailMessage _msg;
        private static string _host;
        private static string _sender;
        private static string _to;
        private static string _subject;
        private static int _port;
        private static bool _activateMail;

        static Mailer()
        {
            _host = ConfigurationManager.AppSettings["SmtpHost"];
            _sender = ConfigurationManager.AppSettings["MailFrom"];
            _to = ConfigurationManager.AppSettings["MailTo"];
            _subject = ConfigurationManager.AppSettings["MailSubject"].Replace("#date#", $"{DateTime.Now.Date:dd MM yyyy}");
            _port = int.Parse(ConfigurationManager.AppSettings["MailPort"], CultureInfo.CreateSpecificCulture("en-US"));
            _activateMail = bool.Parse(ConfigurationManager.AppSettings["ActivateMail"]);
        }

        public static async Task SendAsync(string message, bool isHtml = false)
        {
            if(!_activateMail)
            {
                return;
            }
            _msg = new MailMessage(_sender, _to);
            _msg.IsBodyHtml = isHtml;
            _msg.Body = message;
            _msg.Subject = _subject;
            using (_smtpClient = new SmtpClient(_host, _port))
            {
                _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                await _smtpClient.SendMailAsync(_msg);
            }
        }
    }
}
