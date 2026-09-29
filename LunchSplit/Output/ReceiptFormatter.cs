using LunchSplit.Models;

namespace LunchSplit.Output;

public class ReceiptFormatter
{
    public string Format(
        Bill bill,
        List<Attendee> attendees,
        List<Share> roundedShares)
    {
        string result = "LunchSplit Receipt\n";

        result += "-------------------\n";

        result += "Student Name: Ravnoor Kaur\n";
        result += $"Date: {DateTime.Now:yyyy-MM-dd}\n";
        result += $"Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";

        result += "\nLine Items:\n";

        foreach (Share share in roundedShares)
        {
            result += $"{share.Name}: ${share.Amount:F2}\n";
        }

        return result;
    }
}