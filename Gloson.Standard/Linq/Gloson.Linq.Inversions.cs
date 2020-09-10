using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions (Inversions Count)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Algorithm

    static int CoreMergeSort<T>(T[] arr, T[] temp, int left, int right, IComparer<T> comparer) {
      int mid;
      int result = 0;

      if (right > left) {
        mid = (right + left) / 2;

        result += CoreMergeSort(arr, temp, left, mid, comparer);
        result += CoreMergeSort(arr, temp, mid + 1, right, comparer);
        result += CoreMerge(arr, temp, left, mid + 1, right, comparer);
      }

      return result;
    }

    static int CoreMerge<T>(T[] arr, T[] temp, int left, int mid, int right, IComparer<T> comparer) {
      int i, j, k;
      int result = 0;

      i = left;
      j = mid;
      k = left;

      while (i <= mid - 1 && j <= right)
        if (comparer.Compare(arr[i], arr[j]) <= 0)
          temp[k++] = arr[i++];
        else {
          temp[k++] = arr[j++];
          result += mid - i;
        }

      while (i <= mid - 1)
        temp[k++] = arr[i++];

      while (j <= right)
        temp[k++] = arr[j++];

      for (i = left; i <= right; i++)
        arr[i] = temp[i];

      return result;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Count inversions
    /// </summary>
    public static int InversionsCount<T>(this IEnumerable<T> source, IComparer<T> comparer = null) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == comparer)
        comparer = Comparer<T>.Default;

      if (null == comparer)
        throw new ArgumentNullException(nameof(comparer), $"type {typeof(T).Name} doesn't have default comparer.");

      T[] arr = source.ToArray();

      if (arr.Length <= 1)
        return 0;

      T[] temp = new T[arr.Length];

      return CoreMergeSort(arr, temp, 0, arr.Length - 1, comparer);
    }

    #endregion Public
  }

}
