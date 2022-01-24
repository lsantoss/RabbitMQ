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
        private static readonly string _supportEmail = EmailContact.LSCodeSupportAddress;
        private static readonly string _supportDisplayName = EmailContact.LSCodeSupportDisplayName;
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _htmlMainPath = $@"{_basePath}\Emails\Templates\Html\Main";
        private static readonly string _htmlPartialPath = $@"{_basePath}\Emails\Templates\Html\Partial";
        private static readonly string _cssPath = $@"{_basePath}\Emails\Templates\Css";

        public static string GenerateTemplate(EEmailTemplate emailTemplate, PaymentQueryResult payment, List<QueueLogQueryResult> queueLogs)
        {
            var template = emailTemplate switch
            {
                EEmailTemplate.PaymentSuccess => FileHelper.Read($@"{_htmlMainPath}\payment-success.html"),
                EEmailTemplate.ReversalSuccess => FileHelper.Read($@"{_htmlMainPath}\reversal-success.html"),
                EEmailTemplate.SupportPaymentMaximumAttempts => FileHelper.Read($@"{_htmlMainPath}\payment-maximum-attempts.html"),
                EEmailTemplate.SupportReversalMaximumAttempts => FileHelper.Read($@"{_htmlMainPath}\reversal-maximum-attempts.html"),
                EEmailTemplate.SupportPaymentNotFoundForReversal => FileHelper.Read($@"{_htmlMainPath}\payment-not-found-for-reversal.html"),
                EEmailTemplate.SupportPaymentAlreadyReversed => FileHelper.Read($@"{_htmlMainPath}\payment-already-reversed.html"),
                _ => null,
            };

            if (template != null)
            {
                template = AssignPartialTemplate(template, emailTemplate);
                template = AssignVariableValues(template, emailTemplate, payment, queueLogs);
            }

            return template;
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

        private static string AssignPartialTemplate(string template, EEmailTemplate emailTemplate)
        {
            var css = FileHelper.Read($@"{_cssPath}\style.css");
            template = template.Replace("{#css-style#}", css);

            var partialHeader = FileHelper.Read($@"{_htmlPartialPath}\header.html");
            template = template.Replace("{#partial-header#}", partialHeader);

            switch (emailTemplate)
            {
                case EEmailTemplate.PaymentSuccess:
                case EEmailTemplate.ReversalSuccess:
                    var partialFooter = FileHelper.Read($@"{_htmlPartialPath}\footer.html");
                    return template.Replace("{#partial-footer#}", partialFooter);

                case EEmailTemplate.SupportPaymentMaximumAttempts:
                case EEmailTemplate.SupportReversalMaximumAttempts:
                case EEmailTemplate.SupportPaymentNotFoundForReversal:
                case EEmailTemplate.SupportPaymentAlreadyReversed:
                    var partialFooterSupport = FileHelper.Read($@"{_htmlPartialPath}\footer-support.html");
                    return template.Replace("{#partial-footer-support#}", partialFooterSupport);

                default:
                    return template;
            }
        }

        private static string AssignVariableValues(string template, EEmailTemplate emailTemplate, PaymentQueryResult payment, List<QueueLogQueryResult> queueLogs)
        {
            if (payment != null)
            {
                var dictionary = PrepareKeyDictionary(emailTemplate, payment);
                template = ChangeKeysForValues(template, dictionary);
            }
            else if (queueLogs != null && queueLogs.Count > 0)
            {

            }

            return template;
        }

        private static Dictionary<string, string> PrepareKeyDictionary(EEmailTemplate emailTemplate, PaymentQueryResult payment)
        {
            Dictionary<string, string> dictionary = new();

            var id = payment.Id.ToString();
            var payer = payment.ClientName;
            var clientFirstName = payment.ClientName?.Split(" ").First().Trim();
            var value = payment.Value.ToString("C");
            var date = payment.Date.ToString("dd \\de MMMM \\de yyyy à\\s HH:mm");
            var barcode = PrepareBarcode(payment.BarCode);

            if (emailTemplate == EEmailTemplate.PaymentSuccess)
            {
                dictionary.Add("{#title#}", "Payment Made");
                dictionary.Add("{#client-first-name#}", clientFirstName);
                dictionary.Add("{#id#}", id);
                dictionary.Add("{#value#}", value);
                dictionary.Add("{#barcode#}", barcode);
                dictionary.Add("{#date#}", date);
                dictionary.Add("{#payer#}", payer);
            }

            return dictionary;
        }

        private static string ChangeKeysForValues(string template, Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey("{#title#}"))
                template = template.Replace("{#title#}", dictionary["{#title#}"]);

            if (dictionary.ContainsKey("{#client-first-name#}"))
                template = template.Replace("{#client-first-name#}", dictionary["{#client-first-name#}"]);

            if (dictionary.ContainsKey("{#id#}"))
                template = template.Replace("{#id#}", dictionary["{#id#}"]);

            if (dictionary.ContainsKey("{#value#}"))
                template = template.Replace("{#value#}", dictionary["{#value#}"]);

            if (dictionary.ContainsKey("{#barcode#}"))
                template = template.Replace("{#barcode#}", dictionary["{#barcode#}"]);

            if (dictionary.ContainsKey("{#date#}"))
                template = template.Replace("{#date#}", dictionary["{#date#}"]);

            if (dictionary.ContainsKey("{#payer#}"))
                template = template.Replace("{#payer#}", dictionary["{#payer#}"]);

            return template;
        }

        private static string PrepareBarcode(string barcode)
        {
            return @$"{barcode?[..12]}<br>
                      {barcode?.Substring(12, 12)}<br>
                      {barcode?.Substring(24, 12)}<br>
                      {barcode?[36..]}";
        }
    }
}