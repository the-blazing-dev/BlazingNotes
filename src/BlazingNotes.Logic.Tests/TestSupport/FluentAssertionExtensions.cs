using FluentAssertions.Primitives;

namespace BlazingNotes.Logic.Tests.TestSupport;

public static class FluentAssertionExtensions
{
    public static void BeCloseToNow(this DateTimeAssertions input, int allowedMillisecondsDiff = 1000)
    {
        input.BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(allowedMillisecondsDiff));
    }

    public static void BeCloseToNow(this NullableDateTimeAssertions input, int allowedMillisecondsDiff = 1000)
    {
        input.BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(allowedMillisecondsDiff));
    }

    public static void BeCloseTo(this DateTimeAssertions input, DateTime expected, int allowedMillisecondsDiff = 1000)
    {
        input.BeCloseTo(expected, TimeSpan.FromMilliseconds(allowedMillisecondsDiff));
    }

    public static void BeCloseTo(this NullableDateTimeAssertions input, DateTime expected,
        int allowedMillisecondsDiff = 1000)
    {
        input.BeCloseTo(expected, TimeSpan.FromMilliseconds(allowedMillisecondsDiff));
    }
}