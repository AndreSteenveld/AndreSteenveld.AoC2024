using System.Collections;

namespace AndreSteenveld.AoC;

public static class Coordinate {
    public static bool WithinBoundsOf(this (int x, int y) coordinate, Map map) =>
        map.WithinBounds(coordinate.x, coordinate.y);
} 

public record Map(int Width, int Height, string Area) : IEnumerable<(int x, int y, char f)> {

    public static Map FromInputLines(Map? map, string line) {

        map ??= new Map(0, 0, String.Empty);
        
        return new Map(
            Width: map.Width == 0           ? line.Length 
                 : map.Width == line.Length ? line.Length 
                 : throw new Exception("Jagged map"), 
            Height: map.Height + 1, 
            Area: map.Area + line
        );

    }

    public bool WithinBounds((int x, int y) c) => WithinBounds(c.x, c.y);
    public bool WithinBounds(int x, int y) => x >= 0 && x < Width &&
                                              y >= 0 && y < Height;

    int to_index(int x, int y) => (x + (y * Width));
    (int x, int y) to_coordinate(int i) => (i % Width, i / Height);
    
    public char? this[int i] => i < 0 ? null : i > Area.Length ? null : Area[i];
    public char? this[int x, int y] => WithinBounds(x, y) ? Area[to_index(x, y)] : null;

    public IEnumerable<(int x, int y, char f)> Fields => this;
    
    IEnumerator<(int x, int y, char f)> IEnumerable<(int x, int y, char f)>.GetEnumerator() {
        for (int i = 0; i < Area.Length; i++) {
            var (x, y) = to_coordinate(i);
            yield return (x, y, Area[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable<(int, int, int)>)this).GetEnumerator();
    }
}