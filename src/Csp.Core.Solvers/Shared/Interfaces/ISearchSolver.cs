namespace Csp.Core.Solvers.Shared.Interfaces;

public interface ISearchSolver<T> : ISolver<T>
{
    public void AddPruner(IPruner<ISearchState<T>, T> pruner);
}