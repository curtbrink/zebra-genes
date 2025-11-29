// See https://aka.ms/new-console-template for more information

using ZebraGenes.Types;

// helper funcs
var newBinaryHint = (HintType type, string cat1, string val1, string cat2, string val2) => new PuzzleHint
{
    Type = type,
    PrimaryValue = new CategoryValue(cat1, val1),
    SecondaryValue = new CategoryValue(cat2, val2)
};
var newPositionHint = (string cat1, string val1, List<int> positions) => new PuzzleHint
{
    Type = HintType.AbsolutePosition,
    PrimaryValue = new CategoryValue(cat1, val1),
    PossiblePositions = positions
};


// test one of the ZebraPuzzles.com easy puzzles
var categories = new Dictionary<string, List<string>>
{
    ["Position"] = ["1", "2", "3", "4", "5"],
    ["Purse"] = ["black", "blue", "orange", "pink", "white"],
    ["Name"] = ["Amy", "Gabrielle", "Hannah", "Jennifer", "Samantha"],
    ["Procedure"] = ["botox", "peel", "freezing", "filler", "teeth"],
    ["Car"] = ["convertible", "coupe", "hatchback", "minivan", "SUV"],
    ["Dog"] = ["Basenji", "Chihuahua", "Lhasa Apso", "Pekingese", "Shih Tzu"],
    ["Profession"] = ["accountant", "cleaner", "dancer", "DJ", "musician"]
};

var puzzleMatrix = new CspMatrix(categories);

// test subset elimination
// all purses start at 1 2 3 4 5
// set white to 3
// all purses at 1 2 4 5
// set black to 1 2
// black adjacent to blue, so (blue, 4/5) get pruned
// black/blue both 1 2
// subset elimination should detect and mark orange/pink 1/2 impossible.
List<PuzzleHint> hintList = [
    newBinaryHint(HintType.Equal, "Position", "3", "Purse", "white"),
    newPositionHint("Purse", "black", [1, 2]),
    newBinaryHint(HintType.Adjacent, "Purse", "black", "Purse", "blue")
];

// List<PuzzleHint> hintList = [
//     newBinaryHint(HintType.AdjacentBefore, "Profession", "cleaner", "Purse", "orange"),
//     newPositionHint("Name", "Amy", [1, 5]),
//     newBinaryHint(HintType.Before, "Purse", "blue", "Profession", "accountant"),
//     newBinaryHint(HintType.Before, "Purse", "blue", "Car", "coupe"),
//     newBinaryHint(HintType.Adjacent, "Procedure", "botox", "Procedure", "peel"),
//     newBinaryHint(HintType.Equal, "Position", "5", "Procedure", "freezing"),
//     newBinaryHint(HintType.AdjacentBefore, "Procedure", "botox", "Car", "convertible"),
//     newBinaryHint(HintType.Equal, "Purse", "orange", "Profession", "dancer"),
//     newBinaryHint(HintType.Adjacent, "Dog", "Chihuahua", "Dog", "Basenji"),
//     newBinaryHint(HintType.Before, "Purse", "black", "Profession", "DJ"),
//     newBinaryHint(HintType.Adjacent, "Purse", "black", "Profession", "musician"),
//     newBinaryHint(HintType.Adjacent, "Name", "Samantha", "Profession", "dancer"),
//     newBinaryHint(HintType.Equal, "Name", "Samantha", "Procedure", "peel"),
//     newBinaryHint(HintType.Before, "Purse", "white", "Dog", "Lhasa Apso"),
//     newBinaryHint(HintType.Before, "Dog", "Lhasa Apso", "Dog", "Pekingese"),
//     newBinaryHint(HintType.Equal, "Car", "minivan", "Dog", "Chihuahua"),
//     newBinaryHint(HintType.Before, "Name", "Gabrielle", "Car", "convertible"),
//     newBinaryHint(HintType.Before, "Car", "convertible", "Car", "hatchback"),
//     newBinaryHint(HintType.Adjacent, "Procedure", "teeth", "Name", "Amy"),
//     newBinaryHint(HintType.Before, "Car", "SUV", "Dog", "Pekingese"),
//     newBinaryHint(HintType.Before, "Dog", "Pekingese", "Name", "Jennifer")
// ];

//try a solve
puzzleMatrix.SolvePuzzle(hintList);

puzzleMatrix.PrintEntities();

// List<int> listOfInts = [1, 2, 3, 4, 5];
// var subsets = listOfInts.GetAllSubsetsOfSize(3);
// foreach (var subset in subsets)
// {
//     Console.WriteLine($"{subset[0]}, {subset[1]}, {subset[2]}");
// }