using System.Numerics;
using System.Text;
using AndreSteenveld.AoC;

//Functions.WaitForDebugger(false);

var coordinates = Console.In.ReadLines().Select(Coordinate.FromString).ToArray();
var size = coordinates.SelectMany<Coordinate, int>(xy => [xy.x, xy.y]).Max();

var memory = new MemoryArea(size);

var initial_drop = size switch {
    6 => 12,
    70 => 1024,
    _ => Int32.Parse(args[1])
};

foreach (var coordinate in coordinates.Take(initial_drop)) memory[coordinate] = true;

var route = memory
    .Walk(
        (0, 0), (size, size),
        (bool corrupted) => corrupted is false
    )
    .ToList();

Console.WriteLine($"The length of the initial walk is [ {route.Count - 1} ]");

foreach (var coordinate in coordinates.Skip(initial_drop)) {

    memory[coordinate] = true;
    
    if(route.Contains(coordinate) is false) 
        continue;

    var index = route.IndexOf(coordinate);
    var head = route[..(index - 1)];
    var tail = route[(index + 1)..];
    var loop = memory.Walk(head[^1], tail[0], (bool corrupted) => corrupted is false);
    
    if (loop.Count is 0) {
        
        // Because we only calculate partial routes a corrupted byte can land on an old leg of the route
        // creating an island. This doesn't mean there is no loop (although it should... I think) we're
        // going to calculate the route freshly and if we can't find anything just conclude it is the end.
        route = memory
            .Walk(
                (0, 0), (size, size),
                (bool corrupted) => corrupted is false
            )
            .ToList();
        
        if(route.Count is not 0) continue;
        
        Console.WriteLine($"Dropping [ {(coordinate.x, coordinate.y)} ] cut off all routes");
        return;
    }

    route = [..head.Union(loop), ..loop.Except(head).Except(tail), ..tail.Union(loop)];
}

return;

class MemoryArea : SparseMap<bool> {

    public MemoryArea(int size) {
        Width = 1 + size;
        Height = 1 + size;
        
        fields = new(Width * Height);
        default_value = false;
    }

    public bool this[(int x, int y) c] {
        get => base[c.x, c.y] ?? false;
        set => fields[c] = value;
    }
    
    public new bool this[int x, int y] {
        get => base[x, y] ?? false;
        set => fields[(x, y)] = value;
    }

    public override string ToString() {
        var output = new StringBuilder();

        for (var y = 0; y < Height; y++) {
            
            for (var x = 0; x < Width; x++)
                output.Append(this[x, y] ? '#' : '.');
            
            output.Append('\n');
            
        }

        return output.ToString();
    }
}

class SparseMap<TFieldType> : IPointIndex<TFieldType>, IField<TFieldType>, IHasBounds where TFieldType : struct {
    public int Width { get; protected init; }
    public int Height { get; protected init; }

    protected TFieldType? default_value { get; init; } = default(TFieldType);
    protected Dictionary<Coordinate, TFieldType> fields { get; init; } = new(9);
    
    (int x, int y, TFieldType f)? IPointIndex<(int x, int y, TFieldType f)>.this[int x, int y] =>
        fields.TryGetValue((x, y), out var f) ? (x, y, f) : null;

    public virtual TFieldType? this[int x, int y]  => 
        fields.TryGetValue((x, y), out var f) ? f : default_value;

    public IPointIndex<(int x, int y, TFieldType f)> Field => this;
}

static class _ {

    public static IReadOnlyList<Coordinate> Walk<TFieldType, TMap>(this TMap map, Coordinate origin, Coordinate target, Func<TFieldType, bool> occupy) 
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

                    var cost = costs[current] + 1;

                    if (costs.TryGetValue(neighbour, out var old) is false || cost < old) {
                        costs[neighbour] = cost;
                        frontier.Enqueue(neighbour, cost);
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

    public static string VisualizeOn<TMap>(this IReadOnlyList<Coordinate> points, TMap map) where TMap : IHasBounds {

        var output = new StringBuilder( map.ToString().ReplaceLineEndings("") );

        foreach (var point in points)
            output[to_index(point)] = '\u2591';

        for (var y = 1; y < map.Height; y++)
            output.Insert(y - 1 + map.Width * y, '\n');

        return output.ToString();

        int to_index(Coordinate coordinate) => (coordinate.x + (coordinate.y * map.Width));
        
    }
}
