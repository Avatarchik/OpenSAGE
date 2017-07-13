﻿using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace OpenZH.Data.Tests
{
    internal static class AssertUtility
    {
        // XUnit's built-in collection assert doesn't show *where* the difference is, if the test fails.
        public static void Equal<T>(T[] array1, T[] array2)
            where T : struct
        {
            Assert.Equal(array1.Length, array2.Length);

            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < array1.Length; i++)
            {
                if (!comparer.Equals(array1[i], array2[i]))
                {
                    throw new AssertActualExpectedException(array1[i], array2[i], $"Different values at index {i}");
                }
            }
        }
    }
}