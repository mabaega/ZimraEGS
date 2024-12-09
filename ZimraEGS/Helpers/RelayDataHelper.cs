using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZimraEGS.Helpers
{
    public class RelayDataHelper
    {

        public static string ModifyStringCustomFields2(string editData, string fieldGuid, string? newValue, string? key = null)
        {
            JObject jsonObject = JObject.Parse(editData);
            JObject targetSection;

            // Determine the target section based on the key
            if (string.IsNullOrEmpty(key))
            {
                targetSection = jsonObject; // Work with the root object if no key
            }
            else
            {
                targetSection = FindSectionContainingKey(jsonObject, key);
            }

            if (targetSection == null)
            {
                throw new InvalidOperationException("The section with the specified key was not found.");
            }

            // Get or initialize the "CustomFields2" section
            JObject customFields2 = targetSection["CustomFields2"] as JObject;
            if (customFields2 == null)
            {
                customFields2 = new JObject();
                targetSection["CustomFields2"] = customFields2;
            }

            // Get or initialize the "Strings" section
            JObject strings = customFields2["Strings"] as JObject;
            if (strings == null)
            {
                strings = new JObject();
                customFields2["Strings"] = strings;
            }

            if (newValue == null)
            {
                // If newValue is null, remove the key
                if (strings.ContainsKey(fieldGuid))
                {
                    strings.Remove(fieldGuid);
                }
            }
            else
            {
                // If the key exists, update the value; otherwise, add the new key-value pair
                if (strings.ContainsKey(fieldGuid))
                {
                    strings[fieldGuid] = newValue;  // Update the value if the key exists
                }
                else
                {
                    strings.Add(fieldGuid, newValue);  // Create a new key-value pair if the key doesn't exist
                }
            }

            return jsonObject.ToString(Formatting.Indented);
        }

        public static string ModifyDecimalCustomFields2(string editData, string fieldGuid, double? newValue, string? key = null)
        {
            JObject jsonObject = JObject.Parse(editData);
            JObject targetSection;

            // Determine the target section based on the key
            if (string.IsNullOrEmpty(key))
            {
                targetSection = jsonObject; // Work with the root object if no key
            }
            else
            {
                targetSection = FindSectionContainingKey(jsonObject, key);
            }

            if (targetSection == null)
            {
                throw new InvalidOperationException("The section with the specified key was not found.");
            }

            // Get or initialize the "CustomFields2" section
            JObject customFields2 = targetSection["CustomFields2"] as JObject;
            if (customFields2 == null)
            {
                customFields2 = new JObject();
                targetSection["CustomFields2"] = customFields2;
            }

            // Get or initialize the "Decimals" section
            JObject decimals = customFields2["Decimals"] as JObject;
            if (decimals == null)
            {
                decimals = new JObject();
                customFields2["Decimals"] = decimals;
            }

            if (newValue == null)
            {
                // If newValue is null, remove the key
                if (decimals.ContainsKey(fieldGuid))
                {
                    decimals.Remove(fieldGuid);
                }
            }
            else
            {
                // If the key exists, update the value; otherwise, add the new key-value pair
                if (decimals.ContainsKey(fieldGuid))
                {
                    decimals[fieldGuid] = newValue;  // Update the value if the key exists
                }
                else
                {
                    decimals.Add(fieldGuid, newValue);  // Create a new key-value pair if the key doesn't exist
                }
            }

            return jsonObject.ToString(Formatting.Indented);
        }


        public static string ModifyFieldInCustomFields2(string jsonString, string key, string sectionName, string fieldGuid, string newValue)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            JObject targetSection;

            // Determine the target section based on the key
            if (string.IsNullOrEmpty(key))
            {
                targetSection = jsonObject; // Work with the root object if no key
            }
            else
            {
                targetSection = FindSectionContainingKey(jsonObject, key);
            }

            if (targetSection == null)
            {
                throw new InvalidOperationException("The section with the specified key was not found.");
            }

            // Get or initialize the "CustomFields2" section
            JObject customFields2 = targetSection["CustomFields2"] as JObject;
            if (customFields2 == null)
            {
                customFields2 = new JObject();
                targetSection["CustomFields2"] = customFields2;
            }

            // Handle the desired section ("Strings" or "Decimals")
            JObject specificSection = customFields2[sectionName] as JObject;
            if (specificSection == null)
            {
                specificSection = new JObject();
                customFields2[sectionName] = specificSection;
            }

            // Update or create the field
            specificSection[fieldGuid] = newValue;

            return jsonObject.ToString();
        }

        private static JObject FindSectionContainingKey(JObject jsonObject, string key)
        {
            var queue = new Queue<JObject>();
            queue.Enqueue(jsonObject);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (string.IsNullOrEmpty(key) || current.Property(key) != null)
                {
                    return current;
                }
                foreach (var property in current.Properties())
                {
                    if (property.Value.Type == JTokenType.Object)
                    {
                        queue.Enqueue(property.Value as JObject);
                    }
                }
            }
            return null;
        }


        public static string GetValueJson(string jsonString, string datakey)
        {
            // Parse the JSON string into a JObject
            var jsonObject = JObject.Parse(jsonString);

            // Navigate to BusinessDetails
            var dataValue = jsonObject[datakey];

            // Check if BusinessDetails is not null
            if (dataValue != null)
            {
                // Convert BusinessDetails back to a JSON string
                return dataValue.ToString();
            }

            return "No Data found.";
        }

        /// <summary>
        /// Updates or creates a field in the JSON string at the specified key path.
        /// </summary>
        /// <param name="jsonString">The original JSON string.</param>
        /// <param name="keyPath">Dot-separated path to the field (e.g., "CustomFields2.Strings.new-key").</param>
        /// <param name="value">The value to set for the field.</param>
        /// <returns>The updated JSON string.</returns>
        public static string UpdateOrCreateField(string jsonString, string keyPath, object value)
        {
            // Parse the JSON string into a JObject for manipulation
            var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonString);
            if (jsonObject == null)
                throw new ArgumentException("Invalid JSON string provided.");

            // Split the key path into parts
            var keys = keyPath.Split('.');
            JObject currentObject = jsonObject;
            string lastKey = keys[keys.Length - 1];

            // Traverse the JSON structure to the correct nested object
            for (int i = 0; i < keys.Length - 1; i++)
            {
                string key = keys[i];
                if (currentObject[key] is JObject nextObject)
                {
                    currentObject = nextObject;
                }
                else
                {
                    // If the key doesn't exist or isn't a JObject, create a new JObject
                    var newObject = new JObject();
                    currentObject[key] = newObject;
                    currentObject = newObject;
                }
            }

            // Add or update the field
            currentObject[lastKey] = JToken.FromObject(value);

            // Serialize the updated JSON back to a string
            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        }



        public static string GetJsonDataByGuid(string jsonString, string dataKey)
        {
            JObject fullData;
            try
            {
                fullData = JObject.Parse(jsonString);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Kesalahan saat mem-parsing JSON: " + ex.Message);
                return null;
            }

            if (!Guid.TryParse(dataKey, out Guid guid))
            {
                Console.WriteLine("DataKey tidak valid sebagai GUID.");
                return null;
            }

            JToken targetObject = FindValueInFullData(fullData, guid);

            if (targetObject != null)
            {
                ReplaceGuidsWithJson(targetObject, fullData);
                ReplaceGuidsWithJson(targetObject, fullData);

                string resultJson = JsonConvert.SerializeObject(targetObject, Formatting.Indented);

                if (resultJson != null)
                {
                    resultJson = resultJson.Replace("Customer", "InvoiceParty")
                                                 .Replace("Supplier", "InvoiceParty")
                                                 .Replace("SalesInvoice", "RefInvoice")
                                                 .Replace("PurchaseInvoice", "RefInvoice")
                                                 .Replace("SalesUnitPrice", "UnitPrice")
                                                 .Replace("PurchaseUnitPrice", "UnitPrice");

                    resultJson = RemoveFieldInJson(resultJson, "InvoiceParty", "RefInvoice");
                    resultJson = RemoveFieldInJson(resultJson, "Lines", "RefInvoice");
                    resultJson = RemoveFieldInJson(resultJson, "RefInvoiceCustomTheme");
                    resultJson = RemoveFieldInJson(resultJson, "RefInvoiceFooters");

                }

                return resultJson;
            }
            else
            {
                Console.WriteLine("Objek dengan DataKey tidak ditemukan.");
            }
            return null;
        }

        private static void ReplaceGuidsWithJson(JToken obj, JObject fullData)
        {
            if (obj.Type == JTokenType.Object)
            {
                foreach (var property in obj.Children<JProperty>())
                {
                    if (Guid.TryParse(property.Value.ToString(), out Guid guidValue))
                    {
                        var replacement = FindValueInFullData(fullData, guidValue);
                        if (replacement != null)
                        {
                            property.Value.Replace(replacement);
                        }
                    }
                    else
                    {
                        ReplaceGuidsWithJson(property.Value, fullData);
                    }
                }
            }
            else if (obj.Type == JTokenType.Array)
            {
                foreach (var item in obj.Children<JToken>())
                {
                    ReplaceGuidsWithJson(item, fullData);
                }
            }
        }

        private static JToken FindValueInFullData(JObject fullData, Guid guidValue)
        {
            foreach (var property in fullData.Properties())
            {
                // Cek apakah nama property adalah GUID yang dicari
                if (Guid.TryParse(property.Name, out Guid foundGuid) && foundGuid == guidValue)
                {
                    return property.Value;
                }

                // Cek jika property adalah objek
                if (property.Value.Type == JTokenType.Object)
                {
                    var result = FindValueInFullData((JObject)property.Value, guidValue);
                    if (result != null)
                    {
                        return result;
                    }
                }
                // Cek jika property adalah array
                else if (property.Value.Type == JTokenType.Array)
                {
                    var result = FindInArray(property.Value, guidValue);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static JToken FindInArray(JToken arrayToken, Guid guidValue)
        {
            foreach (var item in arrayToken.Children<JToken>())
            {
                if (item.Type == JTokenType.Object)
                {
                    var result = FindValueInFullData((JObject)item, guidValue);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        public static double ParseTotalValue(string htmlContent)
        {
            try
            {
                string pattern = @"<td[^>]*id=['""]Total['""][^>]*data-value=['""]([^'""]*)['""]";
                var match = System.Text.RegularExpressions.Regex.Match(htmlContent, pattern);

                if (!match.Success)
                    return 0;

                string dataValue = match.Groups[1].Value;

                if (double.TryParse(dataValue, out double result))
                    return Math.Round(result, 2);
            }
            catch
            {
                return 0;
            }

            return 0;
        }

        //FIND VALUE
        public static string FindStringValueInJson(string jsonData, string fieldValue, string jsonKey = null)
        {
            // Parse the JSON string into a JObject
            var fullData = JObject.Parse(jsonData);

            // If jsonKey is provided, find the corresponding token
            if (!string.IsNullOrEmpty(jsonKey))
            {
                var startToken = FindJsonKey(fullData, jsonKey);
                if (startToken != null && startToken.Type == JTokenType.Object)
                {
                    // Now search for the fieldValue within the found jsonKey
                    return FindValueInFullData((JObject)startToken, fieldValue)?.ToString();
                }
                return string.Empty; // If jsonKey is not found or is not an object
            }
            else
            {
                // If no jsonKey is provided, search for the fieldValue from the root
                return FindValueInFullData(fullData, fieldValue)?.ToString();
            }
        }

        // Recursive method to find the specified jsonKey
        private static JToken FindJsonKey(JToken token, string jsonKey)
        {
            // Check if the current token is an object
            if (token.Type == JTokenType.Object)
            {
                // Check if this object has the specified key
                if (token[jsonKey] != null)
                {
                    return token[jsonKey]; // Return the object associated with the jsonKey
                }

                // Recursively search through all properties of the object
                foreach (var property in token.Children<JProperty>())
                {
                    var result = FindJsonKey(property.Value, jsonKey);
                    if (result != null)
                    {
                        return result; // Return the found object
                    }
                }
            }
            // Check if the current token is an array
            else if (token.Type == JTokenType.Array)
            {
                // Recursively search through each item in the array
                foreach (var item in token.Children())
                {
                    var result = FindJsonKey(item, jsonKey);
                    if (result != null)
                    {
                        return result; // Return the found object
                    }
                }
            }

            return null; // Return null if the jsonKey is not found
        }

        // Recursive method to find the value of a specified fieldValue within an object
        public static JToken FindValueInFullData(JToken token, string fieldValue)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    // Check if the property name matches the fieldValue we are looking for
                    if (property.Name == fieldValue)
                    {
                        return property.Value; // Return the value if the property name matches
                    }

                    // Check if the property is an object
                    if (property.Value.Type == JTokenType.Object)
                    {
                        var result = FindValueInFullData(property.Value, fieldValue);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    // Check if the property is an array
                    else if (property.Value.Type == JTokenType.Array)
                    {
                        var result = FindInArray(property.Value, fieldValue);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            return null; // Return null if the fieldValue is not found
        }

        private static JToken FindInArray(JToken arrayToken, string fieldValue)
        {
            foreach (var item in arrayToken.Children<JToken>())
            {
                if (item.Type == JTokenType.Object)
                {
                    var result = FindValueInFullData(item, fieldValue);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null; // Return null if the fieldValue is not found in the array
        }

        //REPLACE VALUE
        // Main method to replace a specific field value by its key
        public static string ReplaceStringValueInJson(string jsonData, string fieldValue, string newValue, string? jsonKey = null)
        {
            // Parse the JSON string into a JObject
            var fullData = JObject.Parse(jsonData);

            // If jsonKey is provided, find the corresponding token
            if (!string.IsNullOrEmpty(jsonKey))
            {
                var startToken = FindJsonKey(fullData, jsonKey);
                if (startToken != null && startToken.Type == JTokenType.Object)
                {
                    // Now replace the fieldValue within the found jsonKey
                    ReplaceValueInFullData((JObject)startToken, fieldValue, newValue);
                }
            }
            else
            {
                // If no jsonKey is provided, replace the fieldValue from the root
                ReplaceValueInFullData(fullData, fieldValue, newValue);
            }

            // Return the modified JSON as a string
            return fullData.ToString();
        }

        // Recursive method to replace the value of a specified fieldValue within an object
        private static void ReplaceValueInFullData(JObject fullData, string fieldValue, string newValue)
        {
            foreach (var property in fullData.Properties())
            {
                // Check if the property name matches the fieldValue we are looking for
                if (property.Name == fieldValue)
                {
                    property.Value = newValue; // Replace the value with newValue
                    return; // Exit after replacing
                }

                // Check if the property is an object
                if (property.Value.Type == JTokenType.Object)
                {
                    ReplaceValueInFullData((JObject)property.Value, fieldValue, newValue);
                }
                // Check if the property is an array
                else if (property.Value.Type == JTokenType.Array)
                {
                    ReplaceInArray(property.Value, fieldValue, newValue);
                }
            }
        }

        private static void ReplaceInArray(JToken arrayToken, string fieldValue, string newValue)
        {
            foreach (var item in arrayToken.Children<JToken>())
            {
                if (item.Type == JTokenType.Object)
                {
                    ReplaceValueInFullData((JObject)item, fieldValue, newValue);
                }
            }
        }


        //REMOVE FIELD
        public static string RemoveFieldInJson(string jsonData, string fieldValue, string jsonKey = null)
        {
            // Parse the JSON string into a JObject
            var fullData = JObject.Parse(jsonData);

            // If jsonKey is provided, find the corresponding token
            if (!string.IsNullOrEmpty(jsonKey))
            {
                var startToken = FindJsonKey(fullData, jsonKey);
                if (startToken != null && startToken.Type == JTokenType.Object)
                {
                    // Now remove the fieldValue within the found jsonKey
                    RemoveFieldFromObject((JObject)startToken, fieldValue);
                }
            }
            else
            {
                // If no jsonKey is provided, remove the fieldValue from the root
                RemoveFieldFromObject(fullData, fieldValue);
            }

            // Return the modified JSON as a string
            return fullData.ToString();
        }

        // Recursive method to remove a specified field from an object
        private static void RemoveFieldFromObject(JObject obj, string fieldValue)
        {
            // Check if the field exists and remove it
            if (obj.Remove(fieldValue))
            {
                return; // Exit if the field was found and removed
            }

            // If the field was not found, check all properties
            foreach (var property in obj.Properties())
            {
                // Check if the property is an object
                if (property.Value.Type == JTokenType.Object)
                {
                    RemoveFieldFromObject((JObject)property.Value, fieldValue);
                }
                // Check if the property is an array
                else if (property.Value.Type == JTokenType.Array)
                {
                    RemoveFromArray(property.Value, fieldValue);
                }
            }
        }

        private static void RemoveFromArray(JToken arrayToken, string fieldValue)
        {
            foreach (var item in arrayToken.Children<JToken>())
            {
                if (item.Type == JTokenType.Object)
                {
                    RemoveFieldFromObject((JObject)item, fieldValue);
                }
            }
        }

    }
}