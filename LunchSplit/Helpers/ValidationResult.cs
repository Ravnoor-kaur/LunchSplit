using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchSplit.Helpers;

public class ValidationResult
{
    public bool IsValid { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}
