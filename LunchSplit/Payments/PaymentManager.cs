/*
 * FILE          : PaymentManager.cs
 * PROJECT       : CSCN72010 - Assignment 3 (LunchSplit)
 * PROGRAMMER    : Ravnoor Kaur
 * FIRST VERSION : 2026-11-16
 */

namespace LunchSplit.Payments;

/*
 * NAME    : PaymentManager
 * PURPOSE : Business logic for charging attendees of a split bill. It
 *           checks requests before sending them, converts network errors
 *           into PaymentGatewayTimeoutException, and charges a whole batch
 *           of attendees. It talks only to IPaymentProcessor, never to
 *           Square directly.
 */
public class PaymentManager
{
    private const string InvalidAmountErrorCode = "INVALID_AMOUNT";
    private const string GatewayTimeoutErrorCode = "GATEWAY_TIMEOUT";

    private readonly IPaymentProcessor paymentProcessor;   // gateway (real or mocked)

    /*
     * FUNCTION    : PaymentManager (constructor)
     * DESCRIPTION : Stores the payment processor this manager will use.
     * PARAMETERS  : IPaymentProcessor paymentProcessor : gateway implementation
     * RETURNS     : n/a
     */
    public PaymentManager(IPaymentProcessor paymentProcessor)
    {
        this.paymentProcessor = paymentProcessor
            ?? throw new ArgumentNullException(nameof(paymentProcessor));
    }

    /*
     * FUNCTION    : ProcessAttendeePaymentAsync
     * DESCRIPTION : Charges one attendee. A zero or negative amount is
     *               rejected without contacting the gateway. Network errors
     *               are rethrown as PaymentGatewayTimeoutException.
     * PARAMETERS  : PaymentRequest request : the payment to make
     * RETURNS     : Task<PaymentResult> : outcome of the payment
     */
    public async Task<PaymentResult> ProcessAttendeePaymentAsync(PaymentRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // short-circuit: never send a $0.00 (or negative) charge to the gateway
        if (request.Amount <= 0m)
        {
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorCode = InvalidAmountErrorCode,
                ErrorMessage = "Payment amount must be greater than zero."
            };
        }

        try
        {
            return await paymentProcessor.ProcessPaymentAsync(request);
        }
        catch (HttpRequestException exception)
        {
            throw new PaymentGatewayTimeoutException(
                "The payment gateway could not be reached.", exception);
        }
        catch (TaskCanceledException exception)   // HttpClient throws this on timeout
        {
            throw new PaymentGatewayTimeoutException(
                "The payment gateway timed out.", exception);
        }
    }

    /*
     * FUNCTION    : ProcessBatchAsync
     * DESCRIPTION : Charges every attendee in the list, one by one. One
     *               failure does not stop the others from being charged.
     * PARAMETERS  : List<PaymentRequest> requests : one request per attendee
     * RETURNS     : Task<BatchPaymentSummary> : all results plus success/failure counts
     */
    public async Task<BatchPaymentSummary> ProcessBatchAsync(List<PaymentRequest> requests)
    {
        if (requests == null)
        {
            throw new ArgumentNullException(nameof(requests));
        }

        BatchPaymentSummary summary = new BatchPaymentSummary();

        foreach (PaymentRequest request in requests)
        {
            PaymentResult result;

            try
            {
                result = await ProcessAttendeePaymentAsync(request);
            }
            catch (PaymentGatewayTimeoutException exception)
            {
                // record the timeout as a failure and keep going with the next attendee
                result = new PaymentResult
                {
                    IsSuccess = false,
                    ErrorCode = GatewayTimeoutErrorCode,
                    ErrorMessage = exception.Message
                };
            }

            summary.Results.Add(result);
        }

        return summary;
    }
}