using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace ROZSED.Std
{
    public static class Num
    {
        // DOUBLE EXTENSIONS ===================================================
        /// <summary>
        /// Angle in radians
        /// </summary>
        public static readonly double
            a180 = Math.PI,
            a90 = a180 / 2,
            a45 = a90 / 2,
            a135 = a180 - a45,
            a270 = a180 + a90,
            a360 = 2 * Math.PI;
        /// <summary>
        /// 180 / Math.PI
        /// </summary>
        public static readonly double rho = 180 / Math.PI;
        /// <summary>
        /// Math.Abs(this value)
        /// </summary>
        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }
        /// <summary>
        /// return value.Abs() &lt; 1e-15;
        /// </summary>
        public static bool IsZero(this double value)
        {
            return value.Abs() < 1e-15;
        }
        /// <summary>
        /// Math.Abs(this value)
        /// </summary>
        public static int Abs(this int value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }
        /// <summary>
        /// return values.Max(); // using System.Linq;
        /// </summary>
        public static double Max(params double[] values)
        {
            return values.Max();
        }
        /// <summary>
        /// return values.Average(); // using System.Linq;
        /// </summary>
        public static double Average(params double[] values)
        {
            return values.Average();
        }
        /// <summary>
        /// Math.Sqrt(this value)
        /// </summary>
        public static double Sqrt(this double value)
        {
            return Math.Sqrt(value);
        }
        /// <summary>
        /// return value % 2 == 0;
        /// </summary>
        public static bool Even(this int value)
        {
            return value % 2 == 0;
        }
        /// <summary>
        /// return value % 2 != 0;
        /// </summary>
        public static bool Odd(this int value)
        {
            return value % 2 != 0;
        }
        /// <summary>
        /// Převede úhel ve stupních na úhel v obloukové míře.
        /// </summary>
        /// <returns></returns>
        public static double Deg2Rad(this double angle)
        {
            return angle / rho;
        }
        /// <summary>
        /// Převede úhel v obloukové míře na úhel ve stupních.
        /// </summary>
        /// <returns></returns>
        public static double Rad2Deg(this double angle)
        {
            return angle * rho;
        }
        /// <summary>
        /// Average angles of nonoriented lines
        /// </summary>
        public static double Average(double angle1, double angle2)
        {
            if (angle1 > angle2)
            {
                angle1 = angle1.I(angle2, a90);
                return angle2 = (angle1 - angle2) / 2 + angle2;
            }
            else
            {
                angle2 = angle2.I(angle1, a90);
                return angle2 = (angle2 - angle1) / 2 + angle1;
            }
        }

        // Intervals ===========================================================
        /// <summary>
        /// return from &lt;= area AND area &lt;= to;
        /// </summary>
        public static bool IsBetween(this double num, double from, double to)
        {
            return from <= num && num <= to;
        }
        /// <summary>
        /// If from > to, expect close colection.
        /// </summary>
        public static bool IsBetween(this int num, int from, int to, bool fromStrict, bool toStrict)
        {
            if (fromStrict && toStrict)
                if (from > to)
                    return from < num || num < to;
                else
                    return from < num && num < to;
            else if (fromStrict)
                if (from > to)
                    return from < num || num <= to;
                else
                    return from < num && num <= to;
            else
                if (from > to)
                    return from <= num || num < to;
                else
                    return from <= num && num < to;
        }
        /// <summary>
        /// Correct 'this' to interval &lt; 0 ; 'period' ) by adding/subtracting 'period'.
        /// </summary>
        public static double I(this double value, double period)
        {
            if (value >= 0)
                return value % period;
            else
                return (value %= period) == 0 ? value : value + period;
        }
        /// <summary>
        /// Correct 'this' to interval &lt; 'from' ; 'from' + 'period' ) by adding/subtracting 'period'.
        /// </summary>
        public static double I(this double value, double from, double period)
        {
            return (value - from).I(period) + from;
        }
        /// <summary>
        /// Correct 'this' to interval ( 'from' ; 'from' + 'period' &gt; by adding/subtracting 'period'.
        /// </summary>
        public static double I1(this double value, double from, double period)
        {
            value -= from;
            if (value > 0)
                return value % period + from;
            else
                return value % period + from + period;
        }
        /// <summary>
        /// Correct 'this' to interval &lt; 0 ; 'period' ) by adding/subtracting 'period'.
        /// </summary>
        public static int I(this int value, int period)
        {
            if (value >= 0)
                return value % period;
            else
                return (value %= period) == 0 ? value : value + period;
        }
        /// <summary>
        /// Correct 'this' to interval &lt; 'from' ; 'from' + 'period' ) by adding/subtracting 'period'.
        /// </summary>
        public static int I(this int value, int from, int period)
        {
            return (value - from).I(period) + from;
        }
        /// <summary>
        /// Correct 'this' to interval ( 'from' ; 'from' + 'period' &gt; by adding/subtracting 'period'.
        /// </summary>
        public static int I1(this int value, int from, int period)
        {
            value -= from;
            if (value > 0)
                return value % period + from;
            else
                return value % period + from + period;
        }
        /// <summary>
        /// Correct 'this' to interval &lt; 'to' - 'period' ; 'to' ) by adding/subtracting 'period'.
        /// </summary>
        public static int I2(this int value, int to, int period)
        {
            to = to - period;
            return (value - to).I(period) + to;
        }
    }
}