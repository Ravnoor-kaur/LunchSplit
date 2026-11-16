/*
 * FILE          : SquarePaymentService.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

using LunchSplit.Payments;
using Square;
using Square.Payments;

namespace LunchSplit.Infrastructure;

/*
 * NAME    : SquarePaymentService
 * PURPOSE : Infrastructure wrapper that implements IPaymentProcessor using
 *           the official Square .NET SDK in Sandbox mode. It converts the
 *           LunchSplit domain objects (PaymentRequest / PaymentResult) to
 *           and from Square SDK objects, so no Square types leak into the
 *           core project. Credentials are read from environment variables
 *           and every request uses an explicit 5 second timeout.
 */
public class SquarePaymentService : IPaymentProcessor
{
    private const string SandboxBaseUrl = "https://connect.squareupsandbox.com";
    private const string AccessTokenVariable = "SQUARE_SANDBOX_ACCESS_TOKEN";
    private const string LocationIdVariable = "SQUARE_SANDBOX_LOCATION_ID";
    private const decimal CentsPerDollar = 100m;
    private const int SuccessStatusCode = 200;
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);
    private const string SquareGenericDeclineCode = "GENERIC_DECLINE";
    private const string CardDeclinedErrorCode = "CARD_DECLINED";

    private readonly SquareClient squareClient;   // Square SDK client (sandbox)
    private readonly string locationId;           // sandbox location that receives payments

    /*
     * FUNCTION    : SquarePaymentService (constructor)
     * DESCRIPTION : Reads the sandbox access token and location ID from
     *               environment variables and creates a Square client
     *               pointed at the Sandbox URL.
     * PARAMETERS  : none
     * RETURNS     : n/a
     */
    public SquarePaymentService()
    {
        string accessToken = ReadRequiredSetting(AccessTokenVariable);
        locationId = ReadRequiredSetting(LocationIdVariable);

        squareClient = new SquareClient(
            accessToken,
            clientOptions: new ClientOptions { BaseUrl = SandboxBaseUrl });
    }

    /*
     * FUNCTION    : ProcessPaymentAsync
     * DESCRIPTION : Sends one payment to Square. A successful payment returns
     *               the Square payment ID. Square API errors (declined card,
     *               bad currency, ...) are returned as a failed PaymentResult
     *               with the Square error code and HTTP status.
     * PARAMETERS  : PaymentRequest request : domain payment to send
     * RETURNS     : Task<PaymentResult> : outcome translated from Square
     */
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // translate the domain request into a Square SDK request
        CreatePaymentRequest squareRequest = new CreatePaymentRequest
        {
            SourceId = request.CardNonce,
            IdempotencyKey = request.IdempotencyKey,
            AmountMoney = new Money
            {
                Amount = ConvertDollarsToCents(request.Amount),
                Currency = ConvertCurrency(request.CurrencyCode)
            },
            LocationId = locationId,
            Autocomplete = true,
            Note = $"LunchSplit payment for {request.AttendeeName}"
        };

        try
        {
            var response = await squareClient.Payments.CreateAsync(squareRequest, CreateRequestOptions());

            return new PaymentResult
            {
                IsSuccess = true,
                TransactionId = response.Payment?.Id,
                HttpStatusCode = SuccessStatusCode
            };
        }
        catch (SquareApiException exception)
        {
            // Square answered with a 4xx/5xx: report it as a failed payment
            var firstError = exception.Errors?.FirstOrDefault();
            string? squareErrorCode = firstError?.Code.ToString();

            return new PaymentResult
            {
                IsSuccess = false,
                ErrorCode = ConvertErrorCode(squareErrorCode),
                ErrorMessage = $"{squareErrorCode}: {firstError?.Detail ?? exception.Message}",
                HttpStatusCode = exception.StatusCode
            };
        }
    }

    /*
     * FUNCTION    : GetTransactionStatusAsync
     * DESCRIPTION : Calls GET /v2/payments/{payment_id} and converts the
     *               Square status text into the PaymentStatus enum.
     * PARAMETERS  : string transactionId : Square payment ID
     * RETURNS     : Task<PaymentStatus> : current payment status
     */
    public async Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            throw new ArgumentException("Transaction ID is required.", nameof(transactionId));
        }

        var response = await squareClient.Payments.GetAsync(
            new GetPaymentsRequest { PaymentId = transactionId },
            CreateRequestOptions());

        return ConvertStatus(response.Payment?.Status?.ToString());
    }

    /*
     * FUNCTION    : CreateRequestOptions
     * DESCRIPTION : Builds per-request options with a 5 second timeout and no
     *               automatic retries, so test runs can never hang.
     * PARAMETERS  : none
     * RETURNS     : RequestOptions : options passed to every Square call
     */
    private static RequestOptions CreateRequestOptions()
    {
        return new RequestOptions
        {
            Timeout = RequestTimeout,
            MaxRetries = 0
        };
    }

    /*
     * FUNCTION    : ConvertDollarsToCents
     * DESCRIPTION : Square expects amounts in the smallest unit (cents).
     * PARAMETERS  : decimal amount : dollar amount, e.g. 12.50
     * RETURNS     : long : amount in cents, e.g. 1250
     */
    private static long ConvertDollarsToCents(decimal amount)
    {
        return (long)Math.Round(amount * CentsPerDollar, MidpointRounding.AwayFromZero);
    }

    /*
     * FUNCTION    : ConvertCurrency
     * DESCRIPTION : Maps an ISO currency code to the Square SDK currency value.
     * PARAMETERS  : string currencyCode : e.g. "CAD"
     * RETURNS     : Currency : matching Square currency
     */
    private static Currency ConvertCurrency(string currencyCode)
    {
        switch (currencyCode.ToUpperInvariant())
        {
            case "CAD":
                return Currency.Cad;
            case "USD":
                return Currency.Usd;
            case "EUR":
                return Currency.Eur;
            default:
                throw new ArgumentException($"Unsupported currency code: {currencyCode}", nameof(currencyCode));
        }
    }

    /*
     * FUNCTION    : ConvertStatus
     * DESCRIPTION : Maps Square's status text to the domain PaymentStatus.
     * PARAMETERS  : string? squareStatus : e.g. "COMPLETED"
     * RETURNS     : PaymentStatus : matching enum value, or Unknown
     */
    private static PaymentStatus ConvertStatus(string? squareStatus)
    {
        switch (squareStatus?.ToUpperInvariant())
        {
            case "APPROVED":
                return PaymentStatus.Approved;
            case "PENDING":
                return PaymentStatus.Pending;
            case "COMPLETED":
                return PaymentStatus.Completed;
            case "CANCELED":
                return PaymentStatus.Canceled;
            case "FAILED":
                return PaymentStatus.Failed;
            default:
                return PaymentStatus.Unknown;
        }
    }

    /*
 * FUNCTION    : ConvertErrorCode
 * DESCRIPTION : Translates Square-specific error codes into LunchSplit
 *               domain error codes. Square's sandbox reports a declined
 *               card as GENERIC_DECLINE, which LunchSplit treats as
 *               CARD_DECLINED. The original Square code is kept in
 *               PaymentResult.ErrorMessage.
 * PARAMETERS  : string? squareErrorCode : error code returned by Square
 * RETURNS     : string? : LunchSplit domain error code
 */
    private static string? ConvertErrorCode(string? squareErrorCode)
    {
        if (squareErrorCode == SquareGenericDeclineCode)
        {
            return CardDeclinedErrorCode;
        }

        return squareErrorCode;
    }

    /*
     * FUNCTION    : ReadRequiredSetting
     * DESCRIPTION : Reads an environment variable and fails clearly if missing,
     *               so credentials are never hardcoded in source files.
     * PARAMETERS  : string variableName : environment variable name
     * RETURNS     : string : the variable's value
     */
    private static string ReadRequiredSetting(string variableName)
    {
        string? value = Environment.GetEnvironmentVariable(variableName);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Environment variable {variableName} is not set. See README for Square Sandbox setup.");
        }

        return value;
    }
}