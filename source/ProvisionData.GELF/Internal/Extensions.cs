/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.GELF.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.Json;

    internal static class GelfMessageExtensions
    {
        public static Byte[] GetBytes(this Message message) => Encoding.UTF8.GetBytes(message.GetJson());

        public static String GetJson(this Message message)
        {
            using var stream = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(stream);

            jsonWriter.WriteStartObject();
            jsonWriter.MaybeWrite("version", message.Version);
            jsonWriter.MaybeWrite("host", message.Host);
            jsonWriter.MaybeWrite("short_message", message.ShortMessage);
            jsonWriter.MaybeWrite("full_message", message.FullMessage);
            jsonWriter.WriteNumber("level", (Int32)message.Level);
            jsonWriter.MaybeWrite("facility", message.Facility);
            jsonWriter.MaybeWrite("line", message.Line);
            jsonWriter.MaybeWrite("file", message.File);
            jsonWriter.MaybeWrite("_unixtimestamp", message.Timestamp);

            foreach (var field in message.AdditionalFields)
            {
                WriteAdditionalField(jsonWriter, field);
            }

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteAdditionalField(Utf8JsonWriter jsonWriter, KeyValuePair<String, Object> field)
        {
            var key = $"_{field.Key}";

            switch (field.Value)
            {
                case null:
                    break;
                case SByte value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Byte value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Int16 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case UInt16 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Int32 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case UInt32 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Int64 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case UInt64 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Single value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Double value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Decimal value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                default:
                    jsonWriter.WriteString(key, field.Value.ToString());
                    break;
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, String? value)
        {
            if (value != null)
            {
                writer.WriteString(property, value);
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, Int32? value)
        {
            if (value.HasValue)
            {
                writer.WriteNumber(property, value.Value);
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, Double? value)
        {
            if (value.HasValue)
            {
                writer.WriteNumber(property, value.Value);
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, DateTime? value)
        {
            if (value.HasValue)
            {
                writer.WriteString(property, value.Value.ToString("o"));
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, TimeSpan? value)
        {
            if (value.HasValue)
            {
                writer.WriteNumber(property, value.Value.TotalSeconds);
            }
        }
    }
}
