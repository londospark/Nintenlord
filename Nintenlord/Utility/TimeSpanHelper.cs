using System;

namespace Nintenlord.Utility
{
    public static class TimeSpanHelper
    {
        public static float FloatDivide(this TimeSpan timeSpan, TimeSpan divideWith) => ((float)timeSpan.Ticks) / ((float)divideWith.Ticks);

        public static TimeSpan Divide(this TimeSpan timeSpan, int divideWith) => TimeSpan.FromTicks(timeSpan.Ticks / divideWith);

        public static int Divide(this TimeSpan timeSpan, TimeSpan divideWith) => (int)(timeSpan.Ticks / divideWith.Ticks);

        public static TimeSpan Multiply(this TimeSpan timeSpan, int value) => TimeSpan.FromTicks(timeSpan.Ticks * value);

        public static TimeSpan Multiply(this TimeSpan timeSpan, float value) => TimeSpan.FromMilliseconds(timeSpan.TotalMilliseconds * value);

        public static TimeSpan Part(this TimeSpan timeSpan, int toMultiply, int toDivide) => TimeSpan.FromTicks(timeSpan.Ticks * toMultiply / toDivide);

        public static TimeSpan Lerp(TimeSpan beginning, TimeSpan end, float i) => beginning + Multiply((end - beginning), i);
    }
}
