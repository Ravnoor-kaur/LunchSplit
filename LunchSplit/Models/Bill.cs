using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchSplit.Models;


public class Bill
{
    public decimal Subtotal { get; } // set only so bill cannot be modifiedd once bill created.
    public decimal Tax { get; }
    public TipMode TipMode { get; }
    public decimal TipInput { get; }

    public Bill(decimal subtotal, decimal tax, TipMode tipMode, decimal tipInput)  // Constructor
    {
        Subtotal = subtotal;
        Tax = tax;
        TipMode = tipMode;
        TipInput = tipInput;
    }

}