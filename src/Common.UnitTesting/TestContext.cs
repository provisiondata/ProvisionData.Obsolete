using NodaTime;

namespace ProvisionData.UnitTesting
{
    public class TestContext
    {
        public TestContext(IClock timeProvider)
        {
            Clock = timeProvider ?? SystemClock.Instance;
        }

        public IClock Clock { get; }
    }
}
