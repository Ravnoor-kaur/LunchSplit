using LunchSplit.Models;

namespace LunchSplit.Helpers;

public class BillValidator
{
    public ValidationResult Validate(
        Bill bill,
        List<Attendee> attendees)
    {

        if (bill.Subtotal < 0)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Subtotal cannot be negative."
            };
        }

        if (bill.Tax < 0)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Tax cannot be negative."
            };
        }


        if (attendees == null || attendees.Count == 0)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "At least one attendee is required."
            };
        }

        if (attendees.Any(a => a.Weight < 0))
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Attendee weight cannot be negative."
            };
        }

        return new ValidationResult
        {
            IsValid = true,
            ErrorMessage = string.Empty
        };

       


    }
}