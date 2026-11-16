/*
 * FILE          : PaymentProcessorMoqTests.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

using LunchSplit.Payments;
using Moq;
using LunchSplit.Engines;
using LunchSplit.Models;

namespace LunchSplit.Tests.Unit;

/*
 * NAME    : PaymentProcessorMoqTests
 * PURPOSE : Tier 2 in-memory unit tests (M3-03 to M3-07). IPaymentProcessor
 *           is replaced with a Moq mock, so PaymentManager's logic is tested
 *           without any network calls to Square.
 */
[TestClass]
public class PaymentProcessorMoqTests
{
    /*
     * FUNCTION    : M3_05_ZeroAmountBill_NeverCallsPaymentProcessor
     * DESCRIPTION : A $0.00 bill must be rejected by PaymentManager before it
     *               reaches the gateway. Verifies ProcessPaymentAsync is
     *               never invoked (Times.Never).
     * PARAMETERS  : none
     * RETURNS     : Task : completes when the test finishes
     */
    [TestMethod]
    public async Task M3_05_ZeroAmountBill_NeverCallsPaymentProcessor()
    {
        // Arrange: mock gateway and a $0.00 request
        Mock<IPaymentProcessor> mockProcessor = new Mock<IPaymentProcessor>();
        PaymentManager paymentManager = new PaymentManager(mockProcessor.Object);

        PaymentRequest zeroAmountRequest = new PaymentRequest
        {
            AttendeeName = "Alex",
            Amount = 0.00m,
            CurrencyCode = "CAD",
            CardNonce = "cnon:card-nonce-ok"
        };

        // Act
        PaymentResult result = await paymentManager.ProcessAttendeePaymentAsync(zeroAmountRequest);

        // Assert: request rejected and the gateway was never called
        Assert.IsFalse(result.IsSuccess);
        mockProcessor.Verify(
            processor => processor.ProcessPaymentAsync(It.IsAny<PaymentRequest>()),
            Times.Never);
    }
    /*
     * FUNCTION    : M3_04_ProcessPayment_PassesExactDollarAmountToProcessor
     * DESCRIPTION : Verifies PaymentManager passes the exact dollar amount and
     *               currency to IPaymentProcessor, exactly once (Mock.Verify).
     * PARAMETERS  : none
     * RETURNS     : Task : completes when the test finishes
     */
    [TestMethod]
    public async Task M3_04_ProcessPayment_PassesExactDollarAmountToProcessor()
    {
        // Arrange: mock gateway that approves every payment
        decimal expectedAmount = 27.35m;
        Mock<IPaymentProcessor> mockProcessor = new Mock<IPaymentProcessor>();
        mockProcessor
            .Setup(processor => processor.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult { IsSuccess = true, TransactionId = "mock-txn-001", HttpStatusCode = 200 });

        PaymentManager paymentManager = new PaymentManager(mockProcessor.Object);
        PaymentRequest request = new PaymentRequest
        {
            AttendeeName = "Jordan",
            Amount = expectedAmount,
            CurrencyCode = "CAD",
            CardNonce = "cnon:card-nonce-ok"
        };

        // Act
        PaymentResult result = await paymentManager.ProcessAttendeePaymentAsync(request);

        // Assert: processor received exactly $27.35 CAD, once
        Assert.IsTrue(result.IsSuccess);
        mockProcessor.Verify(
            processor => processor.ProcessPaymentAsync(
                It.Is<PaymentRequest>(sent => sent.Amount == expectedAmount && sent.CurrencyCode == "CAD")),
            Times.Once);
    }

    /*
 * FUNCTION    : M3_06_FourAttendeeSplit_CallsProcessorExactlyFourTimes
 * DESCRIPTION : Splits an $80 bill between 4 attendees using the A1
 *               Splitter, then charges each share. Verifies
 *               ProcessPaymentAsync is called exactly 4 times.
 * PARAMETERS  : none
 * RETURNS     : Task : completes when the test finishes
 */
    [TestMethod]
    public async Task M3_06_FourAttendeeSplit_CallsProcessorExactlyFourTimes()
    {
        // Arrange: split an $80 bill evenly between 4 attendees (A1 engine)
        Bill bill = new Bill(80.00m, 0.00m, TipMode.None, 0m);
        List<Attendee> attendees =
        [
            new Attendee { Name = "Alex",   Weight = 1, Included = true },
            new Attendee { Name = "Bailey", Weight = 1, Included = true },
            new Attendee { Name = "Casey",  Weight = 1, Included = true },
            new Attendee { Name = "Devon",  Weight = 1, Included = true }
        ];
        List<Share> shares = new Splitter().CalculateShares(bill, attendees, RoundingMode.None);

        // turn each share into a payment request
        List<PaymentRequest> requests = shares
            .Select(share => new PaymentRequest
            {
                AttendeeName = share.Name,
                Amount = share.Amount,
                CurrencyCode = "CAD",
                CardNonce = "cnon:card-nonce-ok"
            })
            .ToList();

        Mock<IPaymentProcessor> mockProcessor = new Mock<IPaymentProcessor>();
        mockProcessor
            .Setup(processor => processor.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult { IsSuccess = true, TransactionId = "mock-txn", HttpStatusCode = 200 });
        PaymentManager paymentManager = new PaymentManager(mockProcessor.Object);

        // Act
        BatchPaymentSummary summary = await paymentManager.ProcessBatchAsync(requests);

        // Assert: one gateway call per attendee, all succeeded
        Assert.AreEqual(4, summary.SuccessCount);
        mockProcessor.Verify(
            processor => processor.ProcessPaymentAsync(It.IsAny<PaymentRequest>()),
            Times.Exactly(4));
    }
}