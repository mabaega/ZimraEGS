using Newtonsoft.Json;
using System.Globalization;

namespace ZimraEGS.ApiClient.Helpers
{
    public class LocalDateTimeConverter : JsonConverter
    {
        private static readonly string[] SupportedDateFormats = new[] {
        "yyyy-MM-ddTHH:mm:ss",     // Local time without 'Z'
        "yyyy-MM-dd h:mm:ss tt",   // Local time with AM/PM
        "yyyy-MM-ddTHH:mm:sszzz",  // ISO with timezone offset
        "yyyy-MM-ddTHH:mm:ss",
        "MM/dd/yyyy h:mm:ss tt",   // US-style AM/PM format
        "MM/dd/yyyy HH:mm:ss",     // US-style 24-hour format
        "dd-MM-yyyy HH:mm:ss",     // European-style 24-hour format
        "yyyy-MM-dd HH:mm:ss",
        "yyyy/MM/dd HH:mm:ss"      // Alternative format
    };

        // Serialize DateTimeOffset to local time (yyyy-MM-ddTHH:mm:ss)
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                // Always serialize as local time in "yyyy-MM-ddTHH:mm:ss" format
                writer.WriteValue(dateTimeOffset.LocalDateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNull();
            }
        }

        // Deserialize the date from multiple formats to local time
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            // Log the input string before processing
            //Console.WriteLine($"Received Date String: {value}");

            // Handle the local time date formats
            if (DateTime.TryParseExact(value, SupportedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var localDateTime))
            {
                // Log the local date parsed
                //Console.WriteLine($"Parsed Local DateTime: {localDateTime}");

                // Return DateTimeOffset in local time
                return new DateTimeOffset(localDateTime, TimeZoneInfo.Local.GetUtcOffset(localDateTime));
            }

            // If parsing fails, throw an exception
            throw new JsonSerializationException($"Unable to parse date: {value}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTimeOffset);
        }
    }
}