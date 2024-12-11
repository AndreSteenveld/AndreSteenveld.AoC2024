using System.Diagnostics;
using AndreSteenveld.AoC;

using static System.Math;

var sw = new Stopwatch() ;; sw.Start();

ulong[] numbers = Console.In.ReadLines().Single().Split(' ').Select(UInt64.Parse).ToArray();

sw.Restart();

var number_of_stones_25 = numbers.Blink(25).Last().Length;

Console.WriteLine($"After blinking 25 times in [ {sw.ElapsedMilliseconds}ms ] the number of stones is :: {number_of_stones_25}") ;; sw.Restart();

var number_of_stones_75 = numbers.BlinkReallyFast(75).Length;

Console.WriteLine($"After blinking 75 times in [ {sw.ElapsedMilliseconds}ms ] the number of stones is :: {number_of_stones_75}") ;

return;

public static class _ {

    public static ulong[] Blink(this ulong[] line) {
        return line
            .SelectMany(
                number => 
                    number is 0                     ? [1UL] 
                    : NumberOfDigits(number) % 2 is 0 ? SplitNumber(number) 
                    :                                   [number * 2024] 
            )
            .ToArray();
        
        int NumberOfDigits(ulong number) {
            for( var (number_of_digits, power) = (1, 1UL) ;; (number_of_digits, power) = (number_of_digits + 1, power * 10))
                if (power <= number && number < power * 10)
                    return number_of_digits;
        }
        
        ulong[] SplitNumber(ulong number) {
            for (ulong power = 10 ;; power *= 100)
                if (number >= power && number < power * 100) {
                    var divider = (ulong)Pow(10, NumberOfDigits(power) / 2);
                    return [number / divider, number % divider];
                }
        }
    }
    
    public static ulong[] BlinkReallyFast(this ulong[] line, int times) {
        do {
            Console.WriteLine($"Fast iteration :: [ {times} // {line.Length} ]");
            line = line.Blink();
        } while (times-- > 0);

        return line;
    }
    
    
    public static IEnumerable<ulong[]> Blink(this ulong[] line, int times = 1) {
        yield return line;
        
        if(times is 0)
            yield break;

        foreach (var l in line.Blink().Blink(times - 1))
            yield return l;
    }
    
    
}