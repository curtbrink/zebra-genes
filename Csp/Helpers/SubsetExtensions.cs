namespace Csp.Helpers;

public static class SubsetExtensions
{
    // not-super-efficient linq heavy recursive function but it gets the job done
    public static List<List<T>> GetAllSubsetsOfSize<T>(this List<T> list, int size)
    {
        if (size > list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be smaller than list.Count");
        }
        
        // e.g. list is [1, 2, 3, 4], and we want size 2.
        // the list we return at this level comprises two "sets of subsets":
        // subsets that include list[0] i.e. 1 -> prepend 1 to all the subsets of [2, 3, 4] of size (size-1)=1.
        // subsets that do not include list[0] -> all the subsets of [2, 3, 4] of size 2.
        
        // "base case": [1, 2, 3, 4] size 1 -> [...1 + subsets of [2, 3, 4] of size 0, ...subsets of [2, 3, 4] of size 1]
        // so true base case is size = 0.
        // "base case": [1, 2, 3] size 3 -> [...1 + subsets of [2, 3] of size 2, ...subsets of [2, 3] of size 3]
        // so another true base case is size = list size => return list.

        if (size == 0)
        {
            // only one way to make a subset of size 0 from any list!
            return [[]];
        }

        if (size == list.Count)
        {
            // only one way to make a subset of the same size as the list!
            return [list];
        }

        
        List<List<T>> result = [];

        var firstElement = list[0];
        var sublist = list[1..];

        var subsetsContainingFirst = sublist.GetAllSubsetsOfSize(size - 1)
            .Select(subset => subset.Prepend(firstElement).ToList()).ToList();
        var subsetsWithoutFirst = sublist.GetAllSubsetsOfSize(size);
        result.AddRange(subsetsContainingFirst);
        result.AddRange(subsetsWithoutFirst);
        return result;
    }
}