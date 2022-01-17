using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Queries.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Domain.Emails.Helpers
{
    public static class EmailHelper
    {
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;

        public static string GenerateEmailContent(EEmailTemplate emailTemplate, PaymentQueryResult payment)
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

            if (html != null)
                html = html.ChangeKeysForValues(emailTemplate, payment);

            return html;
        }

        private static string ChangeKeysForValues(this string html, EEmailTemplate emailTemplate, PaymentQueryResult payment)
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