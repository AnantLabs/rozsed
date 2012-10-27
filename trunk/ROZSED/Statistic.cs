using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROZSED.Std
{
    public static class Statistic
    {
        static double diff, average;
        /// <summary>
        /// Root-mean-square deviation.
        /// </summary>
        /// <param name="diffs">Differences.</param>
        /// <param name="sysErr">Systematic error will be subtracted from each difference in 'diffs' before calculate RMS.</param>
        /// <param name="oldRMS">Only differences lower then 2 * oldRMS will be use for calculatin RMS, i.e 95% differences will be used.</param>
        public static double RMS(this List<double> diffs, double sysErr, double oldRMS)
        {
            List<double> diffSquared;
            if (sysErr != 0)
                diffSquared = diffs.ToList(x => (x - sysErr) * (x - sysErr));
            else
                diffSquared = diffs.ToList(x => x * x);

            var threshold = 4 * oldRMS * oldRMS;
            return diffSquared.Where(x => x <= threshold).Average().Sqrt();
        }
        /// <summary>
        /// Return squared RMS from squared differences 'diffs'.
        /// </summary>
        /// <param name="oldRMS">Only differences lower then 2 * oldRMS will be use for calculatin RMS, i.e 95% differences will be used.</param>
        public static double SqRMS(this List<double> sqDiffs, double oldRMS)
        {
            var threshold = 4 * oldRMS * oldRMS;
            return sqDiffs.Where(x => x <= threshold).Average();
        }
        /// <summary>
        /// Systematic error.
        /// </summary>
        /// <param name="oldRMS">Only differences lower then 2 * oldRMS will be use for calculatin RMS, i.e 95% differences will be used.</param>
        public static double SysErr(this List<double> diffs, double oldRMS)
        {
            return diffs.Where(x => x <= 2 * oldRMS).Average();
        }
        /// <summary>
        /// <para>average = this.Average(func)</para>
        /// <para>v = average - func</para>
        /// <para>sum( v^2 ) / n</para>
        /// </summary>
        public static double SqRMS<T>(this List<T> list, Func<T, double> func)
        {
            average = list.Average<T>(func);
            return list.ToList(x => (diff = average - func(x)) * diff).Average();
        }
        /// <summary>
        /// <para>average = this.Average(func)</para>
        /// <para>v = average - func</para>
        /// <para>sqRMS = sum( v^2 ) / n</para>
        /// <para>while(sqRMS > th) remove item with the bigest v^2;</para>
        /// <para>if (list.Count &lt; minCount) return false; else return true;</para>
        /// <para>change this only if return true.</para>
        /// </summary>
        public static bool RemoveRemote<T>(this List<T> list, Func<T, double> func, double th, int minCount)
        {
            average = list.Average<T>(func);
            var diffs = list.ToList(x => new Pair<T, double>(x, (diff = average - func(x)) * diff));
            while (diffs.Average(x => x.B) > th)
            {
                if (diffs.Count <= minCount)
                    return false;
                diffs.Remove(diffs.OrderBy(x => x.B).Last());
                average = diffs.Average(x => func(x.A));
                diffs.ForEach(x => x.B = (diff = average - func(x.A)) * diff);
            }
            list.SetRange(diffs.ToList(x => x.A));
            return true;
        }
        public static int[][] Combinations(int n)
        {
            var outArr = new int[n * (n - 1) / 2][];
            var komb = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    outArr[komb++] = new int[] { i, j };

            return outArr;
        }
        public static int[][] Combinations(int n, int maxComb)
        {
            var comb = n * (n - 1) / 2;
            var rate = comb / maxComb + 1;
            if (rate == 1)
                maxComb = comb;
            var outArr = new int[maxComb][];
            var shift = 1;

            int i = 0;
            while (true)
            {
                for (int j = i + shift; j < n; j += rate)
                {
                    if (maxComb-- <= 0)
                        return outArr;
                    outArr[maxComb] = new int[] { i, j };
                }
                i++;
                if (i == n)
                {
                    i = 0;
                    shift++;
                }
            }
        }
    }
}
