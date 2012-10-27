using System;

namespace ROZSED.Std
{
    public static class Time
    {
        /// <summary>
        /// Rerutn String represents TimeSpan (duration) from this DateTime to DateTime.Now in format: [D.HH:]mm:ss.ttt
        /// </summary>
        /// <returns></returns>
        public static string ToNow(this DateTime from, double divide = 0)
        {
            TimeSpan span = DateTime.Now - from;
            if (divide != 0)
                return (span.TotalSeconds / divide).ToString("0.000");
            return span.DHHMMSSmmm();
        }
        /// <summary>
        /// Format this TimeSpan to '[D_.HH:]MM:SS.mmm'
        /// </summary>
        public static string DHHMMSSmmm(this TimeSpan span)
        {
            return (span.Days == 0 ? "" : span.Days.ToString() + ".") +
                (span.Hours == 0 ? "" : span.Hours.ToString("00") + ":") +
                span.Minutes.ToString("00") + ":" +
                span.Seconds.ToString("00") + "." +
                span.Milliseconds.ToString("000");
        }
    }
}
