/*
 * FILE          : IPaymentProcessor.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

namespace LunchSplit.Payments;

/*
 * NAME    : IPaymentProcessor
 * PURPOSE : Defines the contract for any payment gateway used by LunchSplit.
 *           The core domain only depends on this interface, so the real
 *           Square service can be swapped for a Moq mock during unit testing.
 */
public interface IPaymentProcessor
{
    /*
     * FUNCTION    : ProcessPaymentAsync
     * DESCRIPTION : Sends a single payment to the payment gateway.
     * PARAMETERS  : PaymentRequest request : amount, currency, nonce and idempotency key
     * RETURNS     : Task<PaymentResult> : success flag, transaction ID or error code
     */
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);

    /*
     * FUNCTION    : GetTransactionStatusAsync
     * DESCRIPTION : Looks up the current status of an existing payment.
     * PARAMETERS  : string transactionId : ID returned by a previous payment
     * RETURNS     : Task<PaymentStatus> : current status of the payment
     */
    Task<PaymentStatus> GetTransactionStatusAsync(string transactionId);
}