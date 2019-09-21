using FluentAssertions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete
namespace ProvisionData
{
    public class SystemDateTimeTests
    {
        [Fact]
        public void SystemDateTime_can_override_the_current_date()
        {
            var expected = new DateTime(1974, 10, 16);

            SystemDateTime.DateIs(expected);
            SystemDateTime.UtcNow.Should().Be(expected);

            SystemDateTime.Reset();
            SystemDateTime.UtcNow.Should().NotBe(expected);
        }

        [Fact]
        public void SystemDateTime_can_auto_reset()
        {
            using (var timeProvider = SystemDateTime.GetTestTimeProvider())
            {
                timeProvider.DateIs(1974, 10, 16);
                SystemDateTime.UtcNow.Should().Be(new DateTime(1974, 10, 16));
            }
            SystemDateTime.UtcNow.Should().NotBe(new DateTime(1974, 10, 16));
        }

        [Fact]
        public async Task SystemDateTime_DateIs_prevents_conflicting_tests()
        {
            var tasks = new Task[12];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ThreadProc(new DateTime(i + 1001, i + 1, i + 1), i + 1);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            SystemDateTime.UtcNow.Date.Should().Be(DateTime.UtcNow.Date);

            static async Task ThreadProc(DateTime dateTime, Int32 worker)
            {
                using var timeProvider = SystemDateTime.GetTestTimeProvider();

                Debug.WriteLine($"Worker {worker} wants exclusive access to the clock!");
                timeProvider.DateIs(dateTime);
                Debug.WriteLine($"Worker {worker} got exclusive access to the clock!");

                for (var i = 0; i < 5; i++)
                {
                    Debug.WriteLine($"Worker {worker} date is: {SystemDateTime.UtcNow}");
                    SystemDateTime.UtcNow.Should().Be(dateTime);
                    await Task.Delay(25).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public void SystemDateTime_DateIs_can_reenter()
        {
            using var timeProvider = SystemDateTime.GetTestTimeProvider();

            timeProvider.DateIs(1975, 11, 11);
            SystemDateTime.UtcNow.Should().Be(new DateTime(1975, 11, 11));

            timeProvider.DateIs(2007, 01, 15);
            SystemDateTime.UtcNow.Should().Be(new DateTime(2007, 01, 15));
        }
    }
}
