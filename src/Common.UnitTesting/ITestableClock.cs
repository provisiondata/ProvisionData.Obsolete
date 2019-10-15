using System;
using NodaTime;

namespace ProvisionData.UnitTesting
{
    public interface ITestableClock : IClock
    {
        void DateIs(Instant instant);
        void DateIs(Int32 year, Int32 monthOfYear, Int32 dayOfMonth);
        void DateIs(Int32 year, Int32 monthOfYear, Int32 dayOfMonth, Int32 hourOfDay, Int32 minuteOfHour);
        void DateIs(Int32 year, Int32 monthOfYear, Int32 dayOfMonth, Int32 hourOfDay, Int32 minuteOfHour, Int32 secondOfMinute);
        void TimeIs(Int32 hourOfDay, Int32 minuteOfHour);
        void TimeIs(Int32 hourOfDay, Int32 minuteOfHour, Int32 secondOfMinute);
        void Reset();
    }
}
