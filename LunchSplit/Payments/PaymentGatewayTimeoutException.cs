/*
 * FILE          : PaymentGatewayTimeoutException.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

namespace LunchSplit.Payments;

/*
 * NAME    : PaymentGatewayTimeoutException
 * PURPOSE : Domain exception thrown when the payment gateway cannot be
 *           reached (network failure or timeout). It hides low-level
 *           HTTP exceptions from the rest of the application.
 */
public class PaymentGatewayTimeoutException : Exception
{
    /*
     * FUNCTION    : PaymentGatewayTimeoutException (constructor)
     * DESCRIPTION : Creates the exception and keeps the original network error.
     * PARAMETERS  : string message           : description of the failure
     *               Exception innerException : the original HTTP/network exception
     * RETURNS     : n/a
     */
    public PaymentGatewayTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}