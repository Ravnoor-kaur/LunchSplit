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
