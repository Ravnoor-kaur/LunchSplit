/*
 * FILE          : PaymentResult.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

namespace LunchSplit.Payments;

/*
 * NAME    : PaymentResult
 * PURPOSE : Domain data object holding the outcome of one payment attempt,
 *           translated from the gateway's response.
 */
public class PaymentResult
{
    public bool IsSuccess { get; set; }          // true when the payment went through
    public string? TransactionId { get; set; }   // gateway payment ID on success
    public string? ErrorCode { get; set; }       // e.g. CARD_DECLINED on failure
    public string? ErrorMessage { get; set; }    // readable failure details
    public int HttpStatusCode { get; set; }      // e.g. 200 or 400
}