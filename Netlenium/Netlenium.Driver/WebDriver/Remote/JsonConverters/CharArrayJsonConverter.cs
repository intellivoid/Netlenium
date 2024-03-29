﻿using System;
using Newtonsoft.Json;

namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Provides a way to convert a Char array to JSON
    /// </summary>
    internal class CharArrayJsonConverter : JsonConverter
    {
        /// <summary>
        /// Checks if the object can be converted
        /// </summary>
        /// <param name="objectType">Type of the object to see if can be converted</param>
        /// <returns>True if can be converted else false</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType != null && objectType.IsAssignableFrom(typeof(char[]));
        }

        /// <summary>
        /// Writes the Object to JSON
        /// </summary>
        /// <param name="writer">A JSON Writer object</param>
        /// <param name="value">Object to be converted</param>
        /// <param name="serializer">JSON Serializer object instance</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer != null)
            {
                // We need a custom writer for char arrays, such as are used with SendKeys.
                // JSON.NET does not properly handle converting unicode characters to \uxxxx.
                writer.WriteStartArray();
                if (value is char[] arrayObject)
                {
                    foreach (var currentChar in arrayObject)
                    {
                        var codepoint = Convert.ToInt32(currentChar);
                        if ((codepoint >= 32) && (codepoint <= 126))
                        {
                            writer.WriteValue(currentChar);
                        }
                        else
                        {
                            var charRepresentation = "\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0');
                            writer.WriteRawValue("\"" + charRepresentation + "\"");
                        }
                    }
                }

                writer.WriteEndArray();
            }
        }

        /// <summary>
        /// Method not implemented
        /// </summary>
        /// <param name="reader">JSON Reader instance</param>
        /// <param name="objectType">Object type being read</param>
        /// <param name="existingValue">Existing Value to be read</param>
        /// <param name="serializer">JSON Serializer instance</param>
        /// <returns>Object from JSON</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
