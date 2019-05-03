using Shouldly;
using Xunit;

namespace ProvisionData.Net
{
    public class EndPointTests
    {
        [Fact]
        public void EndPoints_are_equatable()
        {
            new EndPoint("10.10.10.10").Equals(new EndPoint("10.10.10.10")).ShouldBe(true);
            new EndPoint("10.10.10.10").Equals(new EndPoint("10.10.10.11")).ShouldBe(false);

            new EndPoint("10.10.10.10", 10).Equals(new EndPoint("10.10.10.10", 10)).ShouldBe(true);
            new EndPoint("10.10.10.10", 10).Equals(new EndPoint("10.10.10.10", 11)).ShouldBe(false);

            (new EndPoint("10.10.10.10") == new EndPoint("10.10.10.10")).ShouldBe(true);
            (new EndPoint("10.10.10.10") != new EndPoint("10.10.10.11")).ShouldBe(true);

            (new EndPoint("10.10.10.10", 10) == new EndPoint("10.10.10.10", 10)).ShouldBe(true);
            (new EndPoint("10.10.10.10", 10) != new EndPoint("10.10.10.10", 11)).ShouldBe(true);
        }

        [Fact]
        public void EndPoints_with_same_Address_and_one_with_Port_0_are_equal()
        {
            new EndPoint("10.10.10.10", 0).Equals(new EndPoint("10.10.10.10", 1000)).ShouldBe(true);
        }

        [Fact]
        public void EndPoints_are_compareable()
        {
            new EndPoint("110.10.10.10").ShouldBeLessThan(new EndPoint("120.10.10.10"));
            new EndPoint("110.10.10.10").ShouldBeGreaterThan(new EndPoint("100.10.10.10"));

            new EndPoint("110.10.10.10").ShouldBeLessThan(new EndPoint("110.10.10.10", 100));
            new EndPoint("110.10.10.10", 100).ShouldBeGreaterThan(new EndPoint("110.10.10.10"));

            new EndPoint("110.10.10.10", 1000).ShouldBeLessThan(new EndPoint("110.10.10.10", 2000));
            new EndPoint("110.10.10.10", 2000).ShouldBeGreaterThan(new EndPoint("110.10.10.10", 1000));
        }
    }
}
