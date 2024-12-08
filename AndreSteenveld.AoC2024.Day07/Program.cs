
using System.Numerics;
using AndreSteenveld.AoC;

using static AndreSteenveld.AoC.Functions;

using Calibration = (ulong target, ulong[] sequence);

var calibrations = Console.In.ReadLines()
    .Select(line => {
        var (target, seqence) = line.Replace(":", "").Split(' ').Select(UInt64.Parse).ToArray();
        return (target, seqence);
    })
    .ToArray();

part_1();
part_2();

return;

void part_1() {
    var operators = new Func<ulong, ulong, ulong>[] { addition, multiplication };

    var solvers = calibrations
        .Select(c => c.seqence.Length)
        .Distinct()
        .ToDictionary(c => c,
            c => Enumerable
                .Repeat(operators, c - 1)
                .CartesianProduct()
                .Select(Enumerable.ToArray)
                .ToArray()
        );

    var number_of_solvable_calibrations = calibrations.AsParallel()
        .Where(calibration => {
            var (target, sequence) = calibration;
            var solver = solvers[sequence.Length];

            return solver.Any(operations => {
                ulong sum = sequence[0];

                for (int index = 0; index < operations.Length && sum <= target; index++)
                    sum = operations[index](sum, sequence[index + 1]);

                return sum == target;
            });
        })
        .Aggregate(0UL, (sum, calibration) => sum + calibration.target);

    Console.WriteLine($"Sum of solvable targets (part 1):: {number_of_solvable_calibrations}");
}

void part_2() {
    var operators = new Func<ulong, ulong, ulong>[] { addition, multiplication, concatination };

    var solvers = calibrations
        .Select(c => c.seqence.Length)
        .Distinct()
        .ToDictionary(c => c,
            c => Enumerable
                .Repeat(operators, c - 1)
                .CartesianProduct()
                .Select(Enumerable.ToArray)
                .ToArray()
        );

    var number_of_solvable_calibrations = calibrations.AsParallel()
        .Where(calibration => {
            var (target, sequence) = calibration;
            var solver = solvers[sequence.Length];

            return solver.Any(operations => {
                ulong sum = sequence[0];

                for (int index = 0; index < operations.Length && sum <= target; index++)
                    sum = operations[index](sum, sequence[index + 1]);

                return sum == target;
            });
        })
        .Aggregate(0UL, (sum, calibration) => sum + calibration.target);

    Console.WriteLine($"Sum of solvable targets (part 2):: {number_of_solvable_calibrations}");

    return;

    ulong concatination(ulong l, ulong r) {
        for (ulong power = 10; ; power *= 10)
            if (r < power)
                return (l * power) + r;
    }
}

