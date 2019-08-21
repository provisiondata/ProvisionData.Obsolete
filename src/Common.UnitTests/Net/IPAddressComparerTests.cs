using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ProvisionData.Net
{
    public class IPAddressComparerTests
    {
        [Fact]
        public void IPAddressComparer_works_as_expected()
        {
            var a = new[] { "204.63.46.18", "110.249.212.46", "51.15.144.131", "8.8.8.8", "204.63.42.225", "193.200.164.173" };
            var b = new[] { "8.8.8.8", "51.15.144.131", "110.249.212.46", "193.200.164.173", "204.63.42.225", "204.63.46.18" };

            var list = new List<String>(a);

            list.Sort(new IPAddressComparer());

            list.Should().ContainInOrder(b);
        }
    }
}
