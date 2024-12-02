namespace AndreSteenveld.AoC;

public static partial class EnumerableExtensions {

    public static long Product(this IEnumerable<long> source) =>
        source.Aggregate((p, v) => p * v);

    public static long Product(this IEnumerable<int> source) =>
        source.Convert<int, long>().Product();

    public static long Product<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) =>
        source.Select<TSource, int>(selector).Product();
    
    public static IEnumerable<TTarget> Convert<TSource, TTarget>(this IEnumerable<TSource> source, IFormatProvider? provider = null) where TSource : IConvertible =>
        from element in source select (TTarget)element.ToType(typeof(TTarget), provider);

    public static IEnumerable<(TSource item, int index)> WithIndex<TSource>(this IEnumerable<TSource> source) =>
        source.Select(ValueTuple.Create<TSource, int>);

    public static IEnumerable<TSource> Repeat<TSource>(this IEnumerable<TSource> source){
        while(true)
            foreach(var item in source)
                yield return item;
    }

    public static IEnumerable<TSource> Lag<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, int, TSource> selector) =>
        source.Select(source.First(), selector);

    public static IEnumerable<TSource> Lag<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> selector) =>
        source.Select(source.First(), selector);

    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, TResult initial, Func<TResult, TSource, TResult> selector) =>
        source.Select((item) => initial = selector(initial, item));

    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, TResult initial, Func<TResult, TSource, int, TResult> selector) =>
        source.Select((item, index) => initial = selector(initial, item, index));

    public static IEnumerable<TSource> Next<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> selector){
        
        for(var last = selector(source.Last()); true; last = selector(last))
            yield return last;

    }


    #region Tuple splatting Select
    public static IEnumerable<TResult> Select<T1, T2, TResult>(this IEnumerable<(T1, T2)> source, Func<T1, T2, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2) );

    public static IEnumerable<TResult> Select<T1, T2, T3, TResult>(this IEnumerable<(T1, T2, T3)> source, Func<T1, T2, T3, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, TResult>(this IEnumerable<(T1, T2, T3, T4)> source, Func<T1, T2, T3, T4, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, TResult>(this IEnumerable<(T1, T2, T3, T4, T5)> source, Func<T1, T2, T3, T4, T5, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, T6, TResult>(this IEnumerable<(T1, T2, T3, T4, T5, T6)> source, Func<T1, T2, T3, T4, T5, T6, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6) );

    public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, T6, T7, TResult>(this IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> source, Func<T1, T2, T3, T4, T5, T6, T7, TResult> selector)
        => source.Select( t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7) );

    #endregion

    #region Tuple splatting First
    public static (T1, T2) First<T1, T2>(this IEnumerable<(T1, T2)> source, Func<T1, T2, bool> predicate) => 
        source.First( t => predicate(t.Item1, t.Item2) );

    public static (T1, T2, T3) First<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> source, Func<T1, T2, T3, bool> predicate) => 
        source.First( t => predicate(t.Item1, t.Item2, t.Item3) );

    public static (T1, T2, T3, T4) First<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> source, Func<T1, T2, T3, T4,bool> predicate) => 
        source.First( t => predicate(t.Item1, t.Item2, t.Item3, t.Item4) );

    public static (T1, T2, T3, T4, T5) First<T1, T2, T3, T4, T5>(this IEnumerable<(T1, T2, T3, T4, T5)> source, Func<T1, T2,  T3, T4, T5, bool> predicate) => 
        source.First( t => predicate(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5) );

    public static (T1, T2, T3, T4, T5, T6) First<T1, T2, T3, T4, T5, T6>(this IEnumerable<(T1, T2, T3, T4, T5, T6)> source, Func<T1, T2, T3, T4, T5, T6, bool> predicate) => 
        source.First( t => predicate(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6) );

    public static (T1, T2, T3, T4, T5, T6, T7) First<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> source, Func<T1, T2, T3, T4, T5, T6, T7, bool> predicate) => 
        source.First( t => predicate(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7) );
    #endregion
}