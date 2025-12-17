using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Core.Solvers.Shared.Models.Interfaces;

namespace Csp.Core.Solvers.Shared.Models;

public class MutableDomainStore<T> : IMutableDomainStore<T>
{
    private readonly IDictionary<IVariable, IMutableDomain<T>> _domains;

    public MutableDomainStore(IDictionary<IVariable, IMutableDomain<T>> domains)
    {
        _domains = domains;
    }

    public MutableDomainStore(IReadOnlyDictionary<IVariable, IMutableDomain<T>> domains)
    {
        _domains = new Dictionary<IVariable, IMutableDomain<T>>(domains);
    }

    public MutableDomainStore(IReadOnlyDictionary<IVariable, IDomain<T>> domains)
    {
        _domains = new Dictionary<IVariable, IMutableDomain<T>>();
        foreach (var (key, value) in domains)
        {
            _domains[key] = new MutableDomain<T>(value);
        }
    }
    
    public IDomain<T> GetDomain(IVariable variable) => _domains[variable];

    public IMutableDomain<T> GetMutableDomain(IVariable variable) => _domains[variable];
    
    public void SetMutableDomain(IVariable variable, IMutableDomain<T> domain) => _domains[variable] = domain;

    public IDictionary<IVariable, IMutableDomain<T>> GetAllDomains() => _domains;

    IDictionary<IVariable, IDomain<T>> IDomainStore<T>.GetAllDomains() =>
        _domains.ToDictionary(kvp => kvp.Key, IDomain<T> (kvp) => kvp.Value);
}