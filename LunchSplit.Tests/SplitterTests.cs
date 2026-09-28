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

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            splitter.ComputeTip(subtotal, tipMode, tipInput));
    }

    [TestMethod]
    public void ComputeTip_NeagativeFixedTip_ThrowsException()
    {
        decimal subtotal = 120m;
        TipMode tipMode = TipMode.Fixed;
        decimal tipInput = -10m;

        Splitter splitter = new Splitter();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
         splitter.ComputeTip(subtotal, tipMode, tipInput));
    }

    [TestMethod]
    public void CalculateShares_EqualSplit_ApportionsEvenly()
    {
        Bill bill = new Bill(
            90m,
            0m,
            TipMode.None,
            0m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 1,
            Included = true
        },
        new Attendee
        {
            Name = "Bob",
            Weight = 1,
            Included = true
        },
        new Attendee
        {
            Name = "Charlie",
            Weight = 1,
            Included = true
        }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Splitter splitter = new Splitter();

        List<Share> result =
            splitter.CalculateShares(bill, attendees, roundingMode);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(30m, result[0].Amount);
        Assert.AreEqual(30m, result[1].Amount);
        Assert.AreEqual(30m, result[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_ProportionalSplit_ApportionsWeighted()
    {
        Bill bill = new Bill(
            90m,
            0m,
            TipMode.None,
            0m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 2,
            Included = true
        },
        new Attendee
        {
            Name = "Bob",
            Weight = 1,
            Included = true
        }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Splitter splitter = new Splitter();

        List<Share> result =
            splitter.CalculateShares(bill, attendees, roundingMode);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(60m, result[0].Amount);
        Assert.AreEqual(30m, result[1].Amount);
    }

    [TestMethod]
    public void CalculateShares_ExcludedAttendee_ApportionsZero()
    {
        Bill bill = new Bill(
            90m,
            0m,
            TipMode.None,
            0m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 1,
            Included = true
        },
        new Attendee
        {
            Name = "Bob",
            Weight = 1,
            Included = false
        },
        new Attendee
        {
            Name = "Charlie",
            Weight = 1,
            Included = true
        }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Splitter splitter = new Splitter();

        List<Share> result =
            splitter.CalculateShares(bill, attendees, roundingMode);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(45m, result[0].Amount);
        Assert.AreEqual(0m, result[1].Amount);
        Assert.AreEqual(45m, result[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_TaxAndPercentTip_ApportionsTotal()
    {
        Bill bill = new Bill(
            100m,
            13m,
            TipMode.Percent,
            15m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 1,
            Included = true
        },
        new Attendee
        {
            Name = "Bob",
            Weight = 1,
            Included = true
        }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Splitter splitter = new Splitter();

        List<Share> result =
            splitter.CalculateShares(bill, attendees, roundingMode);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(64m, result[0].Amount);
        Assert.AreEqual(64m, result[1].Amount);
    }

    [TestMethod]
    public void CalculateShares_TaxAndFixedTip_ApportionsTotal()
    {
        Bill bill = new Bill(
            100m,
            13m,
            TipMode.Fixed,
            20m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 1,
            Included = true
        },
        new Attendee
        {
            Name = "Bob",
            Weight = 1,
            Included = true
        }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Splitter splitter = new Splitter();

        List<Share> result =
            splitter.CalculateShares(bill, attendees, roundingMode);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(66.5m, result[0].Amount);
        Assert.AreEqual(66.5m, result[1].Amount);
    }

    [TestMethod]
    public void CalculateShares_AllAttendeesExcluded_ThrowsException()
    {
        Bill bill = new Bill(
            90m,
            0m,
            TipMode.None,
            0m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 1,
            Included = false
        },
        new Attendee
        {
            Name = "Bob",
            Weight = 1,
            Included = false
        }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Splitter splitter = new Splitter();

        Assert.ThrowsException<InvalidOperationException>(() =>
            splitter.CalculateShares(bill, attendees, roundingMode));
    }

    [TestMethod]
    public void CalculateShares_TotalWeightsZero_ThrowsException()
    {
        Bill bill = new Bill(
            90m,
            0m,
            TipMode.None,
            0m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 0,
            Included = true
        },
        new Attendee
        {
            Name = "Bob",
            Weight = 0,
            Included = true
        }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Splitter splitter = new Splitter();

        Assert.ThrowsException<InvalidOperationException>(() =>
            splitter.CalculateShares(bill, attendees, roundingMode));
    }

}