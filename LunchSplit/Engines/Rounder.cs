using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunchSplit.Models;

namespace LunchSplit.Engines;

public class Rounder
{
    public List<Share> RoundShares(
    List<Share> rawShares,
    RoundingMode mode)
    {
        if (rawShares.Count == 0)
        {
            return rawShares;
        }

        if (mode == RoundingMode.None)
        {
            return rawShares;
        }

        if (mode == RoundingMode.Bankers)
        {
            decimal targetTotal = rawShares.Sum(share => share.Amount);

            foreach (Share share in rawShares)
            {
                share.Amount = Math.Round(
                    share.Amount,
                    2,
                    MidpointRounding.ToEven);
            }

            decimal roundedTotal = rawShares.Sum(share => share.Amount);
            decimal remainder = targetTotal - roundedTotal;

            int cents = (int)(remainder * 100);

            for (int i = 0; i < cents; i++)
            {
                rawShares[i].Amount += 0.01m;
            }

            return rawShares;
        }

        if (mode == RoundingMode.RoundUp)
        {
            foreach (Share share in rawShares)
            {
                share.Amount = Math.Ceiling(share.Amount * 100m) / 100m;
            }

            return rawShares;
        }

        if (mode == RoundingMode.RoundDown)
        {
            foreach (Share share in rawShares)
            {
                share.Amount = Math.Floor(share.Amount * 100m) / 100m;
            }

            return rawShares;
        }

        return rawShares;
    }
}

