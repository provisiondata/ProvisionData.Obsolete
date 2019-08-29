using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ProvisionData
{
    public class SystemDateTimeTests
    {
        [Fact]
        public void SystemDateTime_can_auto_reset()
        {
            using (SystemDateTime.DateIs(1974, 10, 16))
            {
                SystemDateTime.UtcNow.Should().Be(new DateTime(1974, 10, 16));
            }
            SystemDateTime.UtcNow.Should().NotBe(new DateTime(1974, 10, 16));
        }

        [Fact]
        public async Task SystemDateTime_DateIs_prevents_conflicting_tests()
        {
            var tasks = new Task[5];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ThreadProc(new DateTime(1974, 10, i + 1), i);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            SystemDateTime.UtcNow.Date.Should().Be(DateTime.UtcNow.Date);
        }

        private async Task ThreadProc(DateTime dateTime, Int32 thread)
        {
            using (SystemDateTime.DateIs(dateTime))
            {
                for (var i = 0; i < 5; i++)
                {
                    SystemDateTime.UtcNow.Should().Be(dateTime);
                    await Task.Delay(50).ConfigureAwait(false);
                }
            }
        }
    }
}
