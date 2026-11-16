/*
 * FILE          : SquareSandboxTests.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

using LunchSplit.Infrastructure;
using LunchSplit.Payments;

namespace LunchSplit.Tests.Integration;

/*
 * NAME    : SquareSandboxTests
 * PURPOSE : Tier 1 live integration tests (M3-01, M3-02, M3-08, M3-09,
 *           M3-10). These call the real Square Sandbox through
 *           SquarePaymentService using Square's magic test nonces.
 *           Requires SQUARE_SANDBOX_ACCESS_TOKEN and
 *           SQUARE_SANDBOX_LOCATION_ID environment variables.
 */
[TestClass]
[TestCategory("Integration")]
public class SquareSandboxTests
{
    private const string CardNonceOk = "cnon:card-nonce-ok";
    private const string CardNonceDeclined = "cnon:card-nonce-declined";
    private const int BadRequestStatusCode = 400;
    private const decimal DefaultAmount = 12.50m;
   

    /*
     * FUNCTION    : CreateRequest
     * DESCRIPTION : Builds a sandbox payment request with a fresh idempotency key.
     * PARAMETERS  : string cardNonce    : Square magic test nonce
     *               string currencyCode : currency to charge in (default CAD)
     * RETURNS     : PaymentRequest : ready-to-send request
     */
    private static PaymentRequest CreateRequest(string cardNonce, string currencyCode = "CAD")
    {
        return new PaymentRequest
        {
            AttendeeName = "Sandbox Tester",
            Amount = DefaultAmount,
            CurrencyCode = currencyCode,
            CardNonce = cardNonce,
            IdempotencyKey = Guid.NewGuid().ToString()
        };
    }

    /*
     * FUNCTION    : M3_01_SandboxAuthorization_OkNonce_ReturnsSuccessAndTransactionId
     * DESCRIPTION : Sends a live payment with the cnon:card-nonce-ok magic
     *               nonce. Asserts IsSuccess is true and a transaction ID
     *               is returned.
     * PARAMETERS  : none
     * RETURNS     : Task : completes when the test finishes
     */
    [TestMethod]
    public async Task M3_01_SandboxAuthorization_OkNonce_ReturnsSuccessAndTransactionId()
    {
        // Arrange
        SquarePaymentService paymentService = new SquarePaymentService();
        PaymentRequest request = CreateRequest(CardNonceOk);

        // Act
        PaymentResult result = await paymentService.ProcessPaymentAsync(request);

        // Assert (message shows the Square error if the call fails)
        Assert.IsTrue(result.IsSuccess, $"Square error: {result.ErrorCode} - {result.ErrorMessage}");
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TransactionId));
    }

    /*
 * FUNCTION    : M3_02_SandboxDecline_DeclinedNonce_ReturnsCardDeclined
 * DESCRIPTION : Sends a live payment with the cnon:card-nonce-declined
 *               magic nonce. Asserts IsSuccess is false and the error
 *               code is CARD_DECLINED.
 * PARAMETERS  : none
 * RETURNS     : Task : completes when the test finishes
 */
    [TestMethod]
    public async Task M3_02_SandboxDecline_DeclinedNonce_ReturnsCardDeclined()
    {
        // Arrange
        SquarePaymentService paymentService = new SquarePaymentService();
        PaymentRequest request = CreateRequest(CardNonceDeclined);

        // Act
        PaymentResult result = await paymentService.ProcessPaymentAsync(request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("CARD_DECLINED", result.ErrorCode, $"Actual error: {result.ErrorCode} - {result.ErrorMessage}");
    }
}