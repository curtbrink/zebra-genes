using Csp.Builders;
using Csp.Interfaces;
using Generator.Zebra.Clues;
using Generator.Zebra.Clues.Abstract;
using Generator.Zebra.Clues.Types;

namespace Generator.Zebra.GroundTruth;

public class ZebraGroundTruthGenerator
{
    // input: zebra builder
    // output: list of attribute variables (keys?) and positions representing a full solution
    public static IDictionary<IVariable, int> GenerateFullSolution(ZebraBuilder builder)
    {
        var (csp, categories) = builder.Build();

        var k = csp.Domains.Keys.ToList()[0];
        var domain = csp.Domains[k].Values.ToList();
        
        var result = new Dictionary<IVariable, int>();

        foreach (var c in categories.Keys)
        {
            var categoryValues = categories[c];
            var shuffledVarKeys = categoryValues.Shuffle()
                .Select(cv => csp.Variables.First(cspv => cspv.Name == $"{c}:{cv}")).ToList();
            for (var i = 0; i < shuffledVarKeys.Count; i++)
            {
                result[shuffledVarKeys[i]] = domain[i];
            }
        }

        return result;
    }

    public static List<Clue> GenerateEntireCluePool(IDictionary<IVariable, int> fullSolution,
        IDictionary<string, List<string>> categories)
    {
        // need a "clue interface/base object"
        // for each var a
        // - add true unary constraint clue
        // - for each var b
        //   - add all true unique binary constraint clues between a and b (adjacent, before, immediate before, etc.)

        List<Clue> allClues = [];

        var categoryKeys = categories.Keys.ToList();
        for (var i = 0; i < categoryKeys.Count; i++)
        {
            var aCat = categoryKeys[i];
            var aCatVals = categories[aCat];
            foreach (var aVar in aCatVals)
            {
                var aCspVar = fullSolution.Keys.First(k => k.Name == $"{aCat}:{aVar}");
                var aVarPosition = fullSolution[aCspVar];
                Console.WriteLine(
                    $"[outer] checking variable [{aCat}:{aVar}] -> position {aVarPosition}, adding position equals clue");
                allClues.Add(new PositionEqualsClue(aCat, aVar, fullSolution[aCspVar]));

                // if we're iterating categories[i] values,
                // we've already added all associations involving categories[i-1] values etc.
                for (var j = i + 1; j < categoryKeys.Count; j++)
                {
                    var bCat = categoryKeys[j];
                    var bCatVals = categories[bCat];
                    // position equals clue for b will be handled by outer loop
                    foreach (var bVar in bCatVals)
                    {
                        var bCspVar = fullSolution.Keys.First(k => k.Name == $"{bCat}:{bVar}");
                        var bVarPosition = fullSolution[bCspVar];
                        Console.WriteLine($"[inner] checking variable [{bCat}:{bVar}] -> position {bVarPosition}");
                        if (aVarPosition == bVarPosition)
                        {
                            Console.WriteLine($"[inner] added attributes equal clue");
                            allClues.Add(new AttributesAreEqualClue(aCat, aVar, bCat, bVar));
                        }
                        else
                        {
                            // which one is in the before position?
                            var aIsBefore = aVarPosition < bVarPosition;
                            var (firstCat, firstVar, firstPos) =
                                aIsBefore ? (aCat, aVar, aVarPosition) : (bCat, bVar, bVarPosition);
                            var (secondCat, secondVar, secondPos) =
                                aIsBefore ? (bCat, bVar, bVarPosition) : (aCat, aVar, aVarPosition);
                            
                            // one is always before if not equal
                            Console.WriteLine($"[inner] added attribute is before clue");
                            allClues.Add(new AttributeIsBeforeClue(firstCat, firstVar, secondCat, secondVar));
                            
                            // are they also adjacent?
                            if (firstPos == secondPos - 1)
                            {
                                Console.WriteLine($"[inner] added attribute is adjacent and immediately before clues");
                                allClues.Add(new AttributeIsImmediatelyBeforeClue(firstCat, firstVar, secondCat,
                                    secondVar));
                                allClues.Add(new AttributesAreAdjacentClue(firstCat, firstVar, secondCat, secondVar));
                            }
                        }
                    }
                }
            }
        }

        return allClues;
    }
}