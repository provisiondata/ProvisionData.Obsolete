using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace ProvisionData
{
    public class EnumerableComparerTest
    {
        private readonly Func<String, Object> _convert = str =>
        {
            try { return Int32.Parse(str); }
            catch { return str; }
        };

        [Fact]
        public void Test()
        {
            var unsorted = new List<String>() { "z24", "z2", "z15", "z1", "New York", "z3", "z20", "z5", "Newark", "z11", "z 21", "z22", "NewYork" };

            var sorted = unsorted.OrderBy(str => Regex.Split(str.Replace(" ", ""), "([0-9]+)").Select(_convert), new EnumerableComparer<Object>());

            sorted.Should().ContainInOrder("Newark", "New York", "NewYork", "z1", "z2", "z3", "z5", "z11", "z15", "z20", "z 21", "z22", "z24");
        }
    }
}

