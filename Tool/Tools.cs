
using System.Collections.Generic;
using UnityEngine;

public class Tools {
    public static int UniqueRandomInt(ICollection<int> l, int min, int max) {
        var retVal = Random.Range(min, max);

        while (l.Contains(retVal)) {
            retVal = Random.Range(min, max);
        }

        return retVal;
    }
}
