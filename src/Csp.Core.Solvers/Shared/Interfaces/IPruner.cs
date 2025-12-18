namespace Csp.Core.Solvers.Shared.Interfaces;

public interface IPruner<in T, TDomain> where T : ISearchState<TDomain>
{
    public void SetSearchState(T? searchState);
    public bool ShouldPrune();
}