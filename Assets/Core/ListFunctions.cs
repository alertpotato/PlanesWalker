using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListFunctions
{
    public static List<T> RemoveDuplicatesAgainstOtherList<T>(
        List<T> sourceList, 
        List<T> comparisonList)
    {
        return sourceList.Except(comparisonList).ToList();
    }
}