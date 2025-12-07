using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Helpers;

public static class DeepCopy
{
    public static IDictionary<TVar, IDomain<T>> Domains<T, TVar>(IDictionary<TVar, IDomain<T>> domains) where TVar : IVariable
    {
        var newDict = new Dictionary<TVar, IDomain<T>>();
        foreach (var v in domains.Keys)
        {
            newDict[v] = new Domain<T>(domains[v].Values.ToList());
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
            
            newDict[orderedV] = new Domain<T>(domain.Values.ToList());
        }

        return newDict;
    }
}