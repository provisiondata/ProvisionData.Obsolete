using System;
using NodaTime;
using NodaTime.Testing;
using ProvisionData.Extensions;

namespace ProvisionData.UnitTesting
{
    internal class TestableClock : ITestableClock
    {
        private IClock _clock;

        internal TestableClock()
            => _clock = SystemClock.Instance;

        public TestableClock(IClock clock)
            => _clock = clock ?? throw new ArgumentNullException(nameof(clock));

        public void DateIs(Instant instant)
            => _clock = new FakeClock(instant);

        public void DateIs(Int32 year, Int32 monthOfYear, Int32 dayOfMonth)
            => _clock = new FakeClock(Instant.FromUtc(year, monthOfYear, dayOfMonth, 0, 0, 0));

        public void DateIs(Int32 year, Int32 monthOfYear, Int32 dayOfMonth, Int32 hourOfDay, Int32 minuteOfHour)
             => _clock = new FakeClock(Instant.FromUtc(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, 0));

        public void DateIs(Int32 year, Int32 monthOfYear, Int32 dayOfMonth, Int32 hourOfDay, Int32 minuteOfHour, Int32 secondOfMinute)
            => _clock = new FakeClock(Instant.FromUtc(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute));

        public void TimeIs(Int32 hourOfDay, Int32 minuteOfHour)
            => _clock = new FakeClock(Instant.FromUtc(Current.Year, Current.Month, Current.Day, hourOfDay, minuteOfHour, 0));

        public void TimeIs(Int32 hourOfDay, Int32 minuteOfHour, Int32 secondOfMinute)
            => _clock = new FakeClock(Instant.FromUtc(Current.Year, Current.Month, Current.Day, hourOfDay, minuteOfHour, secondOfMinute));

        public Instant GetCurrentInstant()
            => _clock.GetCurrentInstant();

        public void Reset()
            => _clock = SystemClock.Instance;

        private DateTime Current => _clock.GetCurrentInstant().ToDateTimeUtc();
    }
}
