using Csp.Builders;
using Csp.Impl;
using Csp.Impl.Constraints.Selfref;
using Csp.Impl.Constraints.Zebra;
using Csp.Interfaces;

namespace CspTests;

public class ZebraTests
{
    [Fact]
    public void TestZebraCsp()
    {
        var domain = new Domain<int>([1, 2]);

        var categories = new Dictionary<string, List<IVariable>>();
        categories["Name"] = [new BaseVariable("Jed"), new BaseVariable("Eddy")];
        categories["Pet"] = [new BaseVariable("Dinosaur"), new BaseVariable("Flea")];

        List<IConstraint<int>> constraints =
        [
            new AllDifferentConstraint(categories["Name"], "Name"),
            new AllDifferentConstraint(categories["Pet"], "Pet"),
            new EqualsConstraint(categories["Name"][0], categories["Pet"][0]), // Jed pos == dinosaur pos
            new OneOfConstraint(categories["Pet"][1], [2]) // Flea is in position 2
        ];
        
        // immutable csp
        var csp = new UniformDomainCsp<int>([.. categories["Name"], .. categories["Pet"]], domain, constraints);
        
        // working objects
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<int>>(csp.Domains);
        var solvedDomains = Gac.Run(runConstraints, runDomains);

        foreach (var k in solvedDomains.Keys)
        {
            var kDomain = solvedDomains[k].Values;
            Assert.Single(kDomain);
        }
    }

    [Fact]
    public void TestBuilder()
    {
        var (zebra, _) = ZebraBuilder.Create(3)
            .AddCategory("Name", ["Riktus", "Psyja", "Arkturus"])
            .AddCategory("Fursona", ["Tiger", "Fox", "Snepfox"])
            .AddConstraint("Riktus").IsAdjacentTo("Snepfox")
            .AddConstraint("Fox").IsAfter("Riktus")
            .AddConstraint("Arkturus").MustBeInPosition([1, 3])
            .AddConstraint("Arkturus").Is("Fox")
            .AddConstraint("Riktus").MustBeInPosition([1, 3])
            .Build();

        var workingDomains = new Dictionary<IVariable, IDomain<int>>(zebra.Domains);
        List<IConstraint<int>> workingConstraints = [..zebra.Constraints];

        var solvedDomains = Gac.Run(workingConstraints, workingDomains);

        Assert.True(true);
    }
    
    [Fact]
    public void TestZebraBuilder1()
    {
        var (zb, _) = ZebraBuilder.Create(4)
            .AddCategory("Shirt", ["black", "green", "orange", "purple"])
            .AddCategory("Name", ["Carl", "Frank", "Henry", "Victor"])
            .AddCategory("Flower", ["daisy", "fern", "hydrangea", "lily"])
            .AddCategory("Arrangement", ["cascading", "even", "layered", "row"])
            .AddConstraint("even").MustBeInPosition(3)
            .AddConstraint("daisy").MustBeInPosition(2)
            .AddConstraint("orange").IsBefore("Frank")
            .AddConstraint("Henry").Has("hydrangea")
            .AddConstraint("Victor").Is("black")
            .AddConstraint("green").Is("cascading")
            .AddConstraint("Frank").Has("fern")
            .AddConstraint("Frank").MustBeInPosition([1, 4])
            .AddConstraint("black").Is("row")
            .AddConstraint("orange").Is("layered")
            .AddConstraint("lily").MustBeInPosition(3)
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<int>>(zb.Domains);
        var workingConstraints = zb.Constraints.ToList();
        
        var domainsToBacktrack = Gac.Run(workingConstraints, workingDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(workingConstraints, domainsToBacktrack);
    }
    
    [Fact]
    public void TestZebraBuilder5()
    {
        var (zb, _) = ZebraBuilder.Create(5)
            .AddCategory("Shirt", ["black", "pink", "red", "white", "yellow"])
            .AddCategory("Name", ["Brian", "Jonathan", "Mark", "Samuel", "Ulysses"])
            .AddCategory("Symptom", ["pain", "dizziness", "fever", "throat", "stomach"])
            .AddCategory("Waiting", ["10min", "25min", "30min", "40min", "45min"])
            .AddCategory("Phone", ["Astrus", "Motoralo", "Sumsang", "Toshina", "Xiamio"])
            .AddCategory("Profession", ["baker", "DJ", "florist", "pharmacist", "cop"])
            .AddConstraint("30min").IsAdjacentTo("DJ")
            .AddConstraint("Motoralo").IsAdjacentTo("DJ")
            .AddConstraint("40min").MustBeInPosition([1, 5])
            .AddConstraint("Mark").Has("Xiamio")
            .AddConstraint("red").IsBefore("Brian")
            .AddConstraint("fever").IsImmediatelyAfter("red")
            .AddConstraint("Jonathan").Has("throat")
            .AddConstraint("cop").IsImmediatelyBefore("florist")
            .AddConstraint("DJ").IsAdjacentTo("yellow")
            .AddConstraint("dizziness").IsAdjacentTo("yellow")
            .AddConstraint("stomach").IsImmediatelyBefore("Motoralo")
            .AddConstraint("25min").IsImmediatelyBefore("throat")
            .AddConstraint("Brian").Has("fever")
            .AddConstraint("Astrus").MustBeInPosition([1, 5])
            .AddConstraint("pain").Is("40min")
            .AddConstraint("florist").IsAdjacentTo("pharmacist")
            .AddConstraint("dizziness").IsImmediatelyAfter("Jonathan")
            .AddConstraint("Ulysses").Has("dizziness")
            .AddConstraint("10min").IsAfter("red")
            .AddConstraint("Mark").IsAfter("pink")
            .AddConstraint("Mark").IsBefore("black")
            .AddConstraint("black").IsBefore("yellow")
            .AddConstraint("Toshina").Is("10min")
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<int>>(zb.Domains);
        var workingConstraints = zb.Constraints.ToList();
        
        var domainsToBacktrack = Gac.Run(workingConstraints, workingDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(workingConstraints, domainsToBacktrack);
    }

    [Fact]
    public void TestZebraBuilder6()
    {
        var (z, _) = ZebraBuilder.Create(4)
            .AddCategory("Shirt", ["green", "orange", "purple", "white"])
            .AddCategory("Name", ["George", "Oliver", "Walter", "Xavier"])
            .AddCategory("Size", ["20gal", "25gal", "35gal", "45gal"])
            .AddCategory("Shape", ["cylinder", "hexagon", "rectangle", "square"])
            .AddConstraint("rectangle").MustBeInPosition(2)
            .AddConstraint("square").MustBeInPosition(1)
            .AddConstraint("35gal").MustBeInPosition(3)
            .AddConstraint("orange").IsBefore("George")
            .AddConstraint("white").MustBeInPosition(1)
            .AddConstraint("35gal").IsAdjacentTo("hexagon")
            .AddConstraint("Oliver").IsAdjacentTo("45gal")
            .AddConstraint("green").Is("35gal")
            .AddConstraint("Oliver").MustBeInPosition([1, 4])
            .AddConstraint("20gal").MustBeInPosition(4)
            .AddConstraint("Walter").Is("hexagon")
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<int>>(z.Domains);
        var workingConstraints = z.Constraints.ToList();
        
        var domainsToBacktrack = Gac.Run(workingConstraints, workingDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(workingConstraints, domainsToBacktrack);
    }
}