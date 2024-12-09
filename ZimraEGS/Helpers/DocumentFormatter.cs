using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ZimraEGS.Helpers
{
    public static class DocumentFormatter
    {
        public static string SerializeObject<T>(T value)
        {
            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            var jsonSerializer = JsonSerializer.CreateDefault();
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonWriter.IndentChar = ' ';
                jsonWriter.Indentation = 4;

                jsonSerializer.Serialize(jsonWriter, value, typeof(T));
            }

            return sw.ToString();
        }
        public static string ExcludeClearanceInvoice(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);
            jsonObject.Property("clearedInvoice")?.Remove();
            return jsonObject.ToString();
        }
    }

}
