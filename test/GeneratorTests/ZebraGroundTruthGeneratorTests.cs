using Csp.Builders;
using Generator.Zebra.GroundTruth;
using Xunit.Abstractions;

namespace GeneratorTests;

public class ZebraGroundTruthGeneratorTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void TestSolutionShuffle()
    {
        // set up categories
        // solution generator doesn't care about constraints/solvability
        var builder = ZebraBuilder.Create(5)
            .AddCategory("Cat1", ["A", "B", "C", "D", "E"])
            .AddCategory("Cat2", ["V", "W", "X", "Y", "Z"])
            .AddCategory("Cat3", ["J", "K", "L", "M", "N"]);
        // 120^3 = 1,728,000 solutions

        var result = ZebraGroundTruthGenerator.GenerateFullSolution(builder);

        var entities = new List<List<string>>();
        for (var i = 0; i < 5; i++)
        {
            entities.Add([]);
        }
        foreach (var k in result.Keys)
        {
            entities[result[k] - 1].Add(k.Name);
        }

        for (var i = 0; i < entities.Count; i++)
        {
            testOutputHelper.WriteLine($"entity {i} ->");
            for (var j = 0; j < entities[i].Count; j++)
            {
                testOutputHelper.WriteLine($"    {entities[i][j]}");
            }
        }
    }
}