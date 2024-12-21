using System.Collections;
using System.Diagnostics;
using AndreSteenveld.AoC;

using static AndreSteenveld.AoC.Direction;
using static AndreSteenveld.AoC.Functions;
//using static System.Math;
using Map = AndreSteenveld.AoC.TFieldMap<char>;

//WaitForDebugger(false);

var sw = new Stopwatch() ;; sw.Start();

Map map = Console.In.ReadLines().AsMap();
var region_map = RegionMap.From(map);

Console.WriteLine("\n" + map + "\n");
Console.WriteLine(region_map + "\n");

var survey = region_map.Regions().Survey().ToList();

Console.WriteLine($"Found [ {survey.Count} ] regions\n");

Console.WriteLine(region_map + "\n");

foreach (var field in survey)
     Console.WriteLine($"{field.ring.Origin} {map[ field.ring.Origin ]} :: {field.ring.Interior().Count()} ({field.surface}) * {field.ring.PerimeterLength} ({field.perimiter})");
//
// var total_price = survey.Aggregate(0UL, (total, s) => total + (ulong)(s.surface * s.perimiter));
//
// // 1026224 too low
//
// Console.WriteLine($"After a complete survey of the farm in [ {sw.ElapsedMilliseconds}ms ] the total price is estimated at :: {total_price}");

return;

// [Flags]
// public enum Direction : byte {
//     None = 0b0000_0000, Zero = None, Point = None,
//     
//     North = 0b0000_0001, N = North,
//     South = 0b0000_0010, S = South,
//         
//     East  = 0b0000_0100, E = East,
//     West  = 0b0000_1000, W = West,
//
//     NorthEast = 0b0001_0000, NE = NorthEast,
//     SouthEast = 0b0010_0000, SE = SouthEast,
//
//     SouthWest = 0b0100_0000, SW = SouthWest,
//     NorthWest = 0b1000_0000, NW = NorthWest,
//
//     AllCardinalDirections   = North | East | South | West,
//     AllOrdinalDirections    = NorthEast | SouthEast | SouthWest | NorthWest,
//
//     AllDirections = AllCardinalDirections | AllOrdinalDirections, All = AllDirections,
//
//     Cardinal = 0b0000_1111, IsCardinal = Cardinal,
//     Ordinal  = 0b1111_0000, IsOrdinal = Ordinal
// }

public class RegionMap : TFieldMap<Direction> {
    
    private RegionMap(int Width, int Height, Direction[] Area) : base(Width, Height, Area){ }

    public static RegionMap From(TFieldMap<char> source) {

        Direction[] borders = new Direction[source.Width * source.Height];
        
        for (var i = 0; i < borders.Length; i++) {

            Coordinate coordinate = source.ToCoordinate(i);

            var f = source.Area[i];
            var b = borders[i];
            
            var field = (
                n  : source[coordinate << N],
                e  : source[coordinate << E],
                s  : source[coordinate << S],
                w  : source[coordinate << W]
            );
            
            if (field.n != f) b |= N;
            if (field.e != f) b |= E;
            if (field.s != f) b |= S;
            if (field.w != f) b |= W;
            
            borders[i] = b;
        }
        
        return new RegionMap(source.Width, source.Height, borders);
        
    }
    
    private IEnumerable<Ring> Regions(IEnumerable<Coordinate> fields, HashSet<Coordinate> origins) {

        foreach (var field in fields.Where(field => is_origin(this[field]) && origins.Add(field)))
            foreach(var r in rings(field))
                yield return r;

        bool is_origin(Direction? field) =>
            field is (Direction.North | Direction.West)
                  or (Direction.West | Direction.North | Direction.East)
                  or (Direction.South | Direction.West | Direction.North)
                  or Direction.Cardinal;
        
        Ring[] rings(Coordinate field) {
        
            var perimeter = new List<Line>();

            var origin = (field.x, field.y);
            
            var o = origin;
            var d = Direction.East;

            do {

                var l = line(o, d);
                
                perimeter.Add(l);
                
                o = l.End;
                
                if(is_origin(this[o]))  
                    origins.Add(o);
                
                d = (d, this[o]) switch {
                    
                    (Direction.East, { } c) when c.HasFlag(Direction.North) => Direction.South,
                    (Direction.East, _) => Direction.North,
                    
                    (Direction.South, { } c) when c.HasFlag(Direction.East) => Direction.West,
                    (Direction.South, _) => Direction.East,
                    
                    (Direction.West, { } c) when c.HasFlag(Direction.South) => Direction.North,
                    (Direction.West, _) => Direction.South,
                    
                    (Direction.North, { } c) when c.HasFlag(Direction.West) => Direction.East,
                    (Direction.North, _) => Direction.West,
                   
                };
                
            } while ((o, d) != (origin, Direction.East));

            Ring[] rings = [new Ring(perimeter.ToArray())];
            
            var (min, max) = rings[0].Box();

            if (max - min is (>= 3, >= 3)) {
                foreach (var child in Regions(rings[0].Interior(false), origins)) {

                    
                    
                        rings = [..rings, child];
                }
            }

            return rings;

        }

        Line line(Coordinate origin, Direction direction) {
            if (this[origin] is Direction.Cardinal)
                return new Line(origin, direction, 0);
            
            for (int l = 0 ;; l++) {
                switch (direction, this[origin + (Coordinate)direction * l]) {
                    
                    case (Direction.North, { } field) when l is not 0 && field.HasFlag(Direction.West) is false || field.HasFlag(Direction.North):
                    case (Direction.North, Direction.West | Direction.North):
                        return new Line(origin, direction, l);
                    
                    case (Direction.East, { } field) when l is not 0 && field.HasFlag(Direction.North) is false || field.HasFlag(Direction.East):
                    case (Direction.East, Direction.North | Direction.East):    
                        return new Line(origin, direction, l);
                    
                    case (Direction.South, { } field) when l is not 0 && field.HasFlag(Direction.East) is false || field.HasFlag(Direction.South):
                    case (Direction.South, Direction.East | Direction.South):    
                        return new Line(origin, direction, l);
                    
                    case (Direction.West, { } field) when l is not 0 && field.HasFlag(Direction.South) is false || field.HasFlag(Direction.West):
                    case (Direction.West, Direction.South | Direction.West):
                        return new Line(origin, direction, l);
                    
                }
            }
        }
            
    }
    
    public IEnumerable<Ring> Regions() => 
        Regions(Coordinate.Space(Width, Height), []);
    
    public override string ToString() {
    
        var characters = Area.Select(
    
                field => field switch {
                    Direction.North => '─',
                    Direction.East  => '│',
                    Direction.South => '─',
                    Direction.West  => '│',
    
                    // Opposing sides
                    Direction.North | Direction.South => '═',
                    Direction.East  | Direction.West => '║',
    
                    // Corners
                    Direction.South | Direction.East => '┘',
                    Direction.South | Direction.West => '└',
                    Direction.North | Direction.East => '┐',
                    Direction.North | Direction.West => '┌',
    
                    // One side open
                    Direction.North | Direction.East  | Direction.South => '─',
                    Direction.East  | Direction.South | Direction.West  => '│',
                    Direction.South | Direction.West  | Direction.North => '─',
                    Direction.West  | Direction.North | Direction.East  => '│',
                    
                    // Fully encircled
                    Direction.AllCardinalDirections => '▯',
                    Direction.None => ' ',
    
                    _ => '◆'
                }
    
            )
            .Chunk(Width)
            .Select( l => String.Concat(l));
    
        return String.Join('\n', characters);
        
    }

}

public record Line(Coordinate Origin, Direction Direction, int Length) {

    public void Deconstruct(out Coordinate origin, out Coordinate end) =>
        (origin, end) = (Origin, End);
    
    public static (Direction direction, int length) Cast(Coordinate origin, Coordinate end) =>
        (origin, end) switch {
            var (l, r) when l == r => (Direction.None, 0),
            var ((lx, ly), (rx, ry)) when lx == rx && ly <  ry => (Direction.North, ly - ry),
            var ((lx, ly), (rx, ry)) when lx >  rx && ly == ry => (Direction.East,  lx - ry),
            var ((lx, ly), (rx, ry)) when lx == rx && ly >  ry => (Direction.South, ry - ly),
            var ((lx, ly), (rx, ry)) when lx <  rx && ly == ry => (Direction.West,  ly - ry),
            _ => throw new Exception("Ordinal direction not there yet")
        };
        
    public Line(Coordinate origin, (Direction direction, int length) vector) : this(origin, vector.direction, vector.length){ }
    
    public Line(Coordinate origin, Coordinate end) : this(origin, Cast(origin, end)) { }
    
    public IEnumerable<Coordinate> Points() {
        yield return Origin;
        for (var i = 0; i < Length; i++)
            yield return Origin + (Coordinate)Direction * i;
    }

    public Coordinate End => Origin + (Coordinate)Direction * Length;
}

public record Ring(Line[] Lines, Direction Compass = Direction.AllCardinalDirections) {

    private static (Coordinate min, Coordinate max) box(IEnumerable<Coordinate> perimiter) => perimiter.Aggregate(
        (min : Coordinate.MaxValue, max : Coordinate.MinValue), 
        (box, point) => (
            (Math.Min(box.min.x, point.x), Math.Min(box.min.y, point.y)), 
            (Math.Max(box.max.x, point.x), Math.Max(box.max.y, point.y))
        )
    );
    
    private static bool contains(IEnumerable<Coordinate> perimiter, Coordinate point) => true 
        && perimiter.Count(p => p.x == point.x && p.y >  point.y) % 2 is 1
        && perimiter.Count(p => p.x <  point.x && p.y == point.y) % 2 is 1
        && perimiter.Count(p => p.x == point.x && p.y <  point.y) % 2 is 1
        && perimiter.Count(p => p.x >  point.x && p.y == point.y) % 2 is 1
        ;
    
    public bool Closed => Lines[0].Origin == Lines[^1].End;

    public IEnumerable<Coordinate> Points() {
        return Lines
            .SelectMany(line => line.Points())
            .Distinct();
    }

    public int Length => Lines.Sum(line => line.Length);

    public Coordinate Origin => Lines[0].Origin;

    public int PerimeterLength => Lines
        .Pair((current, next) =>
            current.Length is 0 ? 1 : (current.Direction, next?.Direction) switch {
                   (Direction.North, Direction.West) or (Direction.East, Direction.North)  
                or (Direction.South, Direction.East) or (Direction.West, Direction.South) 
                => current.Length - 1,
                
                _ => current.Length + 1
            }
        )
        .Sum();

    public IEnumerable<Coordinate> Interior(bool include_perimiter = true) {
        if (Closed is false)
            throw new Exception("Leaky ring");

        var perimeter = Points().ToArray();

        if(include_perimiter) 
            foreach (var point in perimeter.Distinct())
                yield return point;

        var ((min_x, min_y), (max_x, max_y)) = box(perimeter);
        
        if(min_x == max_x || min_y == max_y)
            yield break;
        
        for(int x = min_x; x < max_x; x++)
            for (int y = min_y; y < max_y; y++)
                if (perimeter.Contains((x, y)) is false && contains(perimeter, (x, y)))
                    yield return (x, y);
    }

    public bool Contains(Coordinate point) => Closed && contains(Points(), point);

    public (Coordinate, Coordinate) Box() => box(Points());

}

public static class _ {

    public static IEnumerable<(int surface, int perimiter, Ring ring)> Survey(this IEnumerable<Ring> @this) {

        var regions = @this.ToList();

        for (int i = 0; i < regions.Count; i++) {
            var parent = regions[i];
            
            var totals = regions[(i + 1)..]
                .Where(child => parent.Contains(child.Origin))
                .Aggregate(
                    (surface : 0, perimeter : 0),
                    (result, child) => (
                        result.surface + child.Interior().Count(),
                        result.perimeter + child.PerimeterLength
                    ),
                    total => (
                        surface : parent.Interior().Count() - total.surface,
                        perimeter : parent.PerimeterLength + total.perimeter
                    )
                    
                );

            yield return (totals.surface, totals.perimeter, parent);

        }

    }
    
}