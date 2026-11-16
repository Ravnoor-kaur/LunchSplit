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

    /*
 * FUNCTION    : M3_10_TransactionStatusQuery_ReturnsCompleted
 * DESCRIPTION : Makes a successful live payment, then queries
 *               GET /v2/payments/{payment_id}. Asserts the status
 *               returned by GetTransactionStatusAsync is COMPLETED.
 * PARAMETERS  : none
 * RETURNS     : Task : completes when the test finishes
 */
    [TestMethod]
    public async Task M3_10_TransactionStatusQuery_ReturnsCompleted()
    {
        // Arrange: create a payment to look up
        SquarePaymentService paymentService = new SquarePaymentService();
        PaymentResult payment = await paymentService.ProcessPaymentAsync(CreateRequest(CardNonceOk));
        Assert.IsTrue(payment.IsSuccess, $"Setup payment failed: {payment.ErrorCode} - {payment.ErrorMessage}");

        // Act
        PaymentStatus status = await paymentService.GetTransactionStatusAsync(payment.TransactionId!);

        // Assert
        Assert.AreEqual(PaymentStatus.Completed, status);
    }

    /*
 * FUNCTION    : M3_09_DuplicateIdempotencyKey_ReturnsSameTransactionWithoutDoubleCharge
 * DESCRIPTION : Sends the exact same request (same idempotency key)
 *               twice. Asserts Square returns the original transaction
 *               ID the second time instead of creating a second charge.
 * PARAMETERS  : none
 * RETURNS     : Task : completes when the test finishes
 */
    [TestMethod]
    public async Task M3_09_DuplicateIdempotencyKey_ReturnsSameTransactionWithoutDoubleCharge()
    {
        // Arrange: one request, reused for both calls
        SquarePaymentService paymentService = new SquarePaymentService();
        PaymentRequest request = CreateRequest(CardNonceOk);

        // Act
        PaymentResult firstResult = await paymentService.ProcessPaymentAsync(request);
        PaymentResult secondResult = await paymentService.ProcessPaymentAsync(request);

        // Assert: both succeed and point to the same single payment
        Assert.IsTrue(firstResult.IsSuccess, $"First call failed: {firstResult.ErrorCode} - {firstResult.ErrorMessage}");
        Assert.IsTrue(secondResult.IsSuccess, $"Second call failed: {secondResult.ErrorCode} - {secondResult.ErrorMessage}");
        Assert.AreEqual(firstResult.TransactionId, secondResult.TransactionId);
    }

    /*
 * FUNCTION    : M3_08_UnsupportedCurrency_EurToCadLocation_ReturnsHttp400
 * DESCRIPTION : Sends a live payment in EUR to the CAD sandbox location.
 *               Asserts IsSuccess is false and Square returns an HTTP 400
 *               validation error.
 * PARAMETERS  : none
 * RETURNS     : Task : completes when the test finishes
 */
    [TestMethod]
    public async Task M3_08_UnsupportedCurrency_EurToCadLocation_ReturnsHttp400()
    {
        // Arrange: EUR payment to a CAD location
        SquarePaymentService paymentService = new SquarePaymentService();
        PaymentRequest request = CreateRequest(CardNonceOk, "EUR");

        // Act
        PaymentResult result = await paymentService.ProcessPaymentAsync(request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(BadRequestStatusCode, result.HttpStatusCode, $"Square error: {result.ErrorCode} - {result.ErrorMessage}");
    }

}