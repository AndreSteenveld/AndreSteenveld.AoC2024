using System.Buffers;
using System.Collections;
using System.Numerics;
using System.Text.RegularExpressions;

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
    
    public static bool WithinBoundsOf<T>(this (int x, int y) coordinate, TFieldMap<T> map) where T : struct =>
        map.WithinBounds(coordinate.x, coordinate.y);

    public static CharMap AsMap(this IEnumerable<string> lines) {
        var map = lines.AsMapOfChar();
        return new CharMap(map.Width, map.Height, new String(map.Area));
    }
    
    private static TFieldMap<char> AsMapOfChar(this IEnumerable<string> lines) => 
        lines.AsMapOf(l => l.ToCharArray());
    
    public static TFieldMap<TFieldType> AsMapOf<TFieldType>(this IEnumerable<ICollection<TFieldType>> lines)where TFieldType : struct =>
        lines.Aggregate(
            new TFieldMap<TFieldType>(0, 0, []),
            (map, line) =>
                new (
                    Width : map.Width == 0          ? line.Count
                          : map.Width == line.Count ? line.Count
                          : throw new Exception("Jagged map"),
                    Height : map.Height + 1,
                    Area : [..map.Area, ..line]
                )
        );

    public static TFieldMap<TFieldType> AsMapOf<TFieldType>(this IEnumerable<string> lines, Func<string, ICollection<TFieldType>> converter) where TFieldType : struct =>
        lines
            .Select(converter)
            .AsMapOf(); 
    
    public static TFieldMap<TFieldType> AsMapOf<TFieldType>(this IEnumerable<string> lines, Func<(int x, int y, char f), TFieldType> converter) where TFieldType : struct =>
        lines
            .Select((s, y) => s.Select( (f, x) => converter((x, y, f))).ToArray())
            .AsMapOf();
}

[Flags]
public enum Direction : byte {
    None = 0b0000_0000, Zero = None, Point = None,
    
    North = 0b0000_0001, N = North,
    South = 0b0000_0010, S = South,
        
    East  = 0b0000_0100, E = East,
    West  = 0b0000_1000, W = West,

    NorthEast = 0b0001_0000 | N | E, NE = NorthEast,
    SouthEast = 0b0010_0000 | S | E, SE = SouthEast,

    SouthWest = 0b0100_0000 | S | W, SW = SouthWest,
    NorthWest = 0b1000_0000 | N | W, NW = NorthWest,

    AllCardinalDirections   = North | East | South | West,
    AllOrdinalDirections    = NorthEast | SouthEast | SouthWest | NorthWest,

    AllDirections = AllCardinalDirections | AllOrdinalDirections, All = AllDirections,

    Cardinal = 0b0000_1111, IsCardinal = Cardinal,
    Ordinal  = 0b1111_0000, IsOrdinal = Ordinal
}

public partial record struct Coordinate(int x, int y) : IComparable<Coordinate> {

    [GeneratedRegex(@"\D+")]
    private static partial Regex CoordinateRegex();
    public static Coordinate FromString(string s) => CoordinateRegex().Split(s.Trim()) switch {
        [var x, var y] => (Int32.Parse(x), Int32.Parse(y)),
        _ => throw new Exception($"Can't split [ {s} ]")
    };
    
    public static IEnumerable<Coordinate> Space(int maxX, int maxY) {
        for(var y = 0; y < maxY; y++)
            for(var x = 0; x < maxX; x++)
                yield return (x, y);
    }
    
    public static Coordinate MinValue => (Int32.MinValue, Int32.MinValue);
    public static Coordinate MaxValue => (Int32.MaxValue, Int32.MaxValue);

    public static Coordinate operator <<(Coordinate self, Direction d) => self + (Coordinate)d;
    
    // public static Coordinate operator +(Coordinate self, Direction d) => self + (Coordinate)d;
    // public static Coordinate operator -(Coordinate self, Direction d) => self - (Coordinate)d;
    // public static Coordinate operator *(Coordinate self, Direction d) => self * (Coordinate)d;
    // public static Coordinate operator /(Coordinate self, Direction d) => self / (Coordinate)d;
    // public static Coordinate operator %(Coordinate self, Direction d) => self % (Coordinate)d;
    //
    // public static Coordinate operator +(Coordinate self, (Direction d, int n) ray) => self + ((Coordinate)ray.d * ray.n);
    // public static Coordinate operator -(Coordinate self, (Direction d, int n) ray) => self - ((Coordinate)ray.d * ray.n);
    // public static Coordinate operator *(Coordinate self, (Direction d, int n) ray) => self * ((Coordinate)ray.d * ray.n);
    // public static Coordinate operator /(Coordinate self, (Direction d, int n) ray) => self / ((Coordinate)ray.d * ray.n);
    // public static Coordinate operator %(Coordinate self, (Direction d, int n) ray) => self % ((Coordinate)ray.d * ray.n);
    
    public static Coordinate operator +(Coordinate self, int n) => new(self.x + n, self.y + n);
    public static Coordinate operator -(Coordinate self, int n) => new(self.x - n, self.y - n);
    public static Coordinate operator *(Coordinate self, int n) => new(self.x * n, self.y * n);
    public static Coordinate operator /(Coordinate self, int n) => new(self.x / n, self.y / n);
    public static Coordinate operator %(Coordinate self, int n) => new(self.x % n, self.y % n);
    
    public static Coordinate operator +(Coordinate left, Coordinate right) => new (left.x + right.x, left.y + right.y);
    public static Coordinate operator -(Coordinate left, Coordinate right) => new (left.x - right.x, left.y - right.y);
    public static Coordinate operator *(Coordinate left, Coordinate right) => new (left.x * right.x, left.y * right.y);
    public static Coordinate operator /(Coordinate left, Coordinate right) => new (left.x / right.x, left.y / right.y);
    public static Coordinate operator %(Coordinate left, Coordinate right) => new (left.x % right.x, left.y % right.y);
    
    public static implicit operator ValueTuple<int, int>(Coordinate self) => (self.x, self.y);
    public static implicit operator Coordinate((int x, int y) other) => new (other.x, other.y);
    
    public static explicit operator Coordinate(Direction direction) {

        var (x, y) = (0, 0);

        if (direction.HasFlag(Direction.North)) (x, y) = (x - 0, y - 1);
        if (direction.HasFlag(Direction.East))  (x, y) = (x + 1, y + 0);
        if (direction.HasFlag(Direction.South)) (x, y) = (x + 0, y + 1);
        if (direction.HasFlag(Direction.West))  (x, y) = (x - 1, y - 0);
       
        return (x, y);
        
    }

    public int CompareTo(Coordinate other) =>
        (this - other) switch {
            (0, 0) => 0,
            (<0,0) => -1,
            (>0, 0) => 1,
            ( _, <0) => -1,
            ( _, >0) => 1,
        };
    
    
}

public interface IPointIndex<TReturn> where TReturn : struct {
    public TReturn? this[int x, int y] { get; }
    public TReturn? this[Coordinate c] => this[c.x, c.y];
}

public interface IField<TFieldType> : IPointIndex<(int x, int y, TFieldType f)> {
    
    public IPointIndex<(int x, int y, TFieldType f)> Field { get; }
   
}

public interface IHasBounds {
    public int Width { get; }
    public int Height { get; }
    
    public bool WithinBounds(Coordinate c) => WithinBounds(c.x, c.y);
    public bool WithinBounds(int x, int y) => x >= 0 && x < Width &&
                                              y >= 0 && y < Height;
}

public interface IFieldEnumerable<TFieldType> : IPointIndex<TFieldType>, IHasBounds, IEnumerable<(int x, int y, TFieldType f)> where TFieldType : struct {
    IEnumerator<(int x, int y, TFieldType f)> IEnumerable<(int x, int y, TFieldType f)>.GetEnumerator() {
        foreach (var (x, y) in Coordinate.Space(Width, Height)) {
            switch (this[x, y]) {
                case null: continue;
                case var f: 
                    yield return (x, y, (TFieldType)f);
                    break;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<(int, int, TFieldType)>)this).GetEnumerator();
}

public interface IFields<TFieldType> : IFieldEnumerable<TFieldType> where TFieldType : struct {
    public IEnumerable<(int x, int y, TFieldType f)> Fields => this;
}

public class TFieldMap<TFieldType>(int Width, int Height, TFieldType[] Area) : TFieldMap<TFieldType>.FieldIndexer, IEnumerable<(int x, int y, TFieldType f)> where TFieldType : struct {

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

public class CharMap(int Width, int Height, string Area) : TFieldMap<char>(Width, Height, Area.ToCharArray()) {

    private string? area = Area;
    public new string Area => area ??= new(base.Area);
    
}

