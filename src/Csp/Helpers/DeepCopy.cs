using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Helpers;

public static class DeepCopy
{
    public static IDictionary<IVariable, IDomain<T>> Domains<T>(IDictionary<IVariable, IDomain<T>> domains)
    {
        var newDict = new Dictionary<IVariable, IDomain<T>>();
        foreach (var v in domains.Keys)
        {
            newDict[v] = new Domain<T>(domains[v].Values.ToList());
        }

        return newDict;
    }
    
    public static IDictionary<IOrderedVariable, IDomain<T>> ToOrderedDomains<T, TVar>(IDictionary<TVar, IDomain<T>> domains)
    {
        var newDict = new Dictionary<IOrderedVariable, IDomain<T>>();
        foreach (var v in domains.Keys)
        {
            if (v is not IOrderedVariable orderedV)
                throw new ArgumentOutOfRangeException(nameof(domains), "All variables must be ordered variables");
            
            newDict[orderedV] = new Domain<T>(domains[v].Values.ToList());
        }

        return newDict;
    }
}