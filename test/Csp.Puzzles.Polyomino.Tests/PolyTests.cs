using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Polyomino.Builders;
using Csp.Puzzles.Polyomino.Models;

namespace Csp.Puzzles.Polyomino.Tests;

public class PolyTests
{
    [Fact]
    public void TestPoly()
    {
        var pA = new Models.Polyomino("A", "###\n##.\n##.".Split('\n').ToList());
        var pC = new Models.Polyomino("C", ".##\n###\n##.".Split('\n').ToList());
        var pE = new Models.Polyomino("E", "###\n#..\n###".Split('\n').ToList());
        var pF = new Models.Polyomino("F", "###\n.#.\n###".Split('\n').ToList());
        
        var pBuild = PolyominoBuilder.Create(12, 5)
            .AddPolyomino(pA, 1)
            .AddPolyomino(pC, 1)
            .AddPolyomino(pE, 3)
            .AddPolyomino(pF, 2)
            .Build();

        var workingDomains = new Dictionary<IVariable, IDomain<Placement>>(pBuild.Domains);
        var workingConstraints = pBuild.Constraints.ToList();
    }
}