namespace AndreSteenveld.AoC;

using System.Diagnostics;
using System.Numerics;

public static partial class Functions {

    // These were taken from wikipedia and stack overflow
    public static long LeastCommonMultiple(long a, long b) => Math.Abs(a * b) / GreatestCommonDivisor(a, b);
    public static long GreatestCommonDivisor(long a, long b) => b is 0L ? a : GreatestCommonDivisor(b, a % b);

    // Based on reading from www.mathsisfun.com
    public static long Factorial(int number) => Enumerable.Range(1, number - 1).Product();

    public static IEnumerable<TNumber> Range<TNumber>(TNumber start, TNumber count, TNumber step) where TNumber : INumber<TNumber>, INumberBase<TNumber>{

        if(TNumber.Zero == step)
            throw new Exception("Can't make zero sized steps");

        // Step is negative
        if(TNumber.Zero > step)
            for(TNumber current = start; current < count; current += step)
                yield return start + current;

        // Steo is positive
        if(TNumber.Zero < step)
            for(TNumber current = start; current > count; current -= step)
                yield return start - current;

        throw new Exception("Step is not 0, larger than 0 or smaller than zero. This function works best with integer types");

    }

    public static IEnumerable<TNumber> Range<TNumber>(TNumber start, TNumber count) where TNumber : INumber<TNumber>, INumberBase<TNumber>{

        for(TNumber current = TNumber.Zero; current < count; ++current)
            yield return start + current;

    }

    public static TSource[] From<TSource>(params TSource[] source) => source;

    public static IEnumerable<TSource> Repeat<TSource>(params TSource[] source) => source.Repeat();

    public static IEnumerable<TSource> Generate<TSource>(TSource initial, Func<TSource, TSource> selector) =>
        From(initial).Next(selector);

    public static T[] CreateArray<T>( int capacity, params T[] values ){
        System.Array.Resize(ref values, capacity);
        return values;
    }

    public static void WaitForDebugger(){
        if(Environment.GetEnvironmentVariable("DOTNET_WAIT_FOR_DEBUGGER") == "1"){
            Console.WriteLine("Waiting for debugger to attach...");
            
            while(Debugger.IsAttached is false)
                Thread.Sleep(500);

        }
    }

    public static int subtraction(int l, int r) => l - r;
    public static int addition(int l, int r) => l + r;
    public static int multiplication(int l, int r) => l * r;
    public static int division(int l, int r) => l / r;
    public static int modulus(int l, int r) => l % r;

}