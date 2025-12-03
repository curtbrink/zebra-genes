namespace ZebraGenes.Types;

// value type for indexing CSP grid
public record CategoryValue(string Category, string Value)
{
    public bool IsOrdered => Category == "Position" && int.TryParse(Value, out _);
}

// this class handles applying clue logic to the grid of possible associations, to aid in puzzle solvability
public class CspMatrix
{
    // CSP possibility matrix
    public bool[,] Possibles { get; }
    
    // id and category value key metadata maps
    public Dictionary<CategoryValue, int> IdMap { get; }
    public List<CategoryValue> ValueMap { get; }
    public Dictionary<string, List<string>> Categories { get; }
    public Dictionary<string, List<int>> IdsInCategory { get; }
    
    // helpful for positional hints
    private readonly int _firstPositionId;
    private readonly int _lastPositionId;
    
    // used to signal if a hint produced a change
    private bool _hintCausedChange = false;

    public CspMatrix(Dictionary<string, List<string>> categories)
    {
        ValidateCategoryList(categories);
        Categories = categories;
        (Possibles, IdMap, ValueMap, IdsInCategory, _firstPositionId, _lastPositionId) = InitializeMaps(categories);
    }
    
    /*
     * ============================
     * MAIN ENTRY POINT FOR SOLVING
     * ============================
     */
    public void SolvePuzzle(List<PuzzleHint> hints)
    {
        var isSolved = false;
        var hasContradiction = false; // todo: not checking contradictions yet, purely a solver for now

        while (!isSolved && !hasContradiction)
        {
            // reset our tracker for "was something changed"
            _hintCausedChange = false;
            
            // iterate the whole hint list and apply constraints
            foreach (var hint in hints)
            {
                Console.WriteLine("Applying hint: " + hint);
                ApplyHint(hint);
            }
            
            // next, check for subset eliminations (first level of higher-order deductions)
            CheckAllSubsetImpossibilities();
            
            // todo: check for solve
            if (!_hintCausedChange)
            {
                // board has reached steady state based on all given hints.
                break;
            }
        }
    }
    
    /*
     * ================
     * HINT APPLICATION
     * ================
     */
    public void ApplyHint(PuzzleHint hint)
    {
        if (!hint.IsValid)
        {
            throw new ArgumentOutOfRangeException("Given hint is invalid: " + hint);
        }

        // we can short-circuit these without doing any position lookups
        if (hint.Type == HintType.Equal)
        {
            EliminatePossibilitiesAndPropagate(GetForcedLinkImpossibilities(hint.PrimaryValue, hint.SecondaryValue!));
            return;
        }

        var possibleAPositionIds = GetPossiblePositionsForCategoryValue(hint.PrimaryValue);
        var possibleBPositionIds = GetPossiblePositionsForCategoryValue(hint.SecondaryValue);
        var impossibilities = hint.Type switch
        {
            HintType.Adjacent => GetAdjacentImpossibilities(hint.PrimaryValue, possibleAPositionIds,
                hint.SecondaryValue!, possibleBPositionIds),
            HintType.Before => GetBeforeImpossibilities(hint.PrimaryValue, possibleAPositionIds, hint.SecondaryValue!,
                possibleBPositionIds),
            HintType.AdjacentBefore => GetAdjacentBeforeImpossibilities(hint.PrimaryValue, possibleAPositionIds,
                hint.SecondaryValue!, possibleBPositionIds),
            HintType.AbsolutePosition => GetAbsolutePositionImpossibilities(hint.PrimaryValue, possibleAPositionIds,
                hint.PossiblePositions!),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        EliminatePossibilitiesAndPropagate(impossibilities);
    }
    
    private List<(CategoryValue, CategoryValue)> GetForcedLinkImpossibilities(CategoryValue catValA,
        CategoryValue catValB)
    {
        Console.WriteLine(
            $"[debug] Got forced link: {catValA.Category}:{catValA.Value} <-> {catValB.Category}:{catValB.Value}");
        // if A<->B, then all other A<->cat(B) are false, and all other B<->cat(A) are false.
        var aId = IdMap[catValA];
        var bId = IdMap[catValB];

        var impossibilitiesForB = IdsInCategory[catValA.Category].Where(i => i != aId)
            .Select(i => (catValB, ValueMap[i])).ToList();
        var impossibilitiesForA = IdsInCategory[catValB.Category].Where(i => i != bId)
            .Select(i => (catValA, ValueMap[i])).ToList();
        
        return impossibilitiesForA.Concat(impossibilitiesForB).ToList();
    }

    private List<(CategoryValue, CategoryValue)> GetAdjacentImpossibilities(CategoryValue a, List<int> aPositions,
        CategoryValue b, List<int> bPositions)
    {
        // assume these are in order (i.e. position:1 id x, position:2 id x+1, etc.
        var possibleAAdjacents = GetAdjacentPositions(aPositions);
        var possibleBAdjacents = GetAdjacentPositions(bPositions);

        List<(CategoryValue, CategoryValue)> impossiblePositions = [];
        impossiblePositions.AddRange(aPositions.Where(aPos => !possibleBAdjacents.Contains(aPos))
            .Select(aPos => (a, ValueMap[aPos])).ToList());
        impossiblePositions.AddRange(bPositions.Where(bPos => !possibleAAdjacents.Contains(bPos))
            .Select(bPos => (b, ValueMap[bPos])).ToList());

        return impossiblePositions;
    }

    private List<(CategoryValue, CategoryValue)> GetBeforeImpossibilities(CategoryValue a, List<int> aPositions,
        CategoryValue b, List<int> bPositions)
    {
        // a must be to the left of b => eliminate any a that are >= max(b)
        // b must be to the right of a => eliminate any b that are <= min(a)
        var maxB = bPositions.Max();
        var minA = aPositions.Min();
        
        List<(CategoryValue, CategoryValue)> impossiblePositions = [];
        impossiblePositions.AddRange(aPositions.Where(aPos => aPos >= maxB)
            .Select(aPos => (a, ValueMap[aPos])).ToList());
        impossiblePositions.AddRange(bPositions.Where(bPos => bPos <= minA)
            .Select(bPos => (b, ValueMap[bPos])).ToList());

        return impossiblePositions;
    }
    
    private List<(CategoryValue, CategoryValue)> GetAdjacentBeforeImpossibilities(CategoryValue a, List<int> aPositions,
        CategoryValue b, List<int> bPositions)
    {
        // a must be immediately to the left of b => eliminate any a that are >= max(b)
        // b must be immediately to the right of a => eliminate any b that are <= min(a)
        var immediatelyBeforeBPositions = GetImmediateBeforePositions(bPositions);
        var immediatelyAfterAPositions = GetImmediateAfterPositions(aPositions);
        
        List<(CategoryValue, CategoryValue)> impossiblePositions = [];
        impossiblePositions.AddRange(aPositions.Where(aPos => !immediatelyBeforeBPositions.Contains(aPos))
            .Select(aPos => (a, ValueMap[aPos])).ToList());
        impossiblePositions.AddRange(bPositions.Where(bPos => !immediatelyAfterAPositions.Contains(bPos))
            .Select(bPos => (b, ValueMap[bPos])).ToList());

        return impossiblePositions;
    }

    private List<(CategoryValue, CategoryValue)> GetAbsolutePositionImpossibilities(CategoryValue a,
        List<int> aPositions, List<int> allowedPositions)
    {
        // a must be in one of the allowed positions
        // hint is indexed as 1-x, so we need to map it first
        var mappedAllowedPositions = allowedPositions.Select(pos => _firstPositionId + (pos - 1)).ToList();
        return aPositions.Where(aPos => !mappedAllowedPositions.Contains(aPos)).Select(aPos => (a, ValueMap[aPos]))
            .ToList();
    }

    private List<int> GetPossiblePositionsForCategoryValue(CategoryValue? catVal) =>
        catVal is null ? [] : IdsInCategory["Position"].Where(pId => Possibles[IdMap[catVal], pId]).ToList();

    private List<int> GetAdjacentPositions(List<int> posList) =>
        GetValidPositions(posList, p => new List<int> { p - 1, p + 1 });

    private List<int> GetImmediateBeforePositions(List<int> posList) =>
        GetValidPositions(posList, p => new List<int> { p - 1 });
    
    private List<int> GetImmediateAfterPositions(List<int> posList) =>
        GetValidPositions(posList, p => new List<int> { p + 1 });
    
    private List<int> GetValidPositions(List<int> posList, Func<int, IEnumerable<int>> selector) => posList
        .SelectMany(selector, (_, newId) => newId).Distinct()
        .Where(p => p >= _firstPositionId && p <= _lastPositionId).ToList();

    /*
     * ===================
     * SUBSET ELIMINATIONS
     * ===================
     */
    private void CheckAllSubsetImpossibilities()
    {
        /*
         * pseudocode for this procedure:
         * for each category C1:
               for each category C2 != C1:
                   let n = number of variables in C1  // usually equal to number of values
                   for i = 2 to n-1:  // subsets of size 2..n-1
                       for each subset S of variables in C1 of size i:
                           U = union of the C2 domains of variables in S
                           if size(U) == i:  // naked pair/triple/quad detected
                               inverseS = variables in C1 not in S
                               for each v in inverseS:
                                   for each u in U:
                                       mark (v, u) as impossible
         *
         * we're essentially saying:
         * if we have:
         * variables 1 2 3 4 5
         * a is {1, 2}
         * b is {1, 2} -> our size-2 subset (a, b) has a size-2 domain union of {1, 2}, so we can mark (c/d/e, 1/2).
         * c is {1, 2, 3, 4}
         * d is {3, 4, 5}
         * e is {3, 4, 5}
         * -> then we can prune (c, 1) and (c, 2).
         * this works for any size subset:
         * a is {1, 2, 3}
         * b is {1, 2, 3}
         * c is {1, 2, 3} -> size-3 subset (abc) with size-3 domain union {123}, mark (d/e, 1/2/3).
         * d is {1, 2, 3, 4, 5}
         * e is {2, 3, 4, 5}
         */

        List<(CategoryValue, CategoryValue)> cellsToPrune = [];
        
        var categories = Categories.Keys.ToList();

        foreach (var c1 in categories)
        {
            // precalculate subsets of c1 and inverses
            var n = IdsInCategory[c1].Count;
            List<List<int>> allC1Subsets = [];
            for (var i = 2; i <= n - 2; i++)
            {
                allC1Subsets.AddRange(IdsInCategory[c1].GetAllSubsetsOfSize(i));
            }
            
            // run subset checks on every other category
            foreach (var c2 in from cat in categories where c1 != cat select cat)
            {
                var c2Ids = IdsInCategory[c2];
                foreach (var subset in allC1Subsets)
                {
                    // need to find the union of v2 variables for this subset
                    // i.e. Alice = {a, b} / Bob = {b, c} => subset of (Alice, Bob) = {a, b, c}
                    var unionSet = new HashSet<int>();
                    foreach (var c2Id in from c1SubsetId in subset
                             from c2Id in c2Ids
                             where Possibles[c1SubsetId, c2Id]
                             select c2Id)
                    {
                        unionSet.Add(c2Id);
                    }

                    if (unionSet.Count == subset.Count)
                    {
                        // subset found - eliminate unionSet items from non-subset c1 variables!
                        var c1Variables = subset.Select(id => ValueMap[id]).ToList();
                        var c2Variables = unionSet.Select(id => ValueMap[id]).ToList();
                        Console.WriteLine("SUBSET ELIMINATION POSSIBLE => (");
                        foreach (var c1Var in c1Variables)
                        {
                            Console.WriteLine(c1Var);
                        }
                        Console.WriteLine(") <=> (");
                        foreach (var c2Var in c2Variables)
                        {
                            Console.WriteLine(c2Var);
                        }
                        Console.WriteLine(")");
                        
                        cellsToPrune.AddRange(
                            from c1IdNotInSubset in IdsInCategory[c1].Where(id => !subset.Contains(id)).ToList()
                            from c2IdInUnion in unionSet
                            select (ValueMap[c1IdNotInSubset], ValueMap[c2IdInUnion]));
                    }
                }
            }
        }

        EliminatePossibilitiesAndPropagate(cellsToPrune);
    }
    
    /*
     * ======================
     * CONSTRAINT PROPAGATION
     * ======================
     */
    private void EliminatePossibilitiesAndPropagate(List<(CategoryValue, CategoryValue)> cellsToMark)
    {
        // initialize a queue for impossibility-marking operations.
        // each operation will check for propagations and queue them.
        var queue = new Queue<(CategoryValue, CategoryValue)>();
        foreach (var cell in cellsToMark)
        {
            queue.Enqueue(cell);
        }

        // propagate all marks
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            var propagationsFromCell = MarkCell(cell);
            propagationsFromCell.ForEach(p => queue.Enqueue(p));
        }
    }

    private List<(CategoryValue, CategoryValue)> MarkCell((CategoryValue, CategoryValue) cell)
    {
        var (a, b) = cell;
        // the gist:
        // (for example, we're marking (Name:Alice,Drink:Tea)
        // 1. if this cell is already false, we don't have to do anything
        // 2. mark the cell (Name:Alice,Drink:Tea) false
        // 3. mark the cell (Drink:Tea,Name:Alice) false
        // 4. check forced links on a (Name:Alice)
        //    a. for each category that isn't name:
        //    b. count how many poss[Name:Alice,categoryValue] == true
        //    c. if count is 1, it is a forced link, add that one value to the return list: (Name:Alice, categoryValue)
        // 5. for each forced link on a:
        //    - enqueue (i, b) i.e. if Alice-Norway, mark Norway-Tea false
        //    - re-enforce the forced link - enqueue all Alice/non-Norway-countries and Norway/non-Alice-names
        // 6. check forced links on b (Drink:Tea)
        //    a. repeat the same on Drink:Tea for each category that isn't Drink
        // 7. for each forced link on b:
        //    - enqueue (j, a) i.e. if Tea-Orange, mark Orange-Alice false
        //    - re-enforce the forced link - enqueue all Tea/non-orange-colors and Orange/non-Tea-drinks
        
        // 1. if this cell is already false, we don't have to do anything
        var aId = IdMap[a];
        var bId = IdMap[b];

        if (!Possibles[aId, bId])
        {
            // nothing to propagate
            return [];
        }
        
        // 2. mark the cell (Name:Alice,Drink:Tea) false
        // 3. mark the cell (Drink:Tea,Name:Alice) false
        Possibles[aId, bId] = false;
        Possibles[bId, aId] = false;

        Console.WriteLine($"[debug] Marked {a.Category}:{a.Value}<->{b.Category}:{b.Value} impossible");
        
        // we know something changed!
        _hintCausedChange = true;
        
        // find what we need to propagate
        List<(CategoryValue, CategoryValue)> cellsToPropagate = [];
        
        // 4/6. check forced links on a/b
        var forcedLinksA = GetForcedLinks(a);
        var forcedLinksB = GetForcedLinks(b);
        
        // 5. for each forced link on a:
        //    - enqueue (i, b) i.e. if Alice-Norway, mark Norway-Tea false
        //    - re-enforce the forced link - enqueue all Alice/non-Norway-countries and Norway/non-Alice-names
        // 7. for each forced link on b:
        //    - enqueue (j, a) i.e. if Tea-Orange, mark Orange-Alice false
        //    - re-enforce the forced link - enqueue all Tea/non-orange-colors and Orange/non-Tea-drinks
        foreach (var aLink in forcedLinksA)
        {
            cellsToPropagate.Add((b, aLink));
            cellsToPropagate.AddRange(GetForcedLinkImpossibilities(a, aLink));
        }

        foreach (var bLink in forcedLinksB)
        {
            cellsToPropagate.Add((a, bLink));
            cellsToPropagate.AddRange(GetForcedLinkImpossibilities(b, bLink));
        }

        return cellsToPropagate;
    }

    private List<CategoryValue> GetForcedLinks(CategoryValue catVal)
    {
        var catValId = IdMap[catVal];
        var otherCategories = Categories.Keys.Where(k => k != catVal.Category).ToList();

        List<CategoryValue> forcedLinks = [];
        foreach (var otherCategory in otherCategories)
        {
            var count = 0;
            var foundPossible = -1;
            
            foreach (var id in IdsInCategory[otherCategory])
            {
                if (Possibles[catValId, id])
                {
                    count++;
                    foundPossible = id;
                }
            }

            if (count == 1)
            {
                forcedLinks.Add(ValueMap[foundPossible]);
            }
        }

        return forcedLinks;
    }

    public void PrintEntities(string keyCategory = "Position")
    {
        var indent = "    ";
        foreach (var keyCategoryValue in Categories[keyCategory])
        {
            Console.WriteLine($"{keyCategoryValue} ->");
            
            var forcedLinks = GetForcedLinks(new CategoryValue(keyCategory, keyCategoryValue));
            foreach (var link in forcedLinks)
            {
                Console.WriteLine($"{indent}{link.Category}: {link.Value}");
            }
        }
    }

    private static (bool[,], Dictionary<CategoryValue, int>, List<CategoryValue>, Dictionary<string, List<int>>, int,
        int) InitializeMaps(Dictionary<string, List<string>> categories)
    {
        var idMap = new Dictionary<CategoryValue, int>();
        var valMap = new List<CategoryValue>();
        var idsInCategory = new Dictionary<string, List<int>>();

        var firstPositionId = -1;
        var lastPositionId = -1;

        var id = 0;
        foreach (var category in categories)
        {
            idsInCategory[category.Key] = [];
            
            // we want to store helpers for positional hints as well
            var isPositionCategory = category.Key == "Position";
            if (isPositionCategory)
            {
                firstPositionId = id;
            }
            
            foreach (var categoryValue in categories[category.Key])
            {
                // for any id: _valueMap[id] -> catval -> _idMap[catval] -> id
                var catVal = new CategoryValue(category.Key, categoryValue);
                idMap[catVal] = id;
                valMap.Add(catVal);
                idsInCategory[category.Key].Add(id);

                id++;
            }

            if (isPositionCategory)
            {
                lastPositionId = id - 1;
            }
        }

        // id is now the total number of category values
        var valueCount = id;
        var possMap = new bool[valueCount, valueCount];
        
        // initialize possibilities based on categories
        foreach (var x in valMap)
        {
            foreach (var y in valMap)
            {
                // values are compatible with themselves and values of other categories,
                // but not other values of the same category.
                var compatible = x == y || x.Category != y.Category;
                possMap[idMap[x], idMap[y]] = compatible;
            }
        }

        return (possMap, idMap, valMap, idsInCategory, firstPositionId, lastPositionId);
    }

    private static void ValidateCategoryList(Dictionary<string, List<string>> categories)
    {
        if (categories.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(categories), "category list must not be empty");
        }

        // ensure all categories have the same number of items, and return the total number of value combinations.
        var expectedSize = categories[categories.Keys.ToList()[0]].Count;
        if (categories.Any(c => c.Value.Count != expectedSize))
        {
            throw new ArgumentOutOfRangeException(nameof(categories), "category sizes must be equal");
        }
    }
}