using System.Collections.Generic;

namespace ROZSED.Std
{
    /// <summary>
    /// Pair with easy to use constructor.
    /// </summary>
    /// <typeparam name="TypeA"></typeparam>
    /// <typeparam name="TypeB"></typeparam>
    public struct Pair<TypeA, TypeB>
    {
        public Pair(TypeA a, TypeB b)
        {
            A = a;
            B = b;
        }
        public TypeA A;
        public TypeB B;
    }
    /// <summary>
    /// Triplet with easy to use constructor.
    /// </summary>
    /// <typeparam name="TypeA"></typeparam>
    /// <typeparam name="TypeB"></typeparam>
    /// <typeparam name="TypeC"></typeparam>
    public struct Triplet<TypeA, TypeB, TypeC>
    {
        public Triplet(TypeA a, TypeB b, TypeC c)
        {
            A = a;
            B = b;
            C = c;
        }
        public TypeA A;
        public TypeB B;
        public TypeC C;
    }
    public static class StructExtensions
    {
        public static void Add<TypeA, TypeB>(this List<Pair<TypeA, TypeB>> list, TypeA a, TypeB b)
        {
            list.Add(new Pair<TypeA, TypeB>(a, b));
        }
    }
}
