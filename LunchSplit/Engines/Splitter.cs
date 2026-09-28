using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunchSplit.Models;

namespace LunchSplit.Engines;

public class Splitter
{
    public decimal ComputeTip(
    decimal subtotal, TipMode tipMode, decimal tipInput)
    {
        if (subtotal < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal cannot be negative.");
        }
        if (tipMode == TipMode.Fixed && tipInput < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tipInput), "Fixed tip cannot be negative.");
        }
        if (tipMode == TipMode.None)
        {
            return 0m;
        }
        if (tipMode == TipMode.Percent)
        {
            return subtotal * tipInput / 100m;
        }
        if (tipMode == TipMode.Fixed)
        {
            return tipInput;
        }

        return 0m;
    }

    public List<Share> CalculateShares(
    Bill bill,
    List<Attendee> attendees,
    RoundingMode roundingMode)
    {
        List<Attendee> includedAttendees =
            attendees.Where(a => a.Included).ToList();

        if (includedAttendees.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one attendee must be included.");
        }

        decimal grandTotal =
            bill.Subtotal +
            bill.Tax +
            ComputeTip(bill.Subtotal, bill.TipMode, bill.TipInput);

        int totalWeight =
    includedAttendees.Sum(a => a.Weight);
        if (totalWeight <= 0)
        {
            throw new InvalidOperationException(
                "Total attendee weight must be greater than zero.");
        }

        List<Share> shares = [];
        foreach (Attendee attendee in attendees)
        {
            decimal shareAmount = 0m;

            if (attendee.Included)
            {
                shareAmount =
                    grandTotal * attendee.Weight / totalWeight;
            }
            shares.Add(new Share
            {
                Name = attendee.Name,
                Amount = shareAmount
            });
        }

        return shares;
    }
}