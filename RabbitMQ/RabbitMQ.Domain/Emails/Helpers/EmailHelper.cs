using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Queries.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace RabbitMQ.Domain.Emails.Helpers
{
    public static class EmailHelper
    {
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _supportDisplayName = EmailContact.SupportDisplayName;
        private static readonly string _supportEmail = EmailContact.SupportAddress;

        public static string GenerateTemplate(EEmailTemplate emailTemplate)
        {
            return emailTemplate switch
            {
                EEmailTemplate.PaymentSuccess => FileHelper.Read($@"{_basePath}\Emails\Templates\payment-success.html"),
                EEmailTemplate.ReversalSuccess => FileHelper.Read($@"{_basePath}\Emails\Templates\reversal-success.html"),
                EEmailTemplate.SupportPaymentMaximumAttempts => FileHelper.Read($@"{_basePath}\Emails\Templates\payment-maximum-attempts.html"),
                EEmailTemplate.SupportReversalMaximumAttempts => FileHelper.Read($@"{_basePath}\Emails\Templates\reversal-maximum-attempts.html"),
                EEmailTemplate.SupportPaymentNotFoundForReversal => FileHelper.Read($@"{_basePath}\Emails\Templates\payment-not-found-for-reversal.html"),
                EEmailTemplate.SupportPaymentAlreadyReversed => FileHelper.Read($@"{_basePath}\Emails\Templates\payment-already-reversed.html"),
                _ => null,
            };
        }

        public static string ChangeKeysForValues(EEmailTemplate emailTemplate, string templateHtml, PaymentQueryResult payment, List<QueueLogQueryResult> queueLogs)
        {
            var dictionary = new Dictionary<string, string>();

            if (payment != null)
            {
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
                    templateHtml = templateHtml.Replace("{#client-first-name#}", dictionary["{#client-first-name#}"]);

                if (dictionary.ContainsKey("{#id#}"))
                    templateHtml = templateHtml.Replace("{#id#}", dictionary["{#id#}"]);

                if (dictionary.ContainsKey("{#value#}"))
                    templateHtml = templateHtml.Replace("{#value#}", dictionary["{#value#}"]);

                if (dictionary.ContainsKey("{#barcode#}"))
                    templateHtml = templateHtml.Replace("{#barcode#}", dictionary["{#barcode#}"]);

                if (dictionary.ContainsKey("{#date#}"))
                    templateHtml = templateHtml.Replace("{#date#}", dictionary["{#date#}"]);

                if (dictionary.ContainsKey("{#payer#}"))
                    templateHtml = templateHtml.Replace("{#payer#}", dictionary["{#payer#}"]);
            }
            else if (queueLogs != null && queueLogs.Count > 0)
            {

            }

            return templateHtml;
        }

        public static string GenerateSubject(EEmailTemplate emailTemplate)
        {
            return emailTemplate switch
            {
                EEmailTemplate.PaymentSuccess => "Payment made",
                EEmailTemplate.ReversalSuccess => "Reversal made",
                EEmailTemplate.SupportPaymentMaximumAttempts => "Support - Payment reached maximum attempts",
                EEmailTemplate.SupportReversalMaximumAttempts => "Support - Reversal reached maximum attempts",
                EEmailTemplate.SupportPaymentNotFoundForReversal => "Support - Payment not found to be reversed",
                EEmailTemplate.SupportPaymentAlreadyReversed => "Support - Payment is already reversed",
                _ => null,
            };
        }

        public static (string address, string diplayName) GenerateRecipient(EEmailTemplate emailTemplate, PaymentQueryResult payment)
        {
            return emailTemplate switch
            {
                EEmailTemplate.PaymentSuccess or 
                EEmailTemplate.ReversalSuccess => (payment.ClientEmail, payment.ClientName),

                EEmailTemplate.SupportPaymentMaximumAttempts or 
                EEmailTemplate.SupportReversalMaximumAttempts or 
                EEmailTemplate.SupportPaymentNotFoundForReversal or 
                EEmailTemplate.SupportPaymentAlreadyReversed => (_supportEmail, _supportDisplayName),

                _ => (null, null),
            };
        }

        public static List<Attachment> GenerateAttachments()
        {
            return new List<Attachment>();
        }
    }
}