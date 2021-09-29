using MailManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MailManager
{
    public class MyProducer:CronJobService
    {
        public IConfiguration Configuration { get; }
        static GmailOption _gmailOption;
        public MyProducer(IScheduleConfig<MyProducer> config, IConfiguration configuration)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            Configuration = configuration;
            _gmailOption = Configuration.GetSection("GmailOptions").Get<GmailOption>();
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            Send();
            return base.DoWork(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        public void Send()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread t = new Thread(() => SendGmail("taskthread@gmail.com"));

                t.Start();
                Thread.Sleep(500);
            }
        }

        public void SendGmail(string MessageTo)
        {
            string to, from, pass;
            MailMessage message = new MailMessage();
            to = MessageTo;
            from = _gmailOption.Email;
            pass = _gmailOption.Password;
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Body = "Hi Task";
            message.Subject = "Gmail Sender";
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
