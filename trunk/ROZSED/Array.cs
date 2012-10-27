using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace ROZSED.Std
{
    public static class Array
    {
        // Write to this array =========================================================
        /// <summary>
        /// <para>Performs actions for each cell: thisArr[i, j] = func(thisArr[i, j]); Exclude number of border pixels given by 'limit' parameter.</para>
        /// <para>Example (each cell multiply by two):</para>
        /// <para>thisArr.ForEach( x => 2 * x );</para>
        /// </summary>
        public static TResult[,] ForEach<TResult>(this TResult[,] thisArr, Func<TResult, TResult> func, int limit)
        {
            int H = thisArr.GetLength(0),
                W = thisArr.GetLength(1),
                HLim = H - limit,
                WLim = W - limit;

            for (int i = limit; i < HLim; i++)
                for (int j = limit; j < WLim; j++)
                    thisArr[i, j] = func(thisArr[i, j]);

            return thisArr;
        }
        /// <summary>
        /// <para>Performs actions for each cell: thisArr[i, j] = func(thisArr[i, j]);</para>
        /// <para>Example (each cell multiply by two):</para>
        /// <para>thisArr.ForEach( x => 2 * x );</para>
        /// </summary>
        public static TResult[,] ForEach<TResult>(this TResult[,] thisArr, Func<TResult, TResult> func)
        {
            int I = thisArr.GetLength(0),
                J = thisArr.GetLength(1);

            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    thisArr[i, j] = func(thisArr[i, j]);

            return thisArr;
        }
        /// <summary>
        /// <para>Performs actions for each cell: thisArr[i, j] = func(a[i, j]);</para>
        /// <para>Example:</para>
        /// <para>thisArr.ForEach(a, aVal => 2 * aVal );</para>
        /// </summary>
        public static TResult[,] ForEach<T, TResult>(this TResult[,] thisArr, T[,] a, Func<T, TResult> func)
        {
            int I = thisArr.GetLength(0),
                J = thisArr.GetLength(1);
            if (I != a.GetLength(0) || J != a.GetLength(1))
                throw new ArgumentException("Dimensions mismatch.");

            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    thisArr[i, j] = func(a[i, j]);

            return thisArr;
        }
        /// <summary>
        /// <para>Performs actions for each cell: thisArr[i, j] = func(a[i, j], b[i, j]);</para>
        /// <para>Example:</para>
        /// <para>thisArr.ForEach(a, b, (aVal, bVal) => aVal * bVal );</para>
        /// </summary>
        public static TResult[,] ForEach<T1, T2, TResult>(this TResult[,] thisArr, T1[,] a, T2[,] b, Func<T1, T2, TResult> func)
        {
            int I = thisArr.GetLength(0),
                J = thisArr.GetLength(1);
            if (I != a.GetLength(0) || J != a.GetLength(1)
             || I != b.GetLength(0) || J != b.GetLength(1))
                throw new ArgumentException("Dimensions mismatch.");

            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    thisArr[i, j] = func(a[i, j], b[i, j]);

            return thisArr;
        }
        /// <summary>
        /// <para>Performs actions for each cell: thisArr[i, j] = func(a[i, j], b[i, j], c[i, j]);</para>
        /// <para>Example:</para>
        /// <para>thisArr.ForEach(a, b, c, (aVal, bVal, cVal) => aVal * bVal * cVal );</para>
        /// </summary>
        public static TResult[,] ForEach<T1, T2, T3, TResult>(this TResult[,] thisArr, T1[,] a, T2[,] b, T3[,] c, Func<T1, T2, T3, TResult> func)
        {
            int I = thisArr.GetLength(0),
                J = thisArr.GetLength(1);
            if (I != a.GetLength(0) || J != a.GetLength(1)
             || I != b.GetLength(0) || J != b.GetLength(1)
             || I != c.GetLength(0) || J != c.GetLength(1))
                throw new ArgumentException("Dimensions mismatch.");

            var aVal = thisArr[0, 0];
            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    thisArr[i, j] = func(a[i, j], b[i, j], c[i, j]);

            return thisArr;
        }

        // Create new array ============================================================
        /// <summary>
        /// <para>Performs actions for each cell: outArr[i, j] = func(thisArr[i, j]);</para>
        /// <para>Example (each cell multiply by two):</para>
        /// <para>outArr = thisArr.ForEach( x => 2 * x );</para>
        /// </summary>
        public static TResult[,] ForEach<T1, TResult>(this T1[,] thisArr, Func<T1, TResult> func)
        {
            int I = thisArr.GetLength(0),
                J = thisArr.GetLength(1);

            var outArr = new TResult[I, J];

            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    outArr[i, j] = func(thisArr[i, j]);

            return outArr;
        }
        /// <summary>
        /// <para>Performs actions for each cell: outArr[i, j] = func(a[i, j], b[i, j]);</para>
        /// <para>Example:</para>
        /// <para>outArr = Array.ForEach(a, b, (aVal, bVal) => aVal * bVal );</para>
        /// </summary>
        public static TResult[,] ForEach<T1, T2, TResult>(this T1[,] a, T2[,] b, Func<T1, T2, TResult> func)
        {
            int I = a.GetLength(0),
                J = a.GetLength(1);
            if (I != b.GetLength(0) || J != b.GetLength(1))
                throw new ArgumentException("Dimensions mismatch.");

            var outArr = new TResult[I, J];

            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    outArr[i, j] = func(a[i, j], b[i, j]);

            return outArr;
        }
        /// <summary>
        /// <para>Performs actions for each cell: outArr[i, j] = func(a[i, j], b[i, j], c[i, j]);</para>
        /// <para>Example:</para>
        /// <para>outArr = Array.ForEach(a, b, c, (aVal, bVal, cVal) => aVal * bVal * cVal );</para>
        /// </summary>
        public static TResult[,] ForEach<T1, T2, T3, TResult>(this T1[,] a, T2[,] b, T3[,] c, Func<T1, T2, T3, TResult> func)
        {
            int I = a.GetLength(0),
                J = a.GetLength(1);
            if (I != b.GetLength(0) || J != b.GetLength(1)
             || I != c.GetLength(0) || J != c.GetLength(1))
                throw new ArgumentException("Dimensions mismatch.");

            var outArr = new TResult[I, J];

            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    outArr[i, j] = func(a[i, j], b[i, j], c[i, j]);

            return outArr;
        }

        // =============================================================================
        /// <summary>
        /// Use foreach statement.
        /// </summary>
        public static T[] ToOneDim<T>(this T[,] twoDim)
        {
            var oneDim = new T[twoDim.GetLength(0) * twoDim.GetLength(1)];

            int dstIndex = 0;
            foreach (var value in twoDim)
                oneDim[dstIndex++] = value;

            return oneDim;
        }
        /// <summary>
        /// Use for statement. Exclude number of border pixels given by 'limit' parametr.
        /// </summary>
        public static T[] ToOneDim<T>(this T[,] twoDim, int limit)
        {
            int ILim = twoDim.GetLength(0) - limit,
                JLim = twoDim.GetLength(1) - limit;
            var oneDim = new T[(ILim - limit) * (JLim - limit)];

            int dstIndex = 0;
            for (int i = limit; i < ILim; i++)
                for (int j = limit; j < JLim; j++)
                    oneDim[dstIndex++] = twoDim[i, j];

            return oneDim;
        }
        /// <summary>
        /// Use foreach statement and apply 'func' on each cell.
        /// </summary>
        public static TResult[] ToOneDim<T, TResult>(this T[,] twoDim, Func<T, TResult> func)
        {
            var oneDim = new TResult[twoDim.GetLength(0) * twoDim.GetLength(1)];

            int dstIndex = 0;
            foreach (var value in twoDim)
                oneDim[dstIndex++] = func(value);

            return oneDim;
        }

        // One dimension array =========================================================
        /// <summary>
        /// Try get item, if fail correct index to interval from 0 to 'count'-1.
        /// </summary>
        public static T I<T>(this T[] arr, int index)
        {
            if (0 <= index && index < arr.Length)
                return arr[index]; // Do not use ElementAt() extension. It slow down 1000x.
            else
                return arr[index.I(arr.Length)];
        }
        /// <summary>
        /// <para>Try get item, if fail correct index to interval from 0 to 'count'-1.</para>
        /// <para>This is 3times slower than thisArr[i] also in case if no correction is need.</para>
        /// </summary>
        public static T I<T>(this List<T> list, int index)
        {
            if (0 <= index && index < list.Count)
                return list[index]; // Do not use ElementAt() extension. It slow down 10x.
            else
                return list[index.I(list.Count)];
        }
        /// <summary>
        /// Try get item, if fail correct index to interval from 0 to 'count'-1.
        /// </summary>
        public static T I<T>(this T[] arr, int index, int length)
        {
            if (0 <= index && index < length)
                return arr[index]; // Do not use ElementAt() extension. It slow down 1000x.
            else
                return arr[index.I(length)];
        }
        /// <summary>
        /// <para>Try get item, if fail correct index to interval from 0 to 'count'-1.</para>
        /// <para>This is 3times slower than thisArr[i] also in case if there isn't need for correction.</para>
        /// </summary>
        public static T I<T>(this List<T> list, int index, int count)
        {
            if (0 <= index && index < count)
                return list[index]; // Do not use ElementAt() extension. It slow down 10x.
            else
                return list[index.I(count)];
        }
        /// <summary>
        /// Performs actions for each cell: outArr[i] = func(thisArr[i]);
        /// </summary>
        public static TResult[] ToArray<T1, TResult>(this T1[] thisArr, Func<T1, TResult> func)
        {
            var I = thisArr.Length;
            var outArr = new TResult[I];

            int i = 0;
            foreach (T1 item in thisArr)
            {
                outArr[i] = func(item);
                i++;
            }

            return outArr;
        }
        /// <summary>
        /// Performs actions for each cell: outArr[i] = func(thisArr[i]);
        /// </summary>
        public static TResult[] ToArray<T1, TResult>(this IEnumerable<T1> thisArr, Func<T1, TResult> func)
        {
            var I = thisArr.Count();
            var outArr = new TResult[I];

            int i = 0;
            foreach (T1 item in thisArr)
            {
                outArr[i] = func(item);
                i++;
            }

            return outArr;
        }
        /// <summary>
        /// Performs actions for each cell: outArr[i] = func(thisArr[i]);
        /// </summary>
        public static List<TResult> ToList<T1, TResult>(this T1[] thisArr, Func<T1, TResult> func)
        {
            var I = thisArr.Length;
            var outArr = new List<TResult>(I);

            foreach (T1 item in thisArr)
                outArr.Add(func(item));

            return outArr;
        }
        /// <summary>
        /// Performs actions for each cell: outArr[i] = func(thisArr[i]);
        /// </summary>
        public static List<TResult> ToList<T1, TResult>(this IEnumerable<T1> thisArr, Func<T1, TResult> func)
        {
            var I = thisArr.Count();
            var outArr = new List<TResult>(I);

            foreach (T1 item in thisArr)
                outArr.Add(func(item));

            return outArr;
        }
        /// <summary>
        /// Performs actions for add(thisArr[i]) cell: outArr[i] = func(thisArr[i]);
        /// </summary>
        public static List<TResult> ToList<T1, TResult>(this IEnumerable<T1> thisArr, Func<T1, TResult> func, Func<T1, bool> add)
        {
            var I = thisArr.Count();
            var outArr = new List<TResult>(I);

            foreach (T1 item in thisArr)
                if (add(item))
                    outArr.Add(func(item));

            return outArr;
        }
        /// <summary>
        /// Creates new List form this ArrayList.
        /// </summary>
        /// <param name="thisArr"></param>
        /// <returns></returns>
        public static List<object> ToList(this ArrayList thisArr)
        {
            var list = new List<object>();
            foreach (var item in thisArr)
                list.Add(item);
            return list;
        }
        /// <summary>
        /// foreach (T1 item in addArr) thisArr.Add(func(item));
        /// </summary>
        public static List<TResult> AddRange<T1, TResult>(this List<TResult> thisArr, IEnumerable<T1> addArr, Func<T1, TResult> func)
        {
            foreach (T1 item in addArr)
                thisArr.Add(func(item));

            return thisArr;
        }

        // Sub sequence ================================================================
        /// <summary>
        /// Get subsequence of 'this' if 'from > to' expect that after last continue first element of 'this'.
        /// </summary>
        public static List<T> Sub<T>(this List<T> arr, int from, int to)
        {
            if (from <= to)
                return arr.GetRange(from, to - from + 1);
            else
            {
                var newArr = arr.GetRange(from, arr.Count - from);
                newArr.AddRange(arr.GetRange(0, to + 1));
                return newArr;
            }
        }
        /// <summary>
        /// <para>Reorder 'this': fromTo will be start element.</para>
        /// <para>Example: {0,1,2,3,4}.Sub(2) => {2,3,4,0,1}</para>
        /// </summary>
        public static List<T> Sub<T>(this List<T> arr, int fromTo)
        {
            var newArr = arr.GetRange(fromTo, arr.Count - fromTo);
            newArr.AddRange(arr.GetRange(0, fromTo));
            return newArr;
        }

        // Improving Linq extensions ===================================================
        /// <summary>
        /// Performs actions for each cell: thisArr[i] = action(thisArr[i]);
        /// </summary>
        public static void ForEachFunc<T>(this List<T> thisArr, Func<T, T> action)
        {
            for (int i = 0; i < thisArr.Count; i++) // Do not use IEnumerable<T>. It slow down 3x.
                thisArr[i] = action(thisArr[i]);
            // note: List has own ForEach()
        }
        /// <summary>
        /// Performs actions for each cell: thisArr[i] = action(thisArr[i]);
        /// </summary>
        public static void ForEachFunc<T>(this T[] thisArr, Func<T, T> action)
        {
            for (int i = 0; i < thisArr.Length; i++) // Do not use IEnumerable<T>. It slow down 3x.
                thisArr[i] = action(thisArr[i]);
            // note: List has own ForEach()
        }
        /// <summary>
        /// Performs actions for each cell: action(thisArr[i]);
        /// </summary>
        public static void ForEach<T>(this T[] thisArr, Action<T> action)
        {
            foreach (T item in thisArr)
                action(item);
            // note: List has own ForEach()
        }
        /// <summary>
        /// Performs actions for each cell: action(thisArr[i], i);
        /// </summary>
        public static void ForEach<T>(this T[] thisArr, Action<T, int> action)
        {
            for (int i = 0; i < thisArr.Length; i++) // Do not use IEnumerable<T>. It slow down 3x.
                action(thisArr[i], i);
            // note: List has own ForEach()
        }
        /// <summary>
        /// Returns the number of elements in the Array. It is 2x slower than this.Lenght.
        /// </summary>
        public static int Count<T>(this T[] thisArr)
        {
            return thisArr.Length; // This constuction slow down 2x, but is universal.
        }
        /// <summary>
        /// Returns the number of elements in the List. It is 2x slower than this.Count.
        /// </summary>
        public static int Count<T>(this List<T> thisArr)
        {
            return thisArr.Count; // This constuction slow down 2x, but is universal.
        }
        /// <summary>
        /// Returns the element at a specified index in the Array. It is 2x slower than this[i].
        /// </summary>
        public static T ElementAt<T>(this T[] thisArr, int i)
        {
            return thisArr[i]; // This constuction slow down 2x, but is universal.
        }
        /// <summary>
        /// Returns the element at a specified index in the List. It is 2x slower than this[i].
        /// </summary>
        public static T ElementAt<T>(this List<T> thisArr, int i)
        {
            return thisArr[i]; // This constuction slow down 2x, but is universal.
        }

        // Forward improving ===========================================================
        /// <summary>
        /// this.Reverse();   return this;
        /// </summary>
        public static List<T> ReverseFw<T>(this List<T> thisArr)
        {
            thisArr.Reverse();
            return thisArr;
        }
        /// <summary>
        /// thisArr.Add(item);   return this;
        /// </summary>
        public static T AddFw<T, U>(this T thisArr, U item) where T : List<U>
        {
            thisArr.Add(item);
            return thisArr;
        }
        /// <summary>
        /// thisArr.Add(key, value);    return thisArr;
        /// </summary>
        public static T AddFw<T, TKey, TValue>(this T thisArr, TKey key, TValue value) where T : ICollection<KeyValuePair<TKey, TValue>>
        {
            thisArr.Add(new KeyValuePair<TKey, TValue>(key, value));
            return thisArr;
        }
        /// <summary>
        /// thisArr.Insert(0, item);   return this;
        /// </summary>
        public static T Insert<T, U>(this T thisArr, U item) where T : List<U>
        {
            thisArr.Insert(0, item);
            return thisArr;
        }

        // Cycle improving =============================================================
        /// <summary>
        /// Try remove, if fail use: thisArr.RemoveAt(index.I(thisArr.Count));   return thisArr;
        /// </summary>
        public static List<T> RemoveAtC<T>(this List<T> thisArr, int index)
        {
            try
            {
                thisArr.RemoveAt(index);
            }
            catch
            {
                thisArr.RemoveAt(index.I(thisArr.Count));
            }
            return thisArr;
        }

        // Other =======================================================================
        /// <summary>
        /// <para>arr.Clear();</para>
        /// <para>arr.AddRange(newArr);</para>
        /// <para>return arr;</para>
        /// </summary>
        public static List<T> SetRange<T>(this List<T> arr, IEnumerable<T> newArr)
        {
            arr.Clear();
            arr.AddRange(newArr);
            return arr;
        }
        /// <summary>
        /// <para>arr.Clear();</para>
        /// <para>arr.Add(newItem);</para>
        /// <para>return arr;</para>
        /// </summary>
        public static List<T> SetItem<T>(this List<T> arr, T newItem)
        {
            arr.Clear();
            arr.Add(newItem);
            return arr;
        }
        /// <summary>
        /// Replace specified index with 'newValues'
        /// </summary>
        public static List<T> Replace<T>(this List<T> arr, int index, List<T> newValues)
        {
            arr.RemoveAt(index);
            arr.InsertRange(index, newValues);
            return arr;
        }
        /// <summary>
        /// Replace specified index with 'value'
        /// </summary>
        public static List<T> Replace<T>(this List<T> arr, int index, T value)
        {
            arr.RemoveAt(index);
            arr.Insert(index, value);
            return arr;
        }
        /// <summary>
        /// thisArr.RemoveAt(thisArr.Count - 1);   return thisArr;
        /// </summary>
        public static List<T> RemoveLast<T>(this List<T> thisArr)
        {
            thisArr.RemoveAt(thisArr.Count - 1);
            return thisArr;
        }
    }
}
