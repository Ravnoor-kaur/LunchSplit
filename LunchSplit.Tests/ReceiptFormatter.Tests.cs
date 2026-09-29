using LunchSplit.Models;
using LunchSplit.Output;

namespace LunchSplit.Tests;

[TestClass]
public class ReceiptFormatterTests
{
    [TestMethod]
    public void Format_DefaultBillReceipt_ContainsLineItems()
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
            }
        ];

        List<Share> roundedShares =
        [
            new Share
            {
                Name = "Alice",
                Amount = 45m
            },
            new Share
            {
                Name = "Bob",
                Amount = 45m
            }
        ];

        ReceiptFormatter formatter = new ReceiptFormatter();

        string result =
            formatter.Format(bill, attendees, roundedShares);

        Assert.IsTrue(result.Contains("Alice"));
        Assert.IsTrue(result.Contains("45"));
        Assert.IsTrue(result.Contains("Bob"));
        Assert.IsTrue(result.Contains("45"));
    }

    [TestMethod]
    public void Format_ReceiptOutput_ContainsRequiredMetadata()
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
        }
        ];

        List<Share> roundedShares =
        [
            new Share
        {
            Name = "Alice",
            Amount = 90m
        }
        ];

        ReceiptFormatter formatter = new ReceiptFormatter();

        string result =
            formatter.Format(bill, attendees, roundedShares);

        Assert.IsTrue(result.Contains("Ravnoor Kaur"));
        Assert.IsTrue(result.Contains(DateTime.Now.ToString("yyyy-MM-dd")));
        Assert.IsTrue(result.Contains("Created:"));
    }

    [TestMethod]
    public void Format_CsvReceiptExporter_MatchesSchema()
    {
       // Not implemented yet
        Assert.Fail(
            "PENDING: CSV Receipt Exporter not implemented yet.");
    }

    [TestMethod]
    public void CalculateShares_PaymentRequest_ValidatesStripeMock()
    {
       // not implemented yet
        Assert.Fail(
            "PENDING: Stripe payment request validation not implemented yet.");
    }
}