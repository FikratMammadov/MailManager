using AE.Net.Mail;
using MailManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailManager
{
    public class MyConsumer:CronJobService
    {
        public IConfiguration Configuration { get; }
        GmailOption _consumerOptions;
        ImapClient client;
        private readonly ILogger<MyConsumer> _logger;
        
        public MyConsumer(IScheduleConfig<MyConsumer> config, ILogger<MyConsumer> logger, IConfiguration configuration)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            Configuration = configuration;
            _consumerOptions = Configuration.GetSection("ConsumerOptions").Get<GmailOption>();
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Messages have been sending...");
            return base.StartAsync(cancellationToken);
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            GetMails();
            return base.DoWork(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Messages had been sent");
            return base.StopAsync(cancellationToken);
        }

        public void GetMails()
        {
            Thread.Sleep(5000);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            client = new ImapClient("imap.gmail.com", _consumerOptions.Email, _consumerOptions.Password, AuthMethods.Login, 993, true);

            client.SelectMailbox("INBOX");

            var messages = new List<MailMessage>();

            int uid = client.GetMessageCount() - 1;

            for (int i = 0; i < 20; i++)
            {
                messages.Add(client.GetMessage(uid - i));
            }

            foreach (var message in messages)
            {
                _logger.LogInformation($"Email consumed with {message.Subject} - subject, body: {message.Body}");
            }
        }

    }
}
