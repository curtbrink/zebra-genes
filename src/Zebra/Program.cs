// See https://aka.ms/new-console-template for more information

using Csp.Builders;
using Generator.Zebra.Zebra.GroundTruth;

// test solution and clue generator
var zebra = ZebraBuilder.Create(5)
    .AddCategory("C1", ["a", "b", "c", "d", "e"])
    .AddCategory("C2", ["f", "g", "h", "i", "j"])
    .AddCategory("C3", ["k", "l", "m", "n", "o"]);
    
    var solution = ZebraGroundTruthGenerator.GenerateFullSolution(zebra);

    var (csp, categories) = zebra.Build();
    
    var allClues = ZebraGroundTruthGenerator.GenerateEntireCluePool(solution, categories);
    
    Console.WriteLine($"number of clues altogether: {allClues.Count}");