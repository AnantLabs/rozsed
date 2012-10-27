using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ROZSED.Std
{
    public static class NetExt
    {
        /// <summary>
        /// <para>T temp = a;</para>
        /// <para>a = b;</para>
        /// <para>b = temp;</para>
        /// </summary>
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        /// <summary>
        /// <para>Deep copy is creating a new object and then copying the nonstatic fields of the current object to the new object.</para>
        /// <para>If a field is a value type => a bit-by-bit copy of the field is performed.</para>
        /// <para>If a field is a reference type --> a new copy of the referred object is performed.</para>
        /// <para>Note: the classes to be cloned must be flagged as [Serializable].</para>
        /// </summary>
        public static T DeepCopy<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// return (int)obj;
        /// </summary>
        public static int ToInt(this object obj)
        {
            return (int)obj;
        }
        /// <summary>
        /// return double.Parse(obj.ToString());
        /// </summary>
        public static double ToDouble(this object obj)
        {
            return double.Parse(obj.ToString());
        }
    }
}
