using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Zebra.Builders;
using Generator.Zebra.Zebra.Clues;
using Generator.Zebra.Zebra.Clues.Types;
using Generator.Zebra.Zebra.Types;

namespace Generator.Zebra.Zebra.GroundTruth;

public static class ZebraGroundTruthGenerator
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

    public static List<ZebraClue> GenerateEntireCluePool(IDictionary<IVariable, int> fullSolution,
        IDictionary<string, List<string>> categories)
    {
        // need a "clue interface/base object"
        // for each var a
        // - add true unary constraint clue
        // - for each var b
        //   - add all true unique binary constraint clues between a and b (adjacent, before, immediate before, etc.)

        List<ZebraClue> allClues = [];

        var categoryKeys = categories.Keys.ToList();
        for (var i = 0; i < categoryKeys.Count; i++)
        {
            var aCat = categoryKeys[i];
            var aCatVals = categories[aCat];
            foreach (var aVar in aCatVals)
            {
                var aAttr = new ZebraAttribute(aCat, aVar);
                var aCspVar = fullSolution.Keys.First(k => k.Name == $"{aCat}:{aVar}");
                var aVarPosition = fullSolution[aCspVar];
                Console.WriteLine(
                    $"[outer] checking variable [{aCat}:{aVar}] -> position {aVarPosition}, adding position equals clue");
                allClues.Add(new PositionEqualsClue(aAttr, fullSolution[aCspVar]));

                // if we're iterating categories[i] values,
                // we've already added all associations involving categories[i-1] values etc.
                for (var j = i + 1; j < categoryKeys.Count; j++)
                {
                    var bCat = categoryKeys[j];
                    var bCatVals = categories[bCat];
                    // position equals clue for b will be handled by outer loop
                    foreach (var bVar in bCatVals)
                    {
                        var bAttr = new ZebraAttribute(bCat, bVar);
                        var bCspVar = fullSolution.Keys.First(k => k.Name == $"{bCat}:{bVar}");
                        var bVarPosition = fullSolution[bCspVar];
                        Console.WriteLine($"[inner] checking variable [{bCat}:{bVar}] -> position {bVarPosition}");
                        if (aVarPosition == bVarPosition)
                        {
                            Console.WriteLine($"[inner] added attributes equal clue");
                            allClues.Add(new AttributesAreEqualClue(aAttr, bAttr));
                        }
                        else
                        {
                            // which one is in the before position?
                            var aIsBefore = aVarPosition < bVarPosition;
                            var (firstAttr, firstPos) = aIsBefore ? (aAttr, aVarPosition) : (bAttr, bVarPosition);
                            var (secondAttr, secondPos) = aIsBefore ? (bAttr, bVarPosition) : (aAttr, aVarPosition);
                            
                            // one is always before if not equal
                            Console.WriteLine($"[inner] added attribute is before clue");
                            allClues.Add(new AttributeIsBeforeClue(firstAttr, secondAttr));
                            
                            // are they also adjacent?
                            if (firstPos == secondPos - 1)
                            {
                                Console.WriteLine($"[inner] added attribute is adjacent and immediately before clues");
                                allClues.Add(new AttributeIsImmediatelyBeforeClue(aAttr, bAttr));
                                allClues.Add(new AttributesAreAdjacentClue(aAttr, bAttr));
                            }
                        }
                    }
                }
            }
        }

        return allClues;
    }
}