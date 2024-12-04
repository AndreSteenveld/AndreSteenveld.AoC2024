using AndreSteenveld.AoC;

var lines = Console.In.ReadLines().ToArray();

var reports = Enumerable.ToArray(
    from l in lines
    select l.Split(' ').Select(Int32.Parse).ToArray()
);

var safe = reports
    .Select(differences)
    .Count(differences_within_tolerance);

var dampened = reports
    .Where(report => {
        var diffs = differences(report);

        if (differences_within_tolerance(diffs)) {
            //Console.WriteLine( $"Is within parameters :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
            return true;
        }

        if (differences_within_tolerance(differences(report[1..]))) {
            //Console.WriteLine( $"First reading is faulty :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
            return true;
        }

        if (differences_within_tolerance(differences(report[..^1]))) {
            //Console.WriteLine( $"Last reading is faulty :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
            return true;
        }

        if (diffs.Contains(0) || diffs.Max() > 3 || diffs.Min() < -3) {

            var errors = diffs.Count(is_out_of_bounds);

            if (errors > 1) {
                //Console.WriteLine($"Many Out of bounds :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
                return false;
            }
            
            //Console.WriteLine($"Single Out of bounds :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
            
            var index = Array.FindIndex(diffs, is_out_of_bounds);
           
            if (differences_within_tolerance(differences(report.Copy().Remove(index))))
                return true;

            if (differences_within_tolerance(differences(report.Copy().Remove(index + 1))))
                return true;
            
            bool is_out_of_bounds(int v) => v is > 3 or 0 or < -3;
        }

        var increments = diffs.Count(v => v > 0);
        var decrements = diffs.Count(v => v < 0);

        if (increments > decrements && decrements == 1) {
            //Console.WriteLine( $"Has anomalous decrement :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
            var index = Array.FindIndex(diffs, v => v < 0);

            if (differences_within_tolerance(differences(report.Copy().Remove(index))))
                return true;

            if (differences_within_tolerance(differences(report.Copy().Remove(index + 1))))
                return true;
            
        }

        if (decrements > increments && increments == 1) {
            //Console.WriteLine( $"Has anomalous increment :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
            var index = Array.FindIndex(diffs, v => v > 0);
            
            if (differences_within_tolerance(differences(report.Copy().Remove(index))))
                return true;

            if (differences_within_tolerance(differences(report.Copy().Remove(index + 1))))
                return true;
        }

        //Console.WriteLine( $"Is completely invalid :: [ {String.Join(' ', report)} ] :: [ {String.Join(' ', diffs)} ]");
        
        return false;
    }).ToArray();

Console.WriteLine($"Number of safe reports :: {safe}");
Console.WriteLine($"Number of safe reports with dampening :: {dampened.Count()}");

return;

bool differences_within_tolerance(int[] report) => report.All(v => v is < 0 and >= -3) || report.All(v => v is > 0 and <= 3);
int[] differences(int[] numbers) => numbers.Pair((l, r) => l - r).ToArray();

