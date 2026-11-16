/*
 * FILE          : PaymentStatus.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

namespace LunchSplit.Payments;

/*
 * NAME    : PaymentStatus
 * PURPOSE : Possible states of a payment, matching the status values
 *           returned by the gateway (APPROVED, PENDING, COMPLETED, ...).
 */
public enum PaymentStatus
{
    Unknown,
    Approved,
    Pending,
    Completed,
    Canceled,
    Failed
}