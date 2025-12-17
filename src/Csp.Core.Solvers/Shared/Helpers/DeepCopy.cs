using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Core.Solvers.Shared.Models;
using Csp.Core.Solvers.Shared.Models.Interfaces;

namespace Csp.Core.Solvers.Shared.Helpers;

public static class DeepCopy
{
    public static IDictionary<IVariable, IMutableDomain<T>> ToMutableDomains<T>(
        IReadOnlyDictionary<IVariable, IDomain<T>> domains)
    {
        var newDict = new Dictionary<IVariable, IMutableDomain<T>>();
        foreach (var (v, domain) in domains)
        {
            newDict[v] = ToMutableDomain(domain);
        }

        return newDict;
    }
    
    public static IMutableDomain<T> ToMutableDomain<T>(IDomain<T> domain) => new MutableDomain<T>(domain);
    
    public static IDictionary<TVar, IDomain<T>> Domains<T, TVar>(IDictionary<TVar, IDomain<T>> domains) where TVar : IVariable
    {
        var newDict = new Dictionary<TVar, IDomain<T>>();
        foreach (var v in domains.Keys)
        {
            newDict[v] = new ImmutableDomain<T>(domains[v].Values.ToList());
        }

        return newDict;
    }
    
    public static IDictionary<TVar, IDomain<T>> DowncastDomains<T, TVar>(IDictionary<IVariable, IDomain<T>> domains) where TVar : IVariable
    {
        var newDict = new Dictionary<TVar, IDomain<T>>();
        foreach (var (v, domain) in domains)
        {
            if (v is not TVar orderedV)
                throw new ArgumentOutOfRangeException(nameof(domains), $"All variables must be of type {typeof(TVar).Name}");
            
            newDict[orderedV] = new ImmutableDomain<T>(domain.Values.ToList());
        }

        return newDict;
    }
}