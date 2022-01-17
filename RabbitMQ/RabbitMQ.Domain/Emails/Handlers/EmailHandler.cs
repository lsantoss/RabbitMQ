using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Entities;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Emails.Handlers
{
    public class EmailHandler : BaseHandler, IEmailHandler
    {
        private readonly string _applicationName = ApplicationName.EmailNotifier;
        private readonly string _currentQueue = QueueName.EmailNotifier;
        private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;

        private readonly IEmailSenderService _emailSenderService;
        private readonly IPaymentRepository _paymentRepository;

        public EmailHandler(IRabbitMQService rabbitMQBus,
                            IQueueLogRepository queueLogRepository,
                            IElmahRepository elmahRepository,
                            IPaymentRepository paymentRepository,
                            IEmailSenderService emailSenderService) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _paymentRepository = paymentRepository;
            _emailSenderService = emailSenderService;
        }

        public async Task HandleAsync(EmailCommand emailCommand)
        {
            Console.WriteLine("Handle started.");

            try
            {
                var paymentQueryResult = await _paymentRepository.GetAsync(emailCommand.PaymentId);
                var payment = paymentQueryResult.MapToPayment();

                var html = GetTemplate(emailCommand.EmailTemplate);

                html = ChangeKeysForValues(emailCommand.EmailTemplate, html, payment);

                await _emailSenderService.SendEmailAsync(html, "teste email", new List<(string, string)>() {
                    ("l_santos@hotmail.com.br", "Lucas Santos")
                });

                //Notification

                //Send Email Notification

                //await LogQueueAsync(emailCommand, _applicationName, _currentQueue);

                Console.WriteLine("Email send successfully.");
            }
            catch (Exception ex)
            {
                await ControlMaximumAttemptsAsync(emailCommand, _applicationName, _currentQueue, emailCommand.EmailTemplate, ex);
            }

            Console.WriteLine("Handle finished.\n");
        }

        private string GetTemplate(EEmailTemplate emailTemplate)
        {
            string html = emailTemplate switch
            {
                EEmailTemplate.PaymentSuccess => FileReaderHelper.Read($@"{_basePath}\Emails\Templates\payment-success.html"),
                EEmailTemplate.ReversalSuccess => FileReaderHelper.Read($@"{_basePath}\Emails\Templates\reversal-success.html"),
                EEmailTemplate.SupportPaymentMaximumAttempts => FileReaderHelper.Read($@"{_basePath}\Emails\Templates\payment-maximum-attempts.html"),
                EEmailTemplate.SupportReversalMaximumAttempts => FileReaderHelper.Read($@"{_basePath}\Emails\Templates\reversal-maximum-attempts.html"),
                EEmailTemplate.SupportPaymentNotFoundForReversal => FileReaderHelper.Read($@"{_basePath}\Emails\Templates\payment-not-found-for-reversal.html"),
                EEmailTemplate.SupportPaymentAlreadyReversed => FileReaderHelper.Read($@"{_basePath}\Emails\Templates\payment-already-reversed.html"),
                _ => null,
            };

            return html;
        }

        private string ChangeKeysForValues(EEmailTemplate emailTemplate, string html, Payment payment)
        {
            var dictionary = new Dictionary<string, string>();

            var id = payment.Id.ToString();
            var payer = payment.ClientName;
            var clientFirstName = payment.ClientName?.Split(" ").First().Trim();
            var value = payment.Value.ToString("C");
            var date = payment.Date.ToString("dd \\de MMMM \\de yyyy à\\s HH:mm");
            var barcode = @$"{payment.BarCode?[..12]}<br>
                             {payment.BarCode?.Substring(12, 12)}<br>
                             {payment.BarCode?.Substring(24, 12)}<br>
                             {payment.BarCode?[36..]}";

            if (emailTemplate == EEmailTemplate.PaymentSuccess)
            {
                dictionary.Add("{#client-first-name#}", clientFirstName);
                dictionary.Add("{#id#}", id);
                dictionary.Add("{#value#}", value);
                dictionary.Add("{#barcode#}", barcode);
                dictionary.Add("{#date#}", date);
                dictionary.Add("{#payer#}", payer);
            }

            if (dictionary.ContainsKey("{#client-first-name#}"))
                html = html.Replace("{#client-first-name#}", dictionary["{#client-first-name#}"]);

            if (dictionary.ContainsKey("{#id#}"))
                html = html.Replace("{#id#}", dictionary["{#id#}"]);

            if (dictionary.ContainsKey("{#value#}"))
                html = html.Replace("{#value#}", dictionary["{#value#}"]);

            if (dictionary.ContainsKey("{#barcode#}"))
                html = html.Replace("{#barcode#}", dictionary["{#barcode#}"]);

            if (dictionary.ContainsKey("{#date#}"))
                html = html.Replace("{#date#}", dictionary["{#date#}"]);

            if (dictionary.ContainsKey("{#payer#}"))
                html = html.Replace("{#payer#}", dictionary["{#payer#}"]);

            return html;
        }
    }
}