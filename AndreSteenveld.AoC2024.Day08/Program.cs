using System.Diagnostics;
using AndreSteenveld.AoC;

var sw = new Stopwatch() ;; sw.Start();

var map = Console.In.ReadLines().AsMap();

var stations = map.Fields
    .Where( field => Char.IsLetterOrDigit(field.f) )
    .Aggregate( new Dictionary<char, List<(int x, int y)>>(), (dictionary, station) => {
        var (x, y, f) = station;
        
        if(dictionary.TryGetValue(f, out var stations))
            stations.Add((x, y));
        else 
            dictionary[f] = [(x, y)];
        
        return dictionary;
    });

Console.WriteLine( $"Parsed map [ {map.Width} x {map.Height} ] in {sw.ElapsedMilliseconds}ms ") ;; sw.Restart();

var antinodes = stations.ToDictionary(
        kv => kv.Key,
        kv => Enumerable
            .Distinct(
                from station in kv.Value
                from other in kv.Value
                let offset = ( x : station.x - other.x, y : station.y - other.y) where offset is not (0,0)
                let antinode = (station.x + offset.x, station.y + offset.y) where antinode.WithinBoundsOf(map)
                select antinode
            )
            .ToArray()
    );

var number_of_antinodes = antinodes.SelectMany(kv => kv.Value).Distinct().Count();

Console.WriteLine($"Total number of antinodes [ {sw.ElapsedMilliseconds}ms ] :: {number_of_antinodes}") ;; sw.Restart();

var projected_antinode_lines = stations.ToDictionary(
    kv => kv.Key,
    kv => 
        (
            from station in kv.Value
            from other in kv.Value
            let offset = (x: station.x - other.x, y: station.y - other.y) where offset is not (0, 0)
            select (station, offset)
        )
        .SelectMany( so => {
            var (station, offset) = so;

            return Enumerable
                .Range(1, Int32.MaxValue)
                .Select(step => (station.x + offset.x * step, station.y + offset.y * step))
                .TakeWhile(map.WithinBounds)
                .Append(station);
        })
        .Distinct()
        .ToArray()
    );

var number_of_antinodes_on_line = projected_antinode_lines.SelectMany(kv => kv.Value).Distinct().Count();

Console.WriteLine($"Total number of projected antinode lines [ {sw.ElapsedMilliseconds}ms ] :: {number_of_antinodes_on_line}");