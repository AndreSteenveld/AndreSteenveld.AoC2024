using System.Globalization;
using AndreSteenveld.AoC;

using static AndreSteenveld.AoC.Functions;

var lines = Console.In.ReadLines().ToArray();

var (A, B, C) = (
    Int64.Parse(lines[0]["Register A:".Length..], NumberStyles.Integer),
    Int64.Parse(lines[1]["Register B:".Length..], NumberStyles.Integer),
    Int64.Parse(lines[2]["Register B:".Length..], NumberStyles.Integer)
);

var program = lines[4]["Program:".Length..].Split(',').Select(Int32.Parse).ToArray();

Console.WriteLine($"Program :: [ {String.Join(",", program)} ]");

var frame = StackFrame.Run(A, B, C, program).Last();
Console.WriteLine($"[ {frame.I} ] :: ({frame.A}, {frame.B}, {frame.C}) :: {String.Join(',', frame.O)}");

Parallel.For(0l, Int64.MaxValue, IsQuineUsingSeed(B, C, program));

return;

Action<long, ParallelLoopState> IsQuineUsingSeed(long B, long C, int[] program) => (long A, ParallelLoopState state) => {
    var previous_frames = new HashSet<(long I, long A, long B, long C)>();

    var frame = StackFrame.Start(A, B, C, program);
    var output_length = frame.O.Length;
    
    for (var i = 0l;; i++) {
        if (false == previous_frames.Add((frame.I, frame.A, frame.B, frame.C))) {
            Console.WriteLine($"[ {A} ] entered infinite loop after [ {i} ] iterations");
            return;
        }
        
        if(frame.I >= program.Length)
            return;

        frame = frame.Step();

        if (frame.O.Length == output_length) continue;
        
        output_length = frame.O.Length;

        if (program[output_length - 1] != frame.O[output_length - 1])
            return;
        
        if (program.Length == output_length) {
            Console.WriteLine($"Found quine seed :: {A}");
            state.Stop();
            return;
        }
    }
};

public record StackFrame {

    public void Deconstruct(out long a, out long b, out long c, out long[] o) {
        a = A;
        b = B;
        c = C;
        o = O;
    }
    
    public static StackFrame Start(long A, long B, long C, int[] P) => new (A, B, C, P);
    
    public static IEnumerable<StackFrame> Run(long A, long B, long C, int[] P) {

        var frame = Start(A, B, C, P);

        yield return frame;
        
        while (frame.I < frame.P.Length)
            yield return frame = frame.Step();
        
    }

    private StackFrame(long A, long B, long C, int[] P) {
        this.A = A;
        this.B = B;
        this.C = C;
        this.P = P;
    }

    public long I { get; private init; } = 0;
    public long A { get; private init; } = 0;
    public long B { get; private init; } = 0;
    public long C { get; private init; } = 0;
    
    public int[] P { get; private init; } = [];
    public long[] O { get; private init; } = [];

    private int instruction => P[I];
    private long operand => P[I + 1];
    
    public StackFrame Step() {
        
        var o = (instruction, operand) switch {
            (0 or 2 or 5 or 6 or 7, 0 or 1 or 2 or 3) => operand,
            (0 or 2 or 5 or 6 or 7, 4) => A,
            (0 or 2 or 5 or 6 or 7, 5) => B,
            (0 or 2 or 5 or 6 or 7, 6) => C,
            (0 or 2 or 5 or 6 or 7, 7) => throw new Exception("Invalid program"),
            (_, _) => operand
        };

        var frame = instruction switch {
            0 => ADV(o),
            1 => BXL(o),
            2 => BST(o),
            3 => JNZ(o),
            4 => BXC(o),
            5 => OUT(o),
            6 => BDV(o),
            7 => CDV(o),
        }; 
        
        return frame with { I = frame.I + 2 };
        
    }
    
    //
    // The adv instruction (opcode 0) performs division. The numerator is the value in the A register. The 
    // denominator is found by raising 2 to the power of the instruction's combo operand. (So, an operand of 2 would
    // divide A by 4 (2^2); an operand of 5 would divide A by 2^B.) The result of the division operation is truncated
    // to an integer and then written to the A register.
    //
    StackFrame ADV(long o) => this with { A = (long)(A / Math.Pow(2, o)) };

    //
    // The bdv instruction (opcode 6) works exactly like the adv instruction except that the result is stored 
    // in the B register. (The numerator is still read from the A register.)
    //
    StackFrame BDV(long o) => this with { B = (long)(A / Math.Pow(2, o)) };
    
    //
    // The cdv instruction (opcode 7) works exactly like the adv instruction except that the result is stored 
    // in the C register. (The numerator is still read from the A register.)
    //
    StackFrame CDV(long o) => this with { C = (long)(A / Math.Pow(2, o)) };

    //
    // The bxl instruction (opcode 1) calculates the bitwise XOR of register B and the instruction's literal 
    // operand, then stores the result in register B.
    //
    StackFrame BXL(long o) => this with { B = B ^ o };
    
    //
    // The bst instruction (opcode 2) calculates the value of its combo operand modulo 8 (thereby keeping only its 
    // lowest 3 bits), then writes that value to the B register.
    //
    StackFrame BST(long o) => this with { B = o % 8 };

    //
    // The jnz instruction (opcode 3) does nothing if the A register is 0. However, if the A register is not 
    // zero, it jumps by setting the instruction pointer to the value of its literal operand; if this instruction jumps,
    // the instruction pointer is not increased by 2 after this instruction.
    //
    StackFrame JNZ(long o) => A is 0 ? this : this with { I = o - 2 };

    //
    // The bxc instruction (opcode 4) calculates the bitwise XOR of register B and register C, then stores 
    // the result in register B. (For legacy reasons, this instruction reads an operand but ignores it.)
    //
    StackFrame BXC(long _) => this with { B = B ^ C };

    //
    // The out instruction (opcode 5) calculates the value of its combo operand modulo 8, then outputs that value.
    // (If a program outputs multiple values, they are separated by commas.)
    //
    StackFrame OUT(long o) => this with { O = [..O, o % 8] };
    
}
