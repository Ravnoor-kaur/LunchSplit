using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunchSplit.Models;
using LunchSplit.Engines;

namespace LunchSplit.Tests;

[TestClass]
public class SplitterTests
{
    [TestMethod]
    public void ComputeTip_NoTipMode_ReturnZero()
    {
        decimal Subtotal = 100m;
        TipMode tipMode = TipMode.None;
        decimal tipInput = 0m;

        
        Splitter splitter = new Splitter();
        decimal result = splitter.ComputeTip(Subtotal, tipMode, tipInput);

        // result tip should be zero
        Assert.AreEqual(0m, result);
    }
}