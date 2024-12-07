
using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

using Coordinate = (int x, int y, Direction d);

WaitForDebugger();

var (start, area, max_x, max_y) = Console.In.ReadLines().Aggregate(
    ((Coordinate?)null, String.Empty, 0, 0),
    (result, line) => {

        var (start, map, max_x, max_y) = result;

        max_x = line.Length switch {
            var v when max_x is 0 => v,
            var v when max_x == v => v,
            _ => throw new Exception()
        };

        if (start is not null) 
            return (start, String.Concat(map, line), max_x, max_y + 1);
        
        var x = line.IndexOf((char)Direction.Up);

        if (x is not -1) {
            start = (x : line.IndexOf((char)Direction.Up), max_y, Direction.Up);
            line = line.Replace((char)Direction.Up, (char)Field.Empty);
        }

        return (start, String.Concat(map, line), max_x, max_y + 1);
    }
);


if (start is null)
    throw new Exception("No starting position");

var ByCoordinates = EqualityComparer<Coordinate>.Create(
    (left, right) => (left.x, left.y) == (right.x, right.y),
    coordinate => coordinate.x ^ coordinate.y
);

var map = new Map(max_x, max_y, area);
var route = Patrol(map, (Coordinate)start!, (m, c) => m.WithinBounds(c.x, c.y)).ToList();

var distinct_locations_visited = route[..^1].Distinct(ByCoordinates).Count();

Console.WriteLine($"Distinct locations visited :: {distinct_locations_visited}");

var number_of_obstacles_placeable_for_infinte_loop = route[..^1].AsParallel()
    .Where((coordinate, index) => {
        var step = Step(map, coordinate);

        return map.WithinBounds(step.x, step.y)
               && ByCoordinates.Equals(coordinate, step) is false // Filter out rotations
               && route[0..index].Contains(step, ByCoordinates) is false; // Make sure we haven't already walked there to come to this point
    })
    .Count(coordinate => {
        var index = route.IndexOf(coordinate); 
        var (x, y, _ ) = Step(map, coordinate);

        var map_with_hypothetical_obstruction = map.PlaceObstacle(x, y);
        var hypothetical_path = new List<Coordinate>();

        foreach (var step in Patrol(map_with_hypothetical_obstruction, coordinate, (m, s) => m.WithinBounds(s.x, s.y) && route[0..index].Contains(s) is false)) {
            if (hypothetical_path.Contains(step))
                break;
            hypothetical_path.Add(step);
        }

        var (last_x, last_y, _) = hypothetical_path[^1];

        return map_with_hypothetical_obstruction.WithinBounds(last_x, last_y);
    });

Console.WriteLine($"Distinct locations that can be obstructed :: {number_of_obstacles_placeable_for_infinte_loop}");

return;

IEnumerable<Coordinate> Patrol(Map map, Coordinate start, Func<Map, Coordinate, bool> can_make_next_step) {
    var (x, y, d) = start;

    yield return (x, y, d);

    while (can_make_next_step(map, (x, y, d))){
        (x, y, d) = Step(map, (x, y, d));
        yield return (x, y, d);
    }
}

Coordinate Step(Map map, Coordinate current) {
    var (x, y, d) = current;

    return d switch {
        Direction.Up when map[x, y - 1] is (char?)Field.Empty or null => (x, y - 1, d),
        Direction.Up when map[x, y - 1] is (char?)Field.Obstacle => (x, y, Direction.Right),
        Direction.Right when map[x + 1, y] is (char?)Field.Empty or null => (x + 1, y, d),
        Direction.Right when map[x + 1, y] is (char?)Field.Obstacle => (x, y, Direction.Down),
        Direction.Down when map[x, y + 1] is (char?)Field.Empty or null => (x, y + 1, d),
        Direction.Down when map[x, y + 1] is (char?)Field.Obstacle => (x, y, Direction.Left),
        Direction.Left when map[x - 1, y] is (char?)Field.Empty or null => (x - 1, y, d),
        Direction.Left when map[x - 1, y] is (char?)Field.Obstacle => (x, y, Direction.Up),
    };
}

enum Direction {
    Up = '^',
    Right = '>',
    Down = 'V',
    Left = '<'
}

enum Field {
    Empty = '.',
    Obstacle = '#'
}

public record Map(int MaxX, int MaxY, string Area) {

    public bool WithinBounds(int x, int y) => x >= 0 && x < MaxX &&
                                              y >= 0 && y < MaxY;

    int to_index(int x, int y) => (x + (y * MaxX));

    public Map PlaceObstacle(int x, int y) => 
        this with { Area = Area.ReplaceAt(to_index(x, y), (char)Field.Obstacle) };
    
    public char? this[int x, int y] => WithinBounds(x, y) ? Area[to_index(x, y)] : null;
}