namespace AndreSteenveld.AoC;
public static partial class Extensions {
     public static IEnumerable<string> ReadLines(this TextReader reader){
        for( var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
            yield return line;
     }

     public static string ToString(this IEnumerable<string> lines, bool preserveNewlines = true) =>
         preserveNewlines
             ? String.Join('\n', lines)
             : String.Concat(lines);
     
}