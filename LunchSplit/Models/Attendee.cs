using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchSplit.Models;

public class Attendee
{
    public string Name { get; set; } = string.Empty;

    public int Weight { get; set; }

    public bool Included { get; set; }
}
