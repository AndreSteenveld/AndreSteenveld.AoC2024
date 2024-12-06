
using AndreSteenveld.AoC;
using static AndreSteenveld.AoC.Functions;

WaitForDebugger();

var (rules, updates) = Console.In.ReadLines().Aggregate(
    (rules: new Dictionary<int, int[]>(), updates: new List<int[]>()),
    (result, line) => {

        if (String.IsNullOrWhiteSpace(line)) {
            return result;

        }
        else if (line.Contains('|')) {
            
            var (l, r, _) = line.Split('|').Select(Int32.Parse).ToArray();
            
            if (result.rules.ContainsKey(l)) 
                result.rules[l] = [r, ..result.rules[l]];
            else 
                result.rules[l] = [r];

        }
        else {

            var a = line.Split(',').Select(Int32.Parse).ToArray();
            result.updates.Add(a);
    
        }
        
        return result;
    });

var valid_updates = updates.AsParallel()
    .Where(has_valid_printing_order);

var summed_page_numbers = Enumerable.Sum(from u in valid_updates select u[u.Length / 2]);

Console.WriteLine( $"Summed valid update page numbers :: {summed_page_numbers}");

var fixed_updates = updates.AsParallel()
    .Where(l => has_valid_printing_order(l) is false)
    .Select(order_according_to_rules);

var summed_fixed_page_numbers = Enumerable.Sum(from u in fixed_updates select u[u.Length / 2]);

Console.WriteLine($"Summed fixed page numers :: {summed_fixed_page_numbers}");

return;

int[] order_according_to_rules(int[] update) {
    for (int index = 0; index < update.Length; index++) {
        var n = update[index];

        if (rules.TryGetValue(n, out var rule) is false)
            continue;

        var swap = Array.FindIndex(update, 0, index, rule.Contains); 

        if (swap is -1)
            continue;
        
        (update[swap], update[index]) = (update[index], update[swap]);
        index = swap;
    }

    return update;
}

bool has_valid_printing_order(int[] update) {
    for (int index = 0; index < update.Length; index++) {
        var n = update[index];

        if (rules.TryGetValue(n, out var rule) is false)
            continue;

        if (update[0..index].Any(rule.Contains))
            return false;
    }

    return true;
}
