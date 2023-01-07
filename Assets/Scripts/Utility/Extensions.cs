using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    ///<summary>
    /// Gets random object from list
    ///</summary>
    public static T GetRandomObject<T>(this IReadOnlyList<T> list)
    {
        if (list.Count == 0) throw new UnityException("Can't get random object from empty list");
        return list[Random.Range(0, list.Count)];
    }

    ///<summary>
    /// Gets random object from collection
    /// O(rand_index)
    ///</summary>
    public static T GetRandomObject<T>(this IReadOnlyCollection<T> collection)
    {
        if (collection.Count == 0) throw new UnityException("Can't get random object from empty collection");
        var returnIndex = Random.Range(0, collection.Count);
        var count = 0;
        foreach (var obj in collection)
        {
            if (count == returnIndex) return obj;
            count += 1;
        }

        return default;
    }
}
