using LunchSplit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunchSplit.Models;
using LunchSplit.Helpers;

namespace LunchSplit.Tests;

[TestClass]
public class BillValidatorTests
{
    [TestMethod]
    public void Validate_CompleteBillDetails_ReturnsOk()
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

        BillValidator validator = new BillValidator();


        ValidationResult result =
            validator.Validate(bill, attendees);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(string.Empty, result.ErrorMessage);
    }

    [TestMethod]
    public void Validate_EmptyAttendeeCollection_ReturnsFail()
    {

        Bill bill = new Bill(
     100m,
     13m,
     TipMode.Percent,
     15m);

        List<Attendee> attendees = [];

        BillValidator validator = new BillValidator();

        ValidationResult result =
            validator.Validate(bill, attendees);

        Assert.IsFalse(result.IsValid);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [TestMethod]
    public void Validate_NegativeSubtotal_ReturnsFail()
    {
        Bill bill = new Bill(
            -100m,
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
        }
        ];

        BillValidator validator = new BillValidator();

        ValidationResult result =
            validator.Validate(bill, attendees);

        Assert.IsFalse(result.IsValid);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }
    [TestMethod]
    public void Validate_NegativeTax_ReturnsFail()
    {
        Bill bill = new Bill(
            100m,
            -13m,
            TipMode.Percent,
            15m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = 1,
            Included = true
        }
        ];

        BillValidator validator = new BillValidator();

        ValidationResult result =
            validator.Validate(bill, attendees);

        Assert.IsFalse(result.IsValid);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [TestMethod]
    public void Validate_ZeroAttendees_ReturnsFail()
    {
        Bill bill = new Bill(
            100m,
            13m,
            TipMode.Percent,
            15m);

        List<Attendee> attendees = new List<Attendee>();

        BillValidator validator = new BillValidator();

        ValidationResult result =
            validator.Validate(bill, attendees);

        Assert.IsFalse(result.IsValid);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [TestMethod]
    public void Validate_NegativeAttendeeWeight_ReturnsFail()
    {
        // Arrange
        Bill bill = new Bill(
            100m,
            10m,
            TipMode.None,
            0m);

        List<Attendee> attendees =
        [
            new Attendee
        {
            Name = "Alice",
            Weight = -1,
            Included = true
        }
        ];

        BillValidator validator = new BillValidator();

        // Act
        ValidationResult result =
            validator.Validate(bill, attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
    }
}