/*
 * FILE          : PaymentRequest.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

namespace LunchSplit.Payments;

/*
 * NAME    : PaymentRequest
 * PURPOSE : Domain data object describing one payment that LunchSplit wants
 *           to make. It holds no vendor-specific types, so the core project
 *           never depends on the Square SDK.
 */
public class PaymentRequest
{
    public string AttendeeName { get; set; } = string.Empty;                // who is paying
    public decimal Amount { get; set; }                                     // amount in dollars, e.g. 12.50
    public string CurrencyCode { get; set; } = "CAD";                       // ISO currency code
    public string CardNonce { get; set; } = string.Empty;                   // card token, e.g. cnon:card-nonce-ok
    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString(); // prevents double charges
}