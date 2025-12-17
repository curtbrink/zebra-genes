using Csp.Core.Solvers.Backtrack;
using Csp.Core.Solvers.Gac;
using Csp.Core.Solvers.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Csp.Core.Solvers.Shared.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddBacktrackingGac<T>(this IServiceCollection services)
    {
        services.AddSingleton<IInferenceSolver<T>, GacSolver<T>>();
        services.AddSingleton<ISearchSolver<T>, BacktrackSolver<T>>();
        return services;
    }
}