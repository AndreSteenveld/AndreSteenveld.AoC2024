namespace AndreSteenveld.AoC;

public static class IReadOnlyList_T__extensions
{
    public static int IndexOf<T>(this IReadOnlyList<T> source, T element) {
        for(var i = 0; i < source.Count; i++ )
            if (element is null && source[i] is null || element?.Equals(source[i]) is true)
                return i;

        return -1;
    }
}