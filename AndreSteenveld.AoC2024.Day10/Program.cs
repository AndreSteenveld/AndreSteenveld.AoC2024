using System.Collections;
using System.Diagnostics;
using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

using Coordinate = (int x, int y, char level);
using Map = AndreSteenveld.AoC.TFieldMap<char>;

var sw = new Stopwatch() ;; sw.Start();

var map = Console.In.ReadLines().AsMapOf(line => line.Select(CharacterToLevel).ToArray());

sw.Restart();

var routes = map.Fields
    .Where((Coordinate location) => location.level is Level.Zero)
    .WalkRoute(map, [Level.One, Level.Two, Level.Three, Level.Four, Level.Five, Level.Six, Level.Seven, Level.Eight, Level.Nine])
    .Select( route => (route[0], route[^1]))
    .ToArray();

Console.WriteLine($"Computed all routes in {sw.ElapsedMilliseconds}ms") ;; sw.Restart();

var distinct = routes.Distinct().Count();

Console.WriteLine($"Found [ {distinct} ] distinct in routes {sw.ElapsedMilliseconds}ms ") ;; sw.Restart();

var all_routes = routes.Length;

Console.WriteLine($"Found a total of [ {all_routes} ] routes in {sw.ElapsedMilliseconds}ms ");

return;

char CharacterToLevel(char c) => c switch {
    '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => c,
    _ => Level.Void
};

public static class Level {
    public const char Zero  = '0';
    public const char One   = '1';
    public const char Two   = '2';
    public const char Three = '3';
    public const char Four  = '4';
    public const char Five  = '5';
    public const char Six   = '6';
    public const char Seven = '7';
    public const char Eight = '8';
    public const char Nine  = '9';
    public const char Void  = '.';
}

public static class _ {
    
    public static IEnumerable<Coordinate[]> WalkRoute(this IEnumerable<Coordinate> locations, Map map, params char[] route) =>
        locations.SelectMany(location => location.WalkRoute(map, route));

    public static IEnumerable<Coordinate[]> WalkRoute(this Coordinate location, Map map, params char[] route) {
        if (route.Length == 0)
            return [[location]];
        
        var step = route[0];
        var (x, y, _) = location;

        var routes = new List<Coordinate[]>();

        if (map[x - 0, y - 1] == step) routes.AddRange(map.Field[x - 0, y - 1]?.WalkRoute(map, route[1..]).Select(r => (Coordinate[]) [location, ..r])!);
        if (map[x + 1, y + 0] == step) routes.AddRange(map.Field[x + 1, y + 0]?.WalkRoute(map, route[1..]).Select(r => (Coordinate[]) [location, ..r])!);
        if (map[x + 0, y + 1] == step) routes.AddRange(map.Field[x + 0, y + 1]?.WalkRoute(map, route[1..]).Select(r => (Coordinate[]) [location, ..r])!);
        if (map[x - 1, y - 0] == step) routes.AddRange(map.Field[x - 1, y - 0]?.WalkRoute(map, route[1..]).Select(r => (Coordinate[]) [location, ..r])!);
        
        return routes;
    }

}
