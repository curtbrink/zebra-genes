using Csp.Builders;
using Csp.Gac;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;
using Csp.Types.Polyomino;

namespace CspTests;

public class PolyTests
{
    [Fact]
    public void TestPoly()
    {
        var pA = new Polyomino("A", "###\n##.\n##.".Split('\n').ToList());
        var pC = new Polyomino("C", ".##\n###\n##.".Split('\n').ToList());
        var pE = new Polyomino("E", "###\n#..\n###".Split('\n').ToList());
        var pF = new Polyomino("F", "###\n.#.\n###".Split('\n').ToList());
        
        var pBuild = PolyominoBuilder.Create(12, 5)
            .AddPolyomino(pA, 1)
            .AddPolyomino(pC, 1)
            .AddPolyomino(pE, 3)
            .AddPolyomino(pF, 2)
            .Build();

        var workingDomains = new Dictionary<IVariable, IDomain<Placement>>(pBuild.Domains);
        var workingConstraints = pBuild.Constraints.ToList();
        
        var domainsToBacktrack = Gac.Run(workingConstraints, workingDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(workingConstraints, domainsToBacktrack);
    }
}