using System;
using System.Collections.Generic;

namespace Implementation
{
  internal static class Extensions
  {
    public static void AddIfNotAlreadyInCollection<T>(this ICollection<T> collection, T itemToAdd)
    {
      if (!collection.Contains(itemToAdd))
      {
        collection.Add(itemToAdd);
      }
    }

    public static string[] Split(this string stringToSplit, string stringToSplitBy)
    {
      return stringToSplit.Split(new[] { stringToSplitBy }, StringSplitOptions.None);
    }
  }
}