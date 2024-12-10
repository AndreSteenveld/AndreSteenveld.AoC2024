
using System.Diagnostics;
using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

var sw = new Stopwatch() ;; sw.Start();

var disk = Console.In.ReadLines().Single().ToCharArray();

sw.Restart();

var fragmented_checksum = disk
    .ToBlockList()
    .Compact()
    .Checksum();

Console.WriteLine($"Checksum of compacted disk [ {sw.ElapsedMilliseconds}ms ] :: {fragmented_checksum}");

sw.Restart();

var moving_files_checksum = disk
     .ToBlockList()
     .Compact(fragment: false)
     .Checksum();

Console.WriteLine($"Checksum of moved files [ {sw.ElapsedMilliseconds}ms ] :: {moving_files_checksum}");

return;

class Block {
    
    private Block(int? id, char character, int length) {
        Id = id;
        Character = character;
        Length = length;
    }
    
    public Block(int length) : this(null, '.', length) { }

    public Block(int id, char character, int length) : this((int?)id, character, length) { }
    
    public int? Id { get; private set; }
    public char Character { get; private set; }
    public int Length { get; set; }
    
    public override string ToString() {
        return String.Join("", Repeat(Character).Take(Length));
    }
}

static class _ {
    public static List<Block> ToBlockList(this char[] disk) => disk.Index().Aggregate(new List<Block>(disk.Length), (list, ic) => {
        var (i, c) = ic;

        var characters = (int)Char.GetNumericValue(c);

        var block = i % 2 is 1 
            ? new Block(characters) 
            : new Block(i / 2, (char)('0' + i / 2 % 10), characters);

        list.Add(block);
    
        return list;
    });

    public static ulong Checksum(this List<Block> files) => files
        .SelectMany(b => Repeat(b.Id ?? 0).Take(b.Length))
        .Select(multiplication)
        .Aggregate(0UL, (l, i) => l + (ulong)i);

    public static List<Block> Compact(this List<Block> files, bool fragment = true) {

        var moved = new HashSet<int>(files.Count / 2);
        int data_index = files.Count - 1;
        
        while (true) {

            data_index = fragment
                ? files.FindLastIndex(b => b.Character is not '.')
                : files.FindLastIndex(b => b.Character is not '.' && moved.Contains((int)b.Id!) is false);

            if (data_index is -1)
                return files;
            
            var data_block = files[data_index];
            
            var free_index = fragment
                ? files.FindIndex(b => b.Character is '.' && b.Length > 0)
                : files.FindIndex(b => b.Character is '.' && b.Length > 0 && b.Length >= data_block.Length);

            if (fragment is false) {
                moved.Add((int)data_block.Id!);

                if (free_index is -1) continue;
                if (free_index > data_index) continue;
                
            } else if (fragment && free_index > data_index)
                return files;
         
            var free_block = files[free_index];
            
            if (fragment && free_block.Length < data_block.Length) {
                    
                var remainder = new Block((int)data_block.Id!, data_block.Character, data_block.Length - free_block.Length);
                
                data_block.Length = free_block.Length;
                (files[free_index], files[data_index]) = (files[data_index], files[free_index]);
                
                files.Insert(data_index + 1, remainder);
                
            } else if (free_block.Length == data_block.Length) {
                    
                (files[free_index], files[data_index]) = (files[data_index], files[free_index]);
                
            } else if (free_block.Length > data_block.Length) {

                var remainder = new Block(free_block.Length - data_block.Length);
                
                free_block.Length = data_block.Length;
                (files[free_index], files[data_index]) = (files[data_index], files[free_index]);
                    
                files.Insert(free_index + 1, remainder);    
                
            }  
            
        }
        
    }
    
}