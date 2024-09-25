using TimeZoneConverter;

namespace ssptb.pe.tdlt.storage.common.Helpers;
public static class DatetimeHelper
{
    public const string TimeZone = "SA Pacific Standard Time";

    /// <summary>
    /// Devuelve la hora actual en la zona horaria configurada.
    /// </summary>
    public static DateTime Now()
    {
        TimeZoneInfo zoneInfo = TZConvert.GetTimeZoneInfo(TimeZone);
        return TimeZoneInfo.ConvertTime(DateTime.UtcNow, zoneInfo);
    }

    /// <summary>
    /// Devuelve la hora actual en UTC, ajustada desde la zona horaria configurada.
    /// </summary>
    public static DateTime NowUtc()
    {
        return DateTime.UtcNow; // Se asegura de devolver siempre en UTC para la base de datos
    }
}
