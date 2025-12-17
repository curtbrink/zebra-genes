using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Core.Solvers.Shared.Models;
using Csp.Core.Solvers.Shared.Models.Interfaces;

namespace Csp.Core.Solvers.Shared.Helpers;

public static class DomainExtensions
{
    public static string PrettyPrint<T>(this IMutableDomainStore<T> domainStore)
    {
        var domains = domainStore.GetAllDomains();
        var domainStrings = domains.Keys.Select(k => (k, domains[k].Values.ToList()))
            .ToDictionary(p => p.k, p => p.Item2);
        var domainPrettied =
            string.Join(" ",
                domainStrings.Keys.Select(k => $"{k.Name}={{{string.Join("", domainStrings[k])}}}"));
        return domainPrettied;
    }

    public static bool IsSolved<T>(this IMutableDomainStore<T> domainStore) =>
        domainStore.GetAllDomains().All(kvp => kvp.Value.Values.Count == 1);

    public static IDictionary<IVariable, T>? ToSolvedDomains<T>(this IMutableDomainStore<T> domainStore)
    {
        if (!IsSolved(domainStore)) return null;
        
        var fullSolution = new Dictionary<IVariable, T>();
        foreach (var solvedDomain in domainStore.GetAllDomains())
        {
            fullSolution[solvedDomain.Key] = solvedDomain.Value.Values.First();
        }

        return fullSolution;
    }

    public static IMutableDomainStore<T> Clone<T>(this IMutableDomainStore<T> domainStore)
    {
        IDictionary<IVariable, IMutableDomain<T>> dict = new Dictionary<IVariable, IMutableDomain<T>>();
        foreach (var d in domainStore.GetAllDomains())
        {
            dict[d.Key] = new MutableDomain<T>(d.Value);
        }
        return new MutableDomainStore<T>(dict);
    }
}