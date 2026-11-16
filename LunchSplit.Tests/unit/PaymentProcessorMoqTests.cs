/*
 * FILE          : PaymentProcessorMoqTests.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

using LunchSplit.Payments;
using Moq;

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
}