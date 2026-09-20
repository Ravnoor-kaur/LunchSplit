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
            throw new ArgumentException("Subtotal cannot be negative.");
        }
        if (tipMode == TipMode.None)
        {
            return 0m;
        }

        return 0m;
    }
}
