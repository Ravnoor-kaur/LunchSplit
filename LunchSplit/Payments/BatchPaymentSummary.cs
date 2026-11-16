/*
 * FILE          : BatchPaymentSummary.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

namespace LunchSplit.Payments;

/*
 * NAME    : BatchPaymentSummary
 * PURPOSE : Collects the results of charging every attendee on a split
 *           bill, and reports how many payments succeeded and failed.
 */
public class BatchPaymentSummary
{
    public List<PaymentResult> Results { get; } = new List<PaymentResult>();   // one result per attendee

    public int SuccessCount => Results.Count(result => result.IsSuccess);      // payments that went through
    public int FailureCount => Results.Count(result => !result.IsSuccess);     // payments that failed
}