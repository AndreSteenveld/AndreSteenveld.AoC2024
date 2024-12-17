using System.Buffers;
using System.Collections;

namespace AndreSteenveld.AoC;

public static class MapAndCoordinateExtensions {
    
    // public static void Deconstruct<T>(this Tuple<int, int, T> coordinate, out int x, out int y) {
    //     x = coordinate.Item1;
    //     y = coordinate.Item2;
    // }
    //
    // For some reason I can't get these to work, not sure why...
    //
    // public static void Deconstruct<T>(this (int, int, T) coordinate, out int x, out int y) {
    //     x = coordinate.Item1;
    //     y = coordinate.Item2;
    // }
    
    public static bool WithinBoundsOf<T>(this (int x, int y) coordinate, Map<T> map) where T : struct =>
        map.WithinBounds(coordinate.x, coordinate.y);

    public static Map AsMap(this IEnumerable<string> lines) {
        var map = lines.AsMapOfChar();
        return new Map(map.Width, map.Height, new String(map.Area));
    }
    
    private static Map<char> AsMapOfChar(this IEnumerable<string> lines) => 
        lines.AsMapOf(l => l.ToCharArray());
    
    public static Map<TFieldType> AsMapOf<TFieldType>(this IEnumerable<ICollection<TFieldType>> lines)where TFieldType : struct =>
        lines.Aggregate(
            new Map<TFieldType>(0, 0, []),
            (map, line) =>
                new (
                    Width : map.Width == 0          ? line.Count
                          : map.Width == line.Count ? line.Count
                          : throw new Exception("Jagged map"),
                    Height : map.Height + 1,
                    Area : [..map.Area, ..line]
                )
        );

    public static Map<TFieldType> AsMapOf<TFieldType>(this IEnumerable<string> lines, Func<string, ICollection<TFieldType>> converter) where TFieldType : struct =>
        lines
            .Select(converter)
            .AsMapOf(); 
    
    public static Map<TFieldType> AsMapOf<TFieldType>(this IEnumerable<string> lines, Func<(int x, int y, char f), TFieldType> converter) where TFieldType : struct =>
        lines
            .Select((s, y) => s.Select( (f, x) => converter((x, y, f))).ToArray())
            .AsMapOf();
}

public class Map<TFieldType>(int Width, int Height, TFieldType[] Area) : Map<TFieldType>.FieldIndexer, IEnumerable<(int x, int y, TFieldType f)> where TFieldType : struct {

    public int Width { get; init; } = Width;
    public int Height { get; init; } = Height;
    public TFieldType[] Area { get; init; } = Area;
    
    public interface FieldIndexer {
        (int x, int y, TFieldType f)? this[int x, int y] { get; }
    }
    
    public bool WithinBounds((int x, int y) c) => WithinBounds(c.x, c.y);
    public bool WithinBounds(int x, int y) => x >= 0 && x < Width &&
                                              y >= 0 && y < Height;

    public int ToIndex(int x, int y) => (x + (y * Width));
    public int ToIndex((int x, int y) coordinate) => ToIndex(coordinate.x, coordinate.y);
    public (int x, int y) ToCoordinate(int i) => (i % Width, i / Width);
    public (int x, int y) ToCoordinate(Index i) => ToCoordinate(i.GetOffset(Area.Length));
    
    public TFieldType? this[int i] => i < 0 ? null : i > Area.Length ? null : Area[i];
    public TFieldType? this[int x, int y] => WithinBounds(x, y) ? Area[ToIndex(x, y)] : null;
    public TFieldType? this[(int x, int y) coordinate] => WithinBounds(coordinate.x, coordinate.y) ? Area[ToIndex(coordinate.x, coordinate.y)] : null;

    public FieldIndexer Field => this;
    (int x, int y, TFieldType f)? FieldIndexer.this[int x, int y] => WithinBounds(x, y) ? (x, y, Area[ToIndex(x, y)]!) : null;
    
    public IEnumerable<(int x, int y, TFieldType f)> Fields => this;
    
    IEnumerator<(int x, int y, TFieldType f)> IEnumerable<(int x, int y, TFieldType f)>.GetEnumerator() {
        for (int i = 0; i < Area.Length; i++) {
            var (x, y) = ToCoordinate(i);
            yield return (x, y, Area[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable<(int, int, int)>)this).GetEnumerator();
    }
    
    public override string ToString() {
    
        var lines = Area
            .Chunk(Width)
            .Select( l => String.Concat(l));
    
        return String.Join('\n', lines);
    }

}

public class Map(int Width, int Height, string Area) : Map<char>(Width, Height, Area.ToCharArray()) {

    private string? area = Area;
    public new string Area => area ??= new(base.Area);
    
}

