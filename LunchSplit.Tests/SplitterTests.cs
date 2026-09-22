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
        decimal subtotal = 100m;
        TipMode tipMode = TipMode.None;
        decimal tipInput = 0m;


        Splitter splitter = new Splitter();
        decimal result = splitter.ComputeTip(subtotal, tipMode, tipInput);

        // result tip should be zero
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void CompututeTip_PercentTipMode_ReturnCorrectPercent()
    {
        decimal subtotal = 120m;
        TipMode tipMode = TipMode.Percent;
        decimal tipInput = 15m;

        Splitter splitter = new Splitter();
        decimal result = splitter.ComputeTip(subtotal, tipMode, tipInput);

        // result tip should be 18
        Assert.AreEqual(18m, result);

    }

    [TestMethod]

    public void ComputeTip_FixedTipMode_ReturnFixedAmount()
    {
        decimal subtotal = 120m;
        TipMode tipMode = TipMode.Fixed;
        decimal tipInput = 20m;

        Splitter splitter = new Splitter();
        decimal result = splitter.ComputeTip(subtotal, tipMode, tipInput);

        // result tip should be 20
        Assert.AreEqual(20m, result);
    }

    [TestMethod]
    public void ComputeTip_NegativeSubtotal_ThrowsArgumentException()
    {
        // Arrange
        decimal subtotal = -100m;
        TipMode tipMode = TipMode.None;
        decimal tipInput = 0m;

        // Act & Assert
        Splitter splitter = new Splitter();

        Assert.ThrowsException<ArgumentException>(() =>
            splitter.ComputeTip(subtotal, tipMode, tipInput));
    }
}