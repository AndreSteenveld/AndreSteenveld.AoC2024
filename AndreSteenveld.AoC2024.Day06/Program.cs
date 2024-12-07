
using AndreSteenveld.AoC;

using Coordinate = (int x, int y, Direction d);

var (stack, area, max_x, max_y) = Console.In.ReadLines().Aggregate(
    (new Stack<Coordinate>(), String.Empty, 0, 0),
    (result, line) => {

        var (stack, map, max_x, max_y) = result;

        max_x = line.Length switch {
            var v when max_x is 0 => v,
            var v when max_x == v => v,
            _ => throw new Exception()
        };
        
        var start = (x : line.IndexOf((char)Direction.Up), max_y, Direction.Up);

        if (start.x is not -1) {
            line = line.Replace((char)Direction.Up, (char)Field.Empty);
            stack.Push(start);
        }
         
        return (stack, String.Concat(map, line), max_x, max_y+1);
    }
);

var map = new Map(max_x, max_y, area);

for (var (x, y, d) = stack.Peek(); map.WithinBounds(x, y); (x, y, d) = stack.Peek())
    stack.Push(Step(map, (x, y, d)));

var distinct_locations_visited = stack.ToList().DistinctBy(c => (c.x, c.y)).Count() - 1;

Console.WriteLine($"Distinct locations visited :: {distinct_locations_visited}");

return;

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

    public Map PlaceObstacle((int x, int y) c) => 
        this with { Area = Area.ReplaceAt(to_index(c.x, c.y), (char)Field.Obstacle) };
    
    public char? this[int x, int y] => WithinBounds(x, y) ? Area[to_index(x, y)] : null;
}