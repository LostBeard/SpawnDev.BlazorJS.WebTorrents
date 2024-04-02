using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public static class DateTimeExtensions
    {
        public static long GetEpochTime(this DateTime oDate)
        {
            var oOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan oDiff = oDate.ToUniversalTime() - oOrigin;
            return (long)Math.Floor(oDiff.TotalMilliseconds);
        }
        public static DateTime EpochTimeToDateTime(this long iTicks, bool toLocal = true)
        {
            var oOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            oOrigin = oOrigin.AddMilliseconds(iTicks);
            if (toLocal)
            {
                return oOrigin.ToLocalTime();
            }
            else
            {
                return oOrigin;
            }
        }
    }
    public class DateTimeEpochConverter : JsonConverter<DateTime>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DateTime) == typeToConvert;
        }
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var v = JsonSerializer.Deserialize<long>(ref reader);
            var ret = v.EpochTimeToDateTime();
            return ret;
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var time = value.GetEpochTime();
            JsonSerializer.Serialize(writer, time);
        }
    }
}
