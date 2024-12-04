namespace AndreSteenveld.AoC;

public static class Dictionary_K_V__extensions {
    public static IEnumerable<TResult> Select<TKey, TValue, TResult>(this IDictionary<TKey, TValue> @this, Func<TKey, TValue, TResult> mapper) =>
        @this.Select(kv => mapper(kv.Key, kv.Value));
    
}