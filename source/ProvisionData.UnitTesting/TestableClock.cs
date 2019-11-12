/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.UnitTesting
{
    using System;
    using NodaTime;
    using NodaTime.Testing;
    using ProvisionData.Extensions;

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
