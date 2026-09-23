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
            throw new ArgumentOutOfRangeException(nameof(subtotal),"Subtotal cannot be negative.");
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
}
