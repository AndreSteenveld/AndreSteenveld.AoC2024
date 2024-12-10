namespace AndreSteenveld.AoC;

public static partial class ArrayExtensions {

    public static T[] Concat<T>(this T[] l, T[] r) {
        T[] result = new T[l.Length + r.Length];
        l.CopyTo(result, 0);
        r.CopyTo(result, l.Length);
        return result;
    }
    
    public static T[] Copy<T>(this T[] array) {
        var copy = new T[array.Length];
        array.CopyTo(copy, 0);
        return copy;
    }
    
    public static T[] Remove<T>(this T[] array, int i, int n = 1) {
        // Create a Span that references the array elements.
        var s = array.AsSpan();
        // Move array elements that follow the ones to remove to the front.
        // Caveat: Use `s`, not `a`, or else the result may be invalid.
        s[(i + n)..].CopyTo(s[i..^n]);
        // Cut the last n array elements off.
        return array = array[..^n];
    }
    
    public static void Deconstruct<T>(this T[] array, out T first, out T[] rest){
        first = array.Length > 0 ? array[0] : default(T)!;
        rest = array.Skip(1).ToArray();
    }

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T[] rest)
        => (first, (second, rest)) = array;

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T[] rest)
        => (first, second, (third, rest)) = array;

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T[] rest)
        => (first, second, third, (fourth, rest)) = array;

    public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T fifth, out T[] rest)
        => (first, second, third, fourth, (fifth, rest)) = array;

}