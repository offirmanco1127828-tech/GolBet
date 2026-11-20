namespace GolBet.Services.Helpers;
public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo ColombiaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
    public static DateTime ToColombiaTime(this DateTime utcDate) => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDate, DateTimeKind.Utc), ColombiaZone);
    public static DateTime ToUtcFromColombia(this DateTime localDate)
    {
        var unspecified = DateTime.SpecifyKind(localDate, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, ColombiaZone);
    }
}
