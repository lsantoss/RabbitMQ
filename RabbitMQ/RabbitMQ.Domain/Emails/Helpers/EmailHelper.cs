using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Queries.Results;
using RabbitMQ.Domain.Reversals.Queries.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

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

        public static string GenerateTemplate(EmailCommand emailCommand, PaymentQueryResult payment, ReversalQueryResult reversal, List<QueueLogQueryResult> queueLogs)
        {
            var template = emailCommand.EmailTemplate switch
            {
                EEmailTemplate.PaymentSuccess => FileHelper.Read($@"{_htmlMainPath}\payment-success.html"),
                EEmailTemplate.ReversalSuccess => FileHelper.Read($@"{_htmlMainPath}\reversal-success.html"),
                EEmailTemplate.SupportPaymentMaximumAttempts => FileHelper.Read($@"{_htmlMainPath}\support-payment-maximum-attempts.html"),
                EEmailTemplate.SupportReversalMaximumAttempts => FileHelper.Read($@"{_htmlMainPath}\support-reversal-maximum-attempts.html"),
                EEmailTemplate.SupportPaymentNotFoundForReversal => FileHelper.Read($@"{_htmlMainPath}\support-payment-not-found-for-reversal.html"),
                EEmailTemplate.SupportPaymentAlreadyReversed => FileHelper.Read($@"{_htmlMainPath}\support-payment-already-reversed.html"),
                _ => null,
            };

            if (template != null)
            {
                template = AssignStyleCss(template);
                template = AssignPartialFooter(template, emailCommand);
                template = AssignVariableValues(template, emailCommand, payment, reversal, queueLogs);
            }

            return template;
        }

        private static string AssignStyleCss(string template)
        {
            var css = FileHelper.Read($@"{_cssPath}\style.css");
            return template.Replace("{#css-style#}", $"<style>{css}</style>");
        }

        private static string AssignPartialFooter(string template, EmailCommand emailCommand)
        {
            switch (emailCommand.EmailTemplate)
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

        private static string AssignVariableValues(string template, EmailCommand emailCommand, PaymentQueryResult payment, ReversalQueryResult reversal, List<QueueLogQueryResult> queueLogs)
        {
            var paymentId = emailCommand.PaymentId.ToString();
            var clientName = string.Empty;
            var clientFirstName = string.Empty;
            var value = string.Empty;
            var barcode = string.Empty;
            var paymentDate = string.Empty;
            var reversalId = string.Empty;
            var reversalDate = string.Empty; 
            var queueLogInformation = string.Empty;

            if (payment != null)
            {
                clientName = payment.ClientName;
                clientFirstName = GetClientFirstName(payment.ClientName);
                value = GetFormattedMonetaryValue(payment.Value);
                barcode = GetFormattedBarcode(payment.BarCode);
                paymentDate = GetFormattedDate(payment.Date);
            }

            if (reversal != null)
            {
                reversalId = reversal.Id.ToString();
                reversalDate = GetFormattedDate(reversal.Date);
            }

            if (queueLogs != null && queueLogs.Count > 0)
                queueLogInformation = GetFormattedQueueLogInformation(queueLogs);

            switch (emailCommand.EmailTemplate)
            {
                case EEmailTemplate.PaymentSuccess:
                    return template.Replace("{#title#}", "Payment Made")
                                   .Replace("{#paymentId#}", paymentId)
                                   .Replace("{#client-first-name#}", clientFirstName)
                                   .Replace("{#value#}", value)
                                   .Replace("{#barcode#}", barcode)
                                   .Replace("{#payment-date#}", paymentDate)
                                   .Replace("{#client-name#}", clientName);

                case EEmailTemplate.ReversalSuccess:
                    return template.Replace("{#title#}", "Reversal Made")
                                   .Replace("{#paymentId#}", paymentId)
                                   .Replace("{#client-first-name#}", clientFirstName)
                                   .Replace("{#value#}", value)
                                   .Replace("{#reversal-date#}", reversalDate);

                case EEmailTemplate.SupportPaymentMaximumAttempts:
                    return template.Replace("{#title#}", "Payment Reached Maximum Attempts")
                                   .Replace("{#paymentId#}", paymentId)
                                   .Replace("{#queue-log-information#}", queueLogInformation);

                case EEmailTemplate.SupportReversalMaximumAttempts:
                    return template.Replace("{#title#}", "Reversal Reached Maximum Attempts")
                                   .Replace("{#paymentId#}", paymentId)
                                   .Replace("{#queue-log-information#}", queueLogInformation);

                case EEmailTemplate.SupportPaymentNotFoundForReversal:
                    return template.Replace("{#title#}", "Payment Not Found For Reversal")
                                   .Replace("{#paymentId#}", paymentId);

                case EEmailTemplate.SupportPaymentAlreadyReversed:
                    return template.Replace("{#title#}", "Payment Already Reversed")
                                   .Replace("{#paymentId#}", paymentId)
                                   .Replace("{#reversalId#}", reversalId)
                                   .Replace("{#queue-log-information#}", queueLogInformation);

                default:
                    return template;
            }
        }

        private static string GetFormattedQueueLogInformation(List<QueueLogQueryResult> queueLogs)
        {
            var stringBuilder = new StringBuilder();
            var indice = 0;

            foreach (var item in queueLogs)
            {
                indice++;

                stringBuilder.AppendLine($"Id: {item.Id} <br />");
                stringBuilder.AppendLine($"Worker: {item.Worker} <br />");
                stringBuilder.AppendLine($"Queue: {item.Queue} <br />");
                stringBuilder.AppendLine($"Date: {GetFormattedDate(item.Date)} <br />");
                stringBuilder.AppendLine($"Success: {GetFormattedSuccess(item.Success)} <br />");
                stringBuilder.AppendLine($"NumberAttempts: {item.NumberAttempts} <br />");

                if (!item.Success) 
                    stringBuilder.AppendLine($"Error: {item.Error} <br />");

                if (indice < queueLogs.Count) 
                    stringBuilder.AppendLine($"<hr class='queue-log' />");
            }

            return stringBuilder.ToString();
        }

        private static string GetFormattedSuccess(bool value) => value ? "Yes" : "No";

        private static string GetFormattedMonetaryValue(decimal value) => value.ToString("C");

        private static string GetClientFirstName(string clientName) => clientName?.Split(" ").First().Trim();

        private static string GetFormattedDate(DateTime? date) => date?.ToString("dd \\de MMMM \\de yyyy à\\s HH:mm");

        private static string GetFormattedBarcode(string barcode) => @$"{barcode?[..12]}<br>
                                                                        {barcode?.Substring(12, 12)}<br>
                                                                        {barcode?.Substring(24, 12)}<br>
                                                                        {barcode?[36..]}";
    }
}