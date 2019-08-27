using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace ProvisionData
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void GetAllProperties_returns_inherited_properties()
        {
            TypeExtensions.GetAllProperties(typeof(Foo)).Count().ShouldBe(1);
            TypeExtensions.GetAllProperties(typeof(Bar)).Count().ShouldBe(2);
        }

        private class Foo
        {
            public String Name { get; set; }
        }

        private class Bar : Foo
        {
            public Int32 Age { get; set; }
        }
    }
}
