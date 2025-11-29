namespace Csp.Helpers;

public static class CartesianProductExtensions
{
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> result = [[]];

        foreach (var sequence in sequences)
        {
            result = (from seq in result from item in sequence select new List<T>(seq) { item }).ToList();
        }

        return result;
    }
}