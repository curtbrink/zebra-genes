// namespace Csp.Puzzles.Zebra.Constraints;
//
// public class AllDifferentConstraint(IReadOnlyList<IVariable> scope, string category) : IConstraint<int>
// {
//     public string Name => $"AllDifferent({category})";
//     public string Description => $"Uniqueness for {category}";
//
//     public IReadOnlyList<IVariable> Scope { get; } = scope;
//     
//     public bool IsSatisfiable(IVariable? v, int val, IDictionary<IVariable, IDomain<int>> domains)
//     {
//         // impossible if any other variable in v's category is forced to val
//         return Scope.Where(v2 => v2 != v).Where(v2 => domains[v2].Values.Count <= 1)
//             .All(v2 => !domains[v2].Values.Contains(val));
//     }
// }