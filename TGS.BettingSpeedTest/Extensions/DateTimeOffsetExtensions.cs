using System;

namespace TGS.BettingSpeedTest.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToIso8601String(this DateTimeOffset dateTimeOffset)
        {
            // Returns string format like 2014-09-25T07:07:31+00:00
            return dateTimeOffset.ToString(@"yyyy-MM-dd\THH:mm:ssK");
        }
    }
}