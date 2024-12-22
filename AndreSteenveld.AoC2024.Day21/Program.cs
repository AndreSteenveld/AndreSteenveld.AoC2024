using System.Collections;
using System.Numerics;
using System.Text;
using AndreSteenveld.AoC;

//Functions.WaitForDebugger(false);

var codes = Console.In.ReadLines().ToArray();

//var NumPad = new Pad(3, 4, "789456123 0A");
var NumPad = new Pad(3, 4, " 0A123456789") { InvertedY = true };
var ArrowPad = new Pad(3, 2, " ^A<v>");


Console.WriteLine();

foreach (var code in codes) {

    Console.WriteLine($"{code}: {Input(code)}");
    Console.WriteLine($"{code}: <A^A^^>AvvvA\n");
    
    Console.WriteLine($"{code}: {Input(code, 1)}");
    Console.WriteLine($"{code}: v<<A>>^A<A>AvA<^AA>A<vAAA>^A\n");
    
    Console.WriteLine($"{code}: {Input(code, 2)}");
    Console.WriteLine($"{code}: <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A\n");
    
    Console.WriteLine($"{code}: {Input(code, 3)}");
    Console.WriteLine($"{code}: <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>Av<<A>>^A<A>AvA<^AA>A<vAAA>^A<A^A>^^AvvvA\n");
    
}

return;

string Input(string code, int intermediate_arrow_pads = 0, Pad? pad = null) {
    pad ??= NumPad;
    
    var start = ('A', Array.Empty<string>());
    var movements = code.Aggregate(start, Using(pad), JustThePath);
    var sequence = String.Join('A', movements) + "A";

    return intermediate_arrow_pads is 0 ? sequence : Input(sequence, intermediate_arrow_pads - 1, ArrowPad);
    
    string[] JustThePath(char _, string[] path) => path;

    Func<char, string[], char, (char, string[] path)> Using(Pad pad) => (current, path, next) => (next, [..path, pad.MovesToButton(current, next)]);

}

file interface IArrayMap<TFieldType> : IPointIndex<TFieldType>, IHasBounds, IEnumerable<(Coordinate coordinate, TFieldType field)> where TFieldType : struct {
    
    TFieldType[] Map { get; }
    
    int ToIndex(int x, int y) => x + (y * Width);
    int ToIndex(Coordinate coordinate) => ToIndex(coordinate.x, coordinate.y);
    Coordinate ToCoordinate(int i) => (i % Width, i / Width);
    Coordinate ToCoordinate(Index i) => ToCoordinate(i.GetOffset(Map.Length));
    
    IEnumerator<(Coordinate coordinate, TFieldType field)> IEnumerable<(Coordinate coordinate, TFieldType field)>.GetEnumerator() {
        return Map
            .Select((field, index) => (coordinate : ToCoordinate(index), field : field))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

file static class ArrayMapMethods {
    public static int ToIndex<TFieldType>(this IArrayMap<TFieldType> @this, int x, int y) where TFieldType : struct => @this.ToIndex(x, y);
    public static int ToIndex<TFieldType>(this IArrayMap<TFieldType> @this, Coordinate coordinate) where TFieldType : struct => @this.ToIndex(coordinate);
    public static Coordinate ToCoordinate<TFieldType>(this IArrayMap<TFieldType> @this, int i) where TFieldType : struct => @this.ToCoordinate(i);
    public static Coordinate ToCoordinate<TFieldType>(this IArrayMap<TFieldType> @this, Index i) where TFieldType : struct => @this.ToCoordinate(i);
}

file abstract class ArrayMap<TFieldType> : IArrayMap<TFieldType> where TFieldType : struct {
    public TFieldType? this[int x, int y] => this.WithinBounds(x, y) ? Map[this.ToIndex(x, y)] : null;

    protected ArrayMap(int w, int h, TFieldType[] m) {
        Width = w;
        Height = h;
        Map = m;
    }
    
    protected int Width { get; private init; }
    protected int Height { get; private init; }
    protected TFieldType[] Map { get; private init; }

    int IHasBounds.Width => Width;

    int IHasBounds.Height => Height;

    TFieldType[] IArrayMap<TFieldType>.Map => Map;

    public override string ToString() {
        var output = new StringBuilder();

        for (var y = 0; y < Height; y++) {
            
            for (var x = 0; x < Width; x++)
                output.Append(this[x, y]?.ToString());
            
            output.Append('\n');
        }

        return output.ToString();
    }
}

file class Pad(int w, int h, string m) : ArrayMap<char>(w, h, m.ToCharArray()) {
    public bool InvertedX { get; init; } = false;
    public bool InvertedY { get; init; } = false;
    
    private Dictionary<(char from, char to), string> paths = new(w * h);

    private Coordinate ToCoordinate(char button) => this.ToCoordinate(Array.IndexOf(Map, button));
    
    public string MovesToButton(char from, char to) {
        if (paths.ContainsKey((from, to))) 
            return paths[(from, to)];

        var space = ToCoordinate(' ');
        
        var origin = ToCoordinate(from);
        var target = ToCoordinate(to);

        var points = new List<Coordinate>();

        switch (origin, target) {
            case var (o, t) when o == t:
                break;
            
            case var (o, t) when o + (Coordinate)Direction.West != space && o - t == (Coordinate)(Direction.South | Direction.West):
                points.AddRange([o, o + (Coordinate)Direction.West, t]); 
                break;
            
            case var (o, t) when o + (Coordinate)Direction.West != space && o - t == (Coordinate)(Direction.North | Direction.West): 
                points.AddRange([o, o + (Coordinate)Direction.West, t]); 
                break;

            case var (o, t) when o + (Coordinate)Direction.South != space && o - t == (Coordinate)(Direction.South | Direction.East):
                points.AddRange([o, o + (Coordinate)Direction.South, t]); 
                break;

            case var (o, t) when o + (Coordinate)Direction.North != space && o - t == (Coordinate)(Direction.North | Direction.East):
                points.AddRange([o, o + (Coordinate)Direction.North, t]);
                break;

            case var (o, t) when o - t == (Coordinate)Direction.North
                              || o - t == (Coordinate)Direction.East
                              || o - t == (Coordinate)Direction.South
                              || o - t == (Coordinate)Direction.West:
                points.AddRange([o, t]);
                break;

            case var (o, t) when o.y == space.y && o.y < t.y: 
                for(var yy = o.y; yy <= t.y; yy++) points.Add((o.x, yy));
                goto default;
                
            case var (o, t) when o.x == space.x && o.y > t.y: 
                for(var xx = o.x; xx <= t.x; xx++) points.Add((xx, o.y));
                goto default;
            
            // case var (o, t) when o.x < t.y && o.y < t.y: goto default;
            // case var (o, t) when o.x < t.y && o.y > t.y: goto default; 
            // case var (o, t) when o.x > t.y && o.y > t.y: goto default; 
            // case var (o, t) when o.x < t.y && o.y < t.y: goto default; 
                
            default: 
                {
                    Coordinate tail;
                    
                    if(points.Count is 0) points.Add(origin);
                    
                    tail = points[^1];
                         if(tail.x != target.x && tail.x > target.x) for(var xx = tail.x - 1; xx >= target.x; xx--) points.Add((xx, tail.y));
                    else if(tail.x != target.x && tail.x < target.x) for(var xx = tail.x + 1; xx <= target.x; xx++) points.Add((xx, tail.y));
                    
                    tail = points[^1];
                         if(tail.y != target.y && tail.y > target.y) for(var yy = tail.y - 1; yy >= target.y; yy--) points.Add((tail.x, yy));
                    else if(tail.y != target.y && tail.y < target.y) for(var yy = tail.y + 1; yy <= target.y; yy++) points.Add((tail.x, yy));
                };
                break;
            
        }

        paths[(from, to)] = points.Count is 0 ? "" : points[1..].Aggregate(
            (points[0], ""),
            (previous, path, next) => 
            (
                next, 
                path + ( 
                      next - previous == (Coordinate)Direction.N ? InvertedY ? "v" : "^"
                    : next - previous == (Coordinate)Direction.E ? InvertedX ? "<" : ">"
                    : next - previous == (Coordinate)Direction.S ? InvertedY ? "^" : "v"
                    : next - previous == (Coordinate)Direction.W ? InvertedX ? ">" : "<" : String.Empty
                )
            ),
            (_, path) => path
        );

        if(points.Contains(space))
            Console.WriteLine($"{(from, to)} passes over the forbidden field");
        
        return paths[(from, to)];

    }
    
}

file static class _ {
    
    public static IReadOnlyList<Coordinate> Walk<TFieldType, TMap>(
        this TMap map, Coordinate origin, Coordinate target, 
        Func<TFieldType, bool> occupy,
        Func<Coordinate, Coordinate, int> cost
    ) 
        where TFieldType : struct 
        where TMap : IPointIndex<TFieldType>, IHasBounds
    {

        var estimated_size = Math.Abs(origin.x - target.x) + 1 * Math.Abs(origin.y - target.y) + 1;
        
        var frontier = new PriorityQueue<Coordinate, int>(1 << (BitOperations.Log2((uint)estimated_size) >> 1));
        var origins = new Dictionary<Coordinate, Coordinate>(estimated_size) { [origin] = origin };
        var costs = new Dictionary<Coordinate, int>(estimated_size) { [origin] = 0 };

        frontier.Enqueue(origin, 0);

        while (frontier.Count > 0) {
            var current = frontier.Dequeue();

            if (current == target)
                break;

            for 
            (
                var direction = (byte)Direction.West;
                direction is not 0b0000_0000; // Direction.None;
                direction = (byte)(direction >> 1)
            ) 
            {
                var neighbour = current + (Coordinate)(Direction)direction;
                
                if(map.WithinBounds(neighbour) is false) continue; 
                
                var field = map[neighbour];

                if (field is not null && occupy(field.Value)) {

                    var new_cost = costs[current] + 1 + cost(current, neighbour);

                    if (costs.TryGetValue(neighbour, out var old) is false || new_cost < old) {
                        costs[neighbour] = new_cost;
                        frontier.Enqueue(neighbour, new_cost);
                        origins[neighbour] = current;
                    }
                }
            }
        }

        if (origins.ContainsKey(target) is false)
            return Array.Empty<Coordinate>(); // No path found

        var path = new List<Coordinate>();
        
        for(var step = target; step != origin; step = origins[step])
            path.Add(step);
        
        path.Add(origin);
        path.Reverse();
        
        return path;
    }
    
}

