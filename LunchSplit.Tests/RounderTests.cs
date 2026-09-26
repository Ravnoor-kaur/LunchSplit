using LunchSplit.Engines;
using LunchSplit.Models;

namespace LunchSplit.Tests;

[TestClass]
public class RounderTests
{
    [TestMethod]
    public void RoundShares_NoRounding_PreservesRawDecimals()
    {
        // Arrange
        List<Share> rawShares =
        [
            new Share { Name = "Alice", Amount = 10.1234m },
            new Share { Name = "Bob", Amount = 20.5678m }
        ];

        RoundingMode roundingMode = RoundingMode.None;

        Rounder rounder = new Rounder();

        // Act
        List<Share> result =
            rounder.RoundShares(rawShares, roundingMode);

        // Assert
        Assert.AreEqual(10.1234m, result[0].Amount);
        Assert.AreEqual(20.5678m, result[1].Amount);
    }

    [TestMethod]
    public void RoundShares_BankersRounding_RoundsToNearestEven()
    {
        List<Share> rawShares =
    [
        new Share { Name = "Alice", Amount = 10.325m },
        new Share { Name = "Bob", Amount = 10.335m }
    ];

    RoundingMode roundingMode = RoundingMode.Bankers;

    Rounder rounder = new Rounder();

    List<Share> result =
        rounder.RoundShares(rawShares, roundingMode);

    Assert.AreEqual(10.32m, result[0].Amount);
    Assert.AreEqual(10.34m, result[1].Amount);
    }

    [TestMethod]
    public void RoundShares_RoundUp_AppliesCeilingToCents()
    {
        // Arrange
        List<Share> rawShares =
        [
            new Share { Name = "Alice", Amount = 10.331m }
        ];

        RoundingMode roundingMode = RoundingMode.RoundUp;

        Rounder rounder = new Rounder();

        // Act
        List<Share> result =
            rounder.RoundShares(rawShares, roundingMode);

        // Assert
        Assert.AreEqual(10.34m, result[0].Amount);
    }

    [TestMethod]
    public void RoundShares_RoundDown_AppliesFloorToCents()
    {
        // Arrange
        List<Share> rawShares =
        [
            new Share { Name = "Alice", Amount = 10.339m }
        ];

        RoundingMode roundingMode = RoundingMode.RoundDown;

        Rounder rounder = new Rounder();

        // Act
        List<Share> result =
            rounder.RoundShares(rawShares, roundingMode);

        // Assert
        Assert.AreEqual(10.33m, result[0].Amount);
    }

    [TestMethod]
    public void RoundShares_UnevenSplit_ReconcilesRemainder()
    {
        // Arrange
        List<Share> rawShares =
        [
            new Share { Name = "Alice", Amount = 3.333333m },
        new Share { Name = "Bob", Amount = 3.333333m },
        new Share { Name = "Charlie", Amount = 3.333334m }
        ];

        RoundingMode roundingMode = RoundingMode.Bankers;

        Rounder rounder = new Rounder();

        // Act
        List<Share> result =
            rounder.RoundShares(rawShares, roundingMode);

        // Assert
        Assert.AreEqual(3.34m, result[0].Amount);
        Assert.AreEqual(3.33m, result[1].Amount);
        Assert.AreEqual(3.33m, result[2].Amount);

        Assert.AreEqual(10.00m, result.Sum(share => share.Amount));
    }

    [TestMethod]
    public void RoundShares_EmptyShareCollection_ReturnsEmpty()
    {
        // Arrange
        List<Share> rawShares = [];

        RoundingMode roundingMode = RoundingMode.Bankers;

        Rounder rounder = new Rounder();

        // Act
        List<Share> result =
            rounder.RoundShares(rawShares, roundingMode);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
}