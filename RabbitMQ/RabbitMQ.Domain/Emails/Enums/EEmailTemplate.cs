namespace RabbitMQ.Domain.Emails.Enums
{
    public enum EEmailTemplate
    {
        PaymentSuccess = 1,
        ReversalSuccess = 2,
        SupportPaymentMaximumAttempts = 3,
        SupportReversalMaximumAttempts = 4,
        SupportPaymentNotFoundForReversal = 5,
        SupportPaymentAlreadyReversed = 6
    }
}